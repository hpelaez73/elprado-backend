## Why

La coexistencia de las fachadas de propuestas heredada y v2 duplica endpoints y contratos, y deja el sufijo de versión disperso en los objetos de la solución. La versión relevante debe vivir en el historial de Git, mientras que la API vigente debe tener nombres de código claros y únicos.

## What Changes

- Retirar la fachada heredada de propuestas y sus contratos MCP asociados.
- Consolidar la fachada vigente como `PropuestasEndpointExtensions` y sus contratos como `PropuestasContracts`, eliminando el sufijo `V2` de los tipos, archivos, extensiones y pruebas vinculados.
- Conservar el contrato HTTP vigente bajo `/api/v2/propuestas`, incluido su envelope y los recursos de consulta actuales; la limpieza de nombres internos no altera sus JSON ni su semántica.
- **BREAKING** Eliminar las rutas heredadas bajo `/api/propuestas` y sus operaciones POST; los consumidores deberán usar los recursos GET de `/api/v2/propuestas`.

## Capabilities

### New Capabilities

- Ninguna.

### Modified Capabilities

- `mcp-proposal-business-operations`: retirar la superficie MCP heredada de propuestas.
- `mcp-proposal-v2-contracts`: dejar de requerir compatibilidad con las rutas heredadas, manteniendo la superficie HTTP vigente.

## Impact

- Se modificarán `ElPrado.McpApi`, sus contratos, el registro de endpoints, documentación y pruebas de propuestas.
- Los tipos de dominio y sus usos relacionados con la nueva fachada se renombrarán sin cambiar la forma de los JSON publicados ni procedimientos Firebird.
- La eliminación de `/api/propuestas` es incompatible con clientes que aún dependan de esa superficie.
