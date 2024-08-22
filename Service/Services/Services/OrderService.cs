using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using ProPayments.Service.Data;
using ProPayments.Service.Data.Entities;
using ProPayments.Service.Data.Entities.Enums;
using ProPayments.Service.Dtos.Orders.Request;
using ProPayments.Service.Dtos.Orders.Response;
using ProPayments.Service.Exceptions;
using ProPayments.Service.Mappers;
using ProPayments.Service.Services.BackgroundServices.IServices;
using ProPayments.Service.Services.Hubs;
using ProPayments.Service.Services.Hubs.IClients;
using ProPayments.Service.Services.Services.IServices;
using ProPayments.Service.Services.Shared.IShared;

namespace ProPayments.Service.Services.Services
{
    public class OrderService : IOrderService, IOrderMonitorService
    {
        private readonly SubscriptionContext _context;
        private readonly ISubscriptionService _subscriptionService;
        private readonly IProductKeyService _productKeyService;
        private readonly IInvoiceService _invoiceService;
        private readonly ITransactionService _transactionService;
        private readonly IOrderQueueService _orderQueueService;
        private readonly Mapper _mapper;
        private readonly IHubContext<NotificationHub, INotificationClient> _hubContext;
        private readonly IMonitorService _monitorService;

        public OrderService(SubscriptionContext context, Mapper mapper, ISubscriptionService subscriptionService, IProductKeyService productKeyService, ITransactionService transactionService, IInvoiceService invoiceService, 
        IOrderQueueService orderQueueService, IHubContext<NotificationHub, INotificationClient> hubContext, IMonitorService monitorService)
        {
            _context = context;
            _mapper = mapper;
            _subscriptionService = subscriptionService;
            _productKeyService = productKeyService;
            _transactionService = transactionService;
            _invoiceService = invoiceService;
            _orderQueueService = orderQueueService;
            _hubContext = hubContext;
            _monitorService = monitorService;
        }

        public async Task<Order> CreateOrderAsync(OrderRequest request)
        {
            var user = await _context.Users
                .FindAsync(request.UserId) ?? throw new ServiceException(StatusCodes.Status404NotFound, "User not found");

            var distinctProductOptions = await _context.ProductOptions
                .Where(po => request.ProductOptionsId.Contains(po.Id))
                .Include(po => po.Product)
                .ToListAsync();

            if (distinctProductOptions.Count != request.ProductOptionsId.Distinct().Count())
                throw new ServiceException(StatusCodes.Status404NotFound, "One or more Product options were not found");

            if (!request.IsManual)
            {
                var latestHash = _monitorService.LatestHash
                    ?? throw new ServiceException(StatusCodes.Status502BadGateway, "Bad Gateway: The external service is unreachable or returned an error.");
            }

            using var dbTransaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var productOptions = request.ProductOptionsId
                    .Select(id => distinctProductOptions.First(po => po.Id == id))
                    .ToList();   

                Order order = _mapper.MapToOrderEntity(request);
                order.OrderItems = productOptions
                    .Select(po => new OrderItem
                    {
                        ProductOptionId = po.Id
                    }).ToList();
                _context.Orders.Add(order);
                await _context.SaveChangesAsync();

                Transaction transaction = await _transactionService.CreateTransaction(order, productOptions.Sum(po => po.Price));
                Invoice invoice = await _invoiceService.CreateInvoiceAsync(order, productOptions);

                await _context.SaveChangesAsync();
                await dbTransaction.CommitAsync();

                if (!request.IsManual)
                {
                    _ = Task.Run(() => _orderQueueService.EnqueueOrder(order));
                }

                return order;
            }
            catch (Exception)
            {
                await dbTransaction.RollbackAsync();
                throw;
            }
        }

        public async Task<Order> GetOrderByIdAsync(ulong orderId)
        {
            var order = await _context.Orders
                .FindAsync(orderId) ?? throw new ServiceException(StatusCodes.Status404NotFound, "Order not found");
            return order;
        }

