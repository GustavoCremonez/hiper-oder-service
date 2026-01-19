namespace Hiper.Infrastructure.Messaging;

public static class RabbitMqConstants
{
    public const string OrdersExchange = "hiper.orders";

    public static class RoutingKeys
    {
        public const string OrderCreated = "order.created";
        public const string OrderUpdated = "order.updated";
        public const string OrderCancelled = "order.cancelled";
    }
}
