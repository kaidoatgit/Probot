using Probot.Data.Entities;
using Probot.SubscriptionApi.Services.BackgroundServices.IServices;
using Probot.SubscriptionApi.Services.Shared.IShared;
using Probot.Shared.Enums;
using System.Collections.Concurrent;

namespace Probot.SubscriptionApi.Services.BackgroundServices
{
    public class OrderMonitorService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IOrderQueueService _orderQueueService;
        private readonly IMonitorService _monitorService;
        private static readonly TimeSpan _monitoringPeriod = TimeSpan.FromSeconds(1);
        private readonly object _preventMultipleAccess = new();
        private bool _isProcessing;

        public OrderMonitorService(IServiceProvider serviceProvider, IOrderQueueService orderQueueService, IMonitorService monitorService)
        {
            _serviceProvider = serviceProvider;
            _orderQueueService = orderQueueService;
            _orderQueueService.OrderPlaced += OnOrderPlaced;
            _monitorService = monitorService;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _ = Task.Run(async () => await InitTransactionsOrderChannel(stoppingToken));
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    Console.WriteLine("[Order Monitor] -> Waiting for new orders");
                    await _monitorService.WaitForOrderSignalAsync(stoppingToken);
                    await MonitorOrders(stoppingToken);
                }
                catch (Exception e)
                {
                    Console.WriteLine($"[Order Monitor] -> Program shutdown | Reason: {e.Message}");
                }
            }
        }

        private void OnOrderPlaced(object sender, EventArgs e)
        {
            lock (_preventMultipleAccess)
            {
                if (_isProcessing)
                {
                    return;
                }
                _isProcessing = true;
                _monitorService.NotifyNewOrder();
            }
        }

        private bool IsSetIdle(ConcurrentDictionary<ulong, Order> orders)
        {
            lock (_preventMultipleAccess)
            {
                if (orders.Any())
                {
                    return false;
                }

                _isProcessing = false;
                _monitorService.Cancel();
                return true;
            }
        }

        private async Task MonitorOrders(CancellationToken stoppingToken)
        {

            using var timer = new PeriodicTimer(_monitoringPeriod);
            while (!stoppingToken.IsCancellationRequested && await timer.WaitForNextTickAsync(stoppingToken))
            {
                var orders = _orderQueueService.Orders;
                if (IsSetIdle(orders))
                {
                    Console.WriteLine("[Order Monitor] -> Set to idle");
                    return;
                }
                foreach (var (orderId, order) in orders)
                {
                    try
                    {
                        if (order.Status == OrderStatus.Pending && DateTime.UtcNow > order.ExpiryTime)
                        {
                            order.Status = OrderStatus.Expired;
                            using var scope = _serviceProvider.CreateScope();
                            var orderMonitorService = scope.ServiceProvider.GetRequiredService<IOrderMonitorService>();
                            await orderMonitorService.DeleteOrderAsync(order);
                        }
                        else if (order.Status == OrderStatus.Matched)
                        {
                            order.Status = OrderStatus.Completed;
                            using var scope = _serviceProvider.CreateScope();
                            var orderMonitorService = scope.ServiceProvider.GetRequiredService<IOrderMonitorService>();
                            await orderMonitorService.CompleteOrderAsync(order, stoppingToken);
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error processing order {orderId}: {ex.Message}");
                    }
                }
            }
        }

        private async Task InitTransactionsOrderChannel(CancellationToken stoppingToken)
        {
            try
            {
                Console.WriteLine("[Transaction Channel] -> Open");

                await foreach (var tx in _monitorService.TransactionChannel.Reader.ReadAllAsync(stoppingToken))
                {
                    var orders = _orderQueueService.Orders;
                    foreach (var (orderId, order) in orders)
                    {
                        if (string.Equals(tx.Address, order.Transaction?.PaymentAddress, StringComparison.InvariantCultureIgnoreCase)
                            && order.Transaction?.TotalAmount == tx.Amount
                            && order.Status == OrderStatus.Pending)
                        {
                            Console.WriteLine($"Order matched for transaction: {tx.Hash}");
                            order.Transaction.Hash = tx.Hash;
                            order.Status = OrderStatus.Matched;
                            break;
                        }
                    }
                }
            }
            finally
            {
                Console.WriteLine("[Transaction Channel] -> Closed");
            }
        }

        public override void Dispose()
        {
            base.Dispose();
            (_monitorService as IDisposable)?.Dispose();
        }
    }
}
