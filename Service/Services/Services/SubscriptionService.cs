using Microsoft.EntityFrameworkCore;
using ProPayments.Service.Data;
using ProPayments.Service.Data.Entities;
using ProPayments.Service.Mappers;
using ProPayments.Service.Services.Services.IServices;

namespace ProPayments.Service.Services.Services
{
    public class SubscriptionService : ISubscriptionService
    {
        private readonly SubscriptionContext _context;
        private readonly Mapper _mapper;
        private readonly IUserService _userService;
        private readonly IProductKeyService _productKeyService;

        public SubscriptionService(SubscriptionContext context, Mapper mapper, IUserService userService, IProductKeyService productKeyService)
        {
            _context = context;
            _mapper = mapper;
            _userService = userService;
            _productKeyService = productKeyService;
        }

        public async Task CreateSubscriptionAsync(ProductKey productKey, ulong userSettingId)
        {
            DateTime currentDate = DateTime.UtcNow;
            User user = productKey.User;
            Subscription subscription = new()
            {
                StartDate = currentDate,
                EndDate = currentDate.AddMonths(productKey.Period),
                UserId = user.Id,
                Username = user.Username,
                ProductOptionId = productKey.ProductOptionId,
                Code = productKey.Code,
                UserSettingId = userSettingId
            };
            _context.Subscriptions.Add(subscription);
            await _context.SaveChangesAsync();
        }

