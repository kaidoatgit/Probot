using Microsoft.EntityFrameworkCore;
using ProPayments.Service.Data;
using ProPayments.Service.Data.Entities;
using ProPayments.Service.Dtos.UserSettings.Request;
using ProPayments.Service.Exceptions;
using ProPayments.Service.Services.Services.IServices;

namespace ProPayments.Service.Services.Services
{
    public class SubscriptionService : ISubscriptionService
    {
        private readonly SubscriptionContext _context;
        private readonly IUserSettingService _userSettingService;
        private readonly IProductKeyService _productKeyService;

        public SubscriptionService(SubscriptionContext context, IUserSettingService userSettingService, IProductKeyService productKeyService)
        {
            _context = context;
            _userSettingService = userSettingService;
            _productKeyService = productKeyService;
        }

        public async Task<Subscription> CreateSubscriptionOfTypeAsync<TUserSetting>(UserSettingRequest request) where TUserSetting : UserSetting
        {
            ProductKey productKey = await _productKeyService.GetProductKeyByCodeAsync(request.Code, isActivated: false, includeReferences: true);
            if(productKey.UserId != request.UserId)
            {
                throw new ServiceException(StatusCodes.Status400BadRequest, "Product Key not found or already activated.");
            }

            using var dbTransaction = await _context.Database.BeginTransactionAsync();
            try
            {
                productKey.IsActivated = true;
                (bool isNewSetting, TUserSetting userSetting) = await _userSettingService.CreateUserSettingAsync<TUserSetting>(request);
                Subscription subscription = CreateOrExtendSubscriptionAsync(productKey, isNewSetting, userSetting);
                productKey.Version = Guid.NewGuid();

                await _context.SaveChangesAsync();
                await dbTransaction.CommitAsync();
                return subscription;
            }
            catch (Exception)
            {
                await dbTransaction.RollbackAsync();
                throw;
            }
        }

        // public async Task<Subscription> CreateSubscriptionOfTypeAsync<TUserSetting>(UserSettingRequest request) where TUserSetting : UserSetting
        // {
        //     ProductKey productKey = await _productKeyService.GetProductKeyAsync(request.Code, request.UserId, isActivated: false, includeReferences: true);
        //     using var dbTransaction = await _context.Database.BeginTransactionAsync();
        //     try
        //     {
        //         productKey.IsActivated = true;
        //         (bool isNewSetting, UserSetting userSetting) = await _userSettingService.CreateUserSettingAsync(productKey.ProductOption.Product.Name, request);
        //         Subscription subscription = CreateOrExtendSubscriptionAsync(productKey, isNewSetting, userSetting);
        //         productKey.Version = Guid.NewGuid();

        //         await _context.SaveChangesAsync();
        //         await dbTransaction.CommitAsync();
        //         return subscription;
        //     }
        //     catch (Exception)
        //     {
        //         await dbTransaction.RollbackAsync();
        //         throw;
        //     }
        // }

        public async Task<IEnumerable<Subscription>> GetSubscriptionsOfTypeAsync<TUserSetting>(ulong userId) where TUserSetting : UserSetting
        {
            var subscriptions = await _context.Subscriptions
                .AsNoTracking()
                .Where(s => s.UserId == userId && s.UserSetting is TUserSetting)
                .Include(s => s.UserSetting)
                .GroupBy(s => s.UserSettingId)
                .Select(g => g.OrderByDescending(s => s.Version).FirstOrDefault())
                .ToListAsync();
            
            return subscriptions.Where(s => s != null)!;
        }

        private Subscription CreateOrExtendSubscriptionAsync(ProductKey productKey, bool isNewSetting, UserSetting userSetting, int version = 1)
        {
            Subscription? subscription = null!;
            DateTime currentDate = DateTime.UtcNow;
            User user = productKey.User;

            if(isNewSetting)
            {
                subscription = new()
                {
                    Version = version,
                    StartDate = currentDate,
                    EndDate = currentDate.AddMonths(productKey.Period),
                    UserId = user.Id,
                    Username = user.Username,
                    ProductOptionId = productKey.ProductOptionId,
                    Code = productKey.Code,
                    UserSettingId = userSetting.Id
                };
            }
            else
            {
                Subscription oldSubscription = userSetting.Subscriptions.OrderByDescending(s => s.Version).First();
                version = oldSubscription.Version + 1;

                if(oldSubscription.IsActive)
                {
                    subscription = new()
                    {
                        Version = version,
                        StartDate = oldSubscription.StartDate,
                        EndDate = oldSubscription.EndDate.AddMonths(productKey.Period),
                        UserId = user.Id,
                        Username = user.Username,
                        ProductOptionId = productKey.ProductOptionId,
                        Code = productKey.Code,
                        UserSettingId = userSetting.Id
                    };
                }
                else
                {
                    subscription = new()
                    {
                        Version = version,
                        StartDate = currentDate,
                        EndDate = currentDate.AddMonths(productKey.Period),
                        UserId = user.Id,
                        Username = user.Username,
                        ProductOptionId = productKey.ProductOptionId,
                        Code = productKey.Code,
                        UserSettingId = userSetting.Id
                    };
                }
                oldSubscription.IsActive = false;
            }
            _context.Subscriptions.Add(subscription);
            return subscription;
        }
    
        public async Task<Dictionary<ulong, Dictionary<ulong, int>>> GetUsersActiveSubsCountPerProduct(CancellationToken cancellationToken)
        {
            var usersActiveSubsCountByProduct = await _context.Subscriptions
                .AsNoTracking()
                .Where(s => s.IsActive)
                .Include(s => s.ProductOption)
                    .ThenInclude(po => po!.Product)
                .GroupBy(s => new { s.UserId, s.ProductOption!.Product.RoleId })
                .Select(g => new
                {
                    UserId = g.Key.UserId,
                    RoleId = g.Key.RoleId,
                    SubscriptionCount = g.Count()
                })
                .ToListAsync(cancellationToken);

            var result = usersActiveSubsCountByProduct
                .GroupBy(s => s.UserId)
                .ToDictionary(
                    g => g.Key,
                    g => g.ToDictionary(x => x.RoleId ?? 0, x => x.SubscriptionCount)
                );

            return result;
        }
    }
}
