using Microsoft.Extensions.DependencyInjection;
using Shared.Kafka;

namespace ProductService.Application;


public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddTransient<IProductService, ProductService>();
        services.AddKafka();

        return services;
    }
}