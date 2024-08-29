using DSharpPlus;
using DSharpPlus.Entities;
using ProPayments.Client.Helpers;

namespace ProPayments.Client.Extensions
{
    public static class GuildExtension
    {

        public static async Task TryAddRoleAsync(this DiscordMember member, IReadOnlyDictionary<ulong, DiscordRole> guildRoles,  ulong productRoleId)
        {
            if (guildRoles.TryGetValue(productRoleId, out var role))
            {
                if (!member.Roles.Contains(role))
                {
                    await member.GrantRoleAsync(role);
                }
            }
        }

        public static async Task TryRevokeRoleAsync(this DiscordMember member, IReadOnlyDictionary<ulong, DiscordRole> guildRoles,  ulong productRoleId)
        {
            if (guildRoles.TryGetValue(productRoleId, out var role))
            {
                if (!member.Roles.Contains(role))
                {
                    await member.RevokeRoleAsync(role);
                }
            }
        }

        public static async Task AddRolesAsync(this DiscordMember member, IReadOnlyDictionary<ulong, DiscordRole> guildRoles,  List<ulong> productRolesIds)
        {
            try
            {
                foreach (var id in productRolesIds)
                {
                    if (guildRoles.TryGetValue(id, out var role))
                    {
                        if (!member.Roles.Contains(role))
                        {
                            await member.GrantRoleAsync(role);
                        }
                    }
                    await Task.Delay(TimeSpan.FromSeconds(2));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Error][AddRolesAsync] {ex.Message}");
            }
        }

        public static async Task RevokeRolesAsync(this DiscordMember member, IReadOnlyDictionary<ulong, DiscordRole> guildRoles, List<ulong?> productRolesIds)
        {
            try
            {
                foreach (var id in productRolesIds)
                {
                    if(!id.HasValue)
                    {
                        continue;
                    }
                    if (guildRoles.TryGetValue(id.Value, out var role))
                    {
                        if (!member.Roles.Contains(role))
                        {
                            await member.RevokeRoleAsync(role);
                        }
                    }
                    await Task.Delay(TimeSpan.FromSeconds(2));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Error][RevokeRolesAsync] {ex.Message}");
            }
        }

        public static async Task NotifyOnSubscriptionAlertChannel(this DiscordGuild guild, ulong channelId, ulong userId, int totalProductKeys)
        {
            try
            {
                var channel = guild.GetChannel(channelId);
                var member = await guild.GetMemberAsync(userId);
                var mention = new UserMention(member);
                var embed = EmbedHelper.CreatePaidProductEmbed(totalProductKeys);

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
                var productDetailsButton = new DiscordButtonComponent(ButtonStyle.Secondary, "product_details_btn", "Product Details 📋");
                
                var embed = EmbedHelper.CreateSubscriptionEmbed();
                var message = new DiscordMessageBuilder()
                    .WithEmbed(embed)
                    .AddComponents(subscribeButton, walletSubmissionButton, productDetailsButton);

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