        public async Task<IEnumerable<ProductKey>> CompleteOrderAsync(CompleteOrderRequest request)
        {
            var order = await _context.Orders
                .Where(o => o.Id == request.OrderId)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.ProductOption) 
                .Include(o => o.Invoice)
                .Include(o => o.Transaction)
                .FirstOrDefaultAsync() ?? throw new ServiceException(StatusCodes.Status404NotFound, "Order not found");
            if (order.Status == OrderStatus.Completed) throw new ServiceException(StatusCodes.Status400BadRequest, $"Order {order.Id} is completed");

            var invoice = order.Invoice ?? throw new ServiceException(StatusCodes.Status404NotFound, "Invoice not found");
            if (invoice.Id != request.InvoiceId)
                throw new ServiceException(StatusCodes.Status400BadRequest, $"Order:{order.Id} not matched with Invoice:{request.InvoiceId}");

            using var dbTransaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var transaction = order.Transaction;

                order.Complete();
                transaction.Close(request.Transaction.Hash);
                IEnumerable<ProductKey> productKeys = await _productKeyService.GenerateProductKeys(order);
                invoice.UpdateOrderData(OrderStatus.Completed);
                invoice.UpdateTransactionData(transaction);
                invoice.UpdateCodes(productKeys);

                await _context.SaveChangesAsync();
                await dbTransaction.CommitAsync();

                return productKeys;
            }
            catch (Exception)
            {
                await dbTransaction.RollbackAsync();
                throw;
            }
        }


        #region OrderMonitor
        public async Task DeleteOrderAsync(Order order)
        {
            try
            {
                var invoice = order.Invoice;
                invoice.UpdateOrderData(OrderStatus.Expired);
                _context.Entry(invoice).State = EntityState.Modified;

                _context.Orders.Remove(order);
                await _context.SaveChangesAsync();

                var orderResult = new OrderResult { OrderStatus = OrderStatus.Expired, OrderId = order.Id };
                await _hubContext.Clients.All.ReceiveOrderResult(orderResult);
            }
            catch (Exception) 
            {
                throw;
            }
            finally
            {
                _orderQueueService.TryRemoveOrder(order.Id);
            }
        }

        public async Task CompleteOrderAsync(Order order, CancellationToken stoppingToken)
        {
            OrderResult orderResult = new(){
                OrderId = order.Id,
                UserId = order.UserId,
                OrderStatus = OrderStatus.DbError
            };

            using var dbTransaction = await _context.Database.BeginTransactionAsync(stoppingToken);
            try
            {
                // Ensure the User entity is not tracked
                _context.Entry(order.User).State = EntityState.Unchanged;
                var transaction = order.Transaction;
                var invoice = order.Invoice;

                order.Complete();
                transaction.Close(transaction.Hash!);
                IEnumerable<ProductKey> productKeys = await _productKeyService.GenerateProductKeys(order);
                invoice.UpdateOrderData(OrderStatus.Completed);
                invoice.UpdateCodes(productKeys.ToList());
                invoice.UpdateTransactionData(transaction);

                _context.Orders.Update(order);
                await _context.SaveChangesAsync(stoppingToken);
                await dbTransaction.CommitAsync(stoppingToken);

                orderResult.OrderStatus = OrderStatus.Completed;
                orderResult.TotalProductKeys = productKeys.Count();
                orderResult.ProductRoleIds = invoice.InvoiceItems.Select(ii => ii.ProductRoleId!.Value).Distinct().ToList();
            }
            catch (Exception)
            {
                await dbTransaction.RollbackAsync(stoppingToken);
                await _hubContext.Clients.All.ReceiveOrderResult(orderResult);
                throw;
            }
            finally
            {
                _orderQueueService.TryRemoveOrder(order.Id);
            }
            await _hubContext.Clients.All.ReceiveOrderResult(orderResult);
        }
        #endregion
    }
}
