## Why

`ElPrado.McpApi` devuelve respuestas de error controladas a sus consumidores, pero las excepciones que originan fallas de operaciones no quedan registradas. Sin el detalle de la excepción y su contexto de solicitud en Serilog/Seq, los operadores no pueden diagnosticar con rapidez por qué falló una consulta.

## What Changes

- Registrar en Serilog las excepciones manejadas y no manejadas por la fachada `ElPrado.McpApi`.
- Incluir contexto de la solicitud que permita identificar la operación fallida sin exponer información sensible al consumidor.
- Mantener el contrato actual de mensajes y códigos de error que recibe el consumidor.

## Capabilities

### New Capabilities

- `mcp-api-exception-logging`: Registro estructurado de excepciones de la fachada MCP junto con la preservación de sus respuestas de error públicas.

### Modified Capabilities

- `mcp-api-observability-documentation`: Los eventos de error de excepciones de la fachada MCP se deben enviar a Serilog/Seq con contexto operativo.

## Impact

- Afecta el manejo de errores y los adaptadores HTTP de `ElPrado.McpApi`, especialmente las rutas de propuestas y el inicio de la aplicación.
- Usa la configuración y los sinks Serilog/Seq ya existentes; no cambia rutas, autenticación ni el formato público de las respuestas de error.
