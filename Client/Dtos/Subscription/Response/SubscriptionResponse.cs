
using ProPayments.Client.Dtos.UserSetting.Response;

namespace ProPayments.Client.Dtos.Subscription.Response
{
    public class SubscriptionResponse
    {
        public ulong UserId { get; set; }
        public string Code { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string PeriodDescription { get; set; }
        public ulong ProductRoleId { get; set; }

        public UserSettingResponse? UserSetting { get; set; }
    }
}
