using ProPayments.Service.Data.Entities.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProPayments.Service.Data.Entities
{
    public class Plan
    {
        [Column(Order = 0)]
        public int Id { get; set; }
        [Column(Order = 1)]
        public ulong? RoleId { get; set; }
        [Column(Order = 2)]
        public PlanType Type { get; set; }
        [Column(Order = 3)]
        public string? Description { get; set; }


        public ICollection<PlanOption>? PlanOptions { get; set; }  //Navigation 
        public ICollection<Subscription>? Subscriptions { get; set; }  //Navigation purpose
    }
}
