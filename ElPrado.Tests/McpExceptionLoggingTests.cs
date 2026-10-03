using ElPrado.McpApi.Middlewares;
using ElPrado.McpApi.Observability;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
using System.Text;
using Xunit;

namespace ElPrado.Tests;

public sealed class McpExceptionLoggingTests
{
    [Fact]
    public async Task Excepcion_NoControlada_DevuelveErrorSeguroYRegistraContextoSinCredenciales()
    {
        CapturingLoggerProvider provider = new();
        using ILoggerFactory loggerFactory = LoggerFactory.Create(builder => builder.AddProvider(provider));
        McpExceptionHandlingMiddleware middleware = new(
            _ => throw new InvalidOperationException("database password=not-for-clients"),
            loggerFactory.CreateLogger<McpExceptionHandlingMiddleware>());
        DefaultHttpContext context = CreateContext("POST", "/api/v2/falla", "Bearer test-token-123");

        await middleware.Invoke(context);

        Assert.Equal(StatusCodes.Status500InternalServerError, context.Response.StatusCode);
        string response = Encoding.UTF8.GetString(((MemoryStream)context.Response.Body).ToArray());
        Assert.Contains("INTERNAL_SERVER_ERROR", response);
        Assert.DoesNotContain("password=not-for-clients", response);

        LoggedEntry entry = Assert.Single(provider.Entries);
        Assert.Equal(LogLevel.Error, entry.Level);
        Assert.IsType<InvalidOperationException>(entry.Exception);
        Assert.Equal("POST", entry.Properties["HttpMethod"]);
        Assert.Equal("/api/v2/falla", entry.Properties["RequestPath"]);
        Assert.Equal(500, entry.Properties["StatusCode"]);
        Assert.DoesNotContain("test-token-123", entry.RenderedMessage);
        Assert.DoesNotContain("test-token-123", entry.Properties.Values.Select(value => value?.ToString() ?? string.Empty));
    }

    [Fact]
    public void Excepcion_Controlada_SeRegistraConElEstadoDeSuContrato()
    {
        CapturingLoggerProvider provider = new();
        using ILoggerFactory loggerFactory = LoggerFactory.Create(builder => builder.AddProvider(provider));
        DefaultHttpContext context = CreateContext("GET", "/api/v2/propuestas/41000", "Bearer test-token-123");
        InvalidOperationException exception = new("business operation failed");

        McpExceptionLog.Error(loggerFactory.CreateLogger("Propuestas"), context, exception, StatusCodes.Status422UnprocessableEntity);

        LoggedEntry entry = Assert.Single(provider.Entries);
        Assert.Equal(LogLevel.Error, entry.Level);
        Assert.Same(exception, entry.Exception);
        Assert.Equal("GET", entry.Properties["HttpMethod"]);
        Assert.Equal("/api/v2/propuestas/41000", entry.Properties["RequestPath"]);
        Assert.Equal(422, entry.Properties["StatusCode"]);
        Assert.DoesNotContain("test-token-123", entry.RenderedMessage);
    }

    private static DefaultHttpContext CreateContext(string method, string path, string authorization)
    {
        DefaultHttpContext context = new();
        context.Request.Method = method;
        context.Request.Path = path;
        context.Request.Headers.Authorization = authorization;
        context.Response.Body = new MemoryStream();
        return context;
    }

    private sealed record LoggedEntry(LogLevel Level, Exception? Exception, string RenderedMessage, IReadOnlyDictionary<string, object?> Properties);

    private sealed class CapturingLoggerProvider : ILoggerProvider
    {
        public ConcurrentBag<LoggedEntry> Entries { get; } = new();

        public ILogger CreateLogger(string categoryName) => new CapturingLogger(Entries);

        public void Dispose()
        {
        }
    }

    private sealed class CapturingLogger : ILogger
    {
        private readonly ConcurrentBag<LoggedEntry> _entries;

        public CapturingLogger(ConcurrentBag<LoggedEntry> entries) => _entries = entries;

        public IDisposable? BeginScope<TState>(TState state) where TState : notnull => null;

        public bool IsEnabled(LogLevel logLevel) => true;

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception,
            Func<TState, Exception?, string> formatter)
        {
            IReadOnlyDictionary<string, object?> properties = state is IEnumerable<KeyValuePair<string, object?>> values
                ? values.ToDictionary(pair => pair.Key, pair => pair.Value)
                : new Dictionary<string, object?>();
            _entries.Add(new LoggedEntry(logLevel, exception, formatter(state, exception), properties));
        }
    }
}
