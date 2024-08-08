using ProPayments.Service.Data.Entities;

namespace ProPayments.Service.Services.Shared.IShared
{
    public interface IOrderMonitorService
    {
        Task CompleteOrderAsync(Order order, CancellationToken stoppingToken);
        Task DeleteOrderAsync(Order order);
    }
}
