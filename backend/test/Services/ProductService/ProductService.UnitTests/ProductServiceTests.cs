using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Moq;
using ProductService.Application;
using ProductService.Application.Create;
using ProductService.Domain;
using Shared.Kafka;

namespace ProductService.UnitTests;

public class ProductServiceTests
{
    private readonly IProductService productService;
    private readonly Mock<IProductRepository> productRepository = new Mock<IProductRepository>();
    private readonly IFixture fixture = new Fixture().Customize(new AutoMoqCustomization());
    public ProductServiceTests()
    {
        productService = new Application.ProductService(productRepository.Object, new FakeKafkaProducer());
    }

    [Fact]
    public async Task ShouldGetAllProductSuccessfully()
    {
        var products = new List<Product>
        {
            fixture.Create<Product>(),
            fixture.Create<Product>(),
            fixture.Create<Product>(),
            fixture.Create<Product>(),
            fixture.Create<Product>()
        };

        productRepository.Setup(x => x.GetAllAsync()).ReturnsAsync(products);

        var result = await productService.GetAllAsync();

        result.Should().NotBeNull();
        result.Should().NotBeEmpty();
        result.Should().HaveCount(products.Count);
    }

    [Theory]
    [InlineData("red")]
    [InlineData("blue")]
    public async Task ShouldGetAllProductForAValidColorSuccessfully(string color)
    {
        var products = new List<Product>
        {
            fixture.Build<Product>().With(x => x.Color, ProductColor.Red).Create(),
            fixture.Build<Product>().With(x => x.Color, ProductColor.Red).Create(),
            fixture.Build<Product>().With(x => x.Color, ProductColor.Red).Create(),
            fixture.Build<Product>().With(x => x.Color, ProductColor.Blue).Create(),
            fixture.Build<Product>().With(x => x.Color, ProductColor.Blue).Create(),
        };

        var productColor = Enum.Parse<ProductColor>(color, true);

        productRepository.Setup(x => x.GetAllByColorAsync(productColor)).ReturnsAsync(products.Where(x => x.Color == productColor));

        var result = await productService.GetAllByColorAsync(color);

        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(products.Count(x => x.Color == productColor));
    }

    [Theory]
    [InlineData("gray")]
    [InlineData("orange")]
    public async Task ShouldFailToGetAllProductForAnInValidColor(string color)
    {
        var result = await productService.GetAllByColorAsync(color);

        result.Should().NotBeNull();
        result.IsFailure.Should().BeTrue();
        result.Message.Should().Be($"Products with color: '{color}' not found");
    }

    [Theory]
    [InlineData("yellow")]
    [InlineData("green")]
    public async Task ShouldFailToGetAllProductForColorNotInDatabase(string color)
    {
        var products = new List<Product>
        {
            fixture.Build<Product>().With(x => x.Color, ProductColor.Red).Create(),
            fixture.Build<Product>().With(x => x.Color, ProductColor.Red).Create(),
            fixture.Build<Product>().With(x => x.Color, ProductColor.Red).Create(),
            fixture.Build<Product>().With(x => x.Color, ProductColor.Blue).Create(),
            fixture.Build<Product>().With(x => x.Color, ProductColor.Blue).Create(),
        };

        var productColor = Enum.Parse<ProductColor>(color, true);

        productRepository.Setup(x => x.GetAllByColorAsync(productColor)).ReturnsAsync(products.Where(x => x.Color == productColor));

        var result = await productService.GetAllByColorAsync(color);

        result.Should().NotBeNull();
        result.IsFailure.Should().BeTrue();
        result.Message.Should().Be($"Products with color: '{color}' not found");
    }

    [Fact]
    public async Task ShouldCreateProductSuccessfully()
    {
        var model = fixture.Build<CreateProductDto>()
            .With(x => x.Color, ProductColor.Blue.ToString()).Create();

        var productId = fixture.Create<Guid>();

        productRepository.Setup(x => x.SaveProductAsync(It.IsAny<Product>())).ReturnsAsync(productId);

        var result = await productService.CreateProductAsync(model);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(productId);
    }

    [Fact]
    public async Task ShouldFailToCreateProduct_WhenProductNameAlreadyExist()
    {
        var model = fixture.Build<CreateProductDto>()
            .With(x => x.Color, ProductColor.Blue.ToString()).Create();

        productRepository.Setup(x => x.ProductExistByNameAsync(model.Name)).ReturnsAsync(true);

        var result = await productService.CreateProductAsync(model);

        result.IsFailure.Should().BeTrue();
        result.Message.Should().Be($"Product with Name: '{model.Name}' already exist.");
    }

    [Fact]
    public async Task ShouldFailToCreateProduct_WhenProductSkuAlreadyExist()
    {
        var model = fixture.Build<CreateProductDto>()
            .With(x => x.Color, ProductColor.Blue.ToString()).Create();

        productRepository.Setup(x => x.ProductExistBySkuAsync(model.Sku)).ReturnsAsync(true);

        var result = await productService.CreateProductAsync(model);

        result.IsFailure.Should().BeTrue();
        result.Message.Should().Be($"Product with Sku: '{model.Sku}' already exist.");
    }
}
