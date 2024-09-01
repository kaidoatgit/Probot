using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Probot.Data.Entities;
using Probot.Shared.Enums;

namespace Probot.Data.Configurations;

public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.Property(p => p.Name)
        .HasConversion(
            pn => pn.ToString(),
            pn => (ProductName)Enum.Parse(typeof(ProductName), pn));

        builder.HasData(
            new Product
            { 
                Id = 1, 
                Name = ProductName.ProRaffle, 
                Description = "A tool which automates the process of registring in alphabot raffles"
            }
        );
    }
}
