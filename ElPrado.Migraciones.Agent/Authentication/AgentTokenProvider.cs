using System.Net;
using System.Net.Http.Json;
using ElPrado.Migraciones.Contracts;
using Microsoft.Extensions.Options;

namespace ElPrado.Migraciones.Agent.Authentication;

public interface IAgentTokenProvider
{
    Task<string> GetAccessTokenAsync(CancellationToken cancellationToken = default);
}

public sealed class AgentTokenProvider : IAgentTokenProvider
{
    private readonly HttpClient _client;
    private readonly IOptionsMonitor<AgentAuthenticationOptions> _options;
    private readonly ILogger<AgentTokenProvider> _logger;
    private readonly SemaphoreSlim _refreshLock = new(1, 1);
    private string? _accessToken;
    private DateTimeOffset _expiresAt;
    private bool _credentialsRejected;

    public AgentTokenProvider(HttpClient client, IOptionsMonitor<AgentAuthenticationOptions> options, ILogger<AgentTokenProvider> logger)
    {
        _client = client; _options = options; _logger = logger;
        _options.OnChange(_ => _credentialsRejected = false);
    }

    public async Task<string> GetAccessTokenAsync(CancellationToken cancellationToken = default)
    {
        if (HasUsableToken()) return _accessToken!;
        await _refreshLock.WaitAsync(cancellationToken);
        try
        {
            if (HasUsableToken()) return _accessToken!;
            if (_credentialsRejected) throw new InvalidOperationException("Las credenciales del agente fueron rechazadas; se requiere actualizar la configuración o reiniciar.");
            var token = await RequestWithRetriesAsync(cancellationToken);
            _accessToken = token.AccessToken;
            _expiresAt = token.ExpiresAt;
            return _accessToken;
        }
        finally { _refreshLock.Release(); }
    }

    private bool HasUsableToken()
    {
        var margin = TimeSpan.FromSeconds(_options.CurrentValue.RenewalMarginSeconds);
        return !string.IsNullOrEmpty(_accessToken) && _expiresAt > DateTimeOffset.UtcNow.Add(margin);
    }

    private async Task<AgentTokenResponse> RequestWithRetriesAsync(CancellationToken cancellationToken)
    {
        var attempt = 0;
        while (true)
        {
            try
            {
                var options = _options.CurrentValue;
                using var response = await _client.PostAsJsonAsync("/auth/agent/token", new AgentTokenRequest(options.AgentId, options.AgentSecret), cancellationToken);
                if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    _credentialsRejected = true;
                    _logger.LogError("Migration agent credentials were rejected");
                    throw new InvalidOperationException("Las credenciales del agente fueron rechazadas.");
                }
                if (response.StatusCode == HttpStatusCode.TooManyRequests)
                {
                    await DelayAsync(attempt++, response.Headers.RetryAfter?.Delta, cancellationToken);
                    continue;
                }
                if (!response.IsSuccessStatusCode)
                {
                    await DelayAsync(attempt++, null, cancellationToken);
                    continue;
                }
                var token = await response.Content.ReadFromJsonAsync<AgentTokenResponse>(cancellationToken: cancellationToken);
                return token ?? throw new HttpRequestException("La API devolvió una respuesta de token inválida.");
            }
            catch (HttpRequestException) { await DelayAsync(attempt++, null, cancellationToken); }
            catch (TaskCanceledException) when (!cancellationToken.IsCancellationRequested) { await DelayAsync(attempt++, null, cancellationToken); }
        }
    }

    private async Task DelayAsync(int attempt, TimeSpan? retryAfter, CancellationToken cancellationToken)
    {
        var retry = _options.CurrentValue.Retry;
        var seconds = retryAfter?.TotalSeconds ?? Math.Min(retry.MaxDelaySeconds, retry.BaseDelaySeconds * Math.Pow(2, Math.Min(attempt, 30)));
        seconds = Math.Min(seconds, retry.MaxDelaySeconds);
        var jitter = 1 + ((Random.Shared.NextDouble() * 2 - 1) * retry.JitterPercent / 100d);
        await Task.Delay(TimeSpan.FromSeconds(seconds * jitter), cancellationToken);
    }
}
