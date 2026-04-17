namespace Shared.Events;

public class ProductCreatedEvent : EventBase
{
    public Guid ProductId { get; set; }
    public string Name { get; set; }
    public string Sku { get; set; }
    public decimal Price { get; set; }
}