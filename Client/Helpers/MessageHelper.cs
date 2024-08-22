using System.Text;

namespace ProPayments.Client.Helpers
{
    public static class MessageHelper
    {
        public static readonly string PaymentWalletNotFoundMessage = "Payment wallet not found.";

        public static readonly string MissingProductOrDuration = "You need to select a product and a duration before subscribing.";

        public static readonly string CartIsEmpty = "The cart is empty. To confirm your order, you must have at least 1 item.";

        public static string GenericErrorMessage()
        {
            DateTimeOffset tryLater = DateTimeOffset.UtcNow.AddMinutes(5);
            StringBuilder errorMessage = new();

            errorMessage.AppendLine($"We're sorry, but we are currently unable to complete your request. Please try again <t:{tryLater.ToUnixTimeSeconds()}:R>.");
            errorMessage.AppendLine();
            errorMessage.AppendLine("If the problem persists, contact our support team for assistance. We apologize for the inconvenience.");

            return errorMessage.ToString();
        }

        public static string WelcomeMessage()
        {
            StringBuilder welcomeMessage = new();

            welcomeMessage.AppendLine($"{EmojisHelper.Rocket} **Welcome to Our Service!**");
            welcomeMessage.AppendLine();
            welcomeMessage.AppendLine("We are excited to have you on board. Please let us know if you need any assistance.");
            welcomeMessage.AppendLine();
            welcomeMessage.AppendLine("Here are some tips to get started:");
            welcomeMessage.AppendLine("1. Explore the features available on your dashboard.");
            welcomeMessage.AppendLine("2. Set up your profile to personalize your experience.");
            welcomeMessage.AppendLine("3. Contact support if you have any questions or need help.");

            return welcomeMessage.ToString();
        }

        public static string WalletSubmitSuccess(string walletAddress)
            => $"Wallet **{walletAddress}** submitted with success";

        public static string WalletExist(string walletAddress) 
            => $"{EmojisHelper.X} User with the Wallet Address: **{walletAddress}** already exists. {EmojisHelper.X}";
        public static string WalletFoundInActiveOrder(string walletAddress)
            => $"Wallet Address: **{walletAddress}** is currently associated with an active order by another user.\nPlease wait until the active order is completed or canceled before using this wallet address.";
    }
}
