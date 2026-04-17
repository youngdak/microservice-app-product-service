using Microsoft.EntityFrameworkCore;
using ProductService.Application;
using ProductService.Domain;

namespace ProductService.Infrastructure;

public class ProductRepository(ProductDbContext context) : IProductRepository
{
    public async Task<IEnumerable<Product>> GetAllAsync()
    {
        return await context.Products.AsNoTracking().ToListAsync();
    }

    public async Task<IEnumerable<Product>> GetAllByColorAsync(ProductColor color)
    {
        return await context.Products.Where(x => x.Color == color).AsNoTracking().ToListAsync();
    }

    public async Task<bool> ProductExistByNameAsync(string name)
    {
        return await context.Products.AnyAsync(x => x.Name.ToLower() == name.ToLower());
    }

    public async Task<bool> ProductExistBySkuAsync(string sku)
    {
        return await context.Products.AnyAsync(x => x.Sku.ToLower() == sku.ToLower());
    }

    public async Task<Guid> SaveProductAsync(Product product)
    {
        context.Products.Add(product);
        await context.SaveChangesAsync();

        return product.Id;
    }
}