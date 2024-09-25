using System.Text;
using Probot.Client.Models;
using Probot.Shared.Helpers;

namespace Probot.Client.Helpers
{
    public static class MessageHelper
    {
        public static readonly string PaymentWalletNotFound = "Payment wallet not found.";
        public static readonly string MissingProductOrDuration = "You need to select a product and a duration before subscribing.";
        public static readonly string MaxItemsPerCart = $"The maximum number of items per cart is {Cart.MaxItemsPerCart}.";
        public static readonly string CartIsEmpty = "The cart is empty. To confirm your order, you must have at least 1 item.";
        public static readonly string SettingsNotFound = "Active subscriptions not found. Only active subscriptions can set up notifications";
        public static readonly string MissingUsername = "You need to select a username to enable/disable the alerts.";
        public static readonly string AlertAlreadyDisabled = $"{EmojisHelper.Information} Alert is up to date. No action needed.";

        public static string GenericErrorMessage()
        {
            DateTimeOffset tryLater = DateTimeOffset.UtcNow.AddMinutes(5);
            StringBuilder errorMessage = new();

            errorMessage.AppendLine($"We're sorry, but we are currently unable to complete your request. Please try again <t:{tryLater.ToUnixTimeSeconds()}:R>.");
            errorMessage.AppendLine();
            errorMessage.AppendLine("If the problem persists, contact our support team for assistance. We apologize for the inconvenience.");

            return errorMessage.ToString();
        }

        public static string WalletSubmitSuccess(string walletAddress)
            => $"{EmojisHelper.WhiteCheckMark} Address **{walletAddress}** submitted with success";

        public static string WalletInUse(string walletAddress) 
            => $"{EmojisHelper.X} Address: **{walletAddress}** already in use by another user.";
    }
}
