## Purpose

Permitir que el consumidor `elprado-mcp` se autentique ante la fachada MCP con
una identidad de empleado verificable mediante tokens JWT de corta integración.

## ADDED Requirements

### Requirement: Inicio de sesión de empleado para el consumidor MCP
ElPrado.McpApi SHALL exponer `POST /api/login/usuario` para recibir `alias` y
`clave` de un empleado. Cuando las credenciales son válidas, SHALL devolver una
respuesta de éxito que incluya un JWT de acceso y la identidad mínima del
empleado autenticado. La validación de las credenciales SHALL conservar las
reglas y efectos de auditoría del login de empleado existente de El Prado.

#### Scenario: Credenciales de empleado válidas
- **WHEN** `elprado-mcp` envía un alias y una clave válidos a
  `POST /api/login/usuario`
- **THEN** recibe una respuesta exitosa con un JWT de acceso y los datos de
  identidad necesarios para asociar sus siguientes solicitudes al empleado

#### Scenario: Credenciales inválidas
- **WHEN** un consumidor envía un alias inexistente o una clave incorrecta
- **THEN** el servicio responde con HTTP 401 y no revela cuál de los dos datos
  fue inválido ni emite un token

### Requirement: JWT compatible y con identidad de empleado
El JWT emitido por ElPrado.McpApi SHALL estar firmado con la configuración JWT
del entorno y SHALL incluir emisor, audiencia, vencimiento e identidad del
empleado. Para compatibilidad con los servicios de dominio existentes, SHALL
incluir los claims `NameIdentifier`, `Name`, `CodUsuario`, `CodCliente`,
`CodPropuesta` y `EsTesting`; para un empleado, los valores de cliente y
propuesta SHALL representar la ausencia de esas identidades. El token SHALL
tener una vigencia finita configurada o, mientras no exista configuración
específica, la vigencia de 24 horas usada por ElPrado.WebApi.

#### Scenario: Token emitido para empleado
- **WHEN** el inicio de sesión se completa correctamente
- **THEN** el JWT resultante puede ser validado con la clave y parámetros JWT
  configurados para el entorno, contiene la identidad del empleado y expira al
  finalizar su vigencia

### Requirement: Protección Bearer de las operaciones MCP
ElPrado.McpApi SHALL aceptar autenticación Bearer para sus operaciones de
negocio bajo `/api`, excepto el endpoint de login. Toda operación de negocio
actual o futura SHALL requerir un JWT válido, vigente y firmado con la
configuración del entorno. `GET /health` SHALL permanecer anónimo.

#### Scenario: Operación de negocio con token válido
- **WHEN** `elprado-mcp` invoca una operación protegida con
  `Authorization: Bearer <jwt>` válido
- **THEN** la operación puede leer la identidad del empleado autenticado y
  procesar la solicitud

#### Scenario: Token ausente, inválido o vencido
- **WHEN** un consumidor invoca una operación protegida sin Bearer, con una
  firma inválida o con un token vencido
- **THEN** ElPrado.McpApi rechaza la solicitud con HTTP 401 y no ejecuta la
  operación de negocio

#### Scenario: Health check anónimo
- **WHEN** un monitor solicita `GET /health` sin encabezado Authorization
- **THEN** recibe el estado de disponibilidad del proceso

### Requirement: Contrato de uso desde el servidor MCP hermano
El consumidor `elprado-mcp` SHALL iniciar sesión contra
`POST /api/login/usuario`, conservar el JWT solo durante su vigencia y
reenviarlo como Bearer en cada llamada a rutas protegidas de ElPrado.McpApi.
Las credenciales y la clave JWT SHALL provenir de configuración segura del
entorno y SHALL NOT aparecer en código fuente, logs ni documentación con
valores reales.

#### Scenario: Llamada autenticada desde el servidor MCP
- **WHEN** `elprado-mcp` necesita invocar una operación protegida y dispone de
  un JWT vigente
- **THEN** envía el token en el encabezado Authorization y ElPrado.McpApi
  identifica la solicitud con los claims de ese token
