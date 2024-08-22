using Microsoft.EntityFrameworkCore;
using ProPayments.Service.Data;
using ProPayments.Service.Data.Entities;
using ProPayments.Service.Dtos.ProRaffles.Request;
using ProPayments.Service.Dtos.UserSettings.Request;
using ProPayments.Service.Exceptions;
using ProPayments.Service.Services.Services.IServices;

namespace ProPayments.Service.Services.Services;

public class ProRaffleService : IProRaffleService
{    
    private readonly SubscriptionContext _context;
    private readonly IUserSettingService _userSettingService;
    public ProRaffleService(SubscriptionContext context, IUserSettingService userSettingService)
    {
        _context = context;
        _userSettingService = userSettingService;
    }

    public async Task<UserSetting> CreateProRaffleAsync(UserSettingRequest request)
    {
        return await _userSettingService.CreateUserSettingAsync(request);
    }

    public async Task<bool> UpdateProRaffleKeyAsync(ulong userId, UpdateProRaffleKeyRequest request)
    {
        var proRaffle = await _context.ProRaffles
            .FirstOrDefaultAsync(prs => prs.UserId == userId && prs.Key == request.CurrentKey);
        if(proRaffle == null) return false;

        bool keyExists = await _context.ProRaffles
            .AsNoTracking()
            .AnyAsync(ps => ps.Key == request.NewKey);
        if (keyExists) throw new ServiceException(StatusCodes.Status409Conflict, $"Key:{request.NewKey} already in use.");

        proRaffle.Key = request.NewKey;
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<IEnumerable<ProRaffle>> GetProRaffleSubscriptionsAsync(ulong userId, bool isActive)
    {
        var proRaffles = await _context.ProRaffles
            .Include(s => s.Subscription)
            .Where(s => s.UserId == userId && s.Subscription.IsActive == isActive)
            .ToListAsync();
        return proRaffles;
    }

}
