## REMOVED Requirements

### Requirement: Consultas MCP protegidas de propuestas
**Reason**: La fachada MCP heredada de propuestas se reemplaza por la superficie vigente de consultas bajo `/api/v2/propuestas`.
**Migration**: Los consumidores deben consultar el recurso GET equivalente de `/api/v2/propuestas` con su JWT de empleado.

### Requirement: Contratos independientes del frontend
**Reason**: Los contratos de la fachada heredada se retiran junto con sus rutas.
**Migration**: Los consumidores deben adoptar el envelope y los JSON canónicos publicados para `/api/v2/propuestas`.

### Requirement: MigraciÃ³n del consumidor MCP
**Reason**: Ya no existirá una superficie heredada de propuestas a la que migrar consumidores.
**Migration**: `elprado-mcp` debe usar exclusivamente los recursos GET de `/api/v2/propuestas`.
