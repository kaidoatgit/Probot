using DSharpPlus;
using DSharpPlus.Entities;
using ProPayments.Client.Services.Managers;
using System.Text;

namespace ProPayments.Client.Helpers
{
    public static class ComponentHelper
    {

        public static DiscordMessageBuilder CreateSubscriptionMessage()
        {
            var walletSubmissionButton = new DiscordButtonComponent(ButtonStyle.Success, "wallet_submission_btn", "Submit Wallet");
            var subscribeButton = new DiscordButtonComponent(ButtonStyle.Primary, "subscribe_btn", "Subscribe");
            var planDetailsButton = new DiscordButtonComponent(ButtonStyle.Secondary, "plan_details_btn", "Plan Details");

            StringBuilder instructions = new();
            instructions.AppendLine("1. Register the wallet you will use for payment.");
            instructions.AppendLine("2. Click the 'Subscribe' button to initiate the process.");
            instructions.AppendLine("3. Send the requested amount from the wallet registered in Step 1.");
            instructions.AppendLine("4. Enjoy our 100% AFK bot service!");

            var message = new DiscordMessageBuilder()
                .AddEmbed(new DiscordEmbedBuilder()
                      .WithTitle("Become a privileged member")
                      .AddField("Instructions", instructions.ToString())
                      .WithColor(DiscordColor.Gold)
                      .WithTimestamp(DateTime.UtcNow)
                      .WithFooter("Pro Payments"))
                .AddComponents(subscribeButton, walletSubmissionButton, planDetailsButton);

            return message;
        }

        public static DiscordSelectComponent GetDurationsPricesBasedOnPlanSelected(PlanManager planManager, string selectedPlan)
        {
            var durationOptions = planManager
                        .GetDurationsWithPrices(selectedPlan)!
                        .Select(option => new DiscordSelectComponentOption(option.PeriodDescription, option.Period.ToString()))
                        .AsEnumerable();
            return new DiscordSelectComponent("duration_selection", "Month(s) subscription", durationOptions);
        }
    }
}
