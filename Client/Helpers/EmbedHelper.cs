using DSharpPlus.Entities;
using ProPayments.Client.Models;
using System.Text;

namespace ProPayments.Client.Helpers
{
    public static class EmbedHelper
    {
        private const string SuccessImageUrl = "https://cdn.discordapp.com/attachments/1259290186148483214/1259290567586611321/order_success.webp?ex=668b253b&is=6689d3bb&hm=e28adeb3921b55b82a540207b43f9d7044cd6fb7be7fd36fae1c8c8b9229e450&";

        public static DiscordEmbed CreatePlanDetailsEmbed(List<Plan> Plans)
        {
            var embed = new DiscordEmbedBuilder
            {
                Title = "Plan Details",
                Color = DiscordColor.Gold,
                Footer = new() { Text = "Pro Payments" },
                Timestamp = DateTime.UtcNow
            };

            //Determine maximum lengths for Duration and PriceUSD columns across all plans
            int maxDurationLength = Plans
               .SelectMany(p => p.PlanOptions.Select(pd => pd.PeriodDescription.Length))
               .Max();
            int maxPriceLength = Plans
                .SelectMany(p => p.PlanOptions.Select(pd => pd.Price.ToString("0.00").Length + 1)) // +1 for '$' symbol
            .Max();

            foreach (var plan in Plans)
            {
                var planDetails = plan.PlanOptions;

                StringBuilder description = new();

                // Calculate the necessary spaces for Duration and PriceUSD headers
                string durationHeader = "Duration";
                string priceHeader = "PriceUSD";

                int spacesForDuration = Math.Max(0, maxDurationLength - durationHeader.Length + 2);

                // Format the headers with appropriate spacing
                string headerLine = $"{durationHeader}{new string(' ', spacesForDuration)}\t{priceHeader}";
                description.AppendLine(headerLine);

                foreach (var planDetail in planDetails)
                {
                    string duration = planDetail.PeriodDescription;
                    string price = planDetail.Price.ToString("0.00");

                    // Calculate spaces for Duration and PriceUSD columns
                    string spacesForDurationValue = new string(' ', Math.Max(0, maxDurationLength - duration.Length + 2));

                    // Append the formatted line to the description
                    description.AppendLine($"{duration}{spacesForDurationValue}\t{price}$");
                }

                // Add the formatted description to the Discord embed field
                embed.AddField("\u200B", $"**Plan**: <@&{plan.RoleId}>\n```{description}```");
            }
            return embed.Build();
        }

        public static DiscordEmbed CreateFreePlanEmbed(DateTime startDate, DateTime endDate, ulong? plan)
        {
            var embed = new DiscordEmbedBuilder
            {
                Title = $"Order Completed",
                Description = $"Your order for the **<@&{plan}>** plan has been completed successfully! {EmojisHelper.Tada}",
                Color = DiscordColor.Green,
                Footer = new() { Text = $"Enjoy your testing phase! {EmojisHelper.Smile}" },
                Timestamp = DateTime.UtcNow
            };

            embed.AddField($"{EmojisHelper.Calendar_Spiral} Start Date", $"<t:{((DateTimeOffset)startDate).ToUnixTimeSeconds()}:D>", true);
            embed.AddField($"{EmojisHelper.Calendar_Spiral} End Date", $"<t:{((DateTimeOffset)endDate).ToUnixTimeSeconds()}:D>", true);
            embed.WithImageUrl(SuccessImageUrl);

            return embed.Build();
        }

