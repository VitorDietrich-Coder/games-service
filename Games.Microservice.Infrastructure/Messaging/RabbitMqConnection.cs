using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Games.Microservice.Infrastructure.Messaging
{
    public sealed class RabbitMqConnection : IRabbitMqConnection, IDisposable
    {
        private readonly IConfiguration _configuration;
        private IConnection? _connection;
        private readonly object _lock = new();

        public RabbitMqConnection(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public IModel CreateChannel()
        {
            EnsureConnected();
            return _connection!.CreateModel();
        }

        private void EnsureConnected()
        {
            if (_connection is { IsOpen: true })
                return;

            lock (_lock)
            {
                if (_connection is { IsOpen: true })
                    return;

                var factory = new ConnectionFactory
                {
                    Uri = new Uri(_configuration["RabbitMq:ConnectionString"]),
                    DispatchConsumersAsync = true,
                    AutomaticRecoveryEnabled = true
                };

                int attempts = 0;
                while (attempts < 10)
                {
                    try
                    {
                        _connection = factory.CreateConnection();
                        return;
                    }
                    catch
                    {
                        attempts++;
                        Thread.Sleep(3000);
                    }
                }

                throw new InvalidOperationException("RabbitMQ unavailable");
            }
        }

        public void Dispose()
        {
            _connection?.Close();
        }
    }

}
