using Probot.Data.Entities;

namespace Probot.SubscriptionApi.Services.Shared.IShared
{
    public interface IOrderMonitorService
    {
        Task CompleteOrderAsync(Order order, CancellationToken stoppingToken);
        Task DeleteOrderAsync(Order order);
    }
}
