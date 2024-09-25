using System.Text;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity.Extensions;
using DSharpPlus.SlashCommands;
using DSharpPlus.SlashCommands.Attributes;
using Probot.Client.Clients.ProRaffleApi;
using Probot.Client.Clients.SubscriptionApi;
using Probot.Client.Extensions;
using Probot.Client.Helpers;
using Probot.Client.Mappers;
using Probot.Client.Models;
using Probot.Shared.Dtos.ProRaffleSetting.Request;
using Probot.Shared.Dtos.Subscription.Request;
using Probot.Shared.Enums;
using Probot.Shared.Helpers;

namespace Probot.Client.Commands;
[SlashRequireGuild]
[SlashCommandGroup("pro-raffle", "Commands to manage Pro Raffle automation bot")]
public class ProRaffleCommands : ApplicationCommandModule
{
    private readonly Mapper _mapper;
    private readonly ProRaffleSettingClient _proRaffleSettingClient;
    private readonly SubscriptionClient _subscriptionClient;
    private readonly ProductKeyClient _productKeyClient;
    public ProRaffleCommands(Mapper mapper, ProRaffleSettingClient proRaffleClient, SubscriptionClient subscriptionClient, ProductKeyClient productKeyClient)
    {
        _mapper = mapper;
        _proRaffleSettingClient = proRaffleClient;
        _subscriptionClient = subscriptionClient;
        _productKeyClient = productKeyClient;
    }

