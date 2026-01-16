namespace Hiper.API.Models;

/// <summary>
/// Request para atualização de status do pedido
/// </summary>
/// <param name="NewStatus">Novo status: Pending, Confirmed, Processing, Completed, Cancelled</param>
public record UpdateStatusRequest(string NewStatus);
