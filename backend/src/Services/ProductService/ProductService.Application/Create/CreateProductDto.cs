using ProductService.Domain;

namespace ProductService.Application.Create;

public record CreateProductDto(string Name, string Color, string Description, string Sku, decimal Price)
{
    public static Product Product(CreateProductDto productDto)
    {
        return new Product
        {
            Name = productDto.Name,
            Color = Enum.Parse<ProductColor>(productDto.Color, true),
            Description = productDto.Description,
            Sku = productDto.Sku,
            Price = productDto.Price,
            CreatedAt = DateTime.UtcNow
        };
    }
}
