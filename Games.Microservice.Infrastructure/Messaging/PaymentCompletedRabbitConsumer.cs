using Games.Microservice.Application.EventConsumers;
using Games.Microservice.Contracts;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

public sealed class PaymentCompletedRabbitConsumer : BackgroundService
{
    private readonly IConfiguration _configuration;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<PaymentCompletedRabbitConsumer> _logger;

    private IConnection? _connection;
    private IModel? _channel;

    public PaymentCompletedRabbitConsumer(
        IConfiguration configuration,
        IServiceScopeFactory scopeFactory,
        ILogger<PaymentCompletedRabbitConsumer> logger)
    {
        _configuration = configuration;
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var factory = new ConnectionFactory
        {
            Uri = new Uri(_configuration["RabbitMq:ConnectionString"]),
            DispatchConsumersAsync = true
        };

        int attempts = 0;
        const int maxAttempts = 10;

        while (attempts < maxAttempts && !stoppingToken.IsCancellationRequested)
        {
            try
            {
                _connection = factory.CreateConnection();
                _channel = _connection.CreateModel();
                _logger.LogInformation("Connected to RabbitMQ successfully!");
                break;
            }
            catch (RabbitMQ.Client.Exceptions.BrokerUnreachableException ex)
            {
                attempts++;
                _logger.LogWarning(
                    "RabbitMQ not reachable (attempt {Attempt}/{MaxAttempts}): {Message}",
                    attempts, maxAttempts, ex.Message);

                await Task.Delay(3000, stoppingToken);
            }
        }

        if (_connection == null || _channel == null)
        {
            _logger.LogError("Could not connect to RabbitMQ after {MaxAttempts} attempts", maxAttempts);
            return;
        }

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

            var handler = scope.ServiceProvider.GetRequiredService<PaymentConfirmedConsumer>();

            var json = Encoding.UTF8.GetString(args.Body.ToArray());
            var @event = JsonSerializer.Deserialize<PaymentCompletedIntegrationEvent>(json);

            if (@event != null)
                await handler.Handle(@event);

            _channel.BasicAck(args.DeliveryTag, false);
        };

        _channel.BasicConsume(
            queue: "payments.payment.completed",
            autoAck: false,
            consumer: consumer);

        _logger.LogInformation("Listening to payment.completed events");
    }

    public override void Dispose()
    {
        try
        {
            if (_channel?.IsOpen ?? false)
                _channel.Close();

            if (_connection?.IsOpen ?? false)
                _connection.Close();
        }
        catch (Exception ex)
        {
            _logger.LogWarning("Exception on disposing RabbitMQ connection: {Message}", ex.Message);
        }

        base.Dispose();
    }
}
