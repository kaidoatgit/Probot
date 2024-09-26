using Microsoft.EntityFrameworkCore;
using Probot.Data;
using Probot.Data.Entities;
using Probot.SubscriptionApi.Exceptions;
using Probot.SubscriptionApi.Mappers;
using Probot.SubscriptionApi.Services.Services.IServices;
using Probot.Shared.Dtos.User.Request;
using Probot.Shared.Enums;

namespace Probot.SubscriptionApi.Services.Services
{
    public class UserService : IUserService
    {
        private readonly ProbotContext _context;
        private readonly Mapper _mapper;

        public UserService(ProbotContext context, Mapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<User> CreateUserAsync(UserRequest request)
        {
            bool foundUser = await _context.Users
                .AnyAsync(u => u.Id == request.Id);
            if (foundUser)
            {
                throw new SubscriptionException(ExceptionResult.UserConflict409, $"User:{request.Id} already exists.");
            }

            User user = _mapper.MapToUserEntity(request);
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return user;
        }

        public async Task<User> GetUserByIdAsync(ulong userId, bool? dbTracking)
        {
            IQueryable<User> query = _context.Users.AsNoTracking();
            if(dbTracking.HasValue)
            {
                query = query.AsTracking();
            }

            return await query.SingleOrDefaultAsync(u => u.Id == userId)
                ?? throw new SubscriptionException(ExceptionResult.UserNotFound404, "User not found");
        }
        
        public async Task UpdateWalletAddressAsync(ulong userId, string walletAddress)
        {
            bool walletExists = await _context.Users
                .AnyAsync(u => u.Id != userId && u.WalletAddress == walletAddress);
            if (walletExists)
            {
                throw new SubscriptionException(ExceptionResult.UserAddressConflict409, $"Wallet address:{walletAddress} already in use by another user");
            }

            User user = await GetUserByIdAsync(userId, dbTracking: true);
            user.WalletAddress = walletAddress;
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<User>> GetUsersWithMetricsAsync(CancellationToken cancellationToken)
        {
            var users = await _context.Users
                .AsNoTracking()
                .AsSplitQuery()
                .Select(user => new
                {
                    User = user,
                    ActiveSubscriptions = user.Subscriptions
                        .Where(s => s.IsActive && s.Product != null && s.Product.RoleId.HasValue)
                        .Select(s => new
                        {
                            ProductRoleId = (ulong)s.Product!.RoleId!
                        })
                        .ToList(),
                    InactiveProductKeys = user.ProductKeys
                        .Where(pk => !pk.IsActivated && pk.ProductOption != null && pk.ProductOption.Product.RoleId.HasValue)
                        .Select(pk => new
                        {
                            ProductRoleId = (ulong)pk.ProductOption.Product.RoleId!
                        })
                        .ToList()
                })
                .ToListAsync(cancellationToken);

            foreach (var data in users)
            {
                data.User.Metrics = new Metrics
                {
                    ActiveSubsPerProduct = data.ActiveSubscriptions
                        .GroupBy(s => s.ProductRoleId)
                        .ToDictionary(
                            g => g.Key,
                            g => g.Count()
                        ),
                    InactiveKeysPerProduct = data.InactiveProductKeys
                        .GroupBy(pk => pk.ProductRoleId)
                        .ToDictionary(
                            g => g.Key,
                            g => g.Count()
                        )
                };
            }

            return users.Select(data => data.User).AsEnumerable();
        }
    }
}
