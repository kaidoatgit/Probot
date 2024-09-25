using System.Text.Json.Serialization;

namespace Probot.ProRaffleTool.Clients.Dtos.Alphabot.Response;

public class RegisterInRaffleResponse
{
    [JsonPropertyName("success")]
    public bool Success { get; set; }

    [JsonPropertyName("data")]
    public Data? Data { get; set; }

    [JsonPropertyName("errors")]
    public List<Error>? Errors { get; set; }
}

public class Data
{
    [JsonPropertyName("resultMd")]
    public string? ResultMd { get; set; }

    [JsonPropertyName("validation")]
    public Validation? Validation { get; set; }

    [JsonPropertyName("pendingCheck")]
    public PendingCheck? PendingCheck { get; set; }
}

public class Validation
{
    [JsonPropertyName("entries")]
    public int? Entries { get; set; }
    
    [JsonPropertyName("discordRefresh")]
    public bool? DiscordRefresh { get; set; }

    [JsonPropertyName("discordValid")]
    public bool? DiscordValid { get; set; }

    [JsonPropertyName("twitterValid")]
    public bool? TwitterValid { get; set; }

    [JsonPropertyName("tokensValid")]
    public bool? TokensValid { get; set; }

    [JsonPropertyName("discordValidations")]
    public List<bool>? DiscordValidations { get; set; }

    [JsonPropertyName("twitterValidations")]
    public List<bool>? TwitterValidations { get; set; }

    [JsonPropertyName("tokensValidations")]
    public List<bool>? TokensValidations { get; set; }

    [JsonPropertyName("telegramValidations")]
    public List<bool>? TelegramValidations { get; set; }

    [JsonPropertyName("passwordInvalid")]
    public bool? PasswordInvalid { get; set; }

    [JsonPropertyName("ethBalanceValid")]
    public bool? EthBalanceValid { get; set; }

    [JsonPropertyName("questionsValid")]
    public bool? QuestionsValid { get; set; }

    [JsonPropertyName("questionValidations")]
    public List<bool>? QuestionValidations { get; set; }

    [JsonPropertyName("appCodeInvalid")]
    public bool? AppCodeInvalid { get; set; }

    [JsonPropertyName("emailValid")]
    public bool? EmailValid { get; set; }

    [JsonPropertyName("success")]
    public bool? Success { get; set; }

    [JsonPropertyName("reason")]
    public string? Reason { get; set; }
}

public class PendingCheck
{
    [JsonPropertyName("start")]
    public long? Start { get; set; }

    [JsonPropertyName("complete")]
    public long? Complete { get; set; }

    [JsonPropertyName("completed")]
    public Completed? Completed { get; set; }
}

public class Completed
{
    [JsonPropertyName("rt")]
    public long? Rt { get; set; }

    [JsonPropertyName("follow")]
    public long? Follow { get; set; }

    [JsonPropertyName("nft")]
    public long? Nft { get; set; }

    [JsonPropertyName("discord")]
    public long? Discord { get; set; }

    [JsonPropertyName("email")]
    public long? Email { get; set; }

    [JsonPropertyName("telegram")]
    public long? Telegram { get; set; }
}

public class Error
{
    [JsonPropertyName("message")]
    public string? Message { get; set; }
}
