using Microsoft.EntityFrameworkCore;
using ProPayments.Service.Data;
using ProPayments.Service.Data.Entities;
using ProPayments.Service.Data.Entities.Enums;
using ProPayments.Service.Dtos.Subscriptions.Request;
using ProPayments.Service.Exceptions;
using ProPayments.Service.Mappers;
using ProPayments.Service.Services.Services.IServices;

namespace ProPayments.Service.Services.Services
{
    public class SubscriptionService : ISubscriptionService
    {
        private readonly SubscriptionContext _context;
        private readonly Mapper _mapper;

        public SubscriptionService(SubscriptionContext context, Mapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<Subscription> CreateFreeSubscriptionAsync(SubscriptionRequest request)
        {
            var user = await _context.Users.FindAsync(request.UserId);
            if (user == null) throw new ServiceException(StatusCodes.Status404NotFound, "User not found");

            var planOption = await _context.PlanOptions
                .Where(po => po.Id == request.PlanOptionId)
                .Include(po => po.Plan)
                .Include(po => po.Subscriptions)
                .FirstOrDefaultAsync();

            if (planOption == null) throw new ServiceException(StatusCodes.Status404NotFound, "Plan option not found");
            if (planOption.Plan != null && planOption.Plan.Type != PlanType.Free)
                throw new ServiceException(StatusCodes.Status400BadRequest, $"Invalid plan option: {planOption.Plan.Type}");

            if (planOption.Subscriptions!.Any(s => s.UserId == user.Id))
            {
                throw new ServiceException(StatusCodes.Status409Conflict, $"Free Subscriptions already exists for user: {user.Username}");
            }

            DateTime startDate = DateTime.UtcNow;
            Subscription subscription = new()
            {
                StartDate = startDate,
                EndDate = startDate.AddDays(planOption.Period),
                UserId = user.Id,
                PlanOptionId = planOption.Id,
                PlanId = planOption.PlanId
            };

            _context.Subscriptions.Add(subscription);
            await _context.SaveChangesAsync();
            return subscription;
        }

        public async Task<Subscription> CreatePaidSubscriptionAsync(ulong userId, Invoice invoice)
        {
            var existingSubscription = await _context.Subscriptions
                .Include(s => s.Plan)
                .Where(s => s.UserId == userId && s.PlanId == invoice.PlanId)
               .FirstOrDefaultAsync();

            var subscription = existingSubscription;
            DateTime currentDate = DateTime.UtcNow;
            if (subscription != null)
            {
                if (subscription.IsActive)
                {
                    subscription.EndDate = subscription.EndDate.AddMonths(invoice.PlanPeriod);
                }
                else
                {
                    subscription.EndDate = currentDate.AddMonths(invoice.PlanPeriod);
                    subscription.IsActive = true;
                }
                subscription.UpdatedAt = currentDate;
                subscription.StartDate = currentDate;
                subscription.LastNotificationCheck = currentDate;
                subscription.Username = invoice.Username;
                subscription.PlanOptionId = invoice.PlanOptionId;

                _context.Subscriptions.Update(subscription);
                await _context.SaveChangesAsync();
            }
            else
            {
                subscription = new Subscription
                {
                    StartDate = currentDate,
                    EndDate = currentDate.AddMonths(invoice.PlanPeriod),
                    UserId = userId,
                    Username = invoice.Username,
                    PlanId = invoice.PlanId,
                    PlanOptionId = invoice.PlanOptionId,
                };
                _context.Subscriptions.Add(subscription);
                await _context.SaveChangesAsync();

                //explicity load the reference
                await _context.Entry(subscription).Reference(s => s.Plan).LoadAsync();
            }

            return subscription;
        }
    }
}
