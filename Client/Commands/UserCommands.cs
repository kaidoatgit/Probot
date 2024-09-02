using System.Text;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity.Extensions;
using DSharpPlus.SlashCommands;
using DSharpPlus.SlashCommands.Attributes;
using Microsoft.AspNetCore.Http;
using Probot.Client.Clients.SubscriptionApi;
using Probot.Client.Extensions;
using Probot.Client.Helpers;
using Probot.Client.Mappers;
using Probot.Client.Models;
using Probot.Shared.Dtos.ProRaffle.Request;

namespace Probot.Client.Commands
{
    [SlashRequireGuild]
    [SlashCommandGroup("pro-raffle", "Commands for manage Pro Raffle automation bot")]
    public class UserCommands : ApplicationCommandModule
    {
        private readonly Mapper _mapper;
        private readonly ProRaffleClient _proRaffleClient;
        private readonly SubscriptionClient _subscriptionClient;
        private readonly ProductKeyClient _productKeyClient;
        public UserCommands(Mapper mapper, ProRaffleClient proRaffleClient, SubscriptionClient subscriptionClient, ProductKeyClient productKeyClient)
        {
            _mapper = mapper;
            _proRaffleClient = proRaffleClient;
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

        [SlashCommand("activate-key", "Activate a product key by associating it with your Alphabot API key.")]
        public async Task ActivateProductKeysCommand(InteractionContext ctx,
            [Option("product-key", "Product key which you purchased")] string code,
            [Option("alphabot-key", "API key provided by AlphaBot")] string key)
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
                var productKeyResponse = await _productKeyClient.GetProductKeyAsync(ctx.User.Id, code, isActivated: false);
                if (productKeyResponse.Data == null)
                {
                    if(productKeyResponse.StatusCode == StatusCodes.Status400BadRequest ||
                        productKeyResponse.StatusCode == StatusCodes.Status404NotFound)
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
                var embed = EmbedHelper.CreateActivationCodeEmbed(productKey, key);
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
                    var proRaffleRequest = new ProRaffleRequest
                    {
                        UserId = ctx.User.Id,
                        Code = productKey.Code,
                        AlphabotKey = key
                    };
                    var subscriptionResponse = await _subscriptionClient.CreateProRaffleSubscriptionAsync(proRaffleRequest);
                    if (subscriptionResponse.Data == null)
                    {
                        if(subscriptionResponse.StatusCode == StatusCodes.Status400BadRequest ||
                            subscriptionResponse.StatusCode == StatusCodes.Status404NotFound)
                        {
                            description = $"{EmojisHelper.X} Product key not found or already activated";
                        }
                        else if(subscriptionResponse.StatusCode == StatusCodes.Status409Conflict)
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
                        embed = EmbedHelper.CreateActivationCodeResultEmbed(subscription);
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
                Console.WriteLine($"[ActivateProductKeysCommand] {ex.Message}");
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
                var updateProRaffleKeyRequest = new UpdateProRaffleKeyRequest
                {
                    CurrentKey = currentKey,
                    NewKey = newKey
                };
                var apiResponse = await _proRaffleClient.UpdateProRaffleKeyAsync(ctx.User.Id, updateProRaffleKeyRequest);
                bool isModified = apiResponse.Data;
                if (!isModified)
                {              
                    if(apiResponse.StatusCode == StatusCodes.Status500InternalServerError)
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


        [SlashCommand("bot-status", "Displays your settings for each API key and indicates if Pro Raffle is running")]
        public async Task BotStatusCommand(InteractionContext ctx)
        {
            var embed = new DiscordEmbedBuilder
            {
                Description = $"{EmojisHelper.Warning} It appears that you currently have no active Pro Raffle subscriptions. If you believe this is an error, please contact support.",
                Color = DiscordColor.Orange
            };
            try
            {
                var subscriptionResponse = await _subscriptionClient.GetProRaffleSubscriptionsAsync(ctx.User.Id);
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
                        var proRaffle = (ProRaffle) subscription.UserSetting!;
                        description.AppendLine();
                        description.Append($"{EmojisHelper.Key} | Alphabot Key```{proRaffle.Key}```");
                        description.AppendLine($"{EmojisHelper.Calendar_Spiral} Start Date: <t:{((DateTimeOffset)subscription.StartDate).ToUnixTimeSeconds()}:D>");
                        description.AppendLine($"{EmojisHelper.Calendar_Spiral} End Date: <t:{((DateTimeOffset)subscription.EndDate).ToUnixTimeSeconds()}:D>");
                        description.AppendLine($"{EmojisHelper.Robot} Status: **{(proRaffle.IsPaused ? "Paused" : "Running")}**");
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
    
    
         #region interaction example
        // [SlashCommand("Just-Random", "Testing some commands")]
        // public static async Task RandomAsync(InteractionContext ctx, 
        //     [Option("Mention", "The person to mention if the command has one.")] SnowflakeObject snowflakeObject)
        // {
        //     await ctx.DeferAsync();

        //     var noButton = new DiscordButtonComponent(ButtonStyle.Danger, "activatekey_no_btn", $"No {EmojisHelper.X}");
        //     var yesButton = new DiscordButtonComponent(ButtonStyle.Success, "activatekey_yes_btn", $"Yes {EmojisHelper.WhiteCheckMark}");
        //     var builder = new DiscordFollowupMessageBuilder()
        //         .WithContent("Select an option")
        //         .AddComponents(noButton, yesButton);
        //     var message = await ctx.FollowUpAsync(builder);
        //     var answer = await message.WaitForButtonAsync(ctx.User, TimeSpan.FromMinutes(2));
        //     if (answer.Result.Id == "activatekey_yes_btn")
        //     {
        //         await ctx.EditFollowupAsync(message.Id, new DiscordWebhookBuilder().WithContent("Thanks"));
        //     }
        //     else
        //     {
        //         await ctx.EditFollowupAsync(message.Id, new DiscordWebhookBuilder().WithContent("Thanks"));
        //     }
        // }
        #endregion
    }
}
