using Microsoft.EntityFrameworkCore;
using ProPayments.Service.Data;
using ProPayments.Service.Data.Entities;
using ProPayments.Service.Dtos.ProRaffles.Request;
using ProPayments.Service.Exceptions;
using ProPayments.Service.Services.Services.IServices;

namespace ProPayments.Service.Services.Services;

public class ProRaffleService : IProRaffleService
{    
    private readonly SubscriptionContext _context;
    public ProRaffleService(SubscriptionContext context)
    {
        _context = context;
    }

    public async Task<bool> UpdateProRaffleKeyAsync(ulong userId, UpdateProRaffleKeyRequest request)
    {
        bool isKeyInUse = await _context.ProRaffles
            .AsNoTracking()
            .AnyAsync(pr => pr.Key == request.NewKey);
        if (isKeyInUse) throw new ServiceException(StatusCodes.Status409Conflict, $"Key:{request.NewKey} already in use.");

        var proRaffle = await _context.ProRaffles
            .SingleOrDefaultAsync(pr => pr.UserId == userId && pr.Key == request.CurrentKey);
        if(proRaffle == null) return false;

        proRaffle.Key = request.NewKey;
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<(bool IsNewSetting, ProRaffle ProRaffle)> CreateSettingsAsync(ProRaffleRequest request)
    {
        bool isNewSetting = false;
        var proRaffle = await _context.ProRaffles
            .Include(pr => pr.Subscriptions)
            .SingleOrDefaultAsync(prs => prs.Key == request.AlphabotKey);

        if(proRaffle != null && proRaffle.UserId != request.UserId)
        {
            throw new ServiceException(StatusCodes.Status409Conflict, $"Activation failed for Alphabot Key: `{request.AlphabotKey}`");
        }
        if(proRaffle == null)
        {
            isNewSetting = true;
            proRaffle = new ProRaffle
            {
                Key = request.AlphabotKey,
                UserId = request.UserId
            };
            _context.ProRaffles.Add(proRaffle);
            await _context.SaveChangesAsync();
        }
        else
        {
            proRaffle.IsPaused = false;
            proRaffle.Version = Guid.NewGuid();
        }
        return (isNewSetting, proRaffle);
    }
}
