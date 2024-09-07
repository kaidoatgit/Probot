using Microsoft.EntityFrameworkCore;
using Probot.Data.Entities;
using Probot.SubscriptionApi.Exceptions;
using Probot.SubscriptionApi.Services.Services.IServices;
using Probot.Data;
using Probot.Shared.Dtos.ProRaffleSetting.Request;
using Probot.Shared.Dtos.Subscription.Request;
using Probot.Shared.Enums;

namespace Probot.SubscriptionApi.Services.Services
{
    public class SubscriptionService : ISubscriptionService
    {
        private readonly ProbotContext _context;
        private readonly IProRaffleSettingService _proRaffleSettingService;
        private readonly IProductKeyService _productKeyService;

        public SubscriptionService(ProbotContext context, IProRaffleSettingService proRaffleSettingService, IProductKeyService productKeyService)
        {
            _context = context;
            _proRaffleSettingService = proRaffleSettingService;
            _productKeyService = productKeyService;
        }


        public async Task<Subscription> CreateSubscriptionAsync(SubscriptionRequest request)
        {
            ProductKey productKey = await _productKeyService.GetProductKeyByCodeAsync(request.Code, isActivated: false, includeReferences: true);
            if (productKey.UserId != request.UserId)
            {
                throw new ServiceException(StatusCodes.Status400BadRequest, ServiceResult.ProductKey400, 
                    $"Product Key with {request.Code} does not belong to the user.");
            }

            DateTime currentDate = DateTime.UtcNow;
            var subscription =  new Subscription
            {
                StartDate = currentDate,
                EndDate = currentDate.AddMonths(productKey.Period),
                UserId = productKey.UserId,
                Username = productKey.User.Username,
                ProductId = productKey.ProductOption.ProductId,
                Code = productKey.Code
            };

            using var dbTransaction = await _context.Database.BeginTransactionAsync();
            try
            {
                if(request.ProductSettingRequest != null)
                {                
                    ProductSetting setting = request.ProductSettingRequest switch
                    {
                        ProRaffleSettingRequest proRaffleSettingRequest => await _proRaffleSettingService.CreateSettingsAsync(request.UserId, proRaffleSettingRequest),
                        _ => throw new ServiceException(StatusCodes.Status400BadRequest, ServiceResult.ProductSetting400, "Unsupported product setting")
                    };
                    subscription.ProductSettingId = setting.Id;
                }
                else
                {
                    bool isSubscriptionExists = await _context.Subscriptions
                        .AnyAsync(s => s.UserId == request.UserId && s.ProductId == productKey.ProductOption.ProductId);
                    if(isSubscriptionExists)
                    {
                        throw new ServiceException(StatusCodes.Status409Conflict, ServiceResult.Subscription409, "A subscription for this product already exists.");
                    }
                }

                productKey.IsActivated = true;
                productKey.Version = Guid.NewGuid();

                _context.Subscriptions.Add(subscription);
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

        public async Task<Subscription> ExtendSubscriptionAsync(SubscriptionRequest request)
        {
            ProductKey productKey = await _productKeyService.GetProductKeyByCodeAsync(request.Code, isActivated: false, includeReferences: true);
            if (productKey.UserId != request.UserId)
            {
                throw new ServiceException(StatusCodes.Status400BadRequest, ServiceResult.ProductKey400, 
                    $"Product Key with code:{request.Code} does not belong to the user.");
            }
            
            (Subscription? existingSubscription, ProductSetting? productSetting) = await GetExistingSubscriptionAsync(request, productKey);
            if(existingSubscription == null)
            {
                throw new ServiceException(StatusCodes.Status404NotFound, ServiceResult.Subscription404, "Subscription not found or does not belong to you");
            }

            DateTime currentDate = DateTime.UtcNow;
            var subscription = new Subscription
            {
                Version = existingSubscription.Version + 1,
                StartDate = existingSubscription.IsActive ? existingSubscription.StartDate : currentDate,
                EndDate = existingSubscription.IsActive ? existingSubscription.EndDate.AddMonths(productKey.Period) : currentDate.AddMonths(productKey.Period),
                UserId = existingSubscription.UserId,
                Username = existingSubscription.Username,
                ProductId = existingSubscription.ProductId,
                Code = productKey.Code,
                ProductSettingId = productSetting?.Id
            };
            _context.Subscriptions.Add(subscription);
            existingSubscription.IsActive = false;

            productKey.IsActivated = true;
            productKey.Version = Guid.NewGuid();

            await _context.SaveChangesAsync();
            return subscription;
        }

        private async Task<(Subscription?, ProductSetting?)> GetExistingSubscriptionAsync(SubscriptionRequest request, ProductKey productKey)
        {
            if (request.ProductSettingRequest == null)
            {
                return (await _context.Subscriptions
                    .Where(s => s.UserId == request.UserId && s.ProductId == productKey.ProductOption.ProductId)
                    .OrderByDescending(s => s.Version)
                    .FirstOrDefaultAsync(), null);
            }
            else
            {                
                ProductSetting? setting = request.ProductSettingRequest switch
                {
                    ProRaffleSettingRequest proRaffleSettingRequest => await _proRaffleSettingService.GetSettingsAsync(request.UserId, proRaffleSettingRequest),
                    _ => null
                };

                return (setting?.Subscriptions
                    .OrderByDescending(s => s.Version)
                    .FirstOrDefault(), setting);
            }
        }
        
        public async Task<IEnumerable<Subscription>> GetSubscriptionsAsync(ulong userId, ProductName productName)
        {
            IQueryable<Subscription> query = _context.Subscriptions
                .AsNoTracking()
                .Include(s => s.ProductSetting)
                .Where(s => s.UserId == userId);

            if(productName == ProductName.ProRaffle)
            {
                query = query.Where(s => s.ProductSetting is ProRaffleSetting);
            }            
            
            var subscriptions = await query
                .GroupBy(s => s.ProductSettingId)
                .Select(g => g.OrderByDescending(s => s.Version).FirstOrDefault())
                .ToListAsync();

            return subscriptions.Where(s => s != null)!;
        }
    }
}
