using AutoFixture;
using AutoFixture.AutoMoq;
using FluentValidation.TestHelper;
using ProductService.Application.Create;
using ProductService.Domain;

namespace ProductService.UnitTests;

public class ProductValidatorTests
{
    private readonly CreateProductDtoValidator _validator = new CreateProductDtoValidator();
    private readonly IFixture fixture = new Fixture().Customize(new AutoMoqCustomization());

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public void ShouldHaveErrorWhenNameIsNullOrEmpty(string? name)
    {
        var model = fixture.Build<CreateProductDto>()
            .With(x => x.Color, ProductColor.Blue.ToString())
            .With(x => x.Name, name).Create();

        var result = _validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(x => x.Name);
    }

    [Fact]
    public void ShouldNotHaveErrorWhenNameIsNotNullOrEmpty()
    {
        var model = fixture.Build<CreateProductDto>()
            .With(x => x.Color, ProductColor.Blue.ToString()).Create();

        var result = _validator.TestValidate(model);

        result.ShouldNotHaveValidationErrorFor(x => x.Name);
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public void ShouldHaveErrorWhenDescriptionIsNullOrEmpty(string? description)
    {
        var model = fixture.Build<CreateProductDto>()
            .With(x => x.Color, ProductColor.Blue.ToString())
            .With(x => x.Description, description).Create();

        var result = _validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(x => x.Description);
    }

    [Fact]
    public void ShouldNotHaveErrorWhenDescriptionIsNotNullOrEmpty()
    {
        var model = fixture.Build<CreateProductDto>()
            .With(x => x.Color, ProductColor.Blue.ToString()).Create();

        var result = _validator.TestValidate(model);

        result.ShouldNotHaveValidationErrorFor(x => x.Description);
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public void ShouldHaveErrorWhenSkuIsNullOrEmpty(string? sku)
    {
        var model = fixture.Build<CreateProductDto>()
            .With(x => x.Color, ProductColor.Blue.ToString())
            .With(x => x.Sku, sku).Create();

        var result = _validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(x => x.Sku);
    }

    [Fact]
    public void ShouldNotHaveErrorWhenSkuIsNotNullOrEmpty()
    {
        var model = fixture.Build<CreateProductDto>()
            .With(x => x.Color, ProductColor.Blue.ToString()).Create();

        var result = _validator.TestValidate(model);

        result.ShouldNotHaveValidationErrorFor(x => x.Sku);
    }

    [Theory]
    [InlineData(-1)]
    public void ShouldHaveErrorWhenPriceIsLessThanZero(decimal price)
    {
        var model = fixture.Build<CreateProductDto>()
            .With(x => x.Color, ProductColor.Blue.ToString())
            .With(x => x.Price, price).Create();

        var result = _validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(x => x.Price);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(100)]
    public void ShouldNotHaveErrorWhenPriceIsGreaterThanOrEqualToZero(decimal price)
    {
        var model = fixture.Build<CreateProductDto>()
            .With(x => x.Color, ProductColor.Blue.ToString())
            .With(x => x.Price, price).Create();

        var result = _validator.TestValidate(model);

        result.ShouldNotHaveValidationErrorFor(x => x.Price);
    }

    [Theory]
    [InlineData("gray")]
    [InlineData("orange")]
    public void ShouldHaveErrorWhenColorIsNotValid(string color)
    {
        var model = fixture.Build<CreateProductDto>()
            .With(x => x.Color, color).Create();

        var result = _validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(x => x.Color);
    }

    [Theory]
    [InlineData("red")]
    [InlineData("blue")]
    public void ShouldNotHaveErrorWhenColorIsValid(string color)
    {
        var model = fixture.Build<CreateProductDto>()
            .With(x => x.Color, color).Create();

        var result = _validator.TestValidate(model);

        result.ShouldNotHaveValidationErrorFor(x => x.Color);
    }
}
