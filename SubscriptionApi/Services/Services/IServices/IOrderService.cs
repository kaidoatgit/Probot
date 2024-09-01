using Probot.Data.Entities;
using Probot.Shared.Dtos.Order.Request;

namespace Probot.SubscriptionApi.Services.Services.IServices
{
    public interface IOrderService
    {
        Task<Order> CreateOrderAsync(OrderRequest createOrderRequest);
        Task<Order> GetOrderByIdAsync(ulong orderId);
        Task<IEnumerable<ProductKey>> CompleteOrderAsync(CompleteOrderRequest completeOrderRequest);
    }
}
