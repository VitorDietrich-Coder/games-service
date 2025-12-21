using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Games.Microservice.Infrastructure.Messaging
{
    public class RabbitMqEventBus : IDisposable
    {
        private readonly IConnection _connection;
        private readonly IModel _channel;

        public RabbitMqEventBus(string connectionString)
        {
            var factory = new ConnectionFactory
            {
                Uri = new Uri(connectionString)
            };

            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
        }

        /// <summary>
        /// Publica um evento em uma fila específica
        /// </summary>
        public Task PublishAsync<TEvent>(TEvent @event, string queueName) where TEvent : class
        {
            // Declara a fila (idempotente)
            _channel.QueueDeclare(
                queue: queueName,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null);

            // Serializa o evento
            var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(@event));

            // Cria propriedades e define persistência
            var properties = _channel.CreateBasicProperties();
            properties.Persistent = true;

             _channel.BasicPublish(
                exchange: "",
                routingKey: queueName,
                basicProperties: properties,
                body: body);

            return Task.CompletedTask;
        }

        /// <summary>
        /// Consome eventos de uma fila específica
        /// </summary>
        public void Subscribe<TEvent>(string queueName, Func<TEvent, Task> handleMessage) where TEvent : class
        {
            _channel.QueueDeclare(
                queue: queueName,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null);

            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += async (sender, eventArgs) =>
            {
                var body = eventArgs.Body.ToArray();
                var message = JsonSerializer.Deserialize<TEvent>(body);

                if (message != null)
                {
                    await handleMessage(message);
                }

                _channel.BasicAck(eventArgs.DeliveryTag, false);
            };

            _channel.BasicConsume(queue: queueName, autoAck: false, consumer: consumer);
        }

        public void Dispose()
        {
            _channel?.Close();
            _channel?.Dispose();

            _connection?.Close();
            _connection?.Dispose();
        }
    }
}
