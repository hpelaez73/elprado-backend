## Why

El login JWT ya permite identificar a `elprado-mcp`, pero `ElPrado.McpApi` aún
no expone las operaciones que sus tools consumen. Para que el servidor MCP use
la fachada protegida de extremo a extremo, las consultas de negocio deben
migrarse desde los contratos de frontend a contratos propios de MCP.

## What Changes

- Exponer en `ElPrado.McpApi` las consultas actualmente usadas por las tools de
  propuestas: detalle, titulares, cuenta corriente, servicios, comprobantes,
  contratos e historial.
- Proteger todas esas rutas con el JWT de empleado y usar su contexto de
  identidad al delegar en el dominio.
- Definir contratos MCP explícitos, estables y estructurados, sin reutilizar
  la envoltura HTTP del frontend.
- Adaptar `elprado-mcp` para consumir exclusivamente las nuevas rutas MCP.

## Capabilities

### New Capabilities

- `mcp-proposal-business-operations`: Consultas de propuestas para tools MCP
  mediante rutas protegidas de `ElPrado.McpApi`.

### Modified Capabilities

Ninguna.

## Impact

- `ElPrado.McpApi`: adaptadores HTTP, contratos, DI de servicios de dominio y
  autorización JWT.
- `ElPrado.Services` y repositorios existentes: reutilización de las consultas
  y reglas de negocio sin duplicarlas.
- `elprado-mcp`: base URL y mapeo de respuestas hacia los contratos de la
  nueva fachada.
- `ElPrado.WebApi` conserva sus endpoints y contratos para frontend sin
  cambios.
