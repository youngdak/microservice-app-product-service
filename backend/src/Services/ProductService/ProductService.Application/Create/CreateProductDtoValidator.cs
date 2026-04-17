using FluentValidation;
using ProductService.Domain;

namespace ProductService.Application.Create;

public class CreateProductDtoValidator : AbstractValidator<CreateProductDto>
{
    public CreateProductDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .NotNull();
        RuleFor(x => x.Description).NotEmpty().NotNull();
        RuleFor(x => x.Sku).NotEmpty().NotNull();
        RuleFor(x => x.Price).GreaterThanOrEqualTo(0);
        RuleFor(x => x.Color).Must((color) =>
        {
            return Enum.TryParse(color, true, out ProductColor _);
        }).WithMessage($"The product color is not valid. Color must be one of these: {string.Join(", ", Enum.GetNames<ProductColor>())}");
    }
}