using Probot.Data.Entities;
using Probot.SubscriptionApi.Services.Shared.IShared;
using System.Collections.Concurrent;

namespace Probot.SubscriptionApi.Services.Shared
{
    public class OrderQueueService : IOrderQueueService
    {
        private readonly ConcurrentDictionary<ulong, Order> _orders = new();
        public delegate void OrderEventHandler(object sender, EventArgs e);
        public event OrderEventHandler? OrderPlaced;

        public void EnqueueOrder(Order order)
        {
            _orders.TryAdd(order.Id, order);
            if (_orders.Count == 1)
            {
                OrderPlaced?.Invoke(this, EventArgs.Empty);
            }
            Console.WriteLine($"Total orders:{_orders.Count}");
        }

        public void TryRemoveOrder(ulong orderId)
        {
            _orders.TryRemove(orderId, out _);
            Console.WriteLine($"Total orders:{_orders.Count}");
        }

        public IEnumerable<Order> GetAllOrders()
        {
            return _orders.Values;
        }
    }
}
