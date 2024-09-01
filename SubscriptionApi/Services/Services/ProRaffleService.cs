using Microsoft.EntityFrameworkCore;
using Probot.Data;
using Probot.Data.Entities;
using Probot.SubscriptionApi.Exceptions;
using Probot.SubscriptionApi.Services.Services.IServices;
using Probot.Shared.Dtos.ProRaffle.Request;

namespace Probot.SubscriptionApi.Services.Services;

public class ProRaffleService : IProRaffleService
{    
    private readonly ProbotContext _context;
    public ProRaffleService(ProbotContext context)
    {
        _context = context;
    }

    public async Task UpdateProRaffleKeyAsync(ulong userId, UpdateProRaffleKeyRequest request)
    {
        bool isKeyInUse = await _context.ProRaffles
            .AnyAsync(pr => pr.Key == request.NewKey);
        if (isKeyInUse) throw new ServiceException(StatusCodes.Status409Conflict, $"New Key: {request.NewKey} already in use.");

        var proRaffle = await _context.ProRaffles
            .SingleOrDefaultAsync(pr => pr.UserId == userId && pr.Key == request.CurrentKey)
            ?? throw new ServiceException(StatusCodes.Status404NotFound, $"The current key: {request.CurrentKey} not found or does not belong to the user: {userId}");

        proRaffle.Key = request.NewKey;
        await _context.SaveChangesAsync();
    }

    public async Task<(bool IsNewSetting, ProRaffle ProRaffle)> GetOrCreateSettingsAsync(ProRaffleRequest request)
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
