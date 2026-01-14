using FluentAssertions;
using Hiper.Domain.Entities;

namespace Hiper.UnitTests.Domain;

public class OrderItemTests
{
    [Fact]
    public void Create_ValidData_ReturnsSuccessWithItem()
    {
        var result = OrderItem.Create("Notebook Dell", 2, 3500m);

        result.IsSuccess.Should().BeTrue();
        result.Value.ProductName.Should().Be("Notebook Dell");
        result.Value.Quantity.Should().Be(2);
        result.Value.UnitPrice.Should().Be(3500m);
        result.Value.Subtotal.Should().Be(7000m);
    }

    [Fact]
    public void Create_EmptyProductName_ReturnsFailure()
    {
        var result = OrderItem.Create("", 1, 100m);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be("Nome do produto é obrigatório");
    }

    [Fact]
    public void Create_ZeroQuantity_ReturnsFailure()
    {
        var result = OrderItem.Create("Produto", 0, 100m);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be("Quantidade deve ser maior ou igual a 1");
    }

    [Fact]
    public void Create_NegativeQuantity_ReturnsFailure()
    {
        var result = OrderItem.Create("Produto", -1, 100m);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be("Quantidade deve ser maior ou igual a 1");
    }

    [Fact]
    public void Create_ZeroPrice_ReturnsFailure()
    {
        var result = OrderItem.Create("Produto", 1, 0m);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be("Preço unitário deve ser maior que zero");
    }

    [Fact]
    public void Create_NegativePrice_ReturnsFailure()
    {
        var result = OrderItem.Create("Produto", 1, -10m);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be("Preço unitário deve ser maior que zero");
    }

    [Fact]
    public void Subtotal_CalculatesCorrectly()
    {
        var item = OrderItem.Create("Produto", 5, 99.99m).Value;

        item.Subtotal.Should().Be(499.95m);
    }
}
