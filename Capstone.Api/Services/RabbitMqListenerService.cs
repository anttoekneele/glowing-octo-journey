using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

public class RabbitMqListenerService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<RabbitMqListenerService> _logger;
    private IConnection? _connection;
    private IModel? _channel;

    public RabbitMqListenerService(IServiceProvider serviceProvider, ILogger<RabbitMqListenerService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;

        var factory = new ConnectionFactory
        {
            HostName = "localhost", // or "rabbitmq" if using Docker service name
            UserName = "guest",
            Password = "guest"
        };

        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();

        _channel.ExchangeDeclare(exchange: "communication_events", type: ExchangeType.Topic, durable: true);

        _channel.QueueDeclare(queue: "communication_api_queue",
                             durable: true,
                             exclusive: false,
                             autoDelete: false,
                             arguments: null);

        _channel.QueueBind(queue: "communication_api_queue",
                          exchange: "communication_events",
                          routingKey: "#");
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var consumer = new EventingBasicConsumer(_channel);

        consumer.Received += async (model, ea) =>
        {
            var routingKey = ea.RoutingKey;
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);

            _logger.LogInformation($"Received message: {routingKey} -> {message}");

            try
            {
                var payload = JsonSerializer.Deserialize<CommunicationEventMessage>(message);

                if (payload is not null)
                {
                    using var scope = _serviceProvider.CreateScope();
                    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                    var communication = await db.Communications.FindAsync(Guid.Parse(payload.CommunicationId));
                    if (communication != null)
                    {
                        communication.CurrentStatus = routingKey;
                        communication.LastUpdatedUtc = DateTime.UtcNow;

                        db.CommunicationStatusHistories.Add(new CommunicationStatusHistory
                        {
                            Id = Guid.NewGuid(),
                            CommunicationId = communication.Id,
                            StatusCode = routingKey,
                            OccurredUtc = DateTime.UtcNow
                        });

                        await db.SaveChangesAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing message.");
            }

            _channel.BasicAck(ea.DeliveryTag, multiple: false);
        };

        _channel.BasicConsume(queue: "communication_api_queue",
                              autoAck: false,
                              consumer: consumer);

        return Task.CompletedTask;
    }

    public override void Dispose()
    {
        _channel?.Dispose();
        _connection?.Dispose();
        base.Dispose();
    }
}
