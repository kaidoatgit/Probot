using Microsoft.EntityFrameworkCore;
using ProPayments.Service.Data;
using ProPayments.Service.Data.Entities;
using ProPayments.Service.Dtos.ProRaffles.Request;
using ProPayments.Service.Dtos.UserSettings.Request;
using ProPayments.Service.Services.Services.IServices;

namespace ProPayments.Service.Services.Services
{
    public class UserSettingService : IUserSettingService
    {
        private readonly SubscriptionContext _context;
        private readonly IProRaffleService _proRaffleService;

        public UserSettingService(SubscriptionContext context, IProRaffleService proRaffleService)
        {
            _context = context;
            _proRaffleService = proRaffleService;
        }   

        public async Task<(bool, TUserSetting)> CreateUserSettingAsync<TUserSetting>(UserSettingRequest request)
            where TUserSetting : UserSetting
        {
            bool isNewSetting = false;
            TUserSetting? userSetting = null!;
            
            if (typeof(TUserSetting) == typeof(ProRaffle))
            {
                var result = await _proRaffleService.CreateSettingsAsync((ProRaffleRequest)request);
                isNewSetting = result.IsNewSetting;
                userSetting = result.ProRaffle as TUserSetting;
            }
            return (isNewSetting, userSetting!);
        }
        
        // public async Task<(bool, UserSetting)> CreateUserSettingAsync(ProductName productName, UserSettingRequest request)
        // {
        //     bool isNewSetting = false;
        //     UserSetting? userSetting = null!;

        //     IQueryable<UserSetting> query = _context.UserSettings.Include(us => us.Subscriptions);
        //     switch(productName)
        //     {
        //         case ProductName.ProRaffle:
        //         {
        //             var proRaffleRequest = (ProRaffleRequest)request;
        //             ProRaffle? proRaffle = await query
        //                 .OfType<ProRaffle>()
        //                 .FirstOrDefaultAsync(prs => prs.Key == proRaffleRequest.AlphabotKey);

        //             if(proRaffle != null && proRaffle.UserId != request.UserId)
        //             {
        //                 throw new ServiceException(StatusCodes.Status409Conflict, $"Activation failed for Alphabot Key: `{proRaffleRequest.AlphabotKey}`");
        //             }
        //             if(proRaffle == null)
        //             {
        //                 isNewSetting = true;
        //                 proRaffle = new ProRaffle
        //                 {
        //                     Key = proRaffleRequest.AlphabotKey,
        //                     UserId = request.UserId
        //                 };
        //                 _context.ProRaffles.Add(proRaffle);
        //                 await _context.SaveChangesAsync();
        //             }
        //             else
        //             {
        //                 proRaffle.IsPaused = false;
        //                 proRaffle.Version = Guid.NewGuid();
        //             }
        //             userSetting = proRaffle;
        //             break;
        //         }
        //     }
        //     return (isNewSetting, userSetting);
        // }
    }
}
