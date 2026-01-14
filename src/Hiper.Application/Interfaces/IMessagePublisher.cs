namespace Hiper.Application.Interfaces;

public interface IMessagePublisher
{
    Task PublishAsync<T>(T message, string routingKey);
}
