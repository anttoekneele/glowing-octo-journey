using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

public class LifecycleEventConsumer : BackgroundService
{
    private readonly IConnection _connection;
    private readonly IServiceScopeFactory _scopeFactory;

    public LifecycleEventConsumer(IConnection connection, IServiceScopeFactory scopeFactory)
    {
        _connection = connection;
        _scopeFactory = scopeFactory;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var channel = _connection.CreateModel();
        channel.ExchangeDeclare("comm-lifecycle", ExchangeType.Topic, durable: true);
        var queue = channel.QueueDeclare().QueueName;

        channel.QueueBind(queue, "comm-lifecycle", "#");

        var consumer = new EventingBasicConsumer(channel);
        consumer.Received += async (model, ea) =>
        {
            var body = ea.Body.ToArray();
            var json = Encoding.UTF8.GetString(body);
            var eventType = ea.RoutingKey;
            var payload = JsonSerializer.Deserialize<EventPayloadDto>(json);

            if (payload == null)
            {
                return;
            }

            if (!Guid.TryParse(payload.CommunicationId, out var communicationGuid))
            {
                return;
            }

            using var scope = _scopeFactory.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            var comm = await db.Communications.FindAsync(communicationGuid);
            if (comm != null)
            {
                comm.CurrentStatus = eventType;
                comm.LastUpdatedUtc = payload.TimestampUtc;

                db.CommunicationStatusHistories.Add(new CommunicationStatusHistory
                {
                    CommunicationId = comm.Id,
                    StatusCode = eventType,
                    OccurredUtc = payload.TimestampUtc
                });

                await db.SaveChangesAsync();
            }
        };

        channel.BasicConsume(queue, autoAck: true, consumer: consumer);
        return Task.CompletedTask;
    }
}
