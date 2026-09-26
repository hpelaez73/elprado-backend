## Why

El servidor MCP existente de El Prado necesita una API de backend cuyo contrato este pensado para la invocacion de herramientas por agentes, en lugar de reutilizar contratos HTTP diseñados para el frontend. Esa fachada debe preservar la logica de negocio y acceso a datos ya probados, sin convertir `ElPrado.McpApi` en otro servidor MCP.

## What Changes

- Crear el proyecto web independiente `ElPrado.McpApi`, con Minimal APIs y destino .NET 7.0, e incorporarlo a `ElPradoWeb.sln`.
- Exponer una fachada HTTP REST para que el servidor MCP externo consuma operaciones de El Prado con contratos de solicitud y respuesta orientados a agentes.
- Establecer un formato de respuesta estructurado y predecible para resultados y errores de esta nueva API, separado de los contratos de `ElPrado.WebApi`.
- Reutilizar servicios de dominio, repositorios, infraestructura Firebird, DI y logging existentes, sin duplicar reglas de negocio.
- Incluir un endpoint HTTP de health check para comprobar que el proceso esta activo.
- Configurar archivos por entorno y variables de entorno para secretos y datos sensibles; CORS permanece deshabilitado salvo que un consumidor web concreto lo requiera.
- Dejar una frontera para autenticacion entre servicios futura, sin implementar OAuth, login, JWT fijo ni protocolo MCP en este proyecto.

## Capabilities

### New Capabilities

- `mcp-api-server`: Fachada HTTP REST independiente para el servidor MCP externo, con contratos adaptados a agentes y diagnostico de disponibilidad.

### Modified Capabilities

Ninguna.

## Impact

- Se agregaran `ElPrado.McpApi`, su configuracion y su entrada en `ElPradoWeb.sln`.
- El nuevo proceso podra referenciar proyectos compartidos solo cuando sean necesarios para reutilizar comportamiento existente.
- Se agregaran contratos propios de la fachada sin modificar los contratos que consume el frontend desde `ElPrado.WebApi`.
- No se incorpora `ModelContextProtocol.AspNetCore`, rutas `/mcp`, tools MCP, JSON-RPC ni transporte Streamable HTTP: esas responsabilidades pertenecen al servidor MCP externo.
- No se modifican los contratos ni el despliegue de `ElPrado.WebApi`, `Afip.WebApi` o `ElPrado.Workers`.
