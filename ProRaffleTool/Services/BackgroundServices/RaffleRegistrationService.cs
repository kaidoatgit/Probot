using System.Collections.Concurrent;
using System.Text;
using Microsoft.Extensions.Options;
using Probot.Data.Entities;
using Probot.ProRaffleTool.Clients;
using Probot.ProRaffleTool.Clients.Dtos.Alphabot.Response;
using Probot.ProRaffleTool.Models;
using Probot.ProRaffleTool.Models.Enums;
using Probot.ProRaffleTool.Options;
using Probot.ProRaffleTool.Services.Services.Abstractions;

namespace Probot.ProRaffleTool.Services.BackgroundServices;

public class RaffleRegistrationService: BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<RaffleRegistrationService> _logger;
    private readonly AlphabotClient _alphabotClient;
    private Timer? _timer;
    private readonly IRaffleRateLimiterService _raffleRateLimiterService;
    private readonly ConcurrentDictionary<string, RaffleMetrics> _raffleMetrics = new();
    private BackgroundServicesSettings _settings;

    public RaffleRegistrationService(IServiceProvider serviceProvider, ILogger<RaffleRegistrationService> logger, AlphabotClient alphabotClient, 
        IRaffleRateLimiterService raffleRateLimiterService, IOptionsMonitor<BackgroundServicesSettings> servicesOptions)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
        _alphabotClient = alphabotClient;
        _raffleRateLimiterService = raffleRateLimiterService;

        servicesOptions.OnChange(updatedSettings => 
        {
            _settings = updatedSettings;
            Console.WriteLine($"Is service activated?: {_settings.RunRafflesRegistrations}");
        });
        _settings = servicesOptions.CurrentValue;
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
        if(!_settings.RunRafflesRegistrations)
        {
            Console.WriteLine("Service is not active");
            return;
        }
        _logger.LogInformation($"<RAFFLES BACKGROUND SERVICE> Registering old raffles <RAFFLES BACKGROUND SERVICE>");

        using var scope = _serviceProvider.CreateScope();
        var proRaffleSettingService = scope.ServiceProvider.GetRequiredService<IProRaffleSettingService>();
        var prSettings = await proRaffleSettingService.GetSettingsAsync(isPaused: false);
        
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

    private async Task RegisterRaffles(ProRaffleSetting prSetting)
    {
        try
        {   
            var raffleMetrics = _raffleMetrics.GetOrAdd(prSetting.Username, _ => new RaffleMetrics());
            raffleMetrics.TotalRegistered = 0;

            List<RaffleDetail> raffles = new();
            var twitterRafflesResponse = await _alphabotClient.GetRafflesAsync(prSetting.Key, RaffleType.TWITTER_RAFFLES);
            if(twitterRafflesResponse.Success && twitterRafflesResponse.Data != null)
            {
                raffles.AddRange(twitterRafflesResponse.Data.Raffles
                        .Where(raffle => !string.Equals(raffle.Type, "application", StringComparison.OrdinalIgnoreCase)));
            }
            raffleMetrics.TwitterRafflesCount = raffles.Count;

            for(int i=0; i<1; i++)
            {
                var communityRafflesResponse = await _alphabotClient.GetRafflesAsync(prSetting.Key, RaffleType.COMMUNITY_RAFFLES, i);
                if (!communityRafflesResponse.Success || communityRafflesResponse.Data == null)
                {
                    break;
                }
                
                raffles.AddRange(communityRafflesResponse.Data.Raffles
                        .Where(raffle => !string.Equals(raffle.Type, "application", StringComparison.OrdinalIgnoreCase)));

                if(communityRafflesResponse.Data.FinalPage)
                {
                    break;
                }
                await Task.Delay(2000);
            }
            raffleMetrics.CommunityRafflesCount = raffles.Count - raffleMetrics.TwitterRafflesCount;
            
            if (raffles.Count == 0)
            {
                return;
            }
                
            var raffleRateLimiter = _raffleRateLimiterService.GetOrAdd(prSetting.Username);

            var registrationTasks = raffles.Select(raffle => RegisterRaffleWithThrottle(raffle, prSetting, raffleRateLimiter, raffleMetrics));
            await Task.WhenAll(registrationTasks);
        }
        catch
        {
        }
    }

    private async Task<RegisterInRaffleResponse> RegisterRaffleWithThrottle(RaffleDetail raffle, ProRaffleSetting prSetting, RaffleRateLimiter rateLimiter, RaffleMetrics raffleMetrics)
    {
        await rateLimiter.WaitAsync();
        RegisterInRaffleResponse clientResponse = new();
        try
        {
            clientResponse = await _alphabotClient.RegisterInRaffleAsync(prSetting, raffle.Slug!);
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