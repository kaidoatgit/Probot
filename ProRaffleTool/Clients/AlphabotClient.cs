
using Newtonsoft.Json;
using Probot.ProRaffleTool.Clients.Dtos.Request;
using Probot.ProRaffleTool.Clients.Dtos.Response;
using Probot.ProRaffleTool.Models.Enums;
using Probot.ProRaffleTool.Helpers;
using System.Net;

namespace Probot.ProRaffleTool.Clients;

public class AlphabotClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<AlphabotClient> _logger;

    public AlphabotClient(HttpClient httpClient, ILogger<AlphabotClient> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }
    public async Task<RegisterInRaffleResponse> RegisterInRaffleAsync(string apiKey, ulong userId, string slug)
    {
        RegisterInRaffleResponse clientResponse = new();
        HttpStatusCode? httpStatusCode = HttpStatusCode.Accepted;
        string messageResult = string.Empty;
        try
        {
            using var request = new HttpRequestMessage(HttpMethod.Post, "register");
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", apiKey);
            request.Content = JsonContent.Create(new RegisterInRaffleRequest { Slug = slug });
            HttpResponseMessage response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();

            string result = await response.Content.ReadAsStringAsync();
            clientResponse = JsonConvert.DeserializeObject<RegisterInRaffleResponse>(result)!;

            httpStatusCode = HttpStatusCode.OK;
            messageResult = clientResponse.Data?.ResultMd ?? string.Empty;
        }
        catch (JsonException jsonException)
        {
            httpStatusCode = HttpStatusCode.BadRequest;
            messageResult = " <RegisterRaffle> " + jsonException.Message;
        }
        catch (HttpRequestException httpException)
        {
            httpStatusCode = httpException.StatusCode;
            messageResult = " <RegisterRaffle> " + httpException.Message;
        }
        catch
        {
            httpStatusCode = HttpStatusCode.InternalServerError;
            messageResult = " <RegisterRaffle> Internal error ";
        }
        _logger.LogWarning("{UserId} Http code: {Code} {Message}, Slug: {Slug}, Registration: {flag} {error}",
            userId, httpStatusCode, messageResult, slug, clientResponse.Success,
            clientResponse.Errors?.FirstOrDefault()?.Message);
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
            clientResponse = JsonConvert.DeserializeObject<RaffleResponse>(result)!;
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

    public async Task<IEnumerable<RaffleDetail>> GetRafflesAsync(string apiKey, RaffleType raffleType)
    {
        HttpStatusCode? statusCode = HttpStatusCode.Accepted;
        string messageResult = string.Empty;
        try
        {
            using var request = new HttpRequestMessage(HttpMethod.Get, RaffleUrlBuilder.GetUrl(raffleType));
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", apiKey);

            HttpResponseMessage response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();

            RafflesResponse? result = await response.Content.ReadFromJsonAsync<RafflesResponse>();
            if (result?.Success == true && result?.Data?.Raffles != null)
            {
                return result.Data.Raffles;
            }
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
        return Enumerable.Empty<RaffleDetail>();
    }
}