using ProductService.Domain;

namespace ProductService.Application;

public interface IProductRepository
{
    Task<bool> ProductExistByNameAsync(string name);
    Task<bool> ProductExistBySkuAsync(string sku);
    Task<Guid> SaveProductAsync(Product product);
    Task<IEnumerable<Product>> GetAllAsync();
    Task<IEnumerable<Product>> GetAllByColorAsync(ProductColor color);
}
