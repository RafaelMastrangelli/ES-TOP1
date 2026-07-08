using ESTop1.Api.DTOs;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace ESTop1.Api.Middleware;

public class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;

    public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
    {
        _logger = logger;
    }

    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        _logger.LogError(exception, "Erro não tratado em {Method} {Path}",
            httpContext.Request.Method,
            httpContext.Request.Path);

        var (statusCode, errorCode, message) = exception switch
        {
            UnauthorizedAccessException => (StatusCodes.Status401Unauthorized, "UNAUTHORIZED", exception.Message),
            ArgumentException => (StatusCodes.Status400BadRequest, "VALIDATION_ERROR", exception.Message),
            KeyNotFoundException => (StatusCodes.Status404NotFound, "NOT_FOUND", exception.Message),
            InvalidOperationException => (StatusCodes.Status400BadRequest, "INVALID_OPERATION", exception.Message),
            _ => (StatusCodes.Status500InternalServerError, "INTERNAL_ERROR", "Erro interno do servidor. Tente novamente mais tarde.")
        };

        var response = new ErrorResponse
        {
            Message = message,
            ErrorCode = errorCode,
            Details = httpContext.RequestServices.GetRequiredService<IHostEnvironment>().IsDevelopment()
                ? exception.Message
                : null
        };

        httpContext.Response.StatusCode = statusCode;
        await httpContext.Response.WriteAsJsonAsync(response, cancellationToken);
        return true;
    }
}
