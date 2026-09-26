## Why

`ElPrado.McpApi` ya carga la configuración de Serilog, pero no incluye el sink necesario para enviar eventos a Datalust Seq. Además, sus consumidores y operadores no disponen de una interfaz OpenAPI interactiva para descubrir y probar las rutas HTTP protegidas, a diferencia de `ElPrado.WebApi`.

## What Changes

- Habilitar el envío de logs estructurados de `ElPrado.McpApi` a Datalust Seq usando la configuración Serilog ya existente.
- Incorporar Swagger/OpenAPI para documentar los endpoints Minimal API de la fachada MCP.
- Configurar Swagger UI con autenticación JWT Bearer y exponerla únicamente en Development y Staging, replicando la política de exposición de `ElPrado.WebApi`.
- Mantener sin cambios los contratos de negocio, autenticación, CORS y el comportamiento de producción de las rutas existentes.

## Capabilities

### New Capabilities

- `mcp-api-observability-documentation`: Registro estructurado en Seq y documentación OpenAPI protegida por entorno para la fachada HTTP MCP.

### Modified Capabilities

Ninguna.

## Impact

- Afecta `ElPrado.McpApi/ElPrado.McpApi.csproj`, `ElPrado.McpApi/Program.cs` y su configuración de Serilog.
- Agrega dependencias para el sink Seq y Swashbuckle/OpenAPI, alineadas con las usadas por `ElPrado.WebApi`.
- Seq recibirá los eventos del proceso cuando su URL esté configurada; Swagger estará disponible sólo fuera de producción en los entornos permitidos.
