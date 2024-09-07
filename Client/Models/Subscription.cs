using Newtonsoft.Json;

namespace Probot.Client.Models
{
    public class Subscription
    {
        public ulong UserId { get; set; }
        public string Code { get; set; }
        public ulong ProductRoleId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string PeriodDescription { get; set; }

        public ProductSetting? ProductSetting { get; set; }
        public override string ToString()
        {
            return JsonConvert.SerializeObject(this, Formatting.None);
        }
    }
}
