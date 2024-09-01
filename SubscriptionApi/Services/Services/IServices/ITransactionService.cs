using Probot.Data.Entities;

namespace Probot.SubscriptionApi.Services.Services.IServices
{
    public interface ITransactionService
    {
        Task<Transaction> CreateTransaction(Order order, decimal totalPrice);
    }
}
