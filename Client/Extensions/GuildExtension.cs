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
                var subscriptionMsgBuilder = ComponentHelper.CreateSubscriptionMessage();
                var channel = guild.GetChannel(channelId);
                var firstMessage = (await channel.GetMessagesAsync()).LastOrDefault();

                if (firstMessage == null)
                {
                    var result = await channel.SendMessageAsync(subscriptionMsgBuilder);
                }
                else
                {
                    var existingMessage = await channel.GetMessageAsync(firstMessage.Id);
                    await existingMessage.ModifyAsync(subscriptionMsgBuilder);
                }
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return false;
        }
    }
}
