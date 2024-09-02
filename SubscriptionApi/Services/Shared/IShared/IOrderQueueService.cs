using System.Collections.Concurrent;
using Probot.Data.Entities;
using static Probot.SubscriptionApi.Services.Shared.OrderQueueService;

namespace Probot.SubscriptionApi.Services.Shared.IShared
{
    public interface IOrderQueueService
    {
        ConcurrentDictionary<ulong, Order> Orders { get; }
        event OrderEventHandler OrderPlaced;
        void EnqueueOrder(Order order);
        void TryRemoveOrder(ulong orderId);
    }
}
