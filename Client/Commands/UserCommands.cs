using System.Text;
using System.Text.RegularExpressions;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity.Extensions;
using DSharpPlus.SlashCommands;
using Microsoft.AspNetCore.Http;
using ProPayments.Client.Clients.ProPayments;
using ProPayments.Client.Dtos.ProductKey.Request;
using ProPayments.Client.Dtos.ProRaffle.Request;
using ProPayments.Client.Extensions;
using ProPayments.Client.Helpers;
using ProPayments.Client.Mappers;
using ProPayments.Client.Models;

namespace ProPayments.Client.Commands
{
    public class UserCommands : ApplicationCommandModule
    {
        private readonly Mapper _mapper;
        private readonly UserClient _userClient;
        private readonly ProRaffleClient _proRaffleClient;
        private readonly SubscriptionClient _subscriptionClient;
        public UserCommands(Mapper mapper, ProRaffleClient proRaffleClient, UserClient userClient, SubscriptionClient subscriptionClient)
        {
            _mapper = mapper;
            _proRaffleClient = proRaffleClient;
            _userClient = userClient;
            _subscriptionClient = subscriptionClient;
        }

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
                var productKeyResponse = await _userClient.GetProductKeysAsync(ctx.User.Id, isActivated: false);
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
                var productKeyResponse = await _userClient.GetProductKeyAsync(ctx.User.Id, code, isActivated: false);
                if (productKeyResponse.Data == null)
                {
                    if(productKeyResponse.StatusCode == StatusCodes.Status400BadRequest)
                    {
                        description = $"{EmojisHelper.X} {productKeyResponse.ErrorMessage}";
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

                        if(subscriptionResponse.StatusCode == StatusCodes.Status400BadRequest || subscriptionResponse.StatusCode == StatusCodes.Status409Conflict)
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
                        message = new DiscordMessageBuilder().WithEmbed(embed);
                        await ctx.EditResponseAsync(new DiscordWebhookBuilder(message));
                    }
                    else
                    {
                        Subscription subscription = _mapper.MapToSubscription(subscriptionResponse.Data);
                        embed = EmbedHelper.CreateActivationCodeResultEmbed(subscription);
                        message = new DiscordMessageBuilder().WithEmbed(embed);
                        await ctx.EditResponseAsync(new DiscordWebhookBuilder(message).WithContent($"Activation Success {EmojisHelper.Tada}"));
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
                Description = "It appears that you currently have no active Pro Raffle subscriptions. If you believe this is an error, please contact support.",
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
                    var subscriptions = subscriptionResponse.Data.Select(s => _mapper.MapToSubscription(s)).ToList();
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
    }
}
