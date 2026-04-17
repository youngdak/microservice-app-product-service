using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProductService.Domain;

namespace ProductService.Infrastructure;

public class ProductConfiguration : EntityBaseConfiguration<Product>
{
    public override void Configure(EntityTypeBuilder<Product> builder)
    {
        base.Configure(builder);
        builder.ToTable(nameof(ProductDbContext.Products), schema: "public");

        builder.Property(product => product.Name)
            .HasMaxLength(250)
            .IsRequired();
        builder.HasIndex(product => product.Name)
            .IsUnique();

        builder.Property(product => product.Sku)
            .IsRequired()
            .HasMaxLength(50);
        builder.HasIndex(product => product.Sku)
            .IsUnique();

        builder.Property(product => product.Color)
            .HasConversion<string>()
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(product => product.Price)
            .IsRequired();

        builder.Property(product => product.Description)
            .IsRequired();
    }
}