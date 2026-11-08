using System.Net;
using System.Net.Http.Json;
using ElPrado.Migraciones.Agent.Authentication;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Xunit;

namespace ElPrado.Migraciones.Agent.Tests;

public sealed class AgentTokenProviderTests
{
    [Fact]
    public async Task Concurrent_consumers_share_one_token_request()
    {
        var handler = new TokenHandler();
        var provider = CreateProvider(handler);
        var tokens = await Task.WhenAll(Enumerable.Range(0, 10).Select(_ => provider.GetAccessTokenAsync()));
        Assert.All(tokens, token => Assert.Equal("token", token));
        Assert.Equal(1, handler.Calls);
    }

    [Fact]
    public async Task Unauthorized_credentials_are_not_retried()
    {
        var handler = new TokenHandler(HttpStatusCode.Unauthorized);
        var provider = CreateProvider(handler);
        await Assert.ThrowsAsync<InvalidOperationException>(() => provider.GetAccessTokenAsync());
        await Assert.ThrowsAsync<InvalidOperationException>(() => provider.GetAccessTokenAsync());
        Assert.Equal(1, handler.Calls);
    }

    [Fact]
    public async Task Rate_limit_is_retried_and_recovers()
    {
        var handler = new SequenceHandler(HttpStatusCode.TooManyRequests, HttpStatusCode.OK);
        var provider = CreateProvider(handler);
        Assert.Equal("token", await provider.GetAccessTokenAsync());
        Assert.Equal(2, handler.Calls);
    }

    [Fact]
    public async Task Network_failure_is_retried_and_recovers()
    {
        var handler = new FailingThenSuccessHandler();
        var provider = CreateProvider(handler);
        Assert.Equal("token", await provider.GetAccessTokenAsync());
        Assert.Equal(2, handler.Calls);
    }

    private static AgentTokenProvider CreateProvider(HttpMessageHandler handler) => new(
        new HttpClient(handler) { BaseAddress = new Uri("https://migration.test") },
        new StaticOptionsMonitor<AgentAuthenticationOptions>(new AgentAuthenticationOptions { ApiUrl = "https://migration.test", AgentId = "agent", AgentSecret = "test" }),
        NullLogger<AgentTokenProvider>.Instance);

    private sealed class TokenHandler : HttpMessageHandler
    {
        private readonly HttpStatusCode _status;
        public int Calls;
        public TokenHandler(HttpStatusCode status = HttpStatusCode.OK) => _status = status;
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            Interlocked.Increment(ref Calls);
            var response = new HttpResponseMessage(_status);
            if (_status == HttpStatusCode.OK) response.Content = JsonContent.Create(new { accessToken = "token", expiresAt = DateTimeOffset.UtcNow.AddMinutes(15) });
            return Task.FromResult(response);
        }
    }

    private sealed class SequenceHandler : HttpMessageHandler
    {
        private readonly Queue<HttpStatusCode> _responses;
        public int Calls;
        public SequenceHandler(params HttpStatusCode[] responses) => _responses = new Queue<HttpStatusCode>(responses);
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            Interlocked.Increment(ref Calls);
            var status = _responses.Dequeue();
            var response = new HttpResponseMessage(status);
            if (status == HttpStatusCode.OK) response.Content = JsonContent.Create(new { accessToken = "token", expiresAt = DateTimeOffset.UtcNow.AddMinutes(15) });
            return Task.FromResult(response);
        }
    }

    private sealed class FailingThenSuccessHandler : HttpMessageHandler
    {
        public int Calls;
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            if (Interlocked.Increment(ref Calls) == 1) throw new HttpRequestException("temporary");
            return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK) { Content = JsonContent.Create(new { accessToken = "token", expiresAt = DateTimeOffset.UtcNow.AddMinutes(15) }) });
        }
    }

    private sealed class StaticOptionsMonitor<T> : IOptionsMonitor<T> where T : class
    {
        public StaticOptionsMonitor(T value) => CurrentValue = value;
        public T CurrentValue { get; }
        public T Get(string? name) => CurrentValue;
        public IDisposable OnChange(Action<T, string?> listener) => Empty.Instance;
        private sealed class Empty : IDisposable { public static readonly Empty Instance = new(); public void Dispose() { } }
    }
}
