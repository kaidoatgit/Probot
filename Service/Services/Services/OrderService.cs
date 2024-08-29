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
        private readonly IProductKeyService _productKeyService;
        private readonly IInvoiceService _invoiceService;
        private readonly ITransactionService _transactionService;
        private readonly IOrderQueueService _orderQueueService;
        private readonly Mapper _mapper;
        private readonly IHubContext<NotificationHub, INotificationClient> _hubContext;
        private readonly IMonitorService _monitorService;

        public OrderService(SubscriptionContext context, Mapper mapper, IProductKeyService productKeyService, ITransactionService transactionService, IInvoiceService invoiceService, 
        IOrderQueueService orderQueueService, IHubContext<NotificationHub, INotificationClient> hubContext, IMonitorService monitorService)
        {
            _context = context;
            _mapper = mapper;
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
                .FindAsync(request.UserId)
                ?? throw new ServiceException(StatusCodes.Status404NotFound, "User not found");

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
                List<ProductOption> productOptions = request.ProductOptionsId
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
                .AsNoTracking()
                .FirstOrDefaultAsync(o => o.Id == orderId)
                ?? throw new ServiceException(StatusCodes.Status404NotFound, "Order not found");
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

                /*
                * Problem: when updating an order retrived from memory all related entities are detected as modified
                * Reason: this happens because EF Core doesn't have the original state of the entity from the database to compare against,
                *         so it assumes that all properties may have been changed.
                * Solution: ensure the Order entity initially is not tracked, and manually set property to modified = true
                */
                _context.Entry(order).State = EntityState.Unchanged;
                IEnumerable<ProductKey> productKeys = await _productKeyService.GenerateProductKeys(order, CancellationToken.None);
                order.Complete(_context, productKeys.ToList());

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
                order.Invoice.UpdateOrderData(_context, OrderStatus.Expired);
                _context.Orders.Remove(order);

                // Console.WriteLine("Before Saving:\n" + _context.ChangeTracker.DebugView.LongView);
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
                /*
                * Problem: when updating an order retrived from memory all related entities are detected as modified
                * Reason: this happens because EF Core doesn't have the original state of the entity from the database to compare against,
                *         so it assumes that all properties may have been changed.
                * Solution: ensure the Order entity initially is not tracked, and manually set property to modified = true
                */
                _context.Entry(order).State = EntityState.Unchanged;
                IEnumerable<ProductKey> productKeys = await _productKeyService.GenerateProductKeys(order, stoppingToken);
                order.Complete(_context, productKeys.ToList());

                // _context.Orders.Update(order);
                await _context.SaveChangesAsync(stoppingToken);
                await dbTransaction.CommitAsync(stoppingToken);


                orderResult.OrderStatus = OrderStatus.Completed;
                orderResult.TotalKeysByProduct = productKeys
                    .Where(p => p.ProductOption.Product.RoleId.HasValue) // Ensure RoleId is not null
                    .GroupBy(p => p.ProductOption.Product.RoleId!.Value)  // Group by RoleId
                    .ToDictionary(g => g.Key, g => g.Count());           // Convert to dictionary with counts
                // orderResult.TotalProductKeys = productKeys.Count();
                // orderResult.ProductRoleIds = order.Invoice.InvoiceItems.Select(ii => ii.ProductRoleId!.Value).Distinct().ToList();
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
