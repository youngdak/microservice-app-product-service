namespace ProductService.Api.Endpoints;

public static class EndpointModule
{
    public static void RegisterEndpoints(this IEndpointRouteBuilder app)
    {
        app.RegisterProductEndpoints();
        app.RegisterHealthEndpoints();
    }
}