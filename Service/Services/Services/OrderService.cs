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
        private readonly IAccessCodeService _accessCodeService;
        private readonly IInvoiceService _invoiceService;
        private readonly ITransactionService _transactionService;
        private readonly IOrderQueueService _orderQueueService;
        private readonly Mapper _mapper;
        private readonly IHubContext<NotificationHub, INotificationClient> _hubContext;
        private readonly IMonitorService _monitorService;

        public OrderService(SubscriptionContext context, Mapper mapper, ISubscriptionService subscriptionService, IAccessCodeService accessCodeService, ITransactionService transactionService, IInvoiceService invoiceService, 
        IOrderQueueService orderQueueService, IHubContext<NotificationHub, INotificationClient> hubContext, IMonitorService monitorService)
        {
            _context = context;
            _mapper = mapper;
            _subscriptionService = subscriptionService;
            _accessCodeService = accessCodeService;
            _transactionService = transactionService;
            _invoiceService = invoiceService;
            _orderQueueService = orderQueueService;
            _hubContext = hubContext;
            _monitorService = monitorService;
        }

        public async Task<Order> CreateOrderAsync(OrderRequest request)
        {
            var user = await _context.Users.FindAsync(request.UserId);
            if (user == null) throw new ServiceException(StatusCodes.Status404NotFound, "User not found");

            var distinctPlanOptions = await _context.PlanOptions
                .Where(po => request.PlanOptionsId.Contains(po.Id))
                .Include(po => po.Plan)
                .ToListAsync();

            if (distinctPlanOptions.Count != request.PlanOptionsId.Distinct().Count())
                throw new ServiceException(StatusCodes.Status404NotFound, "One or more Plan options were not found");

            if (!request.IsManual)
            {
                var latestHash = _monitorService.LatestHash;
                if(latestHash == null) throw new ServiceException(StatusCodes.Status502BadGateway, "Bad Gateway: The external service is unreachable or returned an error.");
            }

            using var dbTransaction = await _context.Database.BeginTransactionAsync();
            try
            {
                
                var planOptions = request.PlanOptionsId
                    .Select(id => distinctPlanOptions.First(po => po.Id == id))
                    .ToList();   

                Order order = _mapper.MapToOrderEntity(request);
                order.OrderItems = planOptions
                    .Select(po => new OrderItem
                    {
                        PlanOptionId = po.Id
                    }).ToList();
                _context.Orders.Add(order);
                await _context.SaveChangesAsync();

                Transaction transaction = await _transactionService.CreateTransaction(order, planOptions.Sum(po => po.Price));
                order.TransactionId = transaction.Id;

                Invoice invoice = await _invoiceService.CreateInvoiceAsync(user, order, planOptions);
                order.InvoiceId = invoice.Id;

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
            var order = await _context.Orders.FindAsync(orderId);
            if (order == null) throw new ServiceException(StatusCodes.Status404NotFound, "Order not found");

            return order;
        }

        public async Task<List<AccessCode>> CompleteOrderAsync(CompleteOrderRequest request)
        {
            var order = await _context.Orders
                .Where(o => o.Id == request.OrderId)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.PlanOption) 
                .Include(o => o.Invoice)
                .Include(o => o.Transaction)
                .FirstOrDefaultAsync();
            if (order == null) throw new ServiceException(StatusCodes.Status404NotFound, "Order not found");
            if (order.Status == OrderStatus.Completed) throw new ServiceException(StatusCodes.Status400BadRequest, $"Order {order.Id} is completed");

            if (order.Invoice == null) throw new ServiceException(StatusCodes.Status404NotFound, "Invoice not found");
            if (order.InvoiceId != request.InvoiceId)
                throw new ServiceException(StatusCodes.Status400BadRequest, $"Order:{order.Id} not matched with Invoice:{request.InvoiceId}");

            using var dbTransaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var invoice = order.Invoice!;
                var transaction = order.Transaction!;

                order.Complete();
                transaction.Close(request.Transaction.Hash);
                List<PlanOption> planOptions = order.OrderItems.Select(oi => oi.PlanOption).ToList();
                List<AccessCode> accessCodes = await _accessCodeService.GenerateCodes(planOptions);
                // Subscription subscription = await _subscriptionService.CreatePaidSubscriptionAsync(order.UserId, invoice);

                // order.SubscriptionId = subscription.SubscriptionId;
                invoice.UpdateOrderData(OrderStatus.Completed);
                // invoice.UpdateSubscriptionData(subscription);
                invoice.UpdateTransactionData(transaction);

                await _context.SaveChangesAsync();
                await dbTransaction.CommitAsync();

                return accessCodes;
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
                var invoice = order.Invoice!;
                // invoice.UpdateOrderData(OrderStatus.Expired);
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
            Subscription? subscription;
            using var dbTransaction = await _context.Database.BeginTransactionAsync(stoppingToken);
            try
            {
                // Ensure the User entity is not tracked
                _context.Entry(order.User!).State = EntityState.Unchanged;

                var invoice = order.Invoice!;
                var transaction = order.Transaction!;

                order.Complete();
                transaction.Close(transaction.Hash!);
                List<PlanOption> planOptions = order.OrderItems.Select(oi => oi.PlanOption).ToList();
                List<AccessCode> accessCodes = await _accessCodeService.GenerateCodes(planOptions);

                subscription = await _subscriptionService.CreatePaidSubscriptionAsync(order.UserId, invoice);

                // order.SubscriptionId = subscription.SubscriptionId;
                invoice.UpdateOrderData(OrderStatus.Completed);
                // invoice.UpdateSubscriptionData(subscription);
                invoice.UpdateTransactionData(transaction);

                _context.Orders.Update(order);

                await _context.SaveChangesAsync(stoppingToken);
                await dbTransaction.CommitAsync(stoppingToken);
            }
            catch (Exception)
            {
                await dbTransaction.RollbackAsync(stoppingToken);
                await _hubContext.Clients.All.ReceiveOrderResult(new OrderResult
                {
                    OrderStatus = OrderStatus.DbError,
                    OrderId = order.Id
                });
                throw;
            }
            finally
            {
                _orderQueueService.TryRemoveOrder(order.Id);
            }

            await _hubContext.Clients.All.ReceiveOrderResult(new OrderResult
            {
                OrderId = order.Id,
                OrderStatus = OrderStatus.Completed,
                Subscription = _mapper.MapToSubscriptionResponse(subscription)
            });
        }
        #endregion
    }
}
