using Microsoft.EntityFrameworkCore;
using ProPayments.Service.Data;
using ProPayments.Service.Data.Entities;
using ProPayments.Service.Dtos.Users.Request;
using ProPayments.Service.Exceptions;
using ProPayments.Service.Mappers;
using ProPayments.Service.Services.Services.IServices;

namespace ProPayments.Service.Services.Services
{
    public class UserService : IUserService
    {
        private readonly SubscriptionContext _context;
        private readonly IProductKeyService _productKeyService;
        private readonly Mapper _mapper;

        public UserService(SubscriptionContext context, Mapper mapper, IProductKeyService productKeyService)
        {
            _context = context;
            _mapper = mapper;
            _productKeyService = productKeyService;
        }

        public async Task<User> CreateUserAsync(UserRequest request)
        {
            //.AsNoTracking() is not needed because is using AnyAsync which returns a bool and automatically wont be tracked
            bool foundUser = await _context.Users
                .AnyAsync(u => u.Id == request.Id);
            if (foundUser)
            {
                throw new ServiceException(StatusCodes.Status409Conflict, "User already exists");
            }

            User user = _mapper.MapToUserEntity(request);
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return user;
        }

        public async Task UpdateWalletAddressAsync(ulong userId, string walletAddress)
        {
            bool walletExists = await _context.Users
                .AnyAsync(u => u.WalletAddress == walletAddress && u.Id != userId);
            if (walletExists)
            {
                throw new ServiceException(StatusCodes.Status409Conflict, $"Wallet address:{walletAddress} already in use by another user");
            }

            User? user = await _context.Users.FindAsync(userId)
                ?? throw new ServiceException(StatusCodes.Status404NotFound, "User not found");

            user.WalletAddress = walletAddress;
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<User>> GetUsersWithSubscriptionsAsync()
        {
            var users = await _context.Users
                .AsNoTracking()
                .Include(u => u.Subscriptions.Where(s => s.IsActive))
                    .ThenInclude(s => s.ProductOption!)
                        .ThenInclude(po => po.Product)
                .ToListAsync();

            return users;
        }

        public async Task<User> GetUserByIdAsync(ulong userId)
        {
            return await _context.Users
                .AsNoTracking()
                .SingleOrDefaultAsync(u => u.Id == userId)
                ?? throw new ServiceException(StatusCodes.Status404NotFound, "User not found");
        }

        public async Task<IEnumerable<ProductKey>> GetProductKeysAsync(ulong userId, bool isActivated)
        {
            return await _productKeyService.GetProductKeysForUserAsync(userId, isActivated);
        }

        public async Task<ProductKey> GetProductKeyAsync(ulong userId, string code, bool isActivated)
        {
            ProductKey productKey = await _productKeyService.GetProductKeyByCodeAsync(code, isActivated);
            if(productKey.UserId != userId)
            {
                throw new ServiceException(StatusCodes.Status400BadRequest, "Product Key not found or already activated.");
            }
            return productKey;
        }

        public async Task<HashSet<User>> GetUsersAsync(CancellationToken cancellationToken)
        {
            var users = await _context.Users
                .AsNoTracking()
                .Select(user => new
                {
                    User = user,
                    ActiveSubscriptions = user.Subscriptions
                        .Where(s => s.IsActive && s.ProductOption != null && s.ProductOption.Product.RoleId.HasValue)
                        .Select(s => new
                        {
                            ProductRoleId = (ulong)s.ProductOption!.Product.RoleId!
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
                data.User.Summary = new Summary
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

            return users.Select(data => data.User).ToHashSet();

            // var usersData = await _context.Users
            //     .AsNoTracking()
            //     .Select(user => new
            //     {
            //         UserId = user.Id,
            //         ActiveSubscriptions = user.Subscriptions
            //             .Where(s => s.IsActive && s.ProductOption != null && s.ProductOption.Product.RoleId.HasValue)
            //             .Select(s => new
            //             {
            //                 ProductRoleId = (ulong)s.ProductOption!.Product.RoleId!
            //             })
            //             .ToList(), // Materialize here

            //         InactiveProductKeys = user.ProductKeys
            //             .Where(pk => !pk.IsActivated && pk.ProductOption != null && pk.ProductOption.Product.RoleId.HasValue)
            //             .Select(pk => new
            //             {
            //                 ProductRoleId = (ulong)pk.ProductOption.Product.RoleId!
            //             })
            //             .ToList() // Materialize here
            //     })
            //     .ToListAsync(cancellationToken);

            // var userSummaries = usersData.Select(data => new UserSummary
            // {
            //     UserId = data.UserId,
            //     ActiveSubsPerProduct = data.ActiveSubscriptions
            //         .GroupBy(s => s.ProductRoleId)
            //         .ToDictionary(
            //             g => g.Key,
            //             g => g.Count()
            //         ),
            //     InactiveKeysPerProduct = data.InactiveProductKeys
            //         .GroupBy(pk => pk.ProductRoleId)
            //         .ToDictionary(
            //             g => g.Key,
            //             g => g.Count()
            //         )
            // }).ToList();

            // return userSummaries;
        }
    }
}