    // [SlashCooldown(3, 60*5, SlashCooldownBucketType.User)]
    [SlashCommand("product-keys", "List all product keys and respective details.")]
    public async Task GetProductKeysCommand(InteractionContext ctx)
    {
        var embed = new DiscordEmbedBuilder
        {
            Description = $"{EmojisHelper.Warning} It appears that you currently have no active product keys. If you believe this is an error, please contact support.",
            Color = DiscordColor.Orange
        };
        try
        {
            var productKeyResponse = await _productKeyClient.GetProductKeysAsync(ctx.User.Id, isActivated: false);
            if (productKeyResponse.Data == null)
            {
                embed.Description = MessageHelper.GenericErrorMessage();
                embed.Color = DiscordColor.Red;
            }
            else if(productKeyResponse.Data.Any())
            {
                var inactivatedKeys = productKeyResponse.Data.Select(pk => _mapper.MapToProductKey(pk)).ToList();

                var description = new StringBuilder();
                foreach (var pk in inactivatedKeys)
                {
                    description.Append($"🎟️ Code | Duration: {pk.Period} Month{(pk.Period > 1 ? "s":"")}");
                    description.AppendLine($"```{pk.Code}```");
                }
                embed.Description = description.ToString();
                embed.Color = DiscordColor.Green;
            }

            var message = new DiscordMessageBuilder().WithEmbed(embed);
            await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder(message).AsEphemeral(true));
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[GetProductKeysCommand] {ex.Message}");
        }
    }


    [SlashCommand("new-subscription", "Use a product key to create a new subscription and associate it to Alphabot.")]
    public async Task NewSubscriptionCommand(InteractionContext ctx,
        [MaximumLength(15)]
        [MinimumLength(4)]
        [Option("username", "Unique identifier to associate with this activation")] string username,
        [Option("product-key", "Product key which you purchased")] string code,
        [Option("alphabot-key", "API key provided by Alphabot")] string key)
    {
        
        if (!code.IsProductKeyFormat() || !key.IsAlphabotKeyFormat())
        {
            await ctx.CreateResponseAsync(embed: new DiscordEmbedBuilder
            {
                Description = $"{EmojisHelper.Warning} Incorrect product-key or alphabot-key format.",
                Color = DiscordColor.Orange
            }, true);
            return;
        }

        try
        {
            string description = string.Empty;
            var productKeyResponse = await _productKeyClient.GetProductKeyAsync(code, ctx.User.Id, isActivated: false);
            if (productKeyResponse.Data == null)
            {
                if(productKeyResponse.ExceptionResult == ExceptionResult.ProductKeyNotFound404)
                {
                    description = $"{EmojisHelper.X} Product key not found or already activated";
                }
                else
                {
                    description = MessageHelper.GenericErrorMessage();
                }
                await ctx.CreateResponseAsync(embed: new DiscordEmbedBuilder
                {
                    Description = description,
                    Color = DiscordColor.Red
                }, true);
                return;
            }

            ProductKey productKey = _mapper.MapToProductKey(productKeyResponse.Data);
            var noButton = new DiscordButtonComponent(ButtonStyle.Danger, "activatekey_no_btn", $"No {EmojisHelper.X}");
            var yesButton = new DiscordButtonComponent(ButtonStyle.Success, "activatekey_yes_btn", $"Yes {EmojisHelper.WhiteCheckMark}");
            var embed = EmbedHelper.CreateSubscriptionEmbed(username, productKey, key);
            var message = new DiscordMessageBuilder()
                .WithEmbed(embed)
                .AddComponents(noButton, yesButton);
            await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder(message).AsEphemeral(true));

            var interactivity = ctx.Client.GetInteractivity();
            var orginalMsg = await ctx.GetOriginalResponseAsync();
            var answer = await interactivity.WaitForButtonAsync(orginalMsg, ctx.User, TimeSpan.FromMinutes(2));
            if (answer.TimedOut)
            {
                await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent("You didn't respond in time! Operation cancelled."));
                return;
            }
            if (answer.Result.Id == "activatekey_yes_btn")
            {
                var subscriptionRequest = new SubscriptionRequest
                {
                    UserId = ctx.User.Id,
                    Code = productKey.Code,
                    ProductSettingRequest = new ProRaffleSettingRequest
                    {
                        Username = username,
                        AlphabotKey = key
                    }
                };
                var subscriptionResponse = await _subscriptionClient.CreateSubscriptionAsync(subscriptionRequest);
                if (subscriptionResponse.Data == null)
                {
                    if(subscriptionResponse.ExceptionResult == ExceptionResult.ProductKeyNotFound404)
                    {
                        description = $"{EmojisHelper.X} Product key not found or already activated";
                    }
                    else if(subscriptionResponse.ExceptionResult == ExceptionResult.ProductSettingConflict409)
                    {
                        description = $"{EmojisHelper.X} {subscriptionResponse.ErrorMessage}";
                    }
                    else
                    {
                        description = MessageHelper.GenericErrorMessage();
                    }
                    
                    embed = new DiscordEmbedBuilder
                    {
                        Description = description,
                        Color = DiscordColor.Red
                    };                        
                    await ctx.EditResponseAsync(new DiscordWebhookBuilder(new DiscordMessageBuilder().WithEmbed(embed)));
                }
                else
                {
                    Subscription subscription = _mapper.MapToSubscription(subscriptionResponse.Data);
                    embed = EmbedHelper.CreateSubscriptionResultEmbed(subscription);
                    await ctx.EditResponseAsync(new DiscordWebhookBuilder(new DiscordMessageBuilder().WithEmbed(embed)));
                }
            }
            else if (answer.Result.Id == "activatekey_no_btn")
            {
                await ctx.DeleteResponseAsync();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[NewSubscriptionCommand] {ex.Message}");
        }
    }

    
    [SlashCommand("extend-subscription", "Use a product key to extend an existing subscription")]
    public async Task ExtendSubscriptionCommand(InteractionContext ctx,
        [MaximumLength(15)]
        [MinimumLength(4)]
        [Option("username", "Subscription identifier")] string username,
        [Option("product-key", "Product key which you purchased")] string code)
    {
        if (!code.IsProductKeyFormat())
        {
            await ctx.CreateResponseAsync(embed: new DiscordEmbedBuilder
            {
                Description = $"{EmojisHelper.Warning} Incorrect product-key.",
                Color = DiscordColor.Orange
            }, true);
            return;
        }

        try
        {
            string description = string.Empty;
            var productKeyResponse = await _productKeyClient.GetProductKeyAsync(code, ctx.User.Id, isActivated: false);
            if (productKeyResponse.Data == null)
            {
                if(productKeyResponse.ExceptionResult == ExceptionResult.ProductKeyNotFound404)
                {
                    description = $"{EmojisHelper.X} Product key not found or already activated";
                }
                else
                {
                    description = MessageHelper.GenericErrorMessage();
                }
                await ctx.CreateResponseAsync(embed: new DiscordEmbedBuilder
                {
                    Description = description,
                    Color = DiscordColor.Red
                }, true);
                return;
            }

            ProductKey productKey = _mapper.MapToProductKey(productKeyResponse.Data);
            var noButton = new DiscordButtonComponent(ButtonStyle.Danger, "activatekey_no_btn", $"No {EmojisHelper.X}");
            var yesButton = new DiscordButtonComponent(ButtonStyle.Success, "activatekey_yes_btn", $"Yes {EmojisHelper.WhiteCheckMark}");
            var embed = EmbedHelper.CreateExtendSubscriptionEmbed(username, productKey);
            var message = new DiscordMessageBuilder()
                .WithEmbed(embed)
                .AddComponents(noButton, yesButton);
            await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder(message).AsEphemeral(true));

            var interactivity = ctx.Client.GetInteractivity();
            var orginalMsg = await ctx.GetOriginalResponseAsync();
            var answer = await interactivity.WaitForButtonAsync(orginalMsg, ctx.User, TimeSpan.FromMinutes(2));
            if (answer.TimedOut)
            {
                await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent("You didn't respond in time! Operation cancelled."));
                return;
            }
            if (answer.Result.Id == "activatekey_yes_btn")
            {
                var subscriptionRequest = new SubscriptionRequest
                {
                    UserId = ctx.User.Id,
                    Code = productKey.Code,
                    ProductSettingRequest = new ProRaffleSettingRequest
                    {
                        Username = username
                    }
                };
                var subscriptionResponse = await _subscriptionClient.ExtendSubscriptionAsync(subscriptionRequest);
                if (subscriptionResponse.Data == null)
                {
                    if(subscriptionResponse.ExceptionResult == ExceptionResult.ProductKeyNotFound404)
                    {
                        description = $"{EmojisHelper.X} Product key not found or already activated";
                    }
                    else if(subscriptionResponse.ExceptionResult == ExceptionResult.SubscriptionNotFound404)
                    {
                        description = $"{EmojisHelper.X} {subscriptionResponse.ErrorMessage}";
                    }
                    else
                    {
                        description = MessageHelper.GenericErrorMessage();
                    }
                    
                    embed = new DiscordEmbedBuilder
                    {
                        Description = description,
                        Color = DiscordColor.Red
                    };                        
                    await ctx.EditResponseAsync(new DiscordWebhookBuilder(new DiscordMessageBuilder().WithEmbed(embed)));
                }
                else
                {
                    Subscription subscription = _mapper.MapToSubscription(subscriptionResponse.Data);
                    embed = EmbedHelper.CreateSubscriptionResultEmbed(subscription);
                    await ctx.EditResponseAsync(new DiscordWebhookBuilder(new DiscordMessageBuilder().WithEmbed(embed)));
                }
            }
            else if (answer.Result.Id == "activatekey_no_btn")
            {
                await ctx.DeleteResponseAsync();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[ExtendSubscriptionCommand] {ex.Message}");
        }
    }

    
    [SlashCommand("bot-status", "Displays your settings for subscription and indicates if Pro Raffle is running")]
    public async Task BotStatusCommand(InteractionContext ctx)
    {
        var embed = new DiscordEmbedBuilder
        {
            Description = $"{EmojisHelper.Warning} It appears that you currently have no active Pro Raffle subscriptions. If you believe this is an error, please contact support.",
            Color = DiscordColor.Orange
        };
        try
        {
            var subscriptionResponse = await _subscriptionClient.GetSubscriptionsAsync(ctx.User.Id, ProductName.ProRaffle);
            if (subscriptionResponse.Data == null)
            {
                embed.Description = MessageHelper.GenericErrorMessage();
                embed.Color = DiscordColor.Red;
            }
            else if(subscriptionResponse.Data.Any())
            {
                var subscriptions = subscriptionResponse.Data.Select(s => _mapper.MapToSubscription(s));
                var description = new StringBuilder();
                foreach (var subscription in subscriptions)
                {
                    var proRaffleSetting = (ProRaffleSetting) subscription.ProductSetting!;
                    description.AppendLine();
                    description.Append($"{EmojisHelper.User} | **{proRaffleSetting.Username}**");
                    description.Append($"```{proRaffleSetting.Key}```");
                    description.AppendLine($"{EmojisHelper.Calendar_Spiral} Start Date: <t:{((DateTimeOffset)subscription.StartDate).ToUnixTimeSeconds()}:D>");
                    description.AppendLine($"{EmojisHelper.Calendar_Spiral} End Date: <t:{((DateTimeOffset)subscription.EndDate).ToUnixTimeSeconds()}:D>");
                    description.AppendLine($"{EmojisHelper.Robot} Status: **{(proRaffleSetting.IsPaused ? "Paused" : "Running")}**");
                    description.AppendLine();
                }
                embed.Description = description.ToString();
                embed.Color = DiscordColor.Green;
            }

            var message = new DiscordMessageBuilder().WithEmbed(embed);
            await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder(message).AsEphemeral(true));
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[BotStatusCommand] {ex.Message}");
        }
    }

    
    [SlashCommand("update-alphabot-key", "Replace the current alphabot key with a new one")]
    public async Task UpdateAlphabotKeyCommand(InteractionContext ctx,
        [Option("old-alphabot-key", "Current Alphabot key")] string currentKey,
        [Option("new-alphabot-key", "New Alphabot key")] string newKey)
    {
        
        var embed = new DiscordEmbedBuilder();
        if (!currentKey.IsAlphabotKeyFormat() || !newKey.IsAlphabotKeyFormat())
        {
            await ctx.CreateResponseAsync(embed: new DiscordEmbedBuilder
            {
                Description = $"{EmojisHelper.Warning} Incorrect alphabot-key format.",
                Color = DiscordColor.Orange
            }, true);
            return;
        }

        try
        {
            var updatePRSettingKeyRequest = new UpdatePRSettingKeyRequest
            {
                CurrentKey = currentKey,
                NewKey = newKey
            };
            var apiResponse = await _proRaffleSettingClient.UpdateProRaffleSettingKeyAsync(ctx.User.Id, updatePRSettingKeyRequest);
            bool isModified = apiResponse.Data;
            if (!isModified)
            {              
                if(apiResponse.ExceptionResult == ExceptionResult.InternalServerError500)
                {
                    embed.Description = MessageHelper.GenericErrorMessage(); 
                }
                else
                {
                    embed.Description = $"{EmojisHelper.X} Failed to update Alphabot API Key.";
                }
                embed.Color = DiscordColor.Red;  
            }
            else
            {
                embed.Color = DiscordColor.Green;
                embed.Description = $"{EmojisHelper.WhiteCheckMark} Alphabot API Key successfully updated.";
            }
            await ctx.CreateResponseAsync(embed: embed, true);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[UpdateAlphabotKeyCommand] {ex.Message}");
        }
    }
}
