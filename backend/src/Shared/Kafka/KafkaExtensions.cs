using Microsoft.Extensions.DependencyInjection;

namespace Shared.Kafka;

public static class KafkaExtensions
{
    public static IServiceCollection AddKafka(this IServiceCollection services)
    {
        services.AddSingleton<IKafkaProducer, FakeKafkaProducer>();

        return services;
    }
}