using System.Text;
using System.Text.Json;
using Hiper.Application.Interfaces;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;

namespace Hiper.Infrastructure.Messaging;

public class RabbitMqPublisher : IMessagePublisher, IDisposable
{
    private readonly RabbitMqSettings _settings;
    private IConnection? _connection;
    private IModel? _channel;
    private readonly SemaphoreSlim _connectionLock = new(1, 1);

    public RabbitMqPublisher(IOptions<RabbitMqSettings> settings)
    {
        _settings = settings.Value;
    }

    public Task PublishAsync<T>(T message, string routingKey)
    {
        EnsureConnection();

        var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(message));

        _channel!.BasicPublish(
            exchange: "hiper.orders",
            routingKey: routingKey,
            basicProperties: null,
            body: body
        );

        return Task.CompletedTask;
    }

    private void EnsureConnection()
    {
        if (_connection != null && _connection.IsOpen)
            return;

        _connectionLock.Wait();
        try
        {
            if (_connection != null && _connection.IsOpen)
                return;

            var factory = new ConnectionFactory
            {
                HostName = _settings.Host,
                UserName = _settings.Username,
                Password = _settings.Password
            };

            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();

            _channel.ExchangeDeclare(
                exchange: "hiper.orders",
                type: ExchangeType.Topic,
                durable: true,
                autoDelete: false
            );
        }
        finally
        {
            _connectionLock.Release();
        }
    }

    public void Dispose()
    {
        _channel?.Dispose();
        _connection?.Dispose();
        _connectionLock?.Dispose();
    }
}
