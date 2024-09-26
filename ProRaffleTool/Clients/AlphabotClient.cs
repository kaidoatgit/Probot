using Probot.Data.Entities;
using Probot.ProRaffleTool.Clients.Abstractions;
using Probot.ProRaffleTool.Clients.Dtos.Alphabot.Request;
using Probot.ProRaffleTool.Clients.Dtos.Alphabot.Response;
using Probot.ProRaffleTool.Clients.Helpers;
using Probot.ProRaffleTool.Models;
using Probot.ProRaffleTool.Models.Enums;
using Probot.Shared.Helpers;
using System.Net;
using System.Text;
using System.Text.Json;

namespace Probot.ProRaffleTool.Clients;

public class AlphabotClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<AlphabotClient> _logger;
    private readonly IDiscordClient _discordClient;
    private static string[] reconnectX = { "reconnect", "X account" };
    private static string[] reconnectAlphabot = { "re-connect", "Twitter", "Alphabot" };


    public AlphabotClient(HttpClient httpClient, ILogger<AlphabotClient> logger, IDiscordClient discordClient)
    {
        _httpClient = httpClient;
        _logger = logger;
        _discordClient = discordClient;
    }
    public async Task<RegisterInRaffleResponse> RegisterInRaffleAsync(ProRaffleSetting setting, /*string apiKey, string username,*/ string slug)
    {
        RegisterInRaffleResponse clientResponse = new();
        HttpStatusCode? httpStatusCode = HttpStatusCode.Accepted;
        string messageResult = string.Empty;
        try
        {
            using var request = new HttpRequestMessage(HttpMethod.Post, "register");
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", setting.Key);
            request.Content = JsonContent.Create(new RegisterInRaffleRequest { Slug = slug });
            HttpResponseMessage response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();

            string result = await response.Content.ReadAsStringAsync();
            clientResponse = JsonSerializer.Deserialize<RegisterInRaffleResponse>(result)!;

            httpStatusCode = HttpStatusCode.OK;
            messageResult = clientResponse.Data?.ResultMd ?? string.Empty;
        }
        catch (Exception exception)
        {
            if(exception is JsonException jsonException)
            {                
                httpStatusCode = HttpStatusCode.BadRequest;
                messageResult = " <RegisterRaffle> " + jsonException.Message;
            }
            else if(exception is HttpRequestException httpException)
            {
                httpStatusCode = httpException.StatusCode;
                messageResult = " <RegisterRaffle> " + httpException.Message;
            }
            else
            {
                httpStatusCode = HttpStatusCode.InternalServerError;
                messageResult = " <RegisterRaffle> Internal error ";
            }
            
            _logger.LogError("{Username} Http code: {Code} {Message}, Slug: {Slug}", setting.Username, httpStatusCode, messageResult, slug);
        }
        
        var description = new StringBuilder();
        if(clientResponse.Success && setting.IsRegisteredAlertEnabled)
        {
            description.AppendLine($"{EmojisHelper.WhiteCheckMark} **Raffle**");
            description.AppendLine($"```{slug}```");
            var message = new NotificationMessage 
            {
                Description = description.ToString(),
                Color = 65280,
                Username = setting.Username
            };
            _ = _discordClient.ExecuteWebhookAsync(setting.RegisterAlertId, setting.RegisterAlertToken, message);
        }
        else if(setting.IsErrorAlertEnabled)
        {
            description.AppendLine($"{EmojisHelper.X} **Raffle**");
            description.AppendLine($"```{slug}```");
            description.AppendLine($"**Reason**:");
            string resultMd = clientResponse.Data?.ResultMd ?? "It was not possible to enter in the raffle.";
            description.AppendLine($"```{resultMd}```");
            
            bool isToReconnectX = reconnectX.All(phrase => resultMd.Contains(phrase, StringComparison.OrdinalIgnoreCase));
            bool isToReconnectAlphabot = reconnectAlphabot.All(phrase => resultMd.Contains(phrase, StringComparison.OrdinalIgnoreCase));
            var message = new NotificationMessage 
            {
                Description = description.ToString(),
                Color = 16711680,
                UserId = setting.UserId,
                Username = setting.Username,
                IsMentionable = isToReconnectX || isToReconnectAlphabot,
            };
            _ = _discordClient.ExecuteWebhookAsync(setting.ErrorAlertId, setting.ErrorAlertToken, message);
        }

        _logger.LogInformation("{Username} Http code: {Code} {Message}, Slug: {Slug}, Registration: {flag} {error}",
            setting.Username, httpStatusCode, messageResult, slug, clientResponse.Success, clientResponse.Errors?.FirstOrDefault()?.Message);
        return clientResponse;
    }

    public async Task<RaffleResponse> GetRaffleAsync(string slug, string apiKey)
    {
        RaffleResponse clientResponse = new();
        HttpStatusCode? httpStatusCode = HttpStatusCode.Accepted;
        string messageResult = string.Empty;
        try
        {
            using var request = new HttpRequestMessage(HttpMethod.Get, $"raffles/{slug}");
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", apiKey);
            HttpResponseMessage response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();

            string result = await response.Content.ReadAsStringAsync();
            clientResponse = JsonSerializer.Deserialize<RaffleResponse>(result)!;
        }
        catch (HttpRequestException httpException)
        {
            httpStatusCode = httpException.StatusCode;
            messageResult = " <GetRafflesList> " + httpException.Message;
        }
        catch
        {
            httpStatusCode = HttpStatusCode.InternalServerError;
            messageResult = " <GetRafflesList> Internal error ";
        }
        _logger.LogWarning("Http code: {Code} {Message}, Slug: {Slug}, Registration: {flag} {error}",
            httpStatusCode, messageResult, slug, clientResponse.Success,
            clientResponse.Errors?.FirstOrDefault()?.Message);
        return clientResponse;
    }

    public async Task<RafflesResponse> GetRafflesAsync(string apiKey, RaffleType raffleType, int pageNum = 0)
    {
        RafflesResponse clientResponse = new();
        HttpStatusCode? statusCode = HttpStatusCode.Accepted;
        string messageResult = string.Empty;
        try
        {
            using var request = new HttpRequestMessage(HttpMethod.Get, RaffleUrlBuilder.GetUrl(raffleType, pageNum));
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", apiKey);

            HttpResponseMessage response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();
            
            string result = await response.Content.ReadAsStringAsync();
            clientResponse = JsonSerializer.Deserialize<RafflesResponse>(result)!;
        }
        catch (HttpRequestException httpException)
        {
            statusCode = httpException.StatusCode;
            messageResult = " <GetRafflesList> " + httpException.Message;
        }
        catch
        {
            statusCode = HttpStatusCode.InternalServerError;
            messageResult = " <GetRafflesList> Internal error ";
        }
        return clientResponse;
    }
}