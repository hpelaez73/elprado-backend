## Purpose

Proveer a las tools MCP las consultas de propuestas desde una fachada propia,
autenticada y estable, sin depender de los contratos HTTP del frontend.

## ADDED Requirements

### Requirement: Consultas MCP protegidas de propuestas
ElPrado.McpApi SHALL exponer rutas bajo `/api` para consultar detalle de
propuesta, titulares, cuenta corriente, servicios, comprobantes, contratos e
historial de titulares. Todas SHALL requerir un JWT de empleado válido.

#### Scenario: Consulta autenticada
- **WHEN** `elprado-mcp` solicita una consulta de propuesta con Bearer válido
- **THEN** recibe los datos de negocio solicitados en un contrato MCP
  estructurado

#### Scenario: Consulta no autenticada
- **WHEN** un consumidor invoca cualquiera de esas rutas sin un JWT válido
- **THEN** ElPrado.McpApi responde HTTP 401 sin ejecutar la consulta

### Requirement: Contratos independientes del frontend
Las rutas MCP SHALL aceptar solo los campos necesarios para cada tool y SHALL
devolver respuestas estructuradas para agentes. Los errores de validación,
ausencia de propuesta o fallo controlado SHALL usar el contrato de error MCP y
SHALL NOT exponer detalles internos.

#### Scenario: Datos inválidos o no disponibles
- **WHEN** una tool envía una propuesta inválida o inexistente
- **THEN** recibe un error estructurado que permite al agente informar el
  problema sin interpretar la respuesta de ElPrado.WebApi

### Requirement: Migración del consumidor MCP
`elprado-mcp` SHALL invocar exclusivamente las rutas de negocio de
ElPrado.McpApi usando su JWT vigente y SHALL conservar las entradas y salidas
de sus tools para los clientes MCP.

#### Scenario: Tool migrada
- **WHEN** un cliente MCP invoca una tool existente de propuestas
- **THEN** la tool obtiene su información desde ElPrado.McpApi y conserva su
  contrato MCP publicado
