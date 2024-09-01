using Probot.SubscriptionApi.Services.BackgroundServices.Models;
using System.Threading.Channels;

namespace Probot.SubscriptionApi.Services.BackgroundServices.IServices
{
    public interface IMonitorService
    {
        Task<string?> GetStartingHashAsync();
        string? LatestHash { get; set; }

        Channel<TransactionMessage> TransactionChannel { get; }
        void CloseChannel();

        CancellationToken Token { get; }

        void Cancel();

        Task WaitForOrderSignalAsync(CancellationToken cancellationToken);
        void NotifyNewOrder();
    }
}
