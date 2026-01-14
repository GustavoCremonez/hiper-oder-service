using FluentAssertions;
using Hiper.Domain.Entities;
using Hiper.Domain.Enums;

namespace Hiper.UnitTests.Domain;

public class OrderTests
{
    [Fact]
    public void Create_ValidData_ReturnsSuccessWithOrder()
    {
        var item = OrderItem.Create("Produto Teste", 2, 100m).Value;
        var items = new List<OrderItem> { item };

        var result = Order.Create("João Silva", "joao@example.com", items);

        result.IsSuccess.Should().BeTrue();
        result.Value.CustomerName.Should().Be("João Silva");
        result.Value.CustomerEmail.Should().Be("joao@example.com");
        result.Value.Status.Should().Be(OrderStatus.Pending);
        result.Value.Items.Should().HaveCount(1);
        result.Value.Total.Should().Be(200m);
    }

    [Fact]
    public void Create_EmptyCustomerName_ReturnsFailure()
    {
        var item = OrderItem.Create("Produto", 1, 100m).Value;
        var items = new List<OrderItem> { item };

        var result = Order.Create("", "joao@example.com", items);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be("Nome do cliente é obrigatório");
    }

    [Fact]
    public void Create_EmptyCustomerEmail_ReturnsFailure()
    {
        var item = OrderItem.Create("Produto", 1, 100m).Value;
        var items = new List<OrderItem> { item };

        var result = Order.Create("João Silva", "", items);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be("Email do cliente é obrigatório");
    }

    [Fact]
    public void Create_InvalidEmail_ReturnsFailure()
    {
        var item = OrderItem.Create("Produto", 1, 100m).Value;
        var items = new List<OrderItem> { item };

        var result = Order.Create("João Silva", "emailinvalido", items);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be("Email do cliente é inválido");
    }

    [Fact]
    public void Create_EmptyItems_ReturnsFailure()
    {
        var items = new List<OrderItem>();

        var result = Order.Create("João Silva", "joao@example.com", items);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be("Pedido deve conter pelo menos um item");
    }

    [Fact]
    public void UpdateStatus_ValidTransition_ReturnsSuccess()
    {
        var item = OrderItem.Create("Produto", 1, 100m).Value;
        var order = Order.Create("João", "joao@example.com", new List<OrderItem> { item }).Value;

        var result = order.UpdateStatus(OrderStatus.Confirmed);

        result.IsSuccess.Should().BeTrue();
        order.Status.Should().Be(OrderStatus.Confirmed);
        order.UpdatedAt.Should().NotBeNull();
    }

    [Fact]
    public void UpdateStatus_InvalidTransition_ReturnsFailure()
    {
        var item = OrderItem.Create("Produto", 1, 100m).Value;
        var order = Order.Create("João", "joao@example.com", new List<OrderItem> { item }).Value;
        order.UpdateStatus(OrderStatus.Confirmed);
        order.UpdateStatus(OrderStatus.Processing);
        order.UpdateStatus(OrderStatus.Completed);

        var result = order.UpdateStatus(OrderStatus.Pending);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Contain("não é permitida");
    }

    [Fact]
    public void Total_CalculatesCorrectly()
    {
        var item1 = OrderItem.Create("Produto 1", 2, 100m).Value;
        var item2 = OrderItem.Create("Produto 2", 3, 50m).Value;
        var items = new List<OrderItem> { item1, item2 };

        var order = Order.Create("João", "joao@example.com", items).Value;

        order.Total.Should().Be(350m);
    }
}
