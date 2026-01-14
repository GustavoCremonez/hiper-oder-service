using Hiper.Domain.Enums;

namespace Hiper.Application.UseCases.UpdateOrderStatus;

public record UpdateOrderStatusCommand(
    Guid OrderId,
    OrderStatus NewStatus
);
