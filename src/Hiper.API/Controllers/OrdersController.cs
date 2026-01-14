using Hiper.Application.UseCases.CancelOrder;
using Hiper.Application.UseCases.CreateOrder;
using Hiper.Application.UseCases.GetOrderById;
using Hiper.Application.UseCases.GetOrders;
using Hiper.Application.UseCases.UpdateOrderStatus;
using Hiper.Domain.Enums;
using Microsoft.AspNetCore.Mvc;

namespace Hiper.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrdersController : ControllerBase
{
    private readonly CreateOrderHandler _createOrderHandler;
    private readonly GetOrdersHandler _getOrdersHandler;
    private readonly GetOrderByIdHandler _getOrderByIdHandler;
    private readonly UpdateOrderStatusHandler _updateOrderStatusHandler;
    private readonly CancelOrderHandler _cancelOrderHandler;

    public OrdersController(
        CreateOrderHandler createOrderHandler,
        GetOrdersHandler getOrdersHandler,
        GetOrderByIdHandler getOrderByIdHandler,
        UpdateOrderStatusHandler updateOrderStatusHandler,
        CancelOrderHandler cancelOrderHandler)
    {
        _createOrderHandler = createOrderHandler;
        _getOrdersHandler = getOrdersHandler;
        _getOrderByIdHandler = getOrderByIdHandler;
        _updateOrderStatusHandler = updateOrderStatusHandler;
        _cancelOrderHandler = cancelOrderHandler;
    }

    [HttpPost]
    public async Task<IActionResult> CreateOrder([FromBody] CreateOrderCommand command)
    {
        var result = await _createOrderHandler.HandleAsync(command);

        if (result.IsFailure)
            return BadRequest(new { error = result.Error });

        var orderResult = await _getOrderByIdHandler.HandleAsync(new GetOrderByIdQuery(result.Value));

        return CreatedAtAction(nameof(GetOrderById), new { id = result.Value }, orderResult.Value);
    }

    [HttpGet]
    public async Task<IActionResult> GetOrders()
    {
        var orders = await _getOrdersHandler.HandleAsync(new GetOrdersQuery());
        return Ok(orders);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetOrderById(Guid id)
    {
        var result = await _getOrderByIdHandler.HandleAsync(new GetOrderByIdQuery(id));

        if (result.IsFailure)
            return NotFound(new { error = result.Error });

        return Ok(result.Value);
    }

    [HttpPut("{id}/status")]
    public async Task<IActionResult> UpdateOrderStatus(Guid id, [FromBody] UpdateStatusRequest request)
    {
        if (!Enum.TryParse<OrderStatus>(request.NewStatus, true, out var newStatus))
            return BadRequest(new { error = "Status inválido" });

        var result = await _updateOrderStatusHandler.HandleAsync(
            new UpdateOrderStatusCommand(id, newStatus));

        if (result.IsFailure)
            return BadRequest(new { error = result.Error });

        return Ok(result.Value);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> CancelOrder(Guid id)
    {
        var result = await _cancelOrderHandler.HandleAsync(new CancelOrderCommand(id));

        if (result.IsFailure)
            return BadRequest(new { error = result.Error });

        return NoContent();
    }
}

public record UpdateStatusRequest(string NewStatus);
