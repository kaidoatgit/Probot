using ProPayments.Service.Data.Entities;

namespace ProPayments.Service.Services.Services.IServices
{
    public interface IInvoiceService
    {
        Task<Invoice?> GetInvoiceByIdAsync(ulong invoiceId);
        Task<Invoice> CreateInvoiceAsync(Order order, List<ProductOption> productOptions);
    }
}
