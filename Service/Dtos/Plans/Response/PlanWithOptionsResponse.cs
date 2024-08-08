using ProPayments.Service.Data.Entities.Enums;

namespace ProPayments.Service.Dtos.Plans.Response
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
        public int PlanId { get; set; }
        public decimal Price { get; set; }
        public int Period { get; set; }
        public string PeriodDescription { get; set; } = string.Empty;
    }
}
