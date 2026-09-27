## Why

Un agente no puede consultar el detalle v2 cuando el usuario desconoce el número público de propuesta. La extensión prevista en la sección 12 del contrato necesita una búsqueda semántica y compacta que permita identificar candidatos antes de continuar con las consultas existentes.

## What Changes

- Incorporar `GET /api/v2/propuestas/buscar` para buscar propuestas por nombre de titular, número de documento o número de parcela.
- Definir un contrato paginado y compacto, con datos suficientes para distinguir candidatos sin exponer su detalle completo ni identificadores internos.
- Validar los criterios de búsqueda y la paginación mediante el envelope y códigos de error v2 existentes.
- Mantener sin cambios las siete operaciones v2 actuales y los endpoints v1.

## Capabilities

### New Capabilities

_Ninguna._

### Modified Capabilities

- `mcp-proposal-v2-contracts`: ampliar la superficie autenticada y el contrato v2 de propuestas con la búsqueda de candidatos.

## Impact

- `ElPrado.McpApi`: ruta, contratos de respuesta y pruebas de integración/contrato.
- `ElPrado.Services`, `ElPrado.Data` y DTOs: consulta parametrizada en Firebird y proyección de resultados de búsqueda.
- Swagger/OpenAPI: publicación del nuevo endpoint y sus parámetros de consulta.
