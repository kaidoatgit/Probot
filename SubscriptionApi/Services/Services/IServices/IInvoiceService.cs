using Probot.Data.Entities;

namespace Probot.SubscriptionApi.Services.Services.IServices
{
    public interface IInvoiceService
    {
        Task<Invoice?> GetInvoiceByIdAsync(ulong invoiceId);
        Task<Invoice> CreateInvoiceAsync(Order order, List<ProductOption> productOptions);
    }
}
