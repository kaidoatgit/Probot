using ProPayments.Service.Data.Entities.Enums;

namespace ProPayments.Service.Dtos.Plans.Response
{
    public class PlanResponse
    {
        public int Id { get; set; }
        public ulong? RoleId { get; set; }
        public PlanType Type { get; set; }
        public string? Description { get; set; }
    }
}
