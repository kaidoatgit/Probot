using ProPayments.Service.Data;
using ProPayments.Service.Data.Entities;
using ProPayments.Service.Services.Services.IServices;

namespace ProPayments.Service.Services.Services
{
    public class InvoiceService : IInvoiceService
    {
        private readonly SubscriptionContext _context;

        public InvoiceService(SubscriptionContext context)
        {
            _context = context;
        }

        public async Task<Invoice?> GetInvoiceByIdAsync(ulong invoiceId)
        {
            return await _context.Invoices.FindAsync(invoiceId);
        }

        public async Task<Invoice> CreateInvoiceAsync(User user, Order order, List<PlanOption> planOptions)
        {
            Invoice invoice = new()
            {
                UserId = user.Id,
                Username = user.Username,
                OrderId = order.Id,
                OrderStatus = order.Status,
                OrderExpiryTime = order.ExpiryTime,
                TransactionId = order.TransactionId,
                TotalAmount = order.Transaction!.TotalAmount,
                Token = order.Transaction.Token,
                PaymentAddress = user.WalletAddress,
                RecipientAddress = order.Transaction.RecipientAddress,
                InvoiceItems = planOptions
                    .Select(po => new InvoiceItem
                    {
                        PlanId = po.PlanId,
                        PlanType = po.Plan.Type,
                        PlanRoleId = po.Plan.RoleId,
                        PlanOptionId = po.Id,
                        PlanOptionPrice = po.Price,
                        PlanOptionPeriod = po.Period,
                        PeriodDescription = po.PeriodDescription
                    }).ToList()
            };


            _context.Invoices.Add(invoice);
            await _context.SaveChangesAsync();
            return invoice;
        }
    }
}
