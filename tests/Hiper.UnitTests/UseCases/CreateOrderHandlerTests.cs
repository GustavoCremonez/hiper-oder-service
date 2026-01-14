using FluentAssertions;
using Hiper.Application.Interfaces;
using Hiper.Application.UseCases.CreateOrder;
using Hiper.Domain.Entities;
using Hiper.Domain.Interfaces;
using Moq;

namespace Hiper.UnitTests.UseCases;

public class CreateOrderHandlerTests
{
    private readonly Mock<IOrderRepository> _mockRepository;
    private readonly Mock<IMessagePublisher> _mockPublisher;
    private readonly CreateOrderHandler _handler;

    public CreateOrderHandlerTests()
    {
        _mockRepository = new Mock<IOrderRepository>();
        _mockPublisher = new Mock<IMessagePublisher>();
        _handler = new CreateOrderHandler(_mockRepository.Object, _mockPublisher.Object);
    }

    [Fact]
    public async Task Handle_ValidCommand_ReturnsSuccessWithOrderId()
    {
        var command = new CreateOrderCommand(
            "João Silva",
            "joao@example.com",
            new List<CreateOrderItemCommand>
            {
                new("Notebook", 1, 3500m)
            }
        );

        _mockRepository
            .Setup(r => r.AddAsync(It.IsAny<Order>()))
            .ReturnsAsync((Order o) => o);

        var result = await _handler.HandleAsync(command);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeEmpty();
        _mockRepository.Verify(r => r.AddAsync(It.IsAny<Order>()), Times.Once);
        _mockPublisher.Verify(p => p.PublishAsync(
            It.IsAny<OrderCreatedEvent>(),
            "order.created"), Times.Once);
    }

    [Fact]
    public async Task Handle_EmptyCustomerName_ReturnsFailure()
    {
        var command = new CreateOrderCommand(
            "",
            "joao@example.com",
            new List<CreateOrderItemCommand>
            {
                new("Notebook", 1, 3500m)
            }
        );

        var result = await _handler.HandleAsync(command);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be("Nome do cliente é obrigatório");
        _mockRepository.Verify(r => r.AddAsync(It.IsAny<Order>()), Times.Never);
    }

    [Fact]
    public async Task Handle_InvalidEmail_ReturnsFailure()
    {
        var command = new CreateOrderCommand(
            "João Silva",
            "emailinvalido",
            new List<CreateOrderItemCommand>
            {
                new("Notebook", 1, 3500m)
            }
        );

        var result = await _handler.HandleAsync(command);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be("Email do cliente é inválido");
        _mockRepository.Verify(r => r.AddAsync(It.IsAny<Order>()), Times.Never);
    }

    [Fact]
    public async Task Handle_EmptyItems_ReturnsFailure()
    {
        var command = new CreateOrderCommand(
            "João Silva",
            "joao@example.com",
            new List<CreateOrderItemCommand>()
        );

        var result = await _handler.HandleAsync(command);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be("Pedido deve conter pelo menos um item");
        _mockRepository.Verify(r => r.AddAsync(It.IsAny<Order>()), Times.Never);
    }

    [Fact]
    public async Task Handle_InvalidItem_ReturnsFailure()
    {
        var command = new CreateOrderCommand(
            "João Silva",
            "joao@example.com",
            new List<CreateOrderItemCommand>
            {
                new("Notebook", 0, 3500m)
            }
        );

        var result = await _handler.HandleAsync(command);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be("Quantidade deve ser maior ou igual a 1");
        _mockRepository.Verify(r => r.AddAsync(It.IsAny<Order>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ValidCommand_PublishesEvent()
    {
        var command = new CreateOrderCommand(
            "João Silva",
            "joao@example.com",
            new List<CreateOrderItemCommand>
            {
                new("Notebook", 1, 3500m),
                new("Mouse", 2, 150m)
            }
        );

        _mockRepository
            .Setup(r => r.AddAsync(It.IsAny<Order>()))
            .ReturnsAsync((Order o) => o);

        await _handler.HandleAsync(command);

        _mockPublisher.Verify(p => p.PublishAsync(
            It.Is<OrderCreatedEvent>(e =>
                e.CustomerName == "João Silva" &&
                e.CustomerEmail == "joao@example.com" &&
                e.Total == 3800m),
            "order.created"), Times.Once);
    }
}
