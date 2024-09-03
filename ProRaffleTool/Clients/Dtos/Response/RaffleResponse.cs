using System.Text.Json.Serialization;

namespace Probot.ProRaffleTool.Clients.Dtos.Response;

public class RaffleResponse
{
    [JsonPropertyName("success")]
    public bool Success { get; set; }

    [JsonPropertyName("data")]
    public RaffleData? Data { get; set; }

    [JsonPropertyName("errors")]
    public List<Error>? Errors { get; set; }
}

public class RaffleData
{
    [JsonPropertyName("raffle")]
    public RaffleDetail? Raffle { get; set; }

    [JsonPropertyName("entry")]
    public Entry? Entry { get; set; }

    [JsonPropertyName("entries")]
    public List<Entry> Entries { get; set; } = new();
}

public class RaffleDetail
{
    [JsonPropertyName("_id")]
    public string? _id { get; set; }

    [JsonPropertyName("slug")]
    public string? Slug { get; set; }

    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("type")]
    public string? Type { get; set; }

    [JsonPropertyName("status")]
    public string? Status { get; set; } 

    [JsonPropertyName("visibility")]
    public string? Visibility { get; set; } 

    [JsonPropertyName("description")]
    public string? Description { get; set; }

    [JsonPropertyName("startDate")]
    public long? StartDate { get; set; }

    [JsonPropertyName("endDate")]
    public long? EndDate { get; set; }

    [JsonPropertyName("winnerCount")]
    public int? WinnerCount { get; set; }

    [JsonPropertyName("bannerImageUrl")]
    public string? BannerImageUrl { get; set; }

    [JsonPropertyName("blockchain")]
    public string? Blockchain { get; set; }

    [JsonPropertyName("twitterUrl")]
    public string? TwitterUrl { get; set; }

    [JsonPropertyName("discordUrl")]
    public string? DiscordUrl { get; set; }

    [JsonPropertyName("entryCount")]
    public int? EntryCount { get; set; }

    [JsonPropertyName("reqString")]
    public string? ReqString { get; set; }

    [JsonPropertyName("projectId")]
    public string? ProjectId { get; set; }

    [JsonPropertyName("teamId")]
    public string? TeamId { get; set; }

    [JsonPropertyName("dtc")]
    public bool? Dtc { get; set; }
}

public class Entry
{
    [JsonPropertyName("userId")]
    public string? UserId { get; set; }

    [JsonPropertyName("slug")]
    public string? Slug { get; set; }

    [JsonPropertyName("mintAddress")]
    public string? MintAddress { get; set; }

    [JsonPropertyName("email")]
    public string? Email { get; set; }

    [JsonPropertyName("discordId")]
    public string? DiscordId { get; set; }

    [JsonPropertyName("discordName")]
    public string? DiscordName { get; set; }

    [JsonPropertyName("twitterId")]
    public string? TwitterId { get; set; }

    [JsonPropertyName("twitterName")]
    public string? TwitterName { get; set; }

    [JsonPropertyName("telegramId")]
    public string? TelegramId { get; set; }

    [JsonPropertyName("telegramName")]
    public string? TelegramName { get; set; }

    [JsonPropertyName("image")]
    public string? Image { get; set; }

    [JsonPropertyName("createdDate")]
    public long? CreatedDate { get; set; }

    [JsonPropertyName("entries")]
    public int? Entries { get; set; }

    [JsonPropertyName("tokensEntries")]
    public int? TokensEntries { get; set; }

    [JsonPropertyName("discordEntries")]
    public int? DiscordEntries { get; set; }

    [JsonPropertyName("source")]
    public string? Source { get; set; }

    [JsonPropertyName("lossMultipliedEntries")]
    public int? LossMultipliedEntries { get; set; }

    [JsonPropertyName("answers")]
    public Answers? Answers { get; set; }

    [JsonPropertyName("applicationCode")]
    public string? ApplicationCode { get; set; }

    [JsonPropertyName("approved")]
    public bool? Approved { get; set; }

    [JsonPropertyName("approvedOn")]
    public long? ApprovedOn { get; set; }

    [JsonPropertyName("approvedBy")]
    public string? ApprovedBy { get; set; }

    [JsonPropertyName("winnerImage")]
    public string? WinnerImage { get; set; }

    [JsonPropertyName("winner")]
    public bool? Winner { get; set; }

    [JsonPropertyName("premium")]
    public bool? Premium { get; set; }

    [JsonPropertyName("extraEntry")]
    public bool? ExtraEntry { get; set; }

    [JsonPropertyName("picked")]
    public long? Picked { get; set; }

    [JsonPropertyName("raffleId")]
    public string? RaffleId { get; set; }

    [JsonPropertyName("projectId")]
    public string? ProjectId { get; set; }

    [JsonPropertyName("teamId")]
    public string? TeamId { get; set; }
}

public class Answers
{
    [JsonPropertyName("questionId")]
    public int? QuestionId { get; set; }

    [JsonPropertyName("answer")]
    public string? Answer { get; set; }
}
