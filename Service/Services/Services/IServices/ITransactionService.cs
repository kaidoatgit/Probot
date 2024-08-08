using ProPayments.Service.Data.Entities;

namespace ProPayments.Service.Services.Services.IServices
{
    public interface ITransactionService
    {
        Task<Transaction> CreateTransaction(Order order, PlanOption planOption);
    }
}
