using ProPayments.Service.Data.Entities;
using ProPayments.Service.Dtos.Users.Request;

namespace ProPayments.Service.Services.Services.IServices
{
    public interface IUserService
    {
        Task<User> CreateUserAsync(UserRequest request);
        Task UpdateWalletAddressAsync(ulong userId, string walletAddress);
        Task<User> GetUserByIdAsync(ulong userId);
        Task<IEnumerable<User>> GetUsersWithSubscriptionsAsync();
        Task<IEnumerable<ProductKey>> GetProductKeysAsync(ulong userId, bool isActivated);
        Task<ProductKey> GetProductKeyAsync(ulong userId, string code, bool isActivated);
    }
}
