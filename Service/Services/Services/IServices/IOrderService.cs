using ProPayments.Service.Data.Entities;
using ProPayments.Service.Dtos.Orders.Request;

namespace ProPayments.Service.Services.Services.IServices
{
    public interface IOrderService
    {
        Task<Order> CreateOrderAsync(OrderRequest createOrderRequest);
        Task<Order> GetOrderByIdAsync(ulong orderId);
        Task<IEnumerable<ProductKey>> CompleteOrderAsync(CompleteOrderRequest completeOrderRequest);
    }
}
