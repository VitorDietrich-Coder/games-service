using Games.Microservice.Infrastructure.Interfaces;
using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;
using Users.Microservice.Domain.Core.Events;

namespace Games.Microservice.Infrastructure.Messaging;

public sealed class RabbitMqEventBus : IEventBus, IDisposable
{
    private readonly IConnection _connection;
    public readonly IModel _channel;
    public IModel Channel => _channel; 

    public RabbitMqEventBus(IConfiguration configuration)
    {
        var connectionString = configuration["RabbitMq:ConnectionString"];

        if (string.IsNullOrWhiteSpace(connectionString))
            throw new InvalidOperationException(
                "RabbitMq:ConnectionString is not configured.");

        var factory = new ConnectionFactory
        {
            Uri = new Uri(connectionString),
            DispatchConsumersAsync = true
        };

        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();
    }

    public Task PublishAsync<TEvent>(TEvent @event, string queueName)
        where TEvent : class
    {
        _channel.ExchangeDeclare(
           exchange: "games.events",
           type: ExchangeType.Topic,
           durable: true);

        var body = Encoding.UTF8.GetBytes(
            JsonSerializer.Serialize(@event));

        var properties = _channel.CreateBasicProperties();
        properties.Persistent = true;

        if (@event is DomainEvent domainEvent &&
             !string.IsNullOrWhiteSpace(domainEvent.CorrelationId))
        {
            properties.Headers ??= new Dictionary<string, object>();

            properties.Headers["X-Correlation-Id"] =
                Encoding.UTF8.GetBytes(domainEvent.CorrelationId);

            Console.WriteLine(domainEvent.CorrelationId);
        }

        _channel.BasicPublish(
            exchange: "games.events",
            routingKey: "game.purchased",
            basicProperties: properties,
            body: body);

        return Task.CompletedTask;
    }

    public void Dispose()
    {
        if (_channel.IsOpen)
            _channel.Close();

        if (_connection.IsOpen)
            _connection.Close();
    }
}
