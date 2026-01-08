using Games.Microservice.Application.EventConsumers;
using Games.Microservice.Infrastructure.Messaging;
using Games.Microservice.Infrastructure.Interfaces;
using Games.Microservice.Contracts;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

public sealed class PaymentCompletedRabbitConsumer : BackgroundService
{
    private readonly IEventBus _eventBus;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<PaymentCompletedRabbitConsumer> _logger;

    public PaymentCompletedRabbitConsumer(
        IEventBus eventBus,
        IServiceScopeFactory scopeFactory,
        ILogger<PaymentCompletedRabbitConsumer> logger)
    {
        _eventBus = eventBus;
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (_eventBus is not RabbitMqEventBus rabbitBus)
        {
            _logger.LogError("RabbitMqEventBus is not available. Consumer cannot start.");
            return Task.CompletedTask;
        }

        var channel = rabbitBus.Channel;

        // Declare exchange e queue (idempotente)
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

            var handler = scope.ServiceProvider.GetRequiredService<PaymentConfirmedConsumer>();

            var json = Encoding.UTF8.GetString(args.Body.ToArray());
            var @event = JsonSerializer.Deserialize<PaymentCompletedIntegrationEvent>(json);

            if (@event != null)
            {
                try
                {
                    await handler.Handle(@event);
                    channel.BasicAck(args.DeliveryTag, false);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error processing payment.completed event");
                    // Aqui você pode escolher requeue = true/false dependendo da lógica
                    channel.BasicNack(args.DeliveryTag, false, requeue: true);
                }
            }
            else
            {
                channel.BasicAck(args.DeliveryTag, false);
            }
        };

        channel.BasicConsume(
            queue: "payments.payment.completed",
            autoAck: false,
            consumer: consumer);

        _logger.LogInformation("PaymentCompletedRabbitConsumer is listening to payment.completed events.");

        return Task.CompletedTask;
    }
}
