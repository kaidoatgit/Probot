using System;
using ProPayments.Service.Data;
using ProPayments.Service.Data.Entities;
using ProPayments.Service.Services.Services.IServices;

namespace ProPayments.Service.Services.Services;

public class AccessCodeService : IAccessCodeService
{

    private readonly SubscriptionContext _context;
    public AccessCodeService(SubscriptionContext context)
    {
        _context = context;
    }

    public async Task<List<AccessCode>> GenerateCodes(List<PlanOption> planOptions)
    {
        List<AccessCode> accessCodes = new();
        foreach (var planOption in planOptions)
        {
            var accessCode = new AccessCode
            {
                EndDate = DateTime.UtcNow.AddMonths(planOption.Period),
                // OrderId = order.Id,
                // UserId = order.UserId
            };
            accessCodes.Add(accessCode);
            _context.AccessCodes.Add(accessCode);
        }

        // _context.AccessCodes.UpdateRange(accessCodes);
        await _context.SaveChangesAsync();
        return accessCodes;
    }
}
