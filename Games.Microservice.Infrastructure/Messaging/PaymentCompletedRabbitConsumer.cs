using System.Text;
using System.Text.Json;
using Games.Microservice.Application.EventConsumers;
using Games.Microservice.Contracts;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

public sealed class PaymentCompletedRabbitConsumer : BackgroundService
{
    private readonly IConfiguration _configuration;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<PaymentCompletedRabbitConsumer> _logger;

    private IConnection _connection = default!;
    private IModel _channel = default!;

    public PaymentCompletedRabbitConsumer(
        IConfiguration configuration,
        IServiceScopeFactory scopeFactory,
        ILogger<PaymentCompletedRabbitConsumer> logger)
    {
        _configuration = configuration;
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
       
        var factory = new ConnectionFactory
        {
            Uri = new Uri(_configuration["RabbitMq:ConnectionString"]),
            DispatchConsumersAsync = true
        };

        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();

        _channel.ExchangeDeclare(
            exchange: "payments.events",
            type: ExchangeType.Topic,
            durable: true);

        _channel.QueueDeclare(
            queue: "payments.payment.completed",
            durable: true,
            exclusive: false,
            autoDelete: false);

        _channel.QueueBind(
            queue: "payments.payment.completed",
            exchange: "payments.events",
            routingKey: "payment.completed");

        var consumer = new AsyncEventingBasicConsumer(_channel);

        consumer.Received += async (_, args) =>
        {
            using var scope = _scopeFactory.CreateScope();

            var handler = scope.ServiceProvider
                .GetRequiredService<PaymentConfirmedConsumer>();

            var json = Encoding.UTF8.GetString(args.Body.ToArray());
            var @event =
                JsonSerializer.Deserialize<PaymentCompletedIntegrationEvent>(json);

            await handler.Handle(@event!);

            _channel.BasicAck(args.DeliveryTag, false);
        };

        _channel.BasicConsume(
            queue: "payments.payment.completed",
            autoAck: false,
            consumer: consumer);

        _logger.LogInformation(
            "Listening to payment.completed events");

        return Task.CompletedTask;
    }

    public override void Dispose()
    {
        if (_channel.IsOpen)
            _channel.Close();

        if (_connection.IsOpen)
            _connection.Close();

        base.Dispose();
    }
}
