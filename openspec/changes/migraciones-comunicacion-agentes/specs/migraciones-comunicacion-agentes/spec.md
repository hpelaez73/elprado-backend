# Spec Delta

## Purpose

Proveer comunicación bidireccional autenticada entre la API de Migraciones y cada Agent conectado, con visibilidad administrativa local y una comprobación puntual de conectividad.

## ADDED Requirements

### Requirement: Conexión de agente autenticada
La API SHALL exponer `/hubs/agents` sólo para un JWT que satisfaga `AgentOnly`; la identidad de la conexión SHALL ser el `AgentId` validado del token y no un valor declarado por el cliente.

#### Scenario: Agente habilitado se conecta
- **WHEN** un Agent habilitado establece una conexión con token válido
- **THEN** la API acepta la conexión y la asocia exclusivamente con el `AgentId` del JWT

#### Scenario: Identidad no autorizada intenta conectarse
- **WHEN** un administrador, un token inválido o un token vencido intenta usar el Hub
- **THEN** la conexión es rechazada y no se registra como agente conectado

### Requirement: Transporte seguro del token de Hub
La API SHALL aceptar el token de conexión desde el mecanismo Bearer ordinario y, cuando el transporte WebSocket lo requiera, desde `access_token` sólo en `/hubs/agents`; SHALL impedir que ese valor llegue a registros de solicitudes, errores o trazas.

#### Scenario: Token en negociación o WebSocket del Hub
- **WHEN** el cliente SignalR entrega un token para negociar o actualizar `/hubs/agents`
- **THEN** la API lo valida para ese Hub sin exponer el token en sus logs

#### Scenario: Token por query string fuera del Hub
- **WHEN** una solicitud ajena a `/hubs/agents` incluye `access_token` en la query string
- **THEN** ese valor no se usa como credencial Bearer

### Requirement: Presencia local de agentes conectados
La API SHALL mantener la presencia actual por `AgentId`, estado y hora UTC de conexión, sin exponer identificadores de transporte, tokens, secretos ni inventario persistente de instalaciones.

#### Scenario: Conexión y desconexión ordinarias
- **WHEN** un agente autenticado se conecta y luego se desconecta
- **THEN** aparece durante la conexión y deja de aparecer al desconectarse

#### Scenario: Registro vacío
- **WHEN** no hay conexiones activas en el proceso de la API
- **THEN** la consulta de presencia devuelve una colección vacía

### Requirement: Reemplazo seguro de conexiones duplicadas
La API SHALL permitir como máximo una conexión activa registrada por `AgentId`; una conexión nueva SHALL sustituir y solicitar el cierre de la anterior, y una desconexión tardía de ésta SHALL NOT eliminar el registro nuevo ni permitirle ejecutar operaciones.

#### Scenario: Segunda conexión del mismo agente
- **WHEN** un `AgentId` ya conectado establece una nueva conexión autenticada
- **THEN** la nueva conexión queda registrada como activa y la anterior queda invalidada o cerrada

#### Scenario: Desconexión tardía de conexión reemplazada
- **WHEN** la conexión reemplazada informa su desconexión después del reemplazo
- **THEN** la presencia de la conexión nueva permanece sin cambios

### Requirement: Consulta administrativa de agentes
La API SHALL exponer `GET /agents` protegido por `AdminOnly`, que devuelva los agentes actualmente conectados con `AgentId`, estado y hora UTC de conexión, independiente de los detalles internos del transporte.

#### Scenario: Administrador consulta presencia
- **WHEN** un administrador autenticado solicita `GET /agents`
- **THEN** recibe HTTP 200 con la colección actual sin ConnectionId, tokens ni secretos

#### Scenario: Agente consulta presencia administrativa
- **WHEN** un Agent autenticado solicita `GET /agents`
- **THEN** recibe HTTP 403

### Requirement: Ping/Pong administrativo correlacionado
La API SHALL permitir a un administrador iniciar un Ping puntual sobre un agente conectado y SHALL devolver el resultado de su Pong con el mismo identificador de correlación, tiempos UTC de solicitud y respuesta, o un resultado de timeout; este protocolo de aplicación SHALL ser distinto del keep-alive del transporte.

#### Scenario: Ping exitoso
- **WHEN** un administrador solicita Ping para un agente conectado y éste responde antes del límite
- **THEN** la respuesta identifica la correlación y registra los tiempos de solicitud y recepción sin información secreta

#### Scenario: Ping sin respuesta
- **WHEN** un agente conectado no responde Pong dentro del límite configurado
- **THEN** la operación informa timeout sin marcar automáticamente al agente como desconectado

#### Scenario: Ping a agente ausente
- **WHEN** un administrador solicita Ping para un `AgentId` no conectado
- **THEN** la API devuelve un resultado de no disponible sin revelar información de credenciales

### Requirement: Cliente Agent resiliente
El Agent SHALL conectar salientemente a la API usando su mecanismo existente de token en memoria; SHALL reintentar fallos iniciales y reconexiones agotadas con espera acotada y jitter, sin intentos simultáneos y respetando la cancelación del Worker.

#### Scenario: Recuperación tras falla transitoria
- **WHEN** la API se reinicia, la red se interrumpe temporalmente o falla la conexión inicial
- **THEN** el Agent continúa ejecutándose y vuelve a intentar conectarse con espera acotada hasta recuperar la comunicación o detenerse

#### Scenario: Detención controlada
- **WHEN** el host del Agent inicia su detención
- **THEN** cancela los reintentos y detiene la conexión de forma controlada

#### Scenario: Credenciales inválidas o revocadas
- **WHEN** la renovación o conexión falla por credenciales inválidas o revocadas
- **THEN** el Agent no realiza reintentos agresivos de autenticación y registra un diagnóstico sin secretos

### Requirement: Respuesta del Agent a Ping
El Agent SHALL responder Pong al Ping recibido, preservando el identificador de correlación y sin implementar un sistema genérico de comandos ni un sondeo periódico automático.

#### Scenario: Ping recibido por el Agent
- **WHEN** la API envía un Ping correlacionado a la conexión activa
- **THEN** el Agent responde un Pong con la misma correlación mientras la conexión siga autorizada

### Requirement: Revocación durante una conexión
La API SHALL impedir nuevas conexiones de un agente revocado y SHALL cerrar e invalidar su conexión existente al aplicar la revocación; las operaciones posteriores de esa conexión no SHALL ejecutarse.

#### Scenario: Revocación con conexión activa
- **WHEN** un Agent conectado es deshabilitado o eliminado del registro vigente de la Etapa 2
- **THEN** la API elimina su presencia y cierra o invalida la conexión, y el Agent trata el rechazo como una condición de credenciales no transitoria

#### Scenario: Conexión antigua tras revocación
- **WHEN** una conexión establecida antes de la revocación intenta participar en una operación posterior
- **THEN** la API no le concede la operación autorizada

### Requirement: Observabilidad segura y alcance local
La API y el Agent SHALL registrar eventos operativos de conexión, desconexión, reemplazo, Ping, Pong, timeout y reintentos sin incluir `Authorization`, access tokens, secretos, hashes ni ConnectionId en respuestas administrativas; la presencia SHALL representar sólo el proceso local.

#### Scenario: Diagnóstico de comunicación
- **WHEN** ocurre una conexión, desconexión, reintento o timeout
- **THEN** se registra un evento operacional sin credenciales ni token

#### Scenario: Reinicio de la API
- **WHEN** se reinicia el proceso de la API
- **THEN** su registro de presencia se vacía y los Agent vuelven a conectarse según su política de recuperación
