using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProductService.Application;
using ProductService.Application.Create;
using Shared.Models;

namespace ProductService.Api.Endpoints;

public static class ProductEndpoint
{
    public static void RegisterProductEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("/api/products", [Authorize] async (string? color, [FromServices] IProductService productService) =>
        {
            if (string.IsNullOrEmpty(color))
            {
                var result = await productService.GetAllAsync();
                return ApiResponse.Ok(result);
            }
            else
            {
                var result = await productService.GetAllByColorAsync(color);
                return ApiResponse.FromResult(result, () => result.Value);
            }
        })
        .WithName("All Products")
        .WithDescription("Get all products")
        .Produces<ResponseResult>(400)
        .Produces<ResponseResult<ProductDto>>(200);

        app.MapPost("/api/products", [Authorize] async ([FromBody] CreateProductDto model, [FromServices] IProductService productService) =>
        {
            var result = await productService.CreateProductAsync(model);
            return ApiResponse.FromResult(result, () => result.Value);
        })
        .WithName("New Product")
        .WithDescription("Create a new Product")
        .Produces<ResponseResult>(400)
        .Produces<ResponseResult<Guid>>(200);
    }
}