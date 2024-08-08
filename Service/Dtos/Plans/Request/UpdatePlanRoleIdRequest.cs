using ProPayments.Service.Data.Entities.Enums;

namespace ProPayments.Service.Dtos.Plans.Request
{
    public class UpdatePlanRoleIdRequest
    {
        public ulong? RoleId { get; set; }
        public PlanType Type { get; set; }
    }
}
