using ProPayments.Service.Data.Entities;
using ProPayments.Service.Dtos.Subscriptions.Request;

namespace ProPayments.Service.Services.Services.IServices
{
    public interface ISubscriptionService
    {
        Task<Subscription> CreateFreeSubscriptionAsync(SubscriptionRequest request);
        Task<Subscription> CreatePaidSubscriptionAsync(ulong userId, Invoice invoice);
    }
}
