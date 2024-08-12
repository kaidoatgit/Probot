using DSharpPlus;
using DSharpPlus.Entities;
using ProPayments.Client.Helpers;
using ProPayments.Client.Models;

namespace ProPayments.Client.Extensions
{
    public static class GuildExtension
    {

        public static async Task GrantRoleAsync(this DiscordGuild guild, DiscordUser user, ulong roleId)
        {
            try
            {
                DiscordRole role = guild.Roles.Values.FirstOrDefault(r => r.Id == roleId)!;
                DiscordMember member = (DiscordMember)user;
                await member.GrantRoleAsync(role);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Error][GrantRoleAsync] {ex.Message}");
            }
        }

        public static async Task NotifyOnSubscriptionAlertChannel(this DiscordGuild guild, ulong channelId, Subscription subscription)
        {
            try
            {
                var channel = guild.GetChannel(channelId);
                var member = await guild.GetMemberAsync(subscription.UserId);
                var mention = new UserMention(member);
                var embed = EmbedHelper.CreatePaidPlanEmbed(subscription.PlanRoleId);

                var message = new DiscordMessageBuilder()
                    .WithEmbed(embed)
                    .WithAllowedMention(mention)
                    .WithContent($"||{member.Mention}||");
                await channel.SendMessageAsync(message);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Error][NotifyOnSubscriptionChannel] {ex.Message}");
            }
        }

        public static async Task<bool> AddSubscriptionProcess(this DiscordGuild guild, ulong channelId)
        {
            try
            {
                var walletSubmissionButton = new DiscordButtonComponent(ButtonStyle.Success, "wallet_btn", "Payment Wallet 💳");
                var subscribeButton = new DiscordButtonComponent(ButtonStyle.Primary, "subscribe_btn", "Subscribe 📝");
                var planDetailsButton = new DiscordButtonComponent(ButtonStyle.Secondary, "plan_details_btn", "Plan Details 📋");
                
                var embed = EmbedHelper.CreateSubscriptionEmbed();
                var message = new DiscordMessageBuilder()
                    .WithEmbed(embed)
                    .AddComponents(subscribeButton, walletSubmissionButton, planDetailsButton);

                var channel = guild.GetChannel(channelId);
                var existingMessage = (await channel.GetMessagesAsync()).LastOrDefault();

                if (existingMessage == null)
                {
                    var result = await channel.SendMessageAsync(message);
                }
                else
                {
                    await existingMessage.ModifyAsync(message);
                }
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Error][AddSubscriptionProcess] {ex.Message}");
            }
            return false;
        }

        public static async Task<bool> AddAlphabotCommandsInfo(this DiscordGuild guild, ulong channelId)
        {
            try
            {
                var embed = EmbedHelper.CreateAlphabotCommandsInfoEmbed();
                var message = new DiscordMessageBuilder()
                    .WithEmbed(embed);

                var channel = guild.GetChannel(channelId);
                var firstMessage = (await channel.GetMessagesAsync()).LastOrDefault();

                if (firstMessage == null)
                {
                    var result = await channel.SendMessageAsync(message);
                }
                else
                {
                    var existingMessage = await channel.GetMessageAsync(firstMessage.Id);
                    await existingMessage.ModifyAsync(embed);
                }
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Error][AddAlphabotCommandsInfo] {ex.Message}");
            }
            return false;
        }
    }
}