        public static DiscordEmbed CreateInvoiceEmbed(Invoice invoice)
        {
            StringBuilder description = new();
            description.Append($"Plan: <@&{invoice.PlanRoleId}>");
            description.Append("ㅤㅤㅤㅤㅤㅤㅤㅤㅤ");
            description.Append($"Duration: {invoice.PeriodDescription}");
            var embed = new DiscordEmbedBuilder
            {
                Title = "Invoice",
                Description = description.ToString(),
                Color = DiscordColor.Gold,
                Footer = new() { Text = "Pro Payments" },
                Timestamp = DateTime.UtcNow
            };

            StringBuilder details = new();
            details.AppendLine("🔑\u2000|\u2000Your wallet:");
            details.AppendLine($"```{invoice.PaymentAddress}```");
            details.AppendLine("📬\u2000|\u2000Send to:");
            details.AppendLine($"```{invoice.RecipientAddress}```");
            details.AppendLine($"💸\u2000|\u2000Total amount ({invoice.Token}):");
            details.AppendLine($"```{invoice.TotalAmount.ToString("0.####", System.Globalization.CultureInfo.InvariantCulture)}```");
            details.AppendLine($"Tx expires <t:{invoice.OrderExpiryTime.ToUnixTimeSeconds()}:R>");

            embed.AddField("\u200B", details.ToString(), false);

            return embed.Build();
        }

        public static DiscordEmbed CreatePaidPlanEmbed(DateTime startDate, DateTime endDate, ulong plan)
        {
            var embed = new DiscordEmbedBuilder
            {
                Title = $"Order Completed",
                Description = $"Your order for the **<@&{plan}>** plan has been completed successfully! {EmojisHelper.Tada}",
                Color = DiscordColor.Gold,
                Footer = new() { Text = $"Thank you for your purchase! {EmojisHelper.Pray}" },
                Timestamp = DateTime.UtcNow
            };

            embed.AddField($"{EmojisHelper.Calendar_Spiral} Start Date", $"<t:{((DateTimeOffset)startDate).ToUnixTimeSeconds()}:D>", true);
            embed.AddField($"{EmojisHelper.Calendar_Spiral} End Date", $"<t:{((DateTimeOffset)endDate).ToUnixTimeSeconds()}:D>", true);
            embed.WithImageUrl(SuccessImageUrl);

            return embed.Build();
        }
        
        public static DiscordEmbed CreatePaidPlanEmbed(ulong plan)
        {
            StringBuilder description = new();
            description.AppendLine($"Your order for the **<@&{plan}>** plan has been completed successfully! {EmojisHelper.Tada}");
            description.AppendLine($"Thank you for your purchase! {EmojisHelper.Pray}");
            var embed = new DiscordEmbedBuilder
            {
                Title = $"Order Completed",
                Description = description.ToString(),
                Color = DiscordColor.Gold,
                Footer = new() { Text = "Pro Payments" },
                Timestamp = DateTime.UtcNow
            };

            return embed.Build();
        }
        
        public static DiscordEmbed CreateSubscriptionEndingSoonEmbed(DateTimeOffset subscriptionEndDate, int daysLeft)
        {
            StringBuilder description = new();
            description.AppendLine($"\u200B");
            description.AppendLine($"Your subscription will expire on <t:{subscriptionEndDate.ToUnixTimeSeconds()}:D>");
            description.AppendLine($"You have **{daysLeft}** days left to enjoy all your benefits.");
            description.AppendLine($"**Renew now** to keep all your benefits without any interruptions.");
            description.AppendLine($"\u200B");

            var embed = new DiscordEmbedBuilder
            {
                Title = $"{EmojisHelper.Bell} Heads up! Your subscription is ending soon!",
                Description = description.ToString(),
                Color = DiscordColor.Orange,
                Footer = new() { Text = "Pro Payments" },
                Timestamp = DateTime.UtcNow
            };

            return embed.Build();
        }

        public static DiscordEmbed CreateSubscriptionOverEmbed()
        {
            StringBuilder description = new();
            description.AppendLine($"\u200B");
            description.AppendLine($"**Renew now** to regain all your awesome benefits\nand continue enjoying our services!");
            description.AppendLine($"\u200B");
            var embed = new DiscordEmbedBuilder
            {
                Title = $"{EmojisHelper.X} Oops! Your subscription has expired!",
                Description = description.ToString(),
                Color = DiscordColor.Red,
                Footer = new() { Text = "Pro Payments" },
                Timestamp = DateTime.UtcNow
            };

            return embed.Build();
        }

        public static DiscordEmbed CreatePlanAlreadyUsedEmbed()
        {
            StringBuilder description = new();
            description.AppendLine($"\u200B");
            description.AppendLine($"To continue enjoying our services, please upgrade to one of our paid plans.");
            description.AppendLine($"Don't miss out on the exclusive benefits that come with our subscription plans!");
            description.AppendLine($"\u200B");
            description.AppendLine($"{EmojisHelper.QuestionMark} Need help or have questions? Contact our support team anytime.");
            description.AppendLine($"\u200B");

            var embed = new DiscordEmbedBuilder
            {
                Title = $"{EmojisHelper.Bell} Free Plan Already Used",
                Description = description.ToString(),
                Color = DiscordColor.Orange,
                Footer = new() { Text = "Pro Payments" },
                Timestamp = DateTime.UtcNow
            };

            return embed.Build();
        }

        public static DiscordEmbed CreateAlphabotCommandsInfoEmbed()
        {
            StringBuilder description = new();
            description.AppendLine();
            description.AppendLine("`/add-key`");
            description.AppendLine("__Add__ or __update__ your Alphabot API key to enable raffle automation.");
            description.AppendLine();
            description.AppendLine("`/status`");
            description.AppendLine("Displays your current API key and indicates whether the raffle automation bot is running.");

            var embed = new DiscordEmbedBuilder
            {
                Title = $"Commands info",
                Description = description.ToString(),
                Color = DiscordColor.Orange,
                Footer = new() { Text = "Pro Payments" },
                Timestamp = DateTime.UtcNow
            };

            return embed.Build();
        }


        //public static DiscordEmbed CreateOrderFailureEmbed(string reason, string imageUrl)
        //{
        //    DateTimeOffset tryLater = DateTimeOffset.UtcNow.AddMinutes(5);
        //    var embed = new DiscordEmbedBuilder
        //    {
        //        Title = "❌ Order Failed",
        //        Description = $"Your order could not be completed. Reason: {reason}",
        //        Color = DiscordColor.Red,
        //        Timestamp = DateTime.UtcNow,
        //        Footer = new() { Text = "Need help? Contact our support team anytime." }
        //    };

        //    embed.WithImageUrl(imageUrl);
        //    embed.WithFooter("Please try again.");

        //    return embed.Build();
        //}
    }
}
