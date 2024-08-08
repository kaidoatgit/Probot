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
        private readonly Mapper _mapper;

        public UserService(SubscriptionContext context, Mapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<User> CreateUserAsync(UserRequest request)
        {
            User? user = await _context.Users.FindAsync(request.Id);
            if (user != null)
            {
                throw new ServiceException(StatusCodes.Status409Conflict, "User already exists");
            }

            user = _mapper.MapToUserEntity(request);
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return user;
        }

        public async Task UpdateWalletAddressAsync(ulong userId, string walletAddress)
        {
            User? userToUpdate = await _context.Users.FindAsync(userId);
            if (userToUpdate == null)
                throw new ServiceException(StatusCodes.Status404NotFound, "User not found");

            bool walletExists = await _context.Users
                .AsNoTracking()
                .AnyAsync(u => u.WalletAddress == walletAddress && u.Id != userId);
            if (walletExists)
                throw new ServiceException(StatusCodes.Status409Conflict, $"Wallet address:{walletAddress} already in use by another user");

            userToUpdate.WalletAddress = walletAddress;
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<User>> GetUsersWithSubscriptionsAsync()
        {
            var users = await _context.Users
                .Include(u => u.Subscriptions!.Where(s => s.IsActive))
                    .ThenInclude(s => s.Plan)
                .ToListAsync();

            return users;
        }

        public async Task<User> GetUserByIdAsync(ulong userId)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null) throw new ServiceException(StatusCodes.Status404NotFound, "User not found");
            return user;
        }
    }
}
