using ProductService.Application.Create;
using ProductService.Application.Extensions;
using ProductService.Domain;
using Shared.Models;
using Confluent.Kafka;
using Shared.Kafka;
using Shared.Events;


namespace ProductService.Application;

public class ProductService(IProductRepository productRepository, IKafkaProducer kafkaProducer) : IProductService
{
    public async Task<ResponseResult<Guid>> CreateProductAsync(CreateProductDto dto)
    {
        var validator = new CreateProductDtoValidator();
        var validationResult = await validator.ValidateAsync(dto);
        if (!validationResult.IsValid)
        {
            return ResponseResult.Fail<Guid>(validationResult.ValidationResultError());
        }

        var productExistByName = await productRepository.ProductExistByNameAsync(dto.Name);
        if (productExistByName)
        {
            return ResponseResult.Fail<Guid>($"Product with Name: '{dto.Name}' already exist.");
        }

        var productExistBySku = await productRepository.ProductExistBySkuAsync(dto.Sku);
        if (productExistBySku)
        {
            return ResponseResult.Fail<Guid>($"Product with Sku: '{dto.Sku}' already exist.");
        }

        var product = CreateProductDto.Product(dto);

        var productId = await productRepository.SaveProductAsync(product);

        var productCreatedEvent = new ProductCreatedEvent
        {
            ProductId = product.Id,
            Name = product.Name,
            Price = product.Price
        };

        await kafkaProducer.ProduceAsync("product.created", productCreatedEvent);

        return ResponseResult.Ok(productId);
    }

    public async Task<IEnumerable<ProductDto>> GetAllAsync()
    {
        var products = await productRepository.GetAllAsync();
        return products.Select(ProductDto.CreateProductDto);
    }

    public async Task<ResponseResult<IEnumerable<ProductDto>>> GetAllByColorAsync(string color)
    {
        var message = $"Products with color: '{color}' not found";
        if (!Enum.TryParse(color, true, out ProductColor productColor))
        {
            return ResponseResult.Fail<IEnumerable<ProductDto>>(message);
        }

        var products = await productRepository.GetAllByColorAsync(productColor);
        if (!products.Any())
        {
            return ResponseResult.Fail<IEnumerable<ProductDto>>(message);
        }

        var productDtos = products.Select(ProductDto.CreateProductDto);

        return ResponseResult.Ok(productDtos);
    }
}
