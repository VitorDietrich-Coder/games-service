using Games.Microservice.Infrastructure.Interfaces;
using Games.Microservice.Infrastructure.Messaging;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

public sealed class RabbitMqEventBus : IEventBus
{
    private readonly IRabbitMqConnection _connection;

    public RabbitMqEventBus(IRabbitMqConnection connection)
    {
        _connection = connection;
    }

    public Task PublishAsync<TEvent>(TEvent @event, string routingKey)
        where TEvent : class
    {
        using var channel = _connection.CreateChannel();

        channel.ExchangeDeclare(
            exchange: "games.events",
            type: ExchangeType.Topic,
            durable: true);

        var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(@event));

        channel.BasicPublish(
            exchange: "games.events",
            routingKey: routingKey,
            basicProperties: null,
            body: body);

        return Task.CompletedTask;
    }
}
