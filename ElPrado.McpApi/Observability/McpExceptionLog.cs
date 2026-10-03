using Microsoft.Extensions.Logging;

namespace ElPrado.McpApi.Observability;

public static class McpExceptionLog
{
    public static void Error(ILogger logger, HttpContext context, Exception exception, int statusCode)
    {
        logger.LogError(exception,
            "MCP API request failed with status {StatusCode}. {HttpMethod} {RequestPath}",
            statusCode,
            context.Request.Method,
            context.Request.Path.Value);
    }
}
