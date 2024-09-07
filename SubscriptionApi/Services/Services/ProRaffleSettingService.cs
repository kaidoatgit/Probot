using Microsoft.EntityFrameworkCore;
using Probot.Data;
using Probot.Data.Entities;
using Probot.Shared.Dtos.ProRaffleSetting.Request;
using Probot.Shared.Enums;
using Probot.SubscriptionApi.Exceptions;
using Probot.SubscriptionApi.Services.Services.IServices;

namespace Probot.SubscriptionApi.Services.Services;

public class ProRaffleSettingService : IProRaffleSettingService
{    
    private readonly ProbotContext _context;
    public ProRaffleSettingService(ProbotContext context)
    {
        _context = context;
    }

    public async Task UpdateProRaffleSettingKeyAsync(ulong userId, UpdatePRSettingKeyRequest request)
    {
        bool isKeyInUse = await _context.ProRaffleSettings
            .AnyAsync(pr => pr.Key == request.NewKey);
        if (isKeyInUse) throw new ServiceException(StatusCodes.Status409Conflict, $"New Key: {request.NewKey} already in use.");

        var proRaffleSetting = await _context.ProRaffleSettings
            .SingleOrDefaultAsync(pr => pr.UserId == userId && pr.Key == request.CurrentKey)
            ?? throw new ServiceException(StatusCodes.Status404NotFound, $"The current key: {request.CurrentKey} not found or does not belong to the user: {userId}");

        proRaffleSetting.Key = request.NewKey;
        await _context.SaveChangesAsync();
    }

    public async Task<ProRaffleSetting> CreateSettingsAsync(ulong userId, ProRaffleSettingRequest request)
    {
        string errorMessage = "Alphabot key or Username already in use by another user."; 
        bool isKeyOrUsernameInUse = await _context.ProRaffleSettings
            .AnyAsync(prs => prs.Key == request.AlphabotKey || prs.Username == request.Username);
        if (isKeyOrUsernameInUse)
        {
           throw new ServiceException(StatusCodes.Status409Conflict, ServiceResult.ProductSetting409, errorMessage);
        }

        var proRaffleSetting = new ProRaffleSetting
        {
            UserId = userId,
            Username = request.Username,
            Key = request.AlphabotKey
        };
        _context.ProRaffleSettings.Add(proRaffleSetting);
        try
        {
            await _context.SaveChangesAsync();
        }
        catch (Exception)
        {
            throw new ServiceException(StatusCodes.Status409Conflict, ServiceResult.ProductSetting409, errorMessage);
        }
        return proRaffleSetting;
    }

    public async Task<ProRaffleSetting?> GetSettingsAsync(ulong userId, ProRaffleSettingRequest request)
    {  
        var proRaffleSetting = await _context.ProRaffleSettings
            .Include(pr => pr.Subscriptions)
            .SingleOrDefaultAsync(prs => prs.UserId == userId && prs.Username == request.Username);

        if(proRaffleSetting != null)
        {
            //If the current settings was paused, then unpause and mark as row changed
            proRaffleSetting.IsPaused = false;
            proRaffleSetting.Version = Guid.NewGuid();
        }

        return proRaffleSetting;
    }
}
