## Purpose

Proporcionar una fachada HTTP independiente para que el servidor MCP de El Prado consuma operaciones de negocio mediante contratos predecibles y adecuados para agentes.

## ADDED Requirements

### Requirement: API independiente para consumidores MCP
El sistema SHALL proporcionar `ElPrado.McpApi` como un servicio HTTP REST independiente dirigido a .NET 7.0. El servicio SHALL poder iniciarse y desplegarse sin requerir que `ElPrado.WebApi` ni `ElPrado.Workers` esten ejecutandose, y no SHALL implementar el protocolo MCP ni modificar los contratos HTTP existentes de esas fachadas.

#### Scenario: Inicio aislado del servicio
- **WHEN** se inicia `ElPrado.McpApi` con una configuracion valida para su entorno
- **THEN** el proceso queda disponible para atender solicitudes HTTP de su propia fachada sin iniciar ni redirigir solicitudes a las otras fachadas de El Prado

### Requirement: Contratos HTTP orientados a agentes
Las operaciones de negocio que se incorporen a `ElPrado.McpApi` SHALL exponer contratos especificos para esta fachada, independientes de los contratos destinados al frontend. Las respuestas SHALL representar el resultado de forma estructurada y predecible, incluyendo datos cuando la operacion tiene exito o un codigo y mensaje de error cuando no puede completarse; no SHALL requerir que el consumidor interprete presentacion o textos propios de una interfaz web.

#### Scenario: Operacion de negocio exitosa
- **WHEN** el servidor MCP externo invoca una operacion HTTP valida de `ElPrado.McpApi`
- **THEN** recibe una respuesta estructurada que identifica el resultado y entrega solo los datos necesarios para que un agente continue su flujo

#### Scenario: Error controlado de negocio o validacion
- **WHEN** una operacion no puede completarse por datos invalidos o una regla de negocio existente
- **THEN** la API devuelve un error estructurado con un codigo y mensaje comprensibles para un agente, sin exponer detalles internos ni convertirlo en una respuesta destinada al frontend

### Requirement: Reutilizacion de comportamiento de dominio
Las operaciones de `ElPrado.McpApi` SHALL delegar validacion, reglas de negocio y persistencia en servicios, repositorios e infraestructura compartidos cuando estos cubran la necesidad. La nueva fachada SHALL no duplicar ni alterar el comportamiento de dominio solo para adaptar su representacion HTTP.

#### Scenario: Adaptacion de una operacion existente
- **WHEN** se agrega una operacion de la nueva API que representa una capacidad existente de El Prado
- **THEN** la operacion reutiliza el comportamiento de dominio existente y transforma exclusivamente su contrato de entrada o salida para el consumidor MCP

### Requirement: Comprobacion HTTP de disponibilidad
El servicio SHALL exponer `GET /health` para confirmar que el proceso esta activo. La respuesta SHALL indicar disponibilidad sin incluir informacion sensible y sin requerir acceso a Firebird.

#### Scenario: Health check del proceso activo
- **WHEN** un monitor o el servidor MCP externo solicita `GET /health` al servicio iniciado
- **THEN** recibe una respuesta HTTP exitosa que identifica a `ElPrado.McpApi` como disponible

### Requirement: Configuracion segura y consistente por entorno
El servicio SHALL cargar configuracion base y especifica por entorno, y SHALL permitir que las variables de entorno reemplacen valores configurables, incluidos secretos y datos de conexion. El servicio SHALL registrar eventos usando el formato y destinos configurados para el entorno y SHALL no requerir que secretos se almacenen en archivos versionados.

#### Scenario: Secreto proporcionado por variable de entorno
- **WHEN** un valor sensible necesario para una integracion se proporciona mediante una variable de entorno
- **THEN** el servicio usa ese valor sin exigir que el secreto este presente en `appsettings.json`

#### Scenario: Origen web no configurado
- **WHEN** un navegador realiza una solicitud cross-origin y no se configuraron origenes permitidos para el servicio
- **THEN** el servicio no habilita acceso cross-origin de forma abierta
