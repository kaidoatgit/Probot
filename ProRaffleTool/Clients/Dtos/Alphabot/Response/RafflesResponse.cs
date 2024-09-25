using System.Text.Json.Serialization;

namespace Probot.ProRaffleTool.Clients.Dtos.Alphabot.Response;

public class RafflesResponse
{
    [JsonPropertyName("success")]
    public bool Success { get; set; }

    [JsonPropertyName("data")]
    public RafflesData? Data { get; set; }
}

public class RafflesData
{
    [JsonPropertyName("raffles")]
    public IEnumerable<RaffleDetail> Raffles { get; set; } = new List<RaffleDetail>();

    [JsonPropertyName("finalPage")]
    public bool FinalPage { get; set; }
}