using ProPayments.Client.Clients.ProPayments;
using ProPayments.Client.Dtos.User.Request;
using ProPayments.Client.Helpers;
using ProPayments.Client.Mappers;
using ProPayments.Client.Models;
using System.Collections.Concurrent;

namespace ProPayments.Client.Services.Managers
{
    public class UserManager
    {
        private readonly ConcurrentDictionary<ulong, User> _users = new();
        private readonly UserClient _userClient;
        private readonly OrderManager _orderManager;
        private readonly Mapper _mapper;
        private readonly CancelationTokenManager _tokenManager;

        public UserManager(UserClient userClient, OrderManager orderManager, Mapper mapper, CancelationTokenManager tokenManager)
        {
            _userClient = userClient;
            _orderManager = orderManager;
            _mapper = mapper;
            _tokenManager = tokenManager;
            Console.WriteLine("User Manager created");
        }

        public async Task<bool> LoadUsersToMemoryAsync()
        {
            _users.Clear();
            var apiResponse = await _userClient.GetUsersWithSubscriptionsAsync();
            if (apiResponse.Data == null)
            {
                return false;
            }

            var usersWithsubscriptions = apiResponse.Data;
            foreach (var userWithSubscriptions in usersWithsubscriptions)
            {
                var user = _mapper.MapToUser(userWithSubscriptions);
                Console.WriteLine(user.ToString());
                _users[user.Id] = user;
            }
            return true;
        }

        private async Task<bool> CreateUserAsync(ulong userId, string username, string walletAddress)
        {
            var userRequest = new UserRequest
            {
                Id = userId,
                Username = username,
                WalletAddress = walletAddress
            };
            var apiResponse = await _userClient.RegisterUserAsync(userRequest);

            if (apiResponse.Data == null)
            {
                return false;
            }
            var user = _mapper.MapToUser(apiResponse.Data);
            AddUserToMemory(new(user));
            return true;
        }

        private async Task<bool> UpdateWalletAsync(ulong userId, string walletAddress)
        {
            var apiResponse = await _userClient.UpdateWalletAsync(userId, walletAddress);
            var walletUpdatedSuccessfully = apiResponse.Data;
            if (walletUpdatedSuccessfully)
            {
                UpdateUserInMemory(userId, walletAddress);
                return true;
            }
            return false;
        }

        public async Task<bool> AddOrUpdateUserAsync(ulong userId, string username, string walletAddress)
        {
            bool result;
            var user = GetUserFromMemory(userId);
            if (user == null)
            {
                result = await CreateUserAsync(userId, username, walletAddress);
            }
            else
            {
                result = await UpdateWalletAsync(userId, walletAddress);
            }
            return result;
        }

        public WalletStatus GetWalletAddressStatus(ulong userId, string walletAddress)
        {
            var walletResult = new WalletStatus();
            var user = _users
                .FirstOrDefault(users => string.Equals(users.Value.WalletAddress, walletAddress, StringComparison.InvariantCultureIgnoreCase))
                .Value;
            if (user != null)
            {
                walletResult.Result = Result.WalletExist;
                walletResult.Message = MessageHelper.WalletExist(user.WalletAddress);
            }

            var isWalletFoundInOrder = _orderManager
                .Orders
                .Any(o => string.Equals(o.User?.WalletAddress, walletAddress, StringComparison.InvariantCultureIgnoreCase) && o.User?.Id != userId);
            if (isWalletFoundInOrder)
            {
                walletResult.Result = Result.WalletFoundInOrder;
                walletResult.Message = MessageHelper.WalletFoundInActiveOrder(walletAddress);
            }
            return walletResult;
        }

        public void UpdateUserInMemory(ulong userId, string walletAddress)
        {
            var existingUser = GetUserFromMemory(userId);
            if(existingUser != null)
            {
                var updatedUser = new User(existingUser);
                updatedUser.WalletAddress = walletAddress;
                _users.TryUpdate(userId, updatedUser, existingUser);
            }
        }

        public void AddUserToMemory(User user)
        {
            _users.TryAdd(user.Id, user);
        }

        public void RemoveUserFromMemory(ulong userId)
        {
            _users.TryRemove(userId, out _);
        }

        public User? GetUserFromMemory(ulong userId)
        {
            _users.TryGetValue(userId, out var user);
            return user;
        }

        public void RemoveSubscriptionForUser(ulong userId, ulong subscriptionPlanRoleId)
        {
            var user = GetUserFromMemory(userId);
            Console.WriteLine("Before Removing:\n" + user?.ToString());
            user?.RemoveSubscription(subscriptionPlanRoleId);
            Console.WriteLine("After Removing\n" + user?.ToString());
        }

        public void AddOrUpdateSubscriptionForUser(ulong userId, Subscription subscription)
        {
            var user = GetUserFromMemory(userId);
            Console.WriteLine("Before Add/Update:\n" + user?.ToString());
            user?.AddOrUpdateSubscription(subscription);
            Console.WriteLine("After Add/Update:\n" + user?.ToString());
        }

        public ConcurrentDictionary<ulong, User> Users => _users;
    }
}
