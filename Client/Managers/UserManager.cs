using Probot.Client.Clients.SubscriptionApi;
using Probot.Client.Helpers;
using Probot.Client.Mappers;
using Probot.Client.Models;
using Probot.Shared.Dtos.User.Request;
using System.Collections.Concurrent;

namespace Probot.Client.Managers
{
    public class UserManager
    {
        private readonly ConcurrentDictionary<ulong, User> _users = new();
        private readonly UserClient _userClient;
        private readonly OrderManager _orderManager;
        private readonly Mapper _mapper;

        public UserManager(UserClient userClient, OrderManager orderManager, Mapper mapper)
        {
            _userClient = userClient;
            _orderManager = orderManager;
            _mapper = mapper;
            Console.WriteLine("User Manager created");
        }

        public ConcurrentDictionary<ulong, User> Users => _users;
        
        public async Task<bool> LoadUsersToMemoryAsync()
        {
            _users.Clear();
            var apiResponse = await _userClient.GetUsersWithMetricsAsync();
            if (apiResponse.Data == null)
            {
                return false;
            }

            var usersWithsubscriptions = apiResponse.Data;
            foreach (var userWithMetrics in usersWithsubscriptions)
            {
                var user = _mapper.MapToUser(userWithMetrics);
                Console.WriteLine(user.ToString());
                _users[user.Id] = user;
            }
            return true;
        }

        public async Task<User?> GetUserAsync(ulong userId)
        {
            var apiResponse = await _userClient.GetUserAsync(userId);
            if (apiResponse.Data != null)
            {
                return _mapper.MapToUser(apiResponse.Data);
            }
            return null;
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

        public WalletResult GetWalletAddressStatus(ulong userId, string walletAddress)
        {
            var result = WalletResult.Default;
            var user = _users
                .FirstOrDefault(users => string.Equals(users.Value.WalletAddress, walletAddress, StringComparison.InvariantCultureIgnoreCase))
                .Value;
            if (user != null)
            {
                result = WalletResult.WalletExist;
            }

            var isWalletFoundInOrder = _orderManager
                .Orders
                .Any(o => string.Equals(o.User?.WalletAddress, walletAddress, StringComparison.InvariantCultureIgnoreCase) && o.User?.Id != userId);
            if (isWalletFoundInOrder)
            {
                result = WalletResult.WalletFoundInOrder;
            }
            return result;
        }

        public void UpdateUserInMemory(ulong userId, string walletAddress)
        {
            var existingUser = GetUserFromMemory(userId);
            if(existingUser != null)
            {
                var updatedUser = new User(existingUser)
                {
                    WalletAddress = walletAddress
                };
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

        public void ReplaceMetricsForUser(ulong userId, Metrics? metrics)
        {
            _users.TryUpdate(userId, 
                new User
                {
                    Id = userId,
                    Metrics = metrics,
                    Username = _users[userId].Username,
                    WalletAddress = _users[userId].WalletAddress,
                    Email = _users[userId].Email
                },
                _users[userId]);
        }
    }
}
