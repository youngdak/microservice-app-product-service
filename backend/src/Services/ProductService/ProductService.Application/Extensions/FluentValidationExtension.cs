using FluentValidation.Results;

namespace ProductService.Application.Extensions;

public static class FluentValidationExtension
{
    public static string ValidationResultError(this ValidationResult validationResult)
    {
        return string.Join("\n", validationResult.Errors.Select(x => x.ErrorMessage));
    }
}