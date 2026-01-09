using Games.Microservice.Application.EventConsumers;
using Games.Microservice.Contracts;
using Games.Microservice.Infrastructure.Messaging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

public sealed class PaymentCompletedRabbitConsumer : BackgroundService
{
    private readonly IRabbitMqConnection _connection;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<PaymentCompletedRabbitConsumer> _logger;

    public PaymentCompletedRabbitConsumer(
        IRabbitMqConnection connection,
        IServiceScopeFactory scopeFactory,
        ILogger<PaymentCompletedRabbitConsumer> logger)
    {
        _connection = connection;
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var channel = _connection.CreateChannel();

        channel.ExchangeDeclare(
            exchange: "payments.events",
            type: ExchangeType.Topic,
            durable: true);

        channel.QueueDeclare(
            queue: "payments.payment.completed",
            durable: true,
            exclusive: false,
            autoDelete: false);

        channel.QueueBind(
            queue: "payments.payment.completed",
            exchange: "payments.events",
            routingKey: "payment.completed");

        var consumer = new AsyncEventingBasicConsumer(channel);

        consumer.Received += async (_, args) =>
        {
            using var scope = _scopeFactory.CreateScope();
            var handler = scope.ServiceProvider
                .GetRequiredService<PaymentConfirmedConsumer>();

            var json = Encoding.UTF8.GetString(args.Body.ToArray());
            var @event = JsonSerializer.Deserialize<PaymentCompletedIntegrationEvent>(json);

            if (@event != null)
                await handler.Handle(@event);

            channel.BasicAck(args.DeliveryTag, false);
        };

        channel.BasicConsume(
            queue: "payments.payment.completed",
            autoAck: false,
            consumer: consumer);

        _logger.LogInformation("PaymentCompletedRabbitConsumer started");

        return Task.CompletedTask;
    }
}
