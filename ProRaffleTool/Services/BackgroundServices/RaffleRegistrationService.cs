using System.Collections.Concurrent;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Probot.Data;
using Probot.Data.Entities;
using Probot.ProRaffleTool.Clients;
using Probot.ProRaffleTool.Clients.Dtos.Response;
using Probot.ProRaffleTool.Models.Enums;
using Probot.ProRaffleTool.Services.BackgroundServices.Models;

namespace Probot.ProRaffleTool.Services.BackgroundServices;

public class RaffleRegistrationService: BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<RaffleRegistrationService> _logger;
    private readonly AlphabotClient _alphabotClient;
    private Timer? _timer;
    private readonly ConcurrentDictionary<string, RateLimiter> _rateLimiters = new();
    private readonly ConcurrentDictionary<string, RaffleMetrics> _raffleMetrics = new();

    public RaffleRegistrationService(IServiceProvider serviceProvider, ILogger<RaffleRegistrationService> logger, AlphabotClient alphabotClient)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
        _alphabotClient = alphabotClient;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _timer = 
            new Timer(
                async state => await RegisterOldRafflesAsync(),
                null,
                TimeSpan.FromSeconds(30),
                TimeSpan.FromHours(3));
        
        return Task.CompletedTask;
    }

    public override async Task StopAsync(CancellationToken stoppingToken)
    {
        // Stop the timer
        _timer?.Change(Timeout.Infinite, 0);
        await base.StopAsync(stoppingToken);
    }

    private async Task RegisterOldRafflesAsync()
    {
        using var scope = _serviceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ProbotContext>();
        
        _logger.LogInformation($"<RAFFLES BACKGROUND SERVICE> Registering old raffles <RAFFLES BACKGROUND SERVICE>");
        var currentTime = DateTime.Now;
        var prSettings = await dbContext.ProRaffleSettings
                .Where(pr => !pr.IsPaused)
                .ToListAsync();
        
        var tasks = new List<Task>();
        foreach (var prSetting in prSettings)
        {
            tasks.Add(RegisterRaffles(prSetting));
        }
        await Task.WhenAll(tasks);

        string formattedDateTime = DateTime.Now.ToString("yyyy-MM-dd:HH:mm:ss");
        _logger.LogInformation("<RAFFLES BACKGROUND SERVICE> Old raffles registered at {CurrentTime} <RAFFLES BACKGROUND SERVICE>", formattedDateTime);
        _logger.LogInformation("Metrics Result:\n{Metrics}", LogMetricsResult());
    }

    public async Task RegisterRaffles(ProRaffleSetting prSetting)
    {
        try
        {
            var twitterRaffles = await _alphabotClient.GetRafflesAsync(prSetting.Key, RaffleType.TWITTER_RAFFLES);
            twitterRaffles = twitterRaffles.Where(raffle => !string.Equals(raffle.Type, "application", StringComparison.OrdinalIgnoreCase));

            var communityRaffles = await _alphabotClient.GetRafflesAsync(prSetting.Key, RaffleType.COMMUNITY_RAFFLES);
            communityRaffles = communityRaffles.Where(raffle => !string.Equals(raffle.Type, "application", StringComparison.OrdinalIgnoreCase));
       
            var raffles = twitterRaffles.Concat(communityRaffles)
                .Where(raffle => !string.Equals(raffle.Type, "application", StringComparison.OrdinalIgnoreCase))
                .ToList();
            if (raffles.Count == 0)
            {
                return;
            }
            
            var raffleMetrics = _raffleMetrics.GetOrAdd(prSetting.Username, _ => new RaffleMetrics());
            raffleMetrics.UpdateCount(twitterRaffles.Count(), communityRaffles.Count());
                
            var rateLimiter = _rateLimiters.GetOrAdd(prSetting.Username, _ => new RateLimiter());

            var registrationTasks = raffles.Select(raffle => RegisterRaffleWithThrottle(raffle, prSetting, rateLimiter, raffleMetrics));
            await Task.WhenAll(registrationTasks);
        }
        catch
        {
        }
    }

    private async Task<RegisterInRaffleResponse> RegisterRaffleWithThrottle(RaffleDetail raffle, ProRaffleSetting prSetting, RateLimiter rateLimiter, RaffleMetrics raffleMetrics)
    {
        await rateLimiter.WaitAsync();
        RegisterInRaffleResponse clientResponse = new();
        try
        {
            clientResponse = await _alphabotClient.RegisterInRaffleAsync(prSetting.Key, prSetting.Username, raffle.Slug!);
            if(clientResponse.Success)
            {
                raffleMetrics.TotalRegistered++;
            }
        }
        catch (Exception e)
        {
            _logger.LogError(" <REGISTER RAFFLE WITH THROTTLE> " + e.Message);
        }
        return clientResponse;
    }
    

    private string LogMetricsResult()
    {
        StringBuilder sb = new();
        foreach (var (apiKey, metrics) in _raffleMetrics)
        {
            sb.AppendLine($"Id: {apiKey}");
            sb.Append($"  Twitter Raffles: {metrics.TwitterRafflesCount}");
            sb.Append($", Community Raffles: {metrics.CommunityRafflesCount}");
            sb.Append($", Sum: {metrics.TotalRafflesCount}");
            sb.AppendLine($", Registered: {metrics.TotalRegistered}");            
        }
        return sb.ToString();
    }
}