using ElPrado.Services.Interfaces;
using Microsoft.Extensions.Configuration;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;

namespace ElPrado.Services.Agents
{
    public class OpenAIAgente : IAgenteIA
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;
        private readonly string _modelo;

        public OpenAIAgente(HttpClient httpClient, IConfiguration config)
        {
            _httpClient = httpClient;
            _apiKey = config["IA:OpenAIKey"] ?? throw new ArgumentNullException("OpenAI Key faltante");
            _modelo = config["IA:OpenAIModel"] ?? "gpt-4o";
        }

        public async Task<string> GenerarTextoAsync(string prompt)
        {
            var url = "https://api.openai.com/v1/chat/completions";

            // OpenAI requiere la Key en el Header de Authorization
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _apiKey);

            var payload = new
            {
                model = _modelo,
                messages = new[]
                {
                    new { role = "system", content = "Eres un asistente experto en redacción elocuente y respetuosa." },
                    new { role = "user", content = prompt }
                },
                temperature = 0.4 // Mantener consistencia con Gemini
            };

            try
            {
                var response = await _httpClient.PostAsJsonAsync(url, payload);
                response.EnsureSuccessStatusCode();

                var jsonResponse = await response.Content.ReadFromJsonAsync<JsonElement>();

                // Navegación en el JSON de OpenAI: choices[0].message.content
                return jsonResponse
                    .GetProperty("choices")[0]
                    .GetProperty("message")
                    .GetProperty("content")
                    .GetString() ?? string.Empty;
            }
            catch (Exception ex)
            {
                return $"Error con OpenAI: {ex.Message}";
            }
        }
    }
}
