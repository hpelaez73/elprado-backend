## Why

Los contratos actuales de propuestas de `ElPrado.McpApi` reproducen parte de la forma de los DTO y filtros del frontend, y obligan a los consumidores MCP a conocer identificadores internos y convenciones técnicas. Se necesita una versión v2 orientada a agentes que exponga hechos de negocio explícitos, estables y trazables al dominio sin alterar los consumidores actuales.

## What Changes

- Incorporar siete endpoints `GET` bajo `/api/v2/propuestas`, identificados por número público de propuesta, para detalle, titulares, deuda, servicios, contratos, comprobantes e historial de titulares.
- Adoptar como contratos canónicos y vinculantes los JSON diseñados en `docs/mcp-api-v2-propuestas.md`, secciones 3 y 5 a 11; la implementación SHALL respetar sus nombres, anidamiento, tipos, nulos, colecciones y semántica por endpoint.
- Definir contratos MCP v2 independientes de los DTO del frontend: envelope estable, fechas ISO 8601, importes `decimal`, valores ausentes `null` y colecciones vacías.
- Proyectar estados configurables mediante nombre y flags funcionales provistos por el dominio; eliminar del contrato datos visuales, filtros genéricos y estructuras posicionales heredadas.
- Exponer consultas de comprobantes con filtros de dominio y paginación explícita.
- Mantener las rutas y contratos v1 existentes durante la transición; v2 no modifica ni elimina sus consumidores.
- Extender `ElPrado.Services`, DTOs, repositorios y consultas Firebird cuando sea necesario para satisfacer cada campo y hecho exigido por los contratos canónicos, sin reemplazarlos con heurísticas en `McpApi`.

## Capabilities

### New Capabilities

- `mcp-proposal-v2-contracts`: consultas REST v2 de propuestas orientadas a agentes, con proyecciones semánticas de los hechos de negocio.

### Modified Capabilities

- Ninguna.

## Impact

- Afecta `ElPrado.McpApi` (rutas, contratos, mappers, manejo de errores, Swagger y pruebas), `ElPrado.Services`, sus DTO/modelos de consulta, repositorios y procedimientos/consultas Firebird cuando el origen actual no cubra el contrato canónico.
- Agrega la superficie autenticada `/api/v2/propuestas` sin retirar `/api/propuestas`.
- Requiere pruebas de contrato y de mapeo contra resultados del ERP/Firebird; no incorpora dependencias externas nuevas.
