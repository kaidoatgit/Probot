using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Probot.Data;
using Probot.Data.Entities;
using Probot.SubscriptionApi.Exceptions;
using Probot.SubscriptionApi.Mappers;
using Probot.SubscriptionApi.Services.BackgroundServices.IServices;
using Probot.SubscriptionApi.Services.Hubs;
using Probot.SubscriptionApi.Services.Services.IServices;
using Probot.SubscriptionApi.Services.Shared.IShared;
using Probot.Shared.Dtos.Order.Request;
using Probot.Shared.Dtos.Order.Response;
using Probot.Shared.Enums;

namespace Probot.SubscriptionApi.Services.Services
{
    public class OrderService : IOrderService, IOrderMonitorService
    {
        private readonly ProbotContext _context;
        private readonly IProductKeyService _productKeyService;
        private readonly IInvoiceService _invoiceService;
        private readonly ITransactionService _transactionService;
        private readonly IOrderQueueService _orderQueueService;
        private readonly Mapper _mapper;
        private readonly IHubContext<NotificationHub, INotificationClient> _hubContext;
        private readonly IMonitorService _monitorService;

        public OrderService(ProbotContext context, Mapper mapper, IProductKeyService productKeyService, ITransactionService transactionService, IInvoiceService invoiceService, 
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
                ?? throw new SubscriptionException(ExceptionResult.UserNotFound404, "User not found");

            var distinctProductOptions = await _context.ProductOptions
                .Where(po => request.ProductOptionsId.Contains(po.Id))
                .Include(po => po.Product)
                .ToListAsync();

            if (distinctProductOptions.Count != request.ProductOptionsId.Distinct().Count())
                throw new SubscriptionException(ExceptionResult.ProductOptionNotFound404, "One or more Product options were not found");

            if (!request.IsManual)
            {
                var latestHash = _monitorService.LatestHash
                    ?? throw new SubscriptionException(ExceptionResult.SolanaRpcBadGateway502, "Bad Gateway: The external service is unreachable or returned an error.");
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
                .SingleOrDefaultAsync(o => o.Id == orderId)
                ?? throw new SubscriptionException(ExceptionResult.OrderNotFound404, "Order not found");
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
                .FirstOrDefaultAsync() ?? throw new SubscriptionException(ExceptionResult.OrderNotFound404, "Order not found");
            if (order.Status == OrderStatus.Completed) throw new SubscriptionException(ExceptionResult.OrderBadRequest400, $"Order {order.Id} already completed");

            var invoice = order.Invoice ?? throw new SubscriptionException(ExceptionResult.InvoiceNotFound404, "Invoice not found");
            if (invoice.Id != request.InvoiceId)
                throw new SubscriptionException(ExceptionResult.InvoiceBadRequest400, $"Order:{order.Id} not matched with Invoice:{request.InvoiceId}");

            using var dbTransaction = await _context.Database.BeginTransactionAsync();
            try
            {
                /*
                * Problem: when updating an order retrived from memory all related entities are detected as modified
                * Reason: this happens because EF Core doesn't have the original state of the entity from the database to compare against,
                *         so it assumes that all properties may have been changed.
                * Solution: ensure the Order entity initially is not tracked, and manually set property to modified = true
                */
                _context.Entry(order).State = EntityState.Unchanged;
                IEnumerable<ProductKey> productKeys = await _productKeyService.GenerateProductKeysAsync(order, CancellationToken.None);
                order.Complete(_context, productKeys);

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
                IEnumerable<ProductKey> productKeys = await _productKeyService.GenerateProductKeysAsync(order, stoppingToken);
                order.Complete(_context, productKeys);
                
                orderResult.OrderStatus = OrderStatus.Completed;
                orderResult.PurchasedKeysCountPerProduct = productKeys
                    .Where(p => p.ProductOption.Product!.RoleId.HasValue)
                    .GroupBy(p => p.ProductOption.Product!.RoleId!.Value)
                    .ToDictionary(g => g.Key, g => g.Count());

                await _context.SaveChangesAsync(stoppingToken);
                await dbTransaction.CommitAsync(stoppingToken);
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
