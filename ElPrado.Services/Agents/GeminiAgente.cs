using ElPrado.Services.Interfaces;
using Microsoft.Extensions.Configuration;
using System.Net.Http.Json;
using System.Text.Json;

namespace ElPrado.Services.Agents
{
    public class GeminiAgente : IAgenteIA
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;
        private readonly string _modelo;

        public GeminiAgente(HttpClient httpClient, IConfiguration config)
        {
            _httpClient = httpClient;
            _apiKey = config["IA:GeminiKey"] ?? "TU_API_KEY_DE_GOOGLE_AQUI"; 
            _modelo = config["IA:GeminiModel"] ?? "gemini-2.5-flash";
        }

        public async Task<string> GenerarTextoAsync(string prompt)
        {
            var url = $"https://generativelanguage.googleapis.com/v1beta/models/gemini-2.5-flash:generateContent?key={_apiKey}";
            var payload = new { contents = new[] { new { parts = new[] { new { text = prompt } } } } };

            var response = await _httpClient.PostAsJsonAsync(url, payload);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadFromJsonAsync<JsonElement>();
            return json.GetProperty("candidates")[0].GetProperty("content").GetProperty("parts")[0].GetProperty("text").GetString() ?? string.Empty;
        }
    }
}