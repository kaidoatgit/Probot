using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using Probot.Client.Models;
using Probot.Shared.Helpers;
using System.Text;

namespace Probot.Client.Helpers
{
    public static class EmbedHelper
    {
        private const string _emptySpace = "ㅤ";
        private const string successImageUrl = "https://cdn.discordapp.com/attachments/1259290186148483214/1259290567586611321/order_success.webp?ex=668b253b&is=6689d3bb&hm=e28adeb3921b55b82a540207b43f9d7044cd6fb7be7fd36fae1c8c8b9229e450&";
        private const string webhookImageUrl = "https://cdn.discordapp.com/attachments/1280815012037918741/1288605324663853087/WebhookOAuth.png?ex=66f5caba&is=66f4793a&hm=96cf9afea4817c4e15bace9207fcf45dbc619856e3add5a907ec7f69c016460f&";
        
        public static DiscordEmbed CreateProductDetailsEmbed(List<Product> Products)
        {
            var embed = new DiscordEmbedBuilder
            {
                Title = "Product Details",
                Color = DiscordColor.Gold
            };

            //Determine maximum lengths for Duration and PriceUSD columns across all products
            int maxDurationLength = Products
               .SelectMany(p => p.ProductOptions.Select(pd => pd.PeriodDescription.Length))
               .Max();
            int maxPriceLength = Products
                .SelectMany(p => p.ProductOptions.Select(pd => pd.Price.ToString("0.00").Length + 1)) // +1 for '$' symbol
            .Max();

            StringBuilder description = new();
            foreach (var product in Products)
            {
                var productDetails = product.ProductOptions;

                description.AppendLine($"<@&{product.RoleId}>");
                description.Append("```");
                // Calculate the necessary spaces for Duration and PriceUSD headers
                string durationHeader = "Duration";
                string priceHeader = "PriceUSD";

                int spacesForDuration = Math.Max(0, maxDurationLength - durationHeader.Length + 2);

                // Format the headers with appropriate spacing
                string headerLine = $"{durationHeader}{new string(' ', spacesForDuration)}\t{priceHeader}";
                description.AppendLine(headerLine);

                foreach (var productDetail in productDetails)
                {
                    string duration = productDetail.PeriodDescription;
                    string price = productDetail.Price.ToString("0.00");

                    // Calculate spaces for Duration and PriceUSD columns
                    string spacesForDurationValue = new string(' ', Math.Max(0, maxDurationLength - duration.Length + 2));

                    // Append the formatted line to the description
                    description.AppendLine($"{duration}{spacesForDurationValue}\t{price}$");
                }
                description.Append("```");

                // Add the formatted description to the Discord embed field
                // embed.AddField("\u200B", $"<@&{product.RoleId}>\n```{description}```");
            }
            embed.Description = description.ToString();
            return embed.Build();
        }

        public static DiscordEmbed CreatePaymentWalletsEmbed(string? wallet)
        {
            if(string.IsNullOrEmpty(wallet))
            {
                wallet = "Undefined";
            }
            StringBuilder description = new();
            description.AppendLine($"Solana");
            description.AppendLine($"- ||{wallet}||");
            description.Append(string.Concat(Enumerable.Repeat(_emptySpace, 18)));

            var embed = new DiscordEmbedBuilder
            {
                Title = "Payment Wallets",
                Description = description.ToString(),
                Color = DiscordColor.Gold
            };

            return embed.Build();
        }

        public static DiscordEmbed CreateFreeProductEmbed(DateTime startDate, DateTime endDate, ulong? product)
        {
            var embed = new DiscordEmbedBuilder
            {
                Title = $"Order Completed",
                Description = $"Your order for the **<@&{product}>** product has been completed successfully! {EmojisHelper.Tada}",
                Color = DiscordColor.Green,
                Footer = new() { Text = $"Enjoy your testing phase! {EmojisHelper.Smile}" },
                Timestamp = DateTime.UtcNow
            };

            embed.AddField($"{EmojisHelper.Calendar_Spiral} Start Date", $"<t:{((DateTimeOffset)startDate).ToUnixTimeSeconds()}:D>", true);
            embed.AddField($"{EmojisHelper.Calendar_Spiral} End Date", $"<t:{((DateTimeOffset)endDate).ToUnixTimeSeconds()}:D>", true);
            embed.WithImageUrl(successImageUrl);

            return embed.Build();
        }

        public static DiscordEmbed CreateShoppingCartEmbed(List<CartItem> cartItems)
        {
            StringBuilder description = new("Here are the items currently in your cart:");
            description.AppendLine("```");
            description.AppendLine("Id | Item            | Duration      | Price");
            description.AppendLine("---|-----------------|---------------|---------");

            decimal total = 0;
            foreach (var cartItem in cartItems)
            {
                description.AppendLine(cartItem.ToString());
                total+=cartItem.Price;
            }

            description.AppendLine("-----------------------------------------------");
            description.Append("Total".PadRight(39));
            description.AppendLine($"${total.ToString("0.00", System.Globalization.CultureInfo.InvariantCulture).PadRight(8)}");
            description.AppendLine("```");

            var embed = new DiscordEmbedBuilder
            {
                Title = $"Shopping Cart",
                Description = description.ToString(),
                Color = DiscordColor.Gold
            };
            return embed.Build();
        }

        public static DiscordEmbed CreateInvoiceEmbed(Invoice invoice)
        {
            StringBuilder description = new();
            description.AppendLine("```");
            description.AppendLine("Item                  | Duration      | Price");
            description.AppendLine("----------------------|---------------|---------");

            decimal total = 0;
            foreach (var invoiceItem in invoice.InvoiceItems)
            {
                description.AppendLine(invoiceItem.ToString());
                total+=invoiceItem.ProductOptionPrice;
            }

            description.AppendLine("------------------------------------------------");
            description.Append("Total".PadRight(40));
            description.AppendLine($"${total.ToString("0.00", System.Globalization.CultureInfo.InvariantCulture).PadRight(8)}");
            description.AppendLine("```");

            var embed = new DiscordEmbedBuilder
            {
                Title = "Invoice",
                Description = description.ToString(),
                Color = DiscordColor.Gold,
                Footer = new() { Text = "Pro Payments" },
                Timestamp = DateTime.UtcNow
            };

            StringBuilder details = new();
            details.AppendLine($"{EmojisHelper.Key}\u2000|\u2000Your wallet:");
            details.AppendLine($"```{invoice.PaymentAddress}```");
            details.AppendLine("📬\u2000|\u2000Send to:");
            details.AppendLine($"```{invoice.RecipientAddress}```");
            details.AppendLine($"💸\u2000|\u2000Total amount ({invoice.Coin}):");
            details.AppendLine($"```{invoice.TotalAmount.ToString("0.####", System.Globalization.CultureInfo.InvariantCulture)}```");
            details.AppendLine($"Tx expires <t:{invoice.OrderExpiryTime.ToUnixTimeSeconds()}:R>");

            embed.AddField("\u200B", details.ToString(), false);

            return embed.Build();
        }

        public static DiscordEmbed CreatePaidProductEmbed(int totalProductKeys, DiscordChannel? channel = null)
        {
            StringBuilder description = new();
            description.AppendLine($"**{totalProductKeys}** Product Key{(totalProductKeys > 1 ? "s" : "")} have been acquired.");
            if(channel != null)
            {
                description.AppendLine($"Use command: `/product-keys`  in {channel.Mention} to view all purchased keys details.");
            }

            var embed = new DiscordEmbedBuilder
            {
                Title = $"Order Completed {EmojisHelper.Tada}",
                Description = description.ToString(),
                Color = DiscordColor.Gold,
                Footer = new() { Text = $"Thank you for your purchase! {EmojisHelper.Pray}" },
                Timestamp = DateTime.UtcNow
            };
            embed.WithImageUrl(successImageUrl);

            return embed.Build();
        }
        
        public static DiscordEmbed CreatePaidProductEmbed(ulong product)
        {
            StringBuilder description = new();
            description.AppendLine($"Your order for the **<@&{product}>** product has been completed successfully! {EmojisHelper.Tada}");
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

        public static DiscordEmbed CreateProductAlreadyUsedEmbed()
        {
            StringBuilder description = new();
            description.AppendLine($"\u200B");
            description.AppendLine($"To continue enjoying our services, please upgrade to one of our paid products.");
            description.AppendLine($"Don't miss out on the exclusive benefits that come with our subscription products!");
            description.AppendLine($"\u200B");
            description.AppendLine($"{EmojisHelper.QuestionMark} Need help or have questions? Contact our support team anytime.");
            description.AppendLine($"\u200B");

            var embed = new DiscordEmbedBuilder
            {
                Title = $"{EmojisHelper.Bell} Free Product Already Used",
                Description = description.ToString(),
                Color = DiscordColor.Orange,
                Footer = new() { Text = "Pro Payments" },
                Timestamp = DateTime.UtcNow
            };

            return embed.Build();
        }

        public static DiscordEmbed CreateProRaffleCommandsInfoEmbed()
        {
            StringBuilder description = new();
            description.AppendLine();
            description.AppendLine("`/product-keys`");
            description.AppendLine("List all product keys and respective details.");
            description.AppendLine();
            description.AppendLine("`/new-subscription`");
            description.AppendLine("Use a product key to create a new subscription and associate it to Alphabot. Once activated, Pro Raffle automation will be enabled automatically.");
            description.AppendLine();
            description.AppendLine("`/extend-subscription`");
            description.AppendLine("Use a product key to extend an existing subscription");
            description.AppendLine();
            description.AppendLine("`/update-alphabot-key`");
            description.AppendLine("Replace the current alphabot key with a new one.");
            description.AppendLine();
            description.AppendLine("`/bot-status`");
            description.AppendLine("Displays your settings for each subscription and indicates if Pro Raffle is running");

            var embed = new DiscordEmbedBuilder
            {
                Title = $"Commands info",
                Description = description.ToString(),
                Color = DiscordColor.Orange,
                Footer = new() { Text = "Pro Raffles" },
                Timestamp = DateTime.UtcNow
            };

            return embed.Build();
        }

        public static DiscordEmbed CreateSubscriptionEmbed()
        {
            StringBuilder instructions = new();
            instructions.AppendLine("1. Register the wallet you will use for payment.");
            instructions.AppendLine("2. Click the 'Subscribe' button to initiate the process.");
            instructions.AppendLine("3. Send the requested amount from the wallet registered in Step 1.");
            instructions.AppendLine("4. Enjoy our 100% AFK bot service!");

            var embed = new DiscordEmbedBuilder
            {
                Title = "Become a privileged member",
                Description = instructions.ToString(),
                Color = DiscordColor.Gold,
                Footer = new() { Text = "Subscriptions" },
                Timestamp = DateTime.UtcNow
            };

            return embed.Build();
        }

        public static DiscordEmbed CreateSubscriptionEmbed(string username, ProductKey productKey, string alphabotKey)
        {
            var description = new StringBuilder();
            description.Append($"{EmojisHelper.User} Username");
            description.AppendLine($"```{username}```");
            description.Append($"{EmojisHelper.Ticket} Code | Duration: {productKey.Period} Month{(productKey.Period > 1 ? "s":"")}");
            description.AppendLine($"```{productKey.Code}```");
            description.Append($"{EmojisHelper.Key} Key");
            description.AppendLine($"```{alphabotKey}```");
            description.AppendLine($"Are you sure you want to __create__ a new subscription with this product code and API key{EmojisHelper.QuestionMark}");
            description.AppendLine($"\u200B");
            description.AppendLine("**Note: this action is irreversible.**");

            var embed = new DiscordEmbedBuilder
            {
                Description = description.ToString(),
                Color = DiscordColor.Gold
            };

            return embed.Build();
        }

        public static DiscordEmbed ExtendSubscriptionEmbed(string username, ProductKey productKey)
        {
            var description = new StringBuilder();
            description.Append($"{EmojisHelper.User} Username");
            description.AppendLine($"```{username}```");
            description.Append($"{EmojisHelper.Ticket} Code | Duration: {productKey.Period} Month{(productKey.Period > 1 ? "s":"")}");
            description.AppendLine($"```{productKey.Code}```");
            description.AppendLine($"Are you sure you want to __extend__ your subscription by using this product code{EmojisHelper.QuestionMark}");
            description.AppendLine($"\u200B");
            description.AppendLine("**Note: this action is irreversible.**");

            var embed = new DiscordEmbedBuilder
            {
                Description = description.ToString(),
                Color = DiscordColor.Gold
            };

            return embed.Build();
        }

        public static DiscordEmbed CreateSubscriptionResultEmbed(Subscription subscription)
        {
            var proRaffleSetting = (ProRaffleSetting) subscription.ProductSetting!;
            var description = new StringBuilder();
             description.Append($"{EmojisHelper.User} Username");
            description.AppendLine($"```{proRaffleSetting.Username}```");
            description.Append($"🎟️ Code | Duration: {subscription.PeriodDescription}");
            description.AppendLine($"```{subscription.Code}```");
            description.Append($"{EmojisHelper.Key} Key");
            description.AppendLine($"```{proRaffleSetting.Key}```\u200B");

            var embed = new DiscordEmbedBuilder
            {
                Description = description.ToString(),
                Color = DiscordColor.Gold
            };

            embed.Description = description.ToString();
            embed.Color = DiscordColor.Green;
            embed.AddField($"{EmojisHelper.Calendar_Spiral} Start Date", $"<t:{((DateTimeOffset)subscription.StartDate).ToUnixTimeSeconds()}:D>", true);
            embed.AddField($"{EmojisHelper.Calendar_Spiral} End Date", $"<t:{((DateTimeOffset)subscription.EndDate).ToUnixTimeSeconds()}:D>", true);
            embed.AddField($"\u200B", $"**Activation Success {EmojisHelper.Tada}**\nUse command: `/bot-status` to view all the details of your subscriptions.");

            return embed.Build();
        }

        public static DiscordEmbed CreateOAuthWebhookEmbed()
        {
            var description = new StringBuilder();
            description.AppendLine($"{EmojisHelper.Stopwatch} You have 1 minute to authorize before it expires.");
            description.AppendLine();
            description.AppendLine($"{EmojisHelper.Warning} When authorizing confirm always the oficial link: __**probot.topsecret.ngrok.app**__");
            var embed = new DiscordEmbedBuilder
            {
                Description = description.ToString(),
                Color = DiscordColor.Orange
            };

            embed.WithImageUrl(webhookImageUrl);
            return embed.Build();
        }
    }
}
