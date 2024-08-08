using ProPayments.Client.Models.Enums;

namespace ProPayments.Client.Dtos.Plan.Response
{
    public class PlanWithOptionsResponse
    {
        public int Id { get; set; }
        public ulong? RoleId { get; set; }
        public PlanType Type { get; set; }
        public string? Description { get; set; }
        public List<PlanOptionResponse> PlanOptions { get; set; } = new();
    }

    public class PlanOptionResponse
    {
        public int Id { get; set; }
        public decimal Price { get; set; }
        public int Period { get; set; }
        public string PeriodDescription { get; set; } = string.Empty;
    }
}
