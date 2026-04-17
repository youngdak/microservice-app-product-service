using ProductService.Domain;

namespace ProductService.Application;

public record ProductDto(string Id, string Name, string Color, string Description, string Sku, decimal Price)
{
    public static ProductDto CreateProductDto(Product product)
    {
        return new ProductDto(product.Id.ToString(), product.Name, product.Color.ToString(), product.Description, product.Sku, product.Price);
    }
}
