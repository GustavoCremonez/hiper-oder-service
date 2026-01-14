using FluentAssertions;
using Hiper.Application.UseCases.UpdateOrderStatus;
using Hiper.Domain.Entities;
using Hiper.Domain.Enums;
using Hiper.Domain.Interfaces;
using Moq;

namespace Hiper.UnitTests.UseCases;

public class UpdateOrderStatusHandlerTests
{
    private readonly Mock<IOrderRepository> _mockRepository;
    private readonly UpdateOrderStatusHandler _handler;

    public UpdateOrderStatusHandlerTests()
    {
        _mockRepository = new Mock<IOrderRepository>();
        _handler = new UpdateOrderStatusHandler(_mockRepository.Object);
    }

    [Fact]
    public async Task Handle_ValidTransition_ReturnsSuccess()
    {
        var item = OrderItem.Create("Produto", 1, 100m).Value;
        var order = Order.Create("João", "joao@example.com", new List<OrderItem> { item }).Value;

        _mockRepository
            .Setup(r => r.GetByIdAsync(order.Id))
            .ReturnsAsync(order);

        var command = new UpdateOrderStatusCommand(order.Id, OrderStatus.Confirmed);

        var result = await _handler.HandleAsync(command);

        result.IsSuccess.Should().BeTrue();
        result.Value.Status.Should().Be("Confirmed");
        _mockRepository.Verify(r => r.UpdateAsync(order), Times.Once);
    }

    [Fact]
    public async Task Handle_OrderNotFound_ReturnsFailure()
    {
        _mockRepository
            .Setup(r => r.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync((Order?)null);

        var command = new UpdateOrderStatusCommand(Guid.NewGuid(), OrderStatus.Confirmed);

        var result = await _handler.HandleAsync(command);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be("Pedido não encontrado");
        _mockRepository.Verify(r => r.UpdateAsync(It.IsAny<Order>()), Times.Never);
    }

    [Fact]
    public async Task Handle_InvalidTransition_ReturnsFailure()
    {
        var item = OrderItem.Create("Produto", 1, 100m).Value;
        var order = Order.Create("João", "joao@example.com", new List<OrderItem> { item }).Value;
        order.UpdateStatus(OrderStatus.Confirmed);
        order.UpdateStatus(OrderStatus.Processing);
        order.UpdateStatus(OrderStatus.Completed);

        _mockRepository
            .Setup(r => r.GetByIdAsync(order.Id))
            .ReturnsAsync(order);

        var command = new UpdateOrderStatusCommand(order.Id, OrderStatus.Pending);

        var result = await _handler.HandleAsync(command);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Contain("não é permitida");
        _mockRepository.Verify(r => r.UpdateAsync(It.IsAny<Order>()), Times.Never);
    }
}