        public async Task ExtendSubscriptionAsync(Subscription subscription, ProductKey productKey)
        {
            DateTime currentDate = DateTime.UtcNow;
            if (subscription.IsActive)
            {
                subscription.EndDate = subscription.EndDate.AddMonths(productKey.Period);
            }
            else
            {
                subscription.IsActive = true;
            }
            subscription.UpdatedAt = currentDate;
            subscription.StartDate = currentDate;
            subscription.ProductOptionId = productKey.ProductOptionId;
            subscription.Code = productKey.Code;
            _context.Subscriptions.Update(subscription);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Subscription>> GetSubscriptionsOfTypeAsync<TUserSetting>(ulong userId, bool isActive) where TUserSetting : UserSetting
        {
             var subscriptions = await _context.Subscriptions
                .Include(s => s.UserSetting)
                .Where(s => s.UserId == userId && s.IsActive == isActive)
                .Where(s => s.UserSetting is TUserSetting)
                .ToListAsync();

            return subscriptions;
        }

        // public async Task<IEnumerable<Subscription>> GetSubscriptionsOfTypeAsync(ulong userId, bool isActive) where TUserSetting : UserSetting;
        // {
        //     var subscriptions = await _context.Subscriptions
        //         .Include(s => s.UserSetting)
        //         .Where(s => s.UserId == userId && s.IsActive == isActive)
        //         .Where(sb)
        //         .ToListAsync();
        //     return subscriptions;
        //     // var proRaffles = await _context.ProRaffles
        //     //     .Include(s => s.Subscription)
        //     //     .Where(s => s.UserId == userId && s.Subscription.IsActive == isActive)
        //     //     .ToListAsync();
        //     // return proRaffles;
        // }
        // public async Task<Subscription> CreateSubscriptionAsync(SubscriptionRequest request)
        // {   
        //     var productKey = await _context.ProductKeys
        //         .Include(pk => pk.User)
        //         .Include(pk => pk.ProductOption.Product)
        //         .FirstOrDefaultAsync(pk => pk.Code == request.Code) 
        //         ?? throw new ServiceException(StatusCodes.Status404NotFound, "Product key not found.");
        //     if(productKey.IsActivated) throw new ServiceException(StatusCodes.Status409Conflict, $"Code {request.Code} has already been activated.");
        //     productKey.IsActivated = true;

        //     UserSetting? userSetting = null;
        //     switch(productKey.ProductOption.Product.Name)
        //     {
        //         case ProductName.ProRaffle:
        //         {
        //             userSetting = await _context.ProRaffles
        //                 .Include(prs => prs.Subscription)
        //                 .FirstOrDefaultAsync(prs => prs.Key == request.AlphabotKey);   
        //             break;
        //         }
        //     }

        //     DateTime currentDate = DateTime.UtcNow;
        //     User user = productKey.User;
        //     Subscription? subscription = null;
        //     if(userSetting == null)
        //     {
        //         subscription = new Subscription
        //         {
        //             StartDate = currentDate,
        //             EndDate = currentDate.AddMonths(productKey.Period),
        //             UserId = user.Id,
        //             Username = user.Username,
        //             ProductOptionId = productKey.ProductOptionId,
        //             Code = productKey.Code
        //         };

        //         _context.Subscriptions.Add(subscription);

        //         userSetting = new ProRaffle{
        //             Key = request.AlphabotKey
        //         };
        //         _context.UserSettings.Add(userSetting);
        //         subscription.UserSetting = userSetting;
        //         await _context.SaveChangesAsync();
        //     }

        //     return subscription!;

        //     // var existingSubscription = await _context.Subscriptions
        //     //     .Include(s => s.AlphabotProcess)
        //     //     .Where(s => s.UserId == request.UserId && s.IsActive && s.AlphabotProcess.AlphabotKey == request.AlphabotKey)
        //     //    .FirstOrDefaultAsync();

        //     // var subscription = existingSubscription;
        //     // DateTime currentDate = DateTime.UtcNow;
        //     // if (subscription != null)
        //     // {
        //     //     if (subscription.IsActive)
        //     //     {
        //     //         subscription.EndDate = subscription.EndDate.add(ac.ProductPeriod);
        //     //     }
        //     //     else
        //     //     {
        //     //         // subscription.EndDate = currentDate.AddMonths(invoice.ProductPeriod);
        //     //         subscription.IsActive = true;
        //     //     }
        //     //     subscription.UpdatedAt = currentDate;
        //     //     subscription.StartDate = currentDate;
        //     //     subscription.LastNotificationCheck = currentDate;
        //     //     subscription.Username = invoice.Username;
        //     //     // subscription.ProductOptionId = invoice.ProductOptionId;

        //     //     _context.Subscriptions.Update(subscription);
        //     //     await _context.SaveChangesAsync();
        //     // }
        //     // else
        //     // {
        //     //     subscription = new Subscription
        //     //     {
        //     //         StartDate = currentDate,
        //     //         // EndDate = currentDate.AddMonths(invoice.ProductPeriod),
        //     //         UserId = userId,
        //     //         Username = invoice.Username,
        //     //         // ProductId = invoice.ProductId,
        //     //         // ProductOptionId = invoice.ProductOptionId,
        //     //     };
        //     //     _context.Subscriptions.Add(subscription);
        //     //     await _context.SaveChangesAsync();

        //     //     //explicity load the reference
        //     //     await _context.Entry(subscription).Reference(s => s.Product).LoadAsync();
        //     // }


        //     // DateTime startDate = DateTime.UtcNow;
        //     // Subscription subscription = new()
        //     // {
        //     //     StartDate = startDate,
        //     //     EndDate = startDate.AddMonths(productKey.Period),
        //     //     UserId = request.UserId,
        //     //     Code = request.Code,
        //     // };

        //     // _context.Subscriptions.Add(subscription);
        //     // await _context.SaveChangesAsync();

        //     //explicity load the reference
        //     // await _context.Entry(subscription).Reference(s => s.AlphabotProcess).LoadAsync();

        //     // try
        //     // {
        //     //     _ = await _userService.CreateUserAsync(new(){
        //     //         Id = request.UserId
        //     //     });
        //     // }
        //     // catch (ServiceException ex)
        //     // {
        //     //     if(ex.StatusCode == StatusCodes.Status409Conflict)
        //     //     {

        //     //     }
        //     // }

        // }

        // public async Task<Subscription> CreateFreeSubscriptionAsync(SubscriptionRequest request)
        // {
        //     var user = await _context.Users.FindAsync(request.UserId);
        //     if (user == null) throw new ServiceException(StatusCodes.Status404NotFound, "User not found");

        //     var productOption = await _context.ProductOptions
        //         .Where(po => po.Id == request.ProductOptionId)
        //         .Include(po => po.Product)
        //         .Include(po => po.Subscriptions)
        //         .FirstOrDefaultAsync();

        //     if (productOption == null) throw new ServiceException(StatusCodes.Status404NotFound, "Product option not found");
        //     if (productOption.Product != null && productOption.Product.Type != ProductName.Free)
        //         throw new ServiceException(StatusCodes.Status400BadRequest, $"Invalid product option: {productOption.Product.Type}");

        //     if (productOption.Subscriptions!.Any(s => s.UserId == user.Id))
        //     {
        //         throw new ServiceException(StatusCodes.Status409Conflict, $"Free Subscriptions already exists for user: {user.Username}");
        //     }

        //     DateTime startDate = DateTime.UtcNow;
        //     Subscription subscription = new()
        //     {
        //         StartDate = startDate,
        //         EndDate = startDate.AddDays(productOption.Period),
        //         UserId = user.Id,
        //         ProductOptionId = productOption.Id,
        //         ProductId = productOption.ProductId
        //     };

        //     _context.Subscriptions.Add(subscription);
        //     await _context.SaveChangesAsync();
        //     return subscription;
        // }

        // public async Task<Subscription> CreatePaidSubscriptionAsync(ulong userId, Invoice invoice)
        // {
        //     var existingSubscription = await _context.Subscriptions
        //         .Include(s => s.Product)
        //         .Where(s => s.UserId == userId && s.ProductId == 1)//invoice.ProductId)
        //        .FirstOrDefaultAsync();

        //     var subscription = existingSubscription;
        //     DateTime currentDate = DateTime.UtcNow;
        //     if (subscription != null)
        //     {
        //         if (subscription.IsActive)
        //         {
        //             // subscription.EndDate = subscription.EndDate.AddMonths(invoice.ProductPeriod);
        //         }
        //         else
        //         {
        //             // subscription.EndDate = currentDate.AddMonths(invoice.ProductPeriod);
        //             subscription.IsActive = true;
        //         }
        //         subscription.UpdatedAt = currentDate;
        //         subscription.StartDate = currentDate;
        //         subscription.LastNotificationCheck = currentDate;
        //         subscription.Username = invoice.Username;
        //         // subscription.ProductOptionId = invoice.ProductOptionId;

        //         _context.Subscriptions.Update(subscription);
        //         await _context.SaveChangesAsync();
        //     }
        //     else
        //     {
        //         subscription = new Subscription
        //         {
        //             StartDate = currentDate,
        //             // EndDate = currentDate.AddMonths(invoice.ProductPeriod),
        //             UserId = userId,
        //             Username = invoice.Username,
        //             // ProductId = invoice.ProductId,
        //             // ProductOptionId = invoice.ProductOptionId,
        //         };
        //         _context.Subscriptions.Add(subscription);
        //         await _context.SaveChangesAsync();

        //         //explicity load the reference
        //         await _context.Entry(subscription).Reference(s => s.Product).LoadAsync();
        //     }

        //     return subscription;
        // }


    }
}
