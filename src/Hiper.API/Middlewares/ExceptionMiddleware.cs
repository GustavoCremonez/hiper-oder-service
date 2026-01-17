using System.Net;
using System.Text.Json;
using Hiper.Domain.Exceptions;

namespace Hiper.API.Middlewares;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionMiddleware> _logger;

    public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro não tratado: {Message}", ex.Message);
            await HandleExceptionAsync(context, ex);
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";

        var (statusCode, response) = exception switch
        {
            ValidationException validationEx => (
                HttpStatusCode.BadRequest,
                new ErrorResponse
                {
                    Code = validationEx.Code,
                    Message = validationEx.Message,
                    Errors = validationEx.Errors.Any() ? validationEx.Errors : null
                }
            ),
            NotFoundException notFoundEx => (
                HttpStatusCode.NotFound,
                new ErrorResponse
                {
                    Code = notFoundEx.Code,
                    Message = notFoundEx.Message
                }
            ),
            BusinessRuleException businessEx => (
                HttpStatusCode.UnprocessableEntity,
                new ErrorResponse
                {
                    Code = businessEx.Code,
                    Message = businessEx.Message
                }
            ),
            _ => (
                HttpStatusCode.InternalServerError,
                new ErrorResponse
                {
                    Code = "INTERNAL_ERROR",
                    Message = "Ocorreu um erro interno no servidor. Por favor, tente novamente mais tarde.",
                    Details = exception.Message
                }
            )
        };

        context.Response.StatusCode = (int)statusCode;

        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        return context.Response.WriteAsync(JsonSerializer.Serialize(response, options));
    }
}

public class ErrorResponse
{
    public string Code { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public Dictionary<string, string[]>? Errors { get; set; }
    public string? Details { get; set; }
}
