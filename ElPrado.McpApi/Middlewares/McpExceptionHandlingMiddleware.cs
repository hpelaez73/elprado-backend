using ElPrado.McpApi.Contracts;
using ElPrado.McpApi.Observability;
using Microsoft.Extensions.Logging;

namespace ElPrado.McpApi.Middlewares;

public sealed class McpExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<McpExceptionHandlingMiddleware> _logger;

    public McpExceptionHandlingMiddleware(RequestDelegate next, ILogger<McpExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception exception)
        {
            int statusCode = context.Response.HasStarted
                ? context.Response.StatusCode
                : StatusCodes.Status500InternalServerError;

            McpExceptionLog.Error(_logger, context, exception, statusCode);

            if (context.Response.HasStarted)
                return;

            context.Response.Clear();
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsJsonAsync(
                ApiResponse<object>.Failure("INTERNAL_SERVER_ERROR", "Ocurrió un error inesperado."));
        }
    }
}
