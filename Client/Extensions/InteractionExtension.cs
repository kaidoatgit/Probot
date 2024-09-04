using System.Text;
using DSharpPlus;
using DSharpPlus.Entities;
using Probot.Client.Helpers;
using Probot.Client.Models;

namespace Probot.Client.Extensions
{
    public static class InteractionHelper
    {
        private const string _emptySpace = "ㅤ";
        public static async Task NotifyWithMessage(this DiscordInteraction interaction, string reason, bool defer = false, bool deleteMsg = false, TimeSpan? after = null)
        {
            if (defer)
            {
                await interaction.EditOriginalResponseAsync(new DiscordWebhookBuilder().WithContent(reason));
            }
            else
            {
                await interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
                        new DiscordInteractionResponseBuilder()
                        .WithContent(reason)
                        .AsEphemeral(true));
            }

            if (deleteMsg)
            {
                _ = Task.Run(async () =>
                {
                    if (after != null)
                    {
                        await Task.Delay((TimeSpan)after);
                    }
                    await interaction.DeleteOriginalResponseAsync();
                });
            }
        }

        public static async Task NotifyWithSubscribeOptions(this DiscordInteraction interaction, List<Product> products)
        {
            var productOptions = products
               .Select(p => new DiscordSelectComponentOption(p.Name.ToString(), p.RoleId.ToString()))
               .AsEnumerable();
            var productDropdown = new DiscordSelectComponent("product_selection_menu", "Select a subscription role", productOptions);
            var addItemButton = new DiscordButtonComponent(ButtonStyle.Primary, "add_item_cart_btn", "Add Items 🛒");
            var removeItemButton = new DiscordButtonComponent(ButtonStyle.Danger, "remove_item_cart_btn", "Remove Items 🗑️");
            var confirmButton = new DiscordButtonComponent(ButtonStyle.Success, "confirm_cart_btn", $"Confirm {EmojisHelper.WhiteCheckMark}");

            StringBuilder description = new();
            description.AppendLine($"\u200B");
            description.Append("Cart is empty");
            description.AppendLine(string.Concat(Enumerable.Repeat(_emptySpace, 23)));
            description.AppendLine($"\u200B");

            var msg = new DiscordMessageBuilder()
                .AddEmbed(new DiscordEmbedBuilder()
                      .WithTitle("Shopping Cart")
                      .WithDescription(description.ToString())
                      .WithColor(DiscordColor.Gold))
                .AddComponents(productDropdown)
                .AddComponents(addItemButton, removeItemButton, confirmButton);

            await interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
                    new DiscordInteractionResponseBuilder(msg)
                    .AsEphemeral(true));
        }

        public static async Task NotifyWithPaymentWallets(this DiscordInteraction interaction, string? wallet)
        {
            var solanaButton = new DiscordButtonComponent(ButtonStyle.Primary, "solana_wallet_btn", "Solana");

            var embed = EmbedHelper.CreatePaymentWalletsEmbed(wallet);
            var builder = new DiscordMessageBuilder()
                .WithEmbed(embed)
                .AddComponents(solanaButton);

            await interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
                new DiscordInteractionResponseBuilder(builder)
                .AsEphemeral(true));
        }

        public static async Task<string> NotifyWithWalletModal(this DiscordInteraction interaction, ulong interactionId)
        {
            var walletInput = new TextInputComponent("Solana address", "solana_wallet_input", "Enter your wallet address");

            var modal = new DiscordInteractionResponseBuilder()
                .WithTitle("Register wallet")
                .WithCustomId($"solana_wallet_submission_{interactionId}")
                .AddComponents(walletInput);

            await interaction.CreateResponseAsync(InteractionResponseType.Modal, modal);
            return modal.CustomId;
        }

        public static async Task<string> NotifyWithItemRemovalModal(this DiscordInteraction interaction, ulong interactionId)
        {
            var walletInput = new TextInputComponent("Item ID", "cart_item_id", "Enter the item ID you wish to remove");

            var modal = new DiscordInteractionResponseBuilder()
                .WithTitle("Remove Item")
                .WithCustomId($"cart_item_removal_submission_modal_{interactionId}")
                .AddComponents(walletInput);

            await interaction.CreateResponseAsync(InteractionResponseType.Modal, modal);
            return modal.CustomId;
        }

        public static async Task NotifyWithProductDetails(this DiscordInteraction interaction, List<Product> products)
        {
            var embed = EmbedHelper.CreateProductDetailsEmbed(products);
            var productMessageBuilder = new DiscordMessageBuilder()
                .AddEmbed(embed);

            await interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
                new DiscordInteractionResponseBuilder(productMessageBuilder).AsEphemeral(true));
        }

        public static async Task NotifyWithFreeSubscription(this DiscordInteraction interaction, ulong messageId, Subscription subscription)
        {
            var embed = EmbedHelper.CreateFreeProductEmbed(subscription.StartDate, subscription.EndDate, subscription.ProductRoleId);
            var builder = new DiscordMessageBuilder().WithEmbed(embed);
            await interaction.EditFollowupMessageAsync(messageId, new DiscordWebhookBuilder(builder));
            await interaction.DeleteOriginalResponseAsync();
        }

        public static async Task NotifyWithFreeProductUsed(this DiscordInteraction interaction, ulong messageId)
        {
            var builder = new DiscordMessageBuilder().WithEmbed(EmbedHelper.CreateProductAlreadyUsedEmbed());
            await interaction.EditFollowupMessageAsync(messageId, new DiscordWebhookBuilder(builder));
            await interaction.DeleteOriginalResponseAsync();
        }

        public static async Task NotifyWithServerError(this DiscordInteraction interaction, ulong messageId)
        {
            try
            {
                var embed = new DiscordEmbedBuilder
                {
                    Title = $"{EmojisHelper.Pensive}  Action Unsuccessful",
                    Description = MessageHelper.GenericErrorMessage(),
                    Color = DiscordColor.Red
                };

                var message = new DiscordMessageBuilder().WithEmbed(embed);
                await interaction.DeleteOriginalResponseAsync();
                await interaction.EditFollowupMessageAsync(messageId, new DiscordWebhookBuilder(message));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[NotifyWithServerError] {ex.Message}");
            }
        }

        public static async Task NotifyUserToSendPayment(this DiscordInteraction interaction, Order order)
        {
            var embed = EmbedHelper.CreateInvoiceEmbed(order.Invoice);
            var message = new DiscordMessageBuilder().WithEmbed(embed);
            await interaction.DeleteOriginalResponseAsync();
            await interaction.EditFollowupMessageAsync(order.Interaction!.MessageId, new DiscordWebhookBuilder(message));
        }

        public static async Task NotifyWithPaidSubscription(this DiscordInteraction interaction, ulong messageId, ulong channelId, int totalProductKeys)
        {
            var channel = interaction.Guild.GetChannel(channelId);
            var embed = EmbedHelper.CreatePaidProductEmbed(totalProductKeys, channel);
            var builder = new DiscordMessageBuilder().WithEmbed(embed);
            await interaction.EditFollowupMessageAsync(messageId, new DiscordWebhookBuilder(builder));
        }

    }
}
