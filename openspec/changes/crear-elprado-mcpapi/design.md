## Context

Vease `proposal.md` para la motivacion y `specs/mcp-api-server/spec.md` para el contrato. `elprado-mcp` ya es el servidor que implementa MCP y consume `ElPrado.WebApi`; por tanto, `ElPrado.McpApi` sera una fachada HTTP interna o de servicio a servicio. La solucion actual contiene proyectos compartidos en `net7.0`; `ElPrado.WebApi` registra Firebird/Dapper, servicios de dominio y Serilog, pero sus endpoints y DTOs responden a necesidades del frontend.

## Goals / Non-Goals

**Goals:**

- Aportar un host REST pequeno en .NET 7, disponible de forma independiente para `elprado-mcp`.
- Separar contratos de API para agentes de los contratos de frontend, preservando la logica de negocio compartida.
- Dejar la estructura lista para exponer operaciones de dominio de forma incremental y segura.
- Separar configuracion versionable de valores proporcionados por el entorno.

**Non-Goals:**

- Implementar un servidor MCP, Streamable HTTP, JSON-RPC, tools MCP o cualquier dependencia del SDK MCP.
- Definir en esta change la primera operacion de negocio adaptada; el alcance es la base de la fachada y su contrato comun de respuestas.
- Modificar `ElPrado.WebApi`, migrar sus endpoints o cambiar servicios/repositorios existentes.
- Implementar OAuth, PKCE, login, JWT fijo o autorizacion de usuarios finales.
- Convertir el health check en un readiness check de Firebird.

## Decisions

- Crear `ElPrado.McpApi` con `Microsoft.NET.Sdk.Web`, `net7.0`, nullable e implicit usings, e incorporarlo a `ElPradoWeb.sln`.
  - ASP.NET Core con Minimal APIs provee la superficie HTTP necesaria sin controllers ni otra capa de transporte.
  - Se descarta alojar estas rutas en `ElPrado.WebApi`, porque los contratos y el ciclo de evolucion de los agentes son distintos de los del frontend.

- No agregar `ModelContextProtocol.AspNetCore` ni otra biblioteca MCP.
  - `elprado-mcp` es el limite responsable de protocolo MCP, descubrimiento de tools, transporte y compatibilidad con sus clientes. Esta API solo atiende HTTP REST.
  - Se descarta exponer `/mcp` o implementar JSON-RPC, porque duplicaria responsabilidades que ya cubre el servidor MCP existente.

- Organizar las rutas futuras mediante grupos Minimal API propios de esta fachada y definir DTOs de request/response dentro de `ElPrado.McpApi` (o un proyecto de contratos dedicado solo si aparece un segundo consumidor estable).
  - Los modelos de respuesta tendran resultado, datos o error con codigo/mensaje; asi los agentes no dependen de textos de UI ni de la forma de `ElPrado.WebApi`.
  - Se descarta reutilizar automaticamente los DTOs de frontend: pueden ser adecuados para algunos campos de dominio, pero no constituyen un contrato de agentes estable.

- Mantener las rutas como adaptadores delgados: validan el contrato HTTP, invocan servicios o unit of work existentes y transforman el resultado al contrato de esta API.
  - Las reglas de negocio, consultas y escritura permanecen en proyectos compartidos para evitar divergencia entre fachadas.
  - Se descarta copiar la logica de controllers o reimplementar consultas Dapper en `ElPrado.McpApi`.

- Registrar inicialmente solo infraestructura compartida necesaria para el arranque y futuras operaciones: configuracion por entorno, Serilog, acceso a Firebird y servicios de dominio que se incorporen. No copiar JWT, controllers, Swagger ni Data Protection de `ElPrado.WebApi` sin un consumidor real.

- Cargar `appsettings.json`, luego `appsettings.{Environment}.json` opcional y finalmente variables de entorno. Mantener CORS deshabilitado por defecto; si se agrega un consumidor web concreto, habilitar una lista restrictiva de origenes para sus rutas.
  - El servidor MCP se comunica normalmente como servicio a servicio y no necesita CORS; los secretos y conexiones se inyectan por entorno.

- Mapear `GET /health` como liveness del proceso, separado de las rutas de negocio.
  - Permite a la operacion y a `elprado-mcp` verificar disponibilidad sin depender de Firebird.

- Reservar una frontera para autenticacion de servicio a servicio, sin implementarla en esta entrega.
  - Cuando se defina el mecanismo entre `elprado-mcp` y esta API, se agrega en el pipeline sin cambiar los contratos de negocio.
  - Se descarta un token JWT manual en `.env`, que no constituye un diseno de identidad ni autorizacion sostenible.

## Risks / Trade-offs

- [El primer contrato de negocio para agentes aun no esta definido] → Limitar esta entrega al host, contrato comun y health check; crear una change posterior por cada capacidad de dominio que se adapte.
- [La adaptacion de contratos puede divergir de la logica real] → Mantener rutas delgadas y reutilizar los servicios/repositorios existentes en vez de copiar comportamiento.
- [Los DTOs de frontend se usan accidentalmente como contrato de agentes] → Definir modelos de fachada explicitos y validar respuestas de cada nueva operacion contra sus escenarios MCP.
- [La configuracion actual de WebApi contiene valores sensibles] → No copiar valores al nuevo proyecto; usar placeholders y variables de entorno en despliegue.
- [CORS demasiado permisivo expone la API a navegadores no previstos] → Mantenerlo deshabilitado hasta tener origenes concretos y permitir exclusivamente la lista configurada.

## Migration Plan

1. Agregar el proyecto y su configuracion sin modificar los proyectos de produccion existentes.
2. Restaurar, compilar y ejecutar el nuevo servicio en un entorno de prueba con configuracion no sensible.
3. Verificar `GET /health`, el formato comun de respuestas y el arranque del contenedor de DI.
4. Configurar a `elprado-mcp` para consumir la nueva API solo cuando se incorpore una operacion de negocio concreta en una change posterior.
5. Rollback: retirar el despliegue o ruta de `ElPrado.McpApi`; no hay migraciones de base ni cambios de contrato existentes que revertir.
