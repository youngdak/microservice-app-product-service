
using System.Diagnostics;
using System.Text.Json;
using Microsoft.IdentityModel.Tokens;
using ProductService.Api.Endpoints;

namespace ProductService.Middleware;

public sealed class ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
{
    private readonly RequestDelegate _next = next;
    private readonly ILogger<ExceptionMiddleware> _logger = logger;
    private readonly JsonSerializerOptions _jsonSerializerOptions = new() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var errorId = Activity.Current?.Id ?? context.TraceIdentifier;

        // Log exception with proper logging framework
        _logger.LogError(exception,
            "Unhandled exception occurred. ErrorId: {ErrorId}, Path: {Path}",
            errorId, context.Request.Path);

        var error = $"Contact admin with the error code: {errorId}";
        var result = Serialize(ApiResponseResult<string>.Fail(error));
        var statusCode = 500;

        if (exception is SecurityTokenException)
        {
            statusCode = 401;
            result = Serialize(ApiResponseResult<string>.Fail("Login required"));
        }
        else
        {
            _logger.LogError(exception, "Unhandled exception type: {ExceptionType}", exception.GetType().Name);
        }

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = statusCode;
        await context.Response.WriteAsync(result);
    }

    private string Serialize(object responseBody)
    {
        return JsonSerializer.Serialize(responseBody, _jsonSerializerOptions);
    }
}
