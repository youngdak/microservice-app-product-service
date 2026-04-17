using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace ProductService.Api.Endpoints;

public static class HealthEndpoint
{
    public static void RegisterHealthEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("api/healthcheck", async ([FromServices] HealthCheckService healthCheckService) =>
        {
            var report = await healthCheckService.CheckHealthAsync();
            return report.Status == HealthStatus.Healthy
                ? Results.Ok(report)
                : Results.Json(statusCode: StatusCodes.Status503ServiceUnavailable, data: report);
        })
        .WithName("Check API health")
        .WithDescription("Returns the current health status of the application.");
    }
}