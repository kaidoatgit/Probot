using Probot.Data;
using Probot.Data.Entities;
using Probot.SubscriptionApi.Services.Services.IServices;

namespace Probot.SubscriptionApi.Services.Services
{
    public class InvoiceService : IInvoiceService
    {
        private readonly ProbotContext _context;

        public InvoiceService(ProbotContext context)
        {
            _context = context;
        }

        public async Task<Invoice?> GetInvoiceByIdAsync(ulong invoiceId)
        {
            return await _context.Invoices.FindAsync(invoiceId);
        }

        public async Task<Invoice> CreateInvoiceAsync(Order order, List<ProductOption> productOptions)
        {
            var user = order.User;
            Invoice invoice = new()
            {
                UserId = user.Id,
                Username = user.Username,
                OrderId = order.Id,
                OrderStatus = order.Status,
                OrderExpiryTime = order.ExpiryTime,
                TransactionId = order.Transaction.Id,
                TotalAmount = order.Transaction.TotalAmount,
                Token = order.Transaction.Token,
                PaymentAddress = user.WalletAddress,
                RecipientAddress = order.Transaction.RecipientAddress,
                InvoiceItems = productOptions
                    .Select(po => new InvoiceItem
                    {
                        ProductId = po.ProductId,
                        ProductName = po.Product.Name,
                        ProductRoleId = po.Product.RoleId,
                        ProductOptionId = po.Id,
                        ProductOptionPrice = po.Price,
                        ProductOptionPeriod = po.Period,
                        PeriodDescription = po.PeriodDescription
                    }).ToList()
            };
            
            _context.Invoices.Add(invoice);
            await _context.SaveChangesAsync();
            return invoice;
        }
    }
}
