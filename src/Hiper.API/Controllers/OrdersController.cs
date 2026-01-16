using Hiper.API.Models;
using Hiper.Application.UseCases.CancelOrder;
using Hiper.Application.UseCases.CreateOrder;
using Hiper.Application.UseCases.GetOrderById;
using Hiper.Application.UseCases.GetOrders;
using Hiper.Application.UseCases.UpdateOrderStatus;
using Hiper.Domain.Enums;
using Microsoft.AspNetCore.Mvc;

namespace Hiper.API.Controllers;

/// <summary>
/// Controller responsável pelo gerenciamento de pedidos
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
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

    /// <summary>
    /// Cria um novo pedido
    /// </summary>
    /// <param name="command">Dados do pedido a ser criado</param>
    /// <returns>Pedido criado com status Pending</returns>
    /// <response code="201">Pedido criado com sucesso</response>
    /// <response code="400">Dados inválidos ou erro de validação</response>
    [HttpPost]
    [ProducesResponseType(typeof(object), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateOrder([FromBody] CreateOrderCommand command)
    {
        var result = await _createOrderHandler.HandleAsync(command);

        if (result.IsFailure)
            return BadRequest(new { error = result.Error });

        var orderResult = await _getOrderByIdHandler.HandleAsync(new GetOrderByIdQuery(result.Value));

        return CreatedAtAction(nameof(GetOrderById), new { id = result.Value }, orderResult.Value);
    }

    /// <summary>
    /// Retorna lista paginada de pedidos
    /// </summary>
    /// <param name="page">Número da página (padrão: 1)</param>
    /// <param name="pageSize">Tamanho da página (padrão: 10)</param>
    /// <returns>Lista paginada de pedidos ordenados por data de criação</returns>
    /// <response code="200">Lista de pedidos retornada com sucesso</response>
    [HttpGet]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetOrders([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        var result = await _getOrdersHandler.HandleAsync(new GetOrdersQuery(page, pageSize));
        return Ok(result);
    }

    /// <summary>
    /// Busca um pedido específico por ID
    /// </summary>
    /// <param name="id">ID do pedido</param>
    /// <returns>Dados completos do pedido</returns>
    /// <response code="200">Pedido encontrado</response>
    /// <response code="404">Pedido não encontrado</response>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(object), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetOrderById(Guid id)
    {
        var result = await _getOrderByIdHandler.HandleAsync(new GetOrderByIdQuery(id));

        if (result.IsFailure)
            return NotFound(new { error = result.Error });

        return Ok(result.Value);
    }

    /// <summary>
    /// Atualiza o status de um pedido
    /// </summary>
    /// <param name="id">ID do pedido</param>
    /// <param name="request">Novo status (Pending, Confirmed, Processing, Completed, Cancelled)</param>
    /// <returns>Pedido com status atualizado</returns>
    /// <response code="200">Status atualizado com sucesso</response>
    /// <response code="400">Status inválido ou transição não permitida</response>
    [HttpPut("{id}/status")]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
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

    /// <summary>
    /// Cancela um pedido
    /// </summary>
    /// <param name="id">ID do pedido a ser cancelado</param>
    /// <returns>Sem conteúdo</returns>
    /// <response code="204">Pedido cancelado com sucesso</response>
    /// <response code="400">Pedido não pode ser cancelado</response>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CancelOrder(Guid id)
    {
        var result = await _cancelOrderHandler.HandleAsync(new CancelOrderCommand(id));

        if (result.IsFailure)
            return BadRequest(new { error = result.Error });

        return NoContent();
    }
}