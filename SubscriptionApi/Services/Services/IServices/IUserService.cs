using Probot.Data.Entities;
using Probot.Shared.Dtos.User.Request;

namespace Probot.SubscriptionApi.Services.Services.IServices
{
    public interface IUserService
    {
        Task<User> CreateUserAsync(UserRequest request);
        Task<User> GetUserByIdAsync(ulong userId);
        Task UpdateWalletAddressAsync(ulong userId, string walletAddress);
        Task<IEnumerable<User>> GetUsersWithMetricsAsync(CancellationToken cancellationToken);
    }
}
