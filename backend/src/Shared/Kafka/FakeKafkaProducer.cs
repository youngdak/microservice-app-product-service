using System.Text.Json;

namespace Shared.Kafka;

public class FakeKafkaProducer : IKafkaProducer
{
    public async Task ProduceAsync<T>(string topic, T message)
    {
        var data = JsonSerializer.Serialize<T>(message);
        Console.WriteLine($"{topic} - {data}");
    }
}