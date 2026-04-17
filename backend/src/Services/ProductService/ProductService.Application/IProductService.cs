using ProductService.Application.Create;
using Shared.Models;

namespace ProductService.Application;

public interface IProductService
{
    Task<ResponseResult<Guid>> CreateProductAsync(CreateProductDto dto);
    Task<IEnumerable<ProductDto>> GetAllAsync();
    Task<ResponseResult<IEnumerable<ProductDto>>> GetAllByColorAsync(string color);
}
