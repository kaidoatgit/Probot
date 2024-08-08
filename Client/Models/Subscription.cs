using Newtonsoft.Json;
using ProPayments.Client.Clients.ProPayments.Dtos;

namespace ProPayments.Client.Models
{
    public class Subscription
    {
        public ulong UserId { get; set; }
        public ulong PlanRoleId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this, Formatting.None);
        }
    }
}
