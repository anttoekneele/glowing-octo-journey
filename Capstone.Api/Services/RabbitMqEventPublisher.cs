using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

public interface IEventPublisher
{
    Task PublishEventAsync(string eventType, EventPayloadDto payload);
}

public class RabbitMqEventPublisher : IEventPublisher
{
    private readonly IConnection _connection;

    public RabbitMqEventPublisher(IConnection connection)
    {
        _connection = connection;
    }

    public Task PublishEventAsync(string eventType, EventPayloadDto payload)
    {
        using var channel = _connection.CreateModel();
        var exchangeName = "comm-lifecycle";

        channel.ExchangeDeclare(exchange: exchangeName, type: ExchangeType.Topic, durable: true);

        var json = JsonSerializer.Serialize(payload);
        var body = Encoding.UTF8.GetBytes(json);

        var routingKey = eventType;

        channel.BasicPublish(exchange: exchangeName, routingKey: routingKey, basicProperties: null, body: body);
        return Task.CompletedTask;
    }
}
