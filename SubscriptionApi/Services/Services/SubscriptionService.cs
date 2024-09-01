using Microsoft.EntityFrameworkCore;
using Probot.Data.Entities;
using Probot.SubscriptionApi.Exceptions;
using Probot.SubscriptionApi.Services.Services.IServices;
using Probot.Shared.Dtos.UserSetting.Request;
using Probot.Data;

namespace Probot.SubscriptionApi.Services.Services
{
    public class SubscriptionService : ISubscriptionService
    {
        private readonly ProbotContext _context;
        private readonly IUserSettingService _userSettingService;
        private readonly IProductKeyService _productKeyService;

        public SubscriptionService(ProbotContext context, IUserSettingService userSettingService, IProductKeyService productKeyService)
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
                throw new ServiceException(StatusCodes.Status400BadRequest, $"Product Key with {request.Code} does not belong to the user or does not exist.");
            }

            using var dbTransaction = await _context.Database.BeginTransactionAsync();
            try
            {
                productKey.IsActivated = true;
                (bool isNewSetting, TUserSetting userSetting) = await _userSettingService.GetOrCreateUserSettingAsync<TUserSetting>(request);
                Subscription subscription = CreateSubscriptionAsync(productKey, isNewSetting, userSetting);
                
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

        private Subscription CreateSubscriptionAsync(ProductKey productKey, bool isNewSetting, UserSetting userSetting, int version = 1)
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

        
        public async Task<IEnumerable<Subscription>> GetSubscriptionsAsync(bool? onlyActives = null)
        {
            IQueryable<Subscription> query =  _context.Subscriptions;
            if(onlyActives.HasValue)
            {
                query = query.Where(s => s.IsActive);
            }
            return await query
                .Include(s => s.UserSetting)
                .Include(s => s.ProductOption)
                    .ThenInclude(po => po.Product)
                .ToListAsync();
        }
    }
}
