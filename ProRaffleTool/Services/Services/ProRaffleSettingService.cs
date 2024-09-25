using Microsoft.EntityFrameworkCore;
using Probot.Data;
using Probot.Data.Entities;
using Probot.ProRaffleTool.Clients.Abstractions;
using Probot.ProRaffleTool.Clients.Dtos.Discord.Response;
using Probot.ProRaffleTool.Exceptions;
using Probot.ProRaffleTool.Models;
using Probot.ProRaffleTool.Services.Services.Abstractions;
using Probot.Shared.Dtos.ProRaffleSetting.Request;
using Probot.Shared.Enums;

namespace Probot.ProRaffleTool.Services.Services;

public class ProRaffleSettingService : IProRaffleSettingService
{    
    private readonly ProbotContext _context;
    private readonly IDiscordClient _discordClient;
    public ProRaffleSettingService(ProbotContext context, IDiscordClient discordClient)
    {
        _context = context;
        _discordClient = discordClient;
    }

    public async Task UpdateProRaffleSettingKeyAsync(ulong userId, UpdatePRSettingKeyRequest request)
    {
        bool isKeyInUse = await _context.ProRaffleSettings
            .AnyAsync(pr => pr.Key == request.NewKey);
        if (isKeyInUse) throw new ProRaffleException(ExceptionResult.ProRaffleSettingConflict409, $"New Key: {request.NewKey} already in use.");

        var proRaffleSetting = await _context.ProRaffleSettings
            .SingleOrDefaultAsync(pr => pr.UserId == userId && pr.Key == request.CurrentKey)
            ?? throw new ProRaffleException(ExceptionResult.ProRaffleSettingNotFound404, $"The current key: {request.CurrentKey} not found or does not belong to the user: {userId}");

        proRaffleSetting.Key = request.NewKey;
        await _context.SaveChangesAsync();
    }
    
    public async Task<IEnumerable<ProRaffleSetting>> GetSettingsAsync(ulong? userId, bool? isPaused)
    {  
        IQueryable<ProRaffleSetting> query = _context.ProRaffleSettings
            .AsNoTracking();
            
        if(userId.HasValue)
        {
            query = query.Where(prs => prs.UserId == userId);
        }
        
        if(isPaused.HasValue)
        {
            query = query.Where(prs => prs.IsPaused == isPaused);
        }

        return await query.ToListAsync();
    }

    public async Task<ProRaffleSetting?> GetSettingAsync(ulong settingId, bool dbTracking = false)
    { 
        IQueryable<ProRaffleSetting> query = _context.ProRaffleSettings;

        if(!dbTracking)
        {
            query = query.AsNoTracking();
        }

        var proRaffleSetting = await query.SingleOrDefaultAsync(prs => prs.Id == settingId);
        return proRaffleSetting;
    }
    
    public async Task EnableAlertAsync(OAuthInteractionData interactionData, WebhookResponse webhookResponse)
    {
        var setting = (await GetSettingAsync(interactionData.SettingId, dbTracking: true))!;
        
        string oldWebhookId;
        string oldWebhookToken;
        if (interactionData.RaffleAlertType == RaffleAlertType.Registered)
        {
            oldWebhookId = setting.RegisterAlertId;
            oldWebhookToken = setting.RegisterAlertToken;

            setting.RegisterAlertId = webhookResponse.Id;
            setting.RegisterAlertToken = webhookResponse.Token;
            setting.IsRegisteredAlertEnabled = true;
        }
        else
        {
            oldWebhookId = setting.ErrorAlertId;
            oldWebhookToken = setting.ErrorAlertToken;

            setting.ErrorAlertId =  webhookResponse.Id;
            setting.ErrorAlertToken = webhookResponse.Token;
            setting.IsErrorAlertEnabled = true;
        }
        await _context.SaveChangesAsync();
        await _discordClient.DeleteWebhookAsync(oldWebhookId, oldWebhookToken);
    }

    public async Task<bool> DisableAlertAsync(ulong settingId, RaffleAlertType raffleAlertType)
    {
        var setting = (await GetSettingAsync(settingId, dbTracking: true))!;

        string webhookId;
        string webhookToken;
        if (raffleAlertType == RaffleAlertType.Registered)
        {
            if(!setting.IsRegisteredAlertEnabled)
            {
                return false;
            }

            webhookId = setting.RegisterAlertId;
            webhookToken = setting.RegisterAlertToken;
            setting.IsRegisteredAlertEnabled = false;
        }
        else
        {
            if(!setting.IsErrorAlertEnabled)
            {
                return false;
            }

            webhookId = setting.ErrorAlertId;
            webhookToken = setting.ErrorAlertToken;
            setting.IsErrorAlertEnabled = false;
        }
        await _context.SaveChangesAsync();
        await _discordClient.DeleteWebhookAsync(webhookId, webhookToken);
        return true;
    }
}

