using System.Text.Json.Serialization;

namespace Probot.ProRaffleTool.Dtos.Alphabot.Request;

public class RaffleRequest
{
    [JsonPropertyName("event")]
    public string? Event { get; set; }

    [JsonPropertyName("timestamp")]
    public long Timestamp { get; set; }

    [JsonPropertyName("hash")]
    public string? Hash { get; set; }

    [JsonPropertyName("data")]
    public Data? Data { get; set; }
}

public class Connection
{
    [JsonPropertyName("id")]
    public string? Id { get; set; }

    [JsonPropertyName("image")]
    public string? Image { get; set; }

    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("provider")]
    public string? Provider { get; set; }
}

public class Data
{
    [JsonPropertyName("raffle")]
    public Raffle? Raffle { get; set; }

    [JsonPropertyName("user")]
    public User? User { get; set; }
}

public class Raffle
{
    [JsonPropertyName("bannerImageUrl")]
    public string? BannerImageUrl { get; set; }

    [JsonPropertyName("blockchain")]
    public string? Blockchain { get; set; }

    [JsonPropertyName("connectCaptcha")]
    public bool? ConnectCaptcha { get; set; }

    [JsonPropertyName("connectDiscord")]
    public bool? ConnectDiscord { get; set; }

    [JsonPropertyName("connectEmail")]
    public bool? ConnectEmail { get; set; }

    [JsonPropertyName("connectInstagram")]
    public bool? ConnectInstagram { get; set; }

    [JsonPropertyName("connectPassword")]
    public bool? ConnectPassword { get; set; }

    [JsonPropertyName("connectTelegram")]
    public bool? ConnectTelegram { get; set; }

    [JsonPropertyName("connectTwitter")]
    public bool? ConnectTwitter { get; set; }

    [JsonPropertyName("connectWallet")]
    public bool? ConnectWallet { get; set; }

    [JsonPropertyName("description")]
    public string? Description { get; set; }

    [JsonPropertyName("discordServerRoles")]
    public List<DiscordServerRole>? DiscordServerRoles { get; set; }

    [JsonPropertyName("discordUrl")]
    public string? DiscordUrl { get; set; }

    [JsonPropertyName("endDate")]
    public long? EndDate { get; set; }

    [JsonPropertyName("excludePreviousWinners")]
    public bool? ExcludePreviousWinners { get; set; }

    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("reqString")]
    public string? ReqString { get; set; }

    [JsonPropertyName("requirePremium")]
    public bool? RequirePremium { get; set; }

    [JsonPropertyName("requiredEth")]
    public int? RequiredEth { get; set; }

    [JsonPropertyName("requiredTokens")]
    public List<object>? RequiredTokens { get; set; }

    [JsonPropertyName("signWallet")]
    public bool? SignWallet { get; set; }

    [JsonPropertyName("slug")]
    public string Slug { get; set; }

    [JsonPropertyName("startDate")]
    public long? StartDate { get; set; }

    [JsonPropertyName("status")]
    public string? Status { get; set; }

    [JsonPropertyName("telegramChatRequirements")]
    public List<object>? TelegramChatRequirements { get; set; }

    [JsonPropertyName("twitterFollows")]
    public List<TwitterFollow>? TwitterFollows { get; set; }

    [JsonPropertyName("twitterRetweet")]
    public string? TwitterRetweet { get; set; }

    [JsonPropertyName("twitterRetweetType")]
    public string? TwitterRetweetType { get; set; }

    [JsonPropertyName("twitterUrl")]
    public string? TwitterUrl { get; set; }

    [JsonPropertyName("type")]
    public string? Type { get; set; }

    [JsonPropertyName("visibility")]
    public string? Visibility { get; set; }

    [JsonPropertyName("winnerCount")]
    public int? WinnerCount { get; set; }

    [JsonPropertyName("entryCount")]
    public int? EntryCount { get; set; }

    [JsonPropertyName("_id")]
    public string? _id { get; set; }

    [JsonPropertyName("teamId")]
    public string? TeamId { get; set; }

    [JsonPropertyName("projectId")]
    public string? ProjectId { get; set; }
}


public class DiscordServerRole
{

    [JsonPropertyName("id")]
    public string? Id { get; set; }

    [JsonPropertyName("label")]
    public string? Label { get; set; }

    [JsonPropertyName("icon")]
    public string? Icon { get; set; }

    [JsonPropertyName("inviteLink")]
    public string? InviteLink { get; set; }

    [JsonPropertyName("hasAlphabot")]
    public bool? HasAlphabot { get; set; }

    [JsonPropertyName("roles")]
    public List<Role>? Roles { get; set; }
}


public class Role
{

    [JsonPropertyName("roleId")]
    public string? RoleId { get; set; }

    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("val")]
    public int? Val { get; set; }

    [JsonPropertyName("stacking")]
    public bool? Stacking { get; set; }
}

public class TwitterFollow
{

    [JsonPropertyName("_id")]
    public string? _id { get; set; }

    [JsonPropertyName("id")]
    public string? Id { get; set; }

    [JsonPropertyName("banner")]
    public string? Banner { get; set; }

    [JsonPropertyName("description")]
    public string? Description { get; set; }

    [JsonPropertyName("followerCount")]
    public int? FollowerCount { get; set; }

    [JsonPropertyName("image")]
    public string? Image { get; set; }

    [JsonPropertyName("inserted")]
    public long? Inserted { get; set; }

    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("updated")]
    public long? Updated { get; set; }
}

public class User
{
    [JsonPropertyName("_id")]
    public string? _id { get; set; }

    [JsonPropertyName("address")]
    public string? Address { get; set; }

    [JsonPropertyName("connections")]
    public List<Connection>? Connections { get; set; }
}