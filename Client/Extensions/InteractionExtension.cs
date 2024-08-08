using DSharpPlus;
using DSharpPlus.Entities;
using ProPayments.Client.Helpers;
using ProPayments.Client.Models;

namespace ProPayments.Client.Extensions
{
    public static class InteractionHelper
    {

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

        public static async Task NotifyWithSubscribeOptions(this DiscordInteraction interaction, List<Plan> plans)
        {
            var planOptions = plans
               .Select(p => new DiscordSelectComponentOption(p.Type.ToString(), p.RoleId.ToString()))
               .AsEnumerable();
            var planDropdown = new DiscordSelectComponent("plan_selection", "Select a subscription role", planOptions);

            var confirmButton = new DiscordButtonComponent(ButtonStyle.Primary, "confirm_subscription_btn", "Confirm Subscription");

            var msg = new DiscordMessageBuilder()
                .AddEmbed(new DiscordEmbedBuilder()
                      .WithTitle("Subscription")
                      .WithDescription("Choose your **plan** and **numbers of months** you wish to subscribe.")
                      .WithColor(DiscordColor.Gold))
                .AddComponents(planDropdown)
                .AddComponents(confirmButton);

            await interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
                    new DiscordInteractionResponseBuilder(msg)
                    .AsEphemeral(true));
        }

        public static async Task NotifyWithWalletModal(this DiscordInteraction interaction)
        {
            var walletInput = new TextInputComponent("Solana address", "payment_wallet_input", "Enter your wallet address");

            var modal = new DiscordInteractionResponseBuilder()
                .WithTitle("Register wallet")
                .WithCustomId("wallet_submission_modal")
                .AddComponents(walletInput);

            await interaction.CreateResponseAsync(InteractionResponseType.Modal, modal);
        }

        public static async Task NotifyWithPlanDetails(this DiscordInteraction interaction, List<Plan> plans)
        {
            var embed = EmbedHelper.CreatePlanDetailsEmbed(plans);
            var planMessageBuilder = new DiscordMessageBuilder()
                .AddEmbed(embed);

            await interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
                new DiscordInteractionResponseBuilder(planMessageBuilder).AsEphemeral(true));
        }

        public static async Task NotifyWithFreeSubscription(this DiscordInteraction interaction, ulong messageId, Subscription subscription)
        {
            var embed = EmbedHelper.CreateFreePlanEmbed(subscription.StartDate, subscription.EndDate, subscription.PlanRoleId);
            var builder = new DiscordMessageBuilder().WithEmbed(embed);
            await interaction.EditFollowupMessageAsync(messageId, new DiscordWebhookBuilder(builder));
            await interaction.DeleteOriginalResponseAsync();
        }

        public static async Task NotifyWithFreePlanUsed(this DiscordInteraction interaction, ulong messageId)
        {
            var builder = new DiscordMessageBuilder().WithEmbed(EmbedHelper.CreatePlanAlreadyUsedEmbed());
            await interaction.EditFollowupMessageAsync(messageId, new DiscordWebhookBuilder(builder));
            await interaction.DeleteOriginalResponseAsync();
        }

        public static async Task NotifyWithServerError(this DiscordInteraction interaction, ulong messageId)
        {
            var embed = new DiscordEmbedBuilder
            {
                Title = $"{EmojisHelper.Pensive}  Action Unsuccessful",
                Description = MessageHelper.GenericErrorMessage(),
                Color = DiscordColor.Red
            };

            var builder = new DiscordMessageBuilder().WithEmbed(embed);
            await interaction.EditFollowupMessageAsync(messageId, new DiscordWebhookBuilder(builder));
            await interaction.DeleteOriginalResponseAsync();
        }

        public static async Task NotifyUserToSendPayment(this DiscordInteraction interaction, Order order)
        {
            var embed = EmbedHelper.CreateInvoiceEmbed(order.Invoice!);
            var builder = new DiscordMessageBuilder().WithEmbed(embed);
            await interaction.EditFollowupMessageAsync(order.Interaction!.FollowUpMessageId, new DiscordWebhookBuilder(builder));
            await interaction.DeleteOriginalResponseAsync();
        }

        public static async Task NotifyWithPaidSubscription(this DiscordInteraction interaction, ulong messageId, Subscription subscription)
        {
            var embed = EmbedHelper.CreatePaidPlanEmbed(subscription.StartDate, subscription.EndDate, subscription.PlanRoleId);
            var builder = new DiscordMessageBuilder().WithEmbed(embed);
            await interaction.EditFollowupMessageAsync(messageId, new DiscordWebhookBuilder(builder));
        }

    }
}
