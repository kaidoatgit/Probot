using Microsoft.EntityFrameworkCore;
using ProPayments.Service.Data;
using ProPayments.Service.Data.Entities;
using ProPayments.Service.Data.Entities.Enums;
using ProPayments.Service.Dtos.ProRaffles.Request;
using ProPayments.Service.Dtos.UserSettings.Request;
using ProPayments.Service.Exceptions;
using ProPayments.Service.Services.Services.IServices;

namespace ProPayments.Service.Services.Services
{
    public class UserSettingService : IUserSettingService
    {
        private readonly SubscriptionContext _context;
        private readonly IProductKeyService _productKeyService;
        private readonly ISubscriptionService _subscriptionService;

        public UserSettingService(SubscriptionContext context, IProductKeyService productKeyService, ISubscriptionService subscriptionService)
        {
            _context = context;
            _productKeyService = productKeyService;
            _subscriptionService = subscriptionService;
        }

        public async Task<UserSetting> CreateUserSettingAsync(UserSettingRequest request)
        {   
            ProductKey productKey = await _productKeyService.GetProductKeyAsync(request.Code, request.UserId, isActivated: false, includeReferences: true);
            UserSetting? userSetting = null!;
            bool isNewSetting = false;

            using var dbTransaction = await _context.Database.BeginTransactionAsync();
            try
            {
                productKey.IsActivated = true;
                productKey.Version = Guid.NewGuid();
                
                IQueryable<UserSetting> query = _context.UserSettings.Include(us => us.Subscription);
                switch(productKey.ProductOption.Product.Name)
                {
                    case ProductName.ProRaffle:
                    {
                        var proRaffleRequest = (ProRaffleRequest)request;
                        userSetting = await query
                            .OfType<ProRaffle>()
                            .FirstOrDefaultAsync(prs => prs.Key == proRaffleRequest.AlphabotKey);
                       
                        if(userSetting != null && userSetting.UserId != request.UserId)
                        {
                            throw new ServiceException(StatusCodes.Status409Conflict, $"Activation failed for Alphabot Key: `{proRaffleRequest.AlphabotKey}`");
                        }
                        if(userSetting == null)
                        {
                            isNewSetting = true;
                            userSetting = new ProRaffle
                            {
                                Key = proRaffleRequest.AlphabotKey,
                                UserId = request.UserId
                            };
                            _context.UserSettings.Add(userSetting);
                            await _context.SaveChangesAsync();
                        }
                        break;
                    }
                }
                if(isNewSetting)
                {
                    await _subscriptionService.CreateSubscriptionAsync(productKey, userSetting.Id);
                    await _context.Entry(userSetting).Reference(ps => ps.Subscription).LoadAsync();
                }
                else
                {
                    Subscription subscription = userSetting.Subscription;
                    await _subscriptionService.ExtendSubscriptionAsync(subscription, productKey);
                }
                
                await dbTransaction.CommitAsync();
                return userSetting;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                await dbTransaction.RollbackAsync();
                throw;
            }
        }
    }
}
