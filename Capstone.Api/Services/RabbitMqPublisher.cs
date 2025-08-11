using System.Text;
using System.Text.Json;
using RabbitMQ.Client;
using Capstone.Api.Contracts;

namespace Capstone.Api.Services
{
    public class RabbitMqPublisher
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<RabbitMqPublisher> _logger;

        public RabbitMqPublisher(IConfiguration configuration, ILogger<RabbitMqPublisher> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public void Publish(string eventName, CommunicationEvent communicationEvent)
        {
            var factory = new ConnectionFactory()
            {
                HostName = _configuration["RabbitMq:HostName"] ?? "localhost",
                UserName = _configuration["RabbitMq:UserName"] ?? "guest",
                Password = _configuration["RabbitMq:Password"] ?? "guest"
            };

            using var connection = factory.CreateConnection();
            using var channel = connection.CreateModel();

            // Declare exchange (fanout by default)
            channel.ExchangeDeclare(exchange: "communication_events", type: ExchangeType.Fanout, durable: true);

            var json = JsonSerializer.Serialize(communicationEvent);
            var body = Encoding.UTF8.GetBytes(json);

            var properties = channel.CreateBasicProperties();
            properties.ContentType = "application/json";

            // Publish message
            channel.BasicPublish(
                exchange: "communication_events",
                routingKey: eventName, // fanout ignores this, but good for future direct/topic use
                basicProperties: properties,
                body: body);

            _logger.LogInformation("Published event {EventName}: {Message}", eventName, json);
        }
    }
}
