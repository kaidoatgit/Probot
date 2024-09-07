using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Probot.Data.Entities;

namespace Probot.Data.Configurations;

public class ProductOptionConfiguration : IEntityTypeConfiguration<ProductOption>
{
    public void Configure(EntityTypeBuilder<ProductOption> builder)
    {
       builder.HasData(
            new ProductOption
            {
                Id = 1, ProductId = 1, Price = 12m, Period = 1, PeriodDescription = "1 Month"
            },
            new ProductOption
            {
                Id = 2, ProductId = 1, Price = 20m, Period = 2, PeriodDescription = "2 Months" 
            },
            new ProductOption
            {
                Id = 3, ProductId = 1, Price = 30m, Period = 3, PeriodDescription = "3 Months" 
            }
        );
    }
}
