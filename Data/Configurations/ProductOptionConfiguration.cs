using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Probot.Data.Entities;
using Probot.Shared.Enums;

namespace Probot.Data.Configurations;

public class ProductOptionConfiguration : IEntityTypeConfiguration<ProductOption>
{
    public void Configure(EntityTypeBuilder<ProductOption> builder)
    {
        builder.Property(p => p.PeriodType)
            .HasConversion<string>();

        builder.HasData(
            new ProductOption
            {
                Id = 1, ProductId = 1, Price = 10m, Period = 1, PeriodType = PeriodType.Month, PeriodDescription = "1 Month"
            },
            new ProductOption
            {
                Id = 2, ProductId = 1, Price = 19m, Period = 2, PeriodType = PeriodType.Month, PeriodDescription = "2 Months" 
            },
            new ProductOption
            {
                Id = 3, ProductId = 1, Price = 28m, Period = 3, PeriodType = PeriodType.Month, PeriodDescription = "3 Months" 
            },
            new ProductOption
            {
                Id = 4, ProductId = 1, Price = 0m, Period = 7, PeriodType = PeriodType.Day, PeriodDescription = "7 Days", IsActive = false
            },
            new ProductOption
            {
                Id = 5, ProductId = 1, Price = 0m, Period = 15, PeriodType = PeriodType.Day, PeriodDescription = "15 Days", IsActive = false
            }
        );
    }
}
