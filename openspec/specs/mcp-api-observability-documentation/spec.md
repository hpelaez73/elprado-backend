# mcp-api-observability-documentation Specification

## Purpose

Proporcionar visibilidad operativa en Seq y documentación interactiva y autenticable de la fachada HTTP `ElPrado.McpApi`, sin ampliar su superficie de producción.

## Requirements

### Requirement: Registro estructurado centralizado en Seq
El sistema SHALL emitir los eventos de Serilog de `ElPrado.McpApi` al servidor Datalust Seq configurado, conservando los niveles mínimos y filtros definidos por ambiente. La configuración de Seq SHALL poder sobrescribirse mediante la jerarquía de configuración existente, incluidas las variables de entorno. Las excepciones que produzcan respuestas de error SHALL emitirse con nivel Error, la excepción original y contexto operativo de la solicitud que no incluya credenciales, tokens ni claves secretas.

#### Scenario: Inicio de la API con Seq configurado
- **WHEN** `ElPrado.McpApi` inicia con una URL válida de Seq en su configuración
- **THEN** los eventos estructurados emitidos por la aplicación se entregan al servidor Seq configurado

#### Scenario: Nivel de log definido por ambiente
- **WHEN** la API se inicia en un ambiente que define un nivel mínimo de Serilog distinto
- **THEN** sólo se envían a Seq los eventos admitidos por ese nivel y por los filtros configurados

#### Scenario: Excepción que causa una respuesta de error
- **WHEN** una solicitud de `ElPrado.McpApi` produce una excepción que se traduce en una respuesta de error
- **THEN** Seq recibe un evento Error con la excepción original, el método HTTP, la ruta y el estado de respuesta, sin exponer secretos en sus propiedades

### Requirement: Documento OpenAPI de la fachada MCP
El sistema SHALL publicar un documento OpenAPI que describa las rutas HTTP expuestas por `ElPrado.McpApi`, incluidas sus operaciones anónimas y las que requieren JWT. El documento SHALL declarar el esquema HTTP Bearer JWT para que clientes interactivos puedan proporcionar un token al invocar operaciones protegidas.

#### Scenario: Consulta de documentación en ambiente habilitado
- **WHEN** un operador accede a la documentación en Development o Staging
- **THEN** puede consultar las rutas expuestas y autorizar solicitudes protegidas mediante un token Bearer JWT

#### Scenario: Operación protegida documentada
- **WHEN** un operador revisa una operación de negocio bajo `/api` en la documentación
- **THEN** la documentación indica que la operación requiere autenticación JWT Bearer

### Requirement: Exposición restringida de Swagger UI
El sistema SHALL exponer Swagger UI y su documento OpenAPI únicamente en los ambientes Development y Staging. En Production, el sistema SHALL conservar el comportamiento actual de no exponer esos recursos de documentación.

#### Scenario: Arranque en Development
- **WHEN** `ElPrado.McpApi` se inicia con ambiente Development
- **THEN** Swagger UI y el documento OpenAPI están disponibles

#### Scenario: Arranque en Production
- **WHEN** `ElPrado.McpApi` se inicia con ambiente Production
- **THEN** Swagger UI y el documento OpenAPI no están disponibles
