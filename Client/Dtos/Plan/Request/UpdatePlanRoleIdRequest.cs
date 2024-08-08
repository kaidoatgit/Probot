using ProPayments.Client.Models.Enums;

namespace ProPayments.Client.Dtos.Plan.Request
{
    public class UpdatePlanRoleIdRequest
    {
        public ulong? RoleId { get; set; }
        public PlanType Type { get; set; }
    }
}
