using Probot.Shared.Enums;

namespace Probot.Client.Models
{
    public class Product
    {
        public int Id { get; set; }
        public ulong? RoleId { get; set; }
        public ProductName Name { get; set; }
        public string Description { get; set; } = string.Empty;
        public List<ProductOption> ProductOptions { get; set; } = new();

        public decimal GetPrice(int period)
        {
            return ProductOptions.FirstOrDefault(d => d.Period == period)?.Price ?? 0;
        }

        public int GetProductOptionId(int period)
        {
            return ProductOptions.First(po => po.Period == period).Id;
        }
    }
}
