using ProPayments.Service.Data.Entities;
using static ProPayments.Service.Services.Shared.OrderQueueService;

namespace ProPayments.Service.Services.Shared.IShared
{
    public interface IOrderQueueService
    {
        event OrderEventHandler OrderPlaced;
        void EnqueueOrder(Order order);
        void TryRemoveOrder(ulong orderId);
        IEnumerable<Order> GetAllOrders();
    }
}
