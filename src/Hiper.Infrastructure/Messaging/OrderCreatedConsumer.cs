using System.Text;
using System.Text.Json;
using Hiper.Application.UseCases.CreateOrder;
using Hiper.Domain.Enums;
using Hiper.Domain.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Hiper.Infrastructure.Messaging;

public class OrderCreatedConsumer : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<OrderCreatedConsumer> _logger;
    private readonly RabbitMqSettings _settings;
    private IConnection? _connection;
    private IModel? _channel;

    public OrderCreatedConsumer(
        IServiceProvider serviceProvider,
        ILogger<OrderCreatedConsumer> logger,
        IOptions<RabbitMqSettings> settings)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
        _settings = settings.Value;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await Task.Delay(15000, stoppingToken);

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

        var queueDeclare = _channel.QueueDeclare(
            queue: "hiper.orders.created",
            durable: true,
            exclusive: false,
            autoDelete: false
        );

        _channel.QueueBind(
            queue: queueDeclare.QueueName,
            exchange: "hiper.orders",
            routingKey: "order.created"
        );

        var consumer = new EventingBasicConsumer(_channel);
        consumer.Received += async (model, ea) =>
        {
            try
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                var orderCreatedEvent = JsonSerializer.Deserialize<OrderCreatedEvent>(message);

                if (orderCreatedEvent != null)
                {
                    await ProcessOrderCreatedEventAsync(orderCreatedEvent);
                    _channel.BasicAck(ea.DeliveryTag, false);
                    _logger.LogInformation("Pedido {OrderId} confirmado com sucesso", orderCreatedEvent.OrderId);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao processar evento OrderCreated");
                _channel.BasicNack(ea.DeliveryTag, false, true);
            }
        };

        _channel.BasicConsume(queueDeclare.QueueName, false, consumer);

        await Task.Delay(Timeout.Infinite, stoppingToken);
    }

    private async Task ProcessOrderCreatedEventAsync(OrderCreatedEvent orderCreatedEvent)
    {
        using var scope = _serviceProvider.CreateScope();
        var orderRepository = scope.ServiceProvider.GetRequiredService<IOrderRepository>();

        var order = await orderRepository.GetByIdAsync(orderCreatedEvent.OrderId);

        if (order != null)
        {
            order.UpdateStatus(OrderStatus.Confirmed);
            await orderRepository.UpdateAsync(order);
        }
    }

    public override void Dispose()
    {
        _channel?.Dispose();
        _connection?.Dispose();
        base.Dispose();
    }
}
