using Newtonsoft.Json;
using ProPayments.Client.Clients.ProPayments.Dtos;
using ProPayments.Client.Models.Enums;

namespace ProPayments.Client.Models
{
    public class Order
    {
        public ulong Id { get; set; }
        public User? User { get; set; } = null!;
        public Invoice? Invoice { get; set; } = null!;
        public Interaction? Interaction { get; set; }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this, Formatting.None);
        }
    }
}
