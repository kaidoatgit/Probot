using ProPayments.Client.Models.Enums;

namespace ProPayments.Client.Models
{
    public class Plan
    {
        public int Id { get; set; }
        public ulong? RoleId { get; set; }
        public PlanType Type { get; set; }
        public string Description { get; set; } = string.Empty;
        public List<PlanOption> PlanOptions { get; set; } = new();

        public decimal GetPrice(int period)
        {
            return PlanOptions.FirstOrDefault(d => d.Period == period)?.Price ?? 0;
        }

        public int GetPlanOptionId(int period)
        {
            return PlanOptions.First(po => po.Period == period).Id;
        }
    }
}
