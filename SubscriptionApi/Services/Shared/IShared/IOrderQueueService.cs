using Probot.Data.Entities;
using static Probot.SubscriptionApi.Services.Shared.OrderQueueService;

namespace Probot.SubscriptionApi.Services.Shared.IShared
{
    public interface IOrderQueueService
    {
        event OrderEventHandler OrderPlaced;
        void EnqueueOrder(Order order);
        void TryRemoveOrder(ulong orderId);
        IEnumerable<Order> GetAllOrders();
    }
}
