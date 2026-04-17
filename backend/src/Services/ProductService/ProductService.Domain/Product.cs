namespace ProductService.Domain;

public class Product : EntityBase
{
    public string Name { get; set; } = default!;
    public ProductColor Color { get; set; } = default!;
    public string Description { get; set; } = default!;
    public string Sku { get; set; } = default!;
    public decimal Price { get; set; }
}