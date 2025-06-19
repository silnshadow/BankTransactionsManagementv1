using Confluent.Kafka;
using System.Text.Json;

public class KafkaProducerService
{
    private readonly IProducer<Null, string> _producer;
    private readonly string _topic = "user-login-events";

    public KafkaProducerService()
    {
        var config = new ProducerConfig { BootstrapServers = "localhost:9092" };
        _producer = new ProducerBuilder<Null, string>(config).Build();
    }

    public async Task SendUserLoginEventAsync(UserLoginEvent userEvent)
    {
        var message = JsonSerializer.Serialize(userEvent);
        await _producer.ProduceAsync(_topic, new Message<Null, string> { Value = message });
    }
}

public class UserLoginEvent
{
    public string UserId { get; set; }
    public string Email { get; set; }
    public DateTime LoginTime { get; set; }
}