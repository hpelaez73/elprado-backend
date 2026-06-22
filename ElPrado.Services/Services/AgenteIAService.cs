using ElPrado.Core.Enums;
using ElPrado.Services.Interfaces;
using Microsoft.Extensions.Configuration;

namespace ElPrado.Services.Services
{
    public class AgenteIAService
    {
        private readonly IAgenteIA _agente;

        public AgenteIAService(Func<string, IAgenteIA> agenteFactory, IConfiguration config)
        {
            // Aquí eliges dinámicamente qué agente usar desde la configuración
            string proveedor = config["IA:ProveedorActivo"] ?? "Gemini";
            _agente = agenteFactory(proveedor);
        }

        public async Task<string> MejorarTextoAsync(
            string texto,
            ContextoIA contexto)
        {
            string prompt = contexto switch
            {
                ContextoIA.Condolencia =>
                    $"Mejora la redacción del siguiente mensaje de condolencia para que suene más elocuente y respetuoso, utilizando las palabras y frases importantes, manteniendo el significado original. Devuelve solo el texto mejorado, sin agregar introducciones ni despedidas: \"{texto}\"",

                ContextoIA.Comentario =>
                    $"Mejora la redacción del siguiente recuerdo o comentario para un obituario para que suene más sentido y claro, utilizando las palabras y frases importantes, manteniendo el significado original. Devuelve solo el texto mejorado, sin agregar introducciones ni despedidas: \"{texto}\"",

                ContextoIA.Biografia =>
                    $"Mejora la redacción del siguiente texto biográfico para que suene más fluido y atractivo, utilizando las palabras y frases importantes, manteniendo el significado original. Devuelve solo el texto mejorado, sin agregar introducciones ni despedidas: \"{texto}\"",

                _ =>
                    $"Mejora la redacción del siguiente texto para que sea más legible y sin errores ortográficos, manteniendo el significado original. Devuelve solo el texto mejorado, sin agregar introducciones ni despedidas: \"{texto}\""
            };

            return await _agente.GenerarTextoAsync(prompt);
        }   
    }
}
