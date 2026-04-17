using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using ProductService.Application;
using ProductService.Application.Create;
using ProductService.IntegrationTests.Config;

namespace ProductService.IntegrationTests;

public class ProductTest(ApiWebApplicationFactory factory) : BaseApiIntegrationTest(factory)
{
    private readonly IFixture fixture = new Fixture().Customize(new AutoMoqCustomization());

    [Fact]
    public async Task Get_ShouldBeDeniedAccessAsync()
    {
        var responseMessage = await GetFakesAsync("products");
        responseMessage.StatusCode.Should().Be(System.Net.HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Get_ShouldGetAllProductSuccessfully()
    {
        await ClearTableAsync("Products");
        Authenticate(Guid.NewGuid().ToString());

        var products = GetProducts(5);
        await SaveJsonFakeAsync(products, "products");

        var result = await GetFakesAsync<ProductDto>("products");
        result.Status.Should().Be("Success");
        result.Data.Should().HaveCount(products.Length);
    }

    [Theory]
    [InlineData("green", 3)]
    [InlineData("yellow", 2)]
    public async Task Get_ShouldGetAllProductForAValidColorSuccessfully(string color, int count)
    {
        Authenticate(Guid.NewGuid().ToString());

        var products = GetProducts(count, color);
        await SaveJsonFakeAsync(products, "products");

        var result = await GetFakesAsync<ProductDto>("products", new Dictionary<string, string> { { "color", color } });
        result.Status.Should().Be("Success");
        result.Data.Should().HaveCount(products.Length);
    }

    [Theory]
    [InlineData("gray")]
    [InlineData("orange")]
    public async Task Get_ShouldFailToGetAllProductForColorNotInDatabase(string color)
    {
        Authenticate(Guid.NewGuid().ToString());

        var products = GetProducts(5);
        await SaveJsonFakeAsync(products, "products");

        var result = await GetFakesAsync<ProductDto>("products", new Dictionary<string, string> { { "color", color } });
        result.Status.Should().Be("Fail");
        result.Message.Should().Be($"Products with color: '{color}' not found");
    }

    [Fact]
    public async Task Post_ShouldCreateProductSuccessfully()
    {
        Authenticate(Guid.NewGuid().ToString());

        var products = GetProducts(1);
        var result = await SaveJsonFakeAsync<CreateProductDto, Guid>(products[0], "products");

        result.Status.Should().Be("Success");
        result.Data.Should().NotBe(Guid.Empty);
    }

    [Fact]
    public async Task Post_ShouldFailToCreateProduct_WhenProductNameAlreadyExist()
    {
        Authenticate(Guid.NewGuid().ToString());

        var productName = $"Test_{Guid.NewGuid()}";
        var product = GetProducts(1, productName: productName);
        await SaveJsonFakeAsync<CreateProductDto, Guid>(product[0], "products");

        var newProduct = GetProducts(1, productName: productName);
        var result = await SaveJsonFakeAsync<CreateProductDto, string>(newProduct[0], "products");

        result.Status.Should().Be("Fail");
        result.Message.Should().Be($"Product with Name: '{productName}' already exist.");
    }

    [Fact]
    public async Task Post_ShouldFailToCreateProduct_WhenProductSkuAlreadyExist()
    {
        Authenticate(Guid.NewGuid().ToString());

        var productSku = $"Test_{Guid.NewGuid()}";
        var product = GetProducts(1, sku: productSku);
        await SaveJsonFakeAsync<CreateProductDto, Guid>(product[0], "products");

        var newProduct = GetProducts(1, sku: productSku);
        var result = await SaveJsonFakeAsync<CreateProductDto, string>(newProduct[0], "products");

        result.Status.Should().Be("Fail");
        result.Message.Should().Be($"Product with Sku: '{productSku}' already exist.");
    }

    private CreateProductDto[] GetProducts(int numberOfProducts, string color = "red", string productName = null, string sku = null)
    {
        var products = new CreateProductDto[numberOfProducts];
        for (int i = 0; i < products.Length; i++)
        {
            var product = fixture.Build<CreateProductDto>()
            .With(x => x.Color, color);

            if (!string.IsNullOrEmpty(productName))
            {
                product = product.With(x => x.Name, productName);
            }

            if (!string.IsNullOrEmpty(sku))
            {
                product = product.With(x => x.Sku, sku);
            }

            products[i] = product.Create();
        }

        return products;
    }

    private async Task ClearTableAsync(string tableName)
    {
        using (var conn = new Npgsql.NpgsqlConnection(factory.ConnectionString))
        {
            await conn.OpenAsync();
            var cmd = new Npgsql.NpgsqlCommand($"DELETE FROM public.\"{tableName}\"", conn);
            await cmd.ExecuteNonQueryAsync();
        }
    }
}
