# Spec Delta

## Purpose

Proveer identidades administrativas y de agente verificables para Migraciones, con credenciales externas y sin persistencia de usuarios ni secretos en Firebird.

## ADDED Requirements

### Requirement: Inicio de sesión administrativo externo
La API de Migraciones SHALL exponer `POST /auth/admin/login` para validar usuario y contraseña configurados externamente y devolver un access token administrativo y su expiración sólo cuando ambos sean válidos.

#### Scenario: Credenciales administrativas válidas
- **CUANDO** un administrador envía el usuario y la contraseña configurados mediante HTTPS
- **ENTONCES** recibe HTTP 200 con un access token administrativo y su expiración

#### Scenario: Credenciales administrativas inválidas
- **CUANDO** se envía un usuario inexistente o una contraseña incorrecta
- **ENTONCES** la API responde HTTP 401 sin token ni indicación de cuál dato fue incorrecto

### Requirement: Identidad administrativa protegida
La API SHALL exponer `GET /auth/me` protegido por `AdminOnly`, devolviendo sólo la identidad administrativa mínima del token autenticado.

#### Scenario: Consulta administrativa autenticada
- **CUANDO** se invoca `GET /auth/me` con token administrativo válido
- **ENTONCES** la API responde HTTP 200 sin secretos ni token en el cuerpo

#### Scenario: Token de agente en consulta administrativa
- **CUANDO** se invoca `GET /auth/me` con token agent válido
- **ENTONCES** la API responde HTTP 403

### Requirement: Autenticación de instalación de agente
La API SHALL exponer `POST /auth/agent/token` para verificar `AgentId` y `AgentSecret` contra el registro externo vigente y devolver un access token agent y su expiración.

#### Scenario: Agente autorizado solicita token
- **CUANDO** un agente habilitado presenta `AgentId` y `AgentSecret` correctos mediante HTTPS
- **ENTONCES** recibe HTTP 200 con token que identifica al `AgentId` y no contiene el secreto

#### Scenario: Credencial de agente inválida
- **CUANDO** el `AgentId` no existe, está deshabilitado o el secreto es incorrecto
- **ENTONCES** la API responde HTTP 401 sin token ni detalle sobre la causa

### Requirement: Independencia entre agentes
La validación y autorización SHALL asociar cada token agent sólo con su `AgentId`, sin que el secreto, estado o revocación de otro agente le conceda acceso.

#### Scenario: Token no representa a otro agente
- **CUANDO** dos agentes habilitados obtienen tokens y uno consulta su identidad
- **ENTONCES** la respuesta identifica exclusivamente al `AgentId` emisor del token

#### Scenario: Revocación aislada
- **CUANDO** se revoca un agente mientras otro permanece habilitado
- **ENTONCES** sólo el token del agente revocado deja de satisfacer `AgentOnly`

### Requirement: Identidad de agente protegida
La API SHALL exponer `GET /auth/agent/me` protegido por `AgentOnly`, devolviendo la identidad mínima del agente autenticado y sin secretos.

#### Scenario: Consulta de agente autenticada
- **CUANDO** se invoca `GET /auth/agent/me` con token agent válido de un agente habilitado
- **ENTONCES** la API responde HTTP 200 con su `AgentId`

#### Scenario: Token administrativo en consulta de agente
- **CUANDO** se invoca `GET /auth/agent/me` con token administrativo válido
- **ENTONCES** la API responde HTTP 403

### Requirement: Tokens firmados y diferenciados
La API SHALL emitir JWT con firma, issuer, audience y expiración configurados, y `migration_identity` con valor `admin` o `agent`. Cada policy SHALL aceptar exclusivamente su tipo de identidad.

#### Scenario: Token adulterado o vencido
- **CUANDO** se presenta un token con firma modificada o vencido a una ruta protegida
- **ENTONCES** la API responde HTTP 401 y no ejecuta la operación

#### Scenario: Issuer, audience o identidad no aceptados
- **CUANDO** se presenta un token firmado con issuer, audience o identidad no aceptados por la ruta
- **ENTONCES** la API responde HTTP 401 o HTTP 403 según corresponda y no concede acceso

### Requirement: Revocación individual de agentes
La API SHALL impedir que un agente revocado obtenga tokens nuevos y que sus tokens existentes satisfagan `AgentOnly`, sin base de datos.

#### Scenario: Agente revocado con token vigente
- **CUANDO** se deshabilita o elimina un agente y el cambio queda aplicado
- **ENTONCES** una solicitud posterior con su token vigente a `AgentOnly` responde HTTP 403

#### Scenario: Fuente sin recarga dinámica
- **CUANDO** el registro viene de una fuente sin recarga dinámica
- **ENTONCES** la revocación queda efectiva tras el reinicio o redespliegue documentado de la API

### Requirement: Rotación de secreto de agente
La API SHALL aceptar sólo el secreto configurado vigente para emitir nuevos tokens. Rotar el secreto no SHALL revocar JWT emitidos mientras el agente continúe habilitado.

#### Scenario: Secreto anterior tras rotación
- **CUANDO** se aplica un nuevo hash de secreto para un agente habilitado
- **ENTONCES** el secreto anterior recibe HTTP 401 y un JWT previo continúa autorizado hasta expirar

#### Scenario: Rotación con revocación requerida
- **CUANDO** se requiere invalidar inmediatamente tokens anteriores a una rotación
- **ENTONCES** al deshabilitar o eliminar al agente sus tokens reciben HTTP 403 tras aplicar la configuración

### Requirement: Configuración segura y parámetros validados
La API SHALL obtener hashes, agentes y parámetros JWT/PBKDF2 desde fuentes externas y SHALL rechazar configuración requerida ausente, vacía o fuera de límites validados.

#### Scenario: Registro de agentes vacío
- **CUANDO** la configuración activa no contiene agentes autorizados
- **ENTONCES** ninguna solicitud de token agent recibe token

#### Scenario: Configuración criptográfica inválida
- **CUANDO** falta o es inválida clave HMAC, duración JWT o parámetro PBKDF2 requerido
- **ENTONCES** la API no queda disponible para autenticar ni autorizar

### Requirement: Generación segura de hashes iniciales
La documentación SHALL proporcionar un mecanismo local que lea una credencial sin exponerla como argumento ni en logs y produzca sólo el hash PBKDF2 para almacenar externamente.

#### Scenario: Aprovisionamiento sin secreto visible
- **CUANDO** un operador genera un hash siguiendo la documentación
- **ENTONCES** puede cargar el hash sin que el secreto figure en comandos, archivos versionados o diagnósticos

### Requirement: Transporte y proxies de confianza
Fuera de pruebas locales controladas, la API SHALL rechazar HTTP en rutas de autenticación o protegidas antes de procesar credenciales o tokens. Sólo SHALL aceptar forwarded headers de proxies o redes autorizados.

#### Scenario: Solicitud insegura a autenticación
- **CUANDO** se envían credenciales por HTTP donde se exige HTTPS
- **ENTONCES** la solicitud se rechaza antes de validar credenciales o emitir token

#### Scenario: Forwarded header no confiable
- **CUANDO** un cliente no autorizado declara HTTPS mediante forwarded header
- **ENTONCES** la API no usa ese header para tratar la solicitud como segura

### Requirement: Exposición segura y rutas operativas
La API SHALL mantener `GET /health` anónimo y Swagger sólo en Development; errores, respuestas y logs SHALL NOT exponer contraseñas, secretos ni tokens.

#### Scenario: Health check anónimo
- **CUANDO** un monitor solicita `GET /health` sin autenticación
- **ENTONCES** recibe estado de salud sin requerir token

#### Scenario: Error de autenticación
- **CUANDO** falla autenticación o autorización
- **ENTONCES** respuesta y logs no incluyen contraseña, `AgentSecret`, hash, Authorization ni token

### Requirement: Resistencia a intentos repetidos
La API SHALL limitar intentos repetidos contra ambos endpoints de token y responder HTTP 429 sin validar ni emitir token al exceder el límite configurado.

#### Scenario: Límite de inicio de sesión excedido
- **CUANDO** un origen supera su cuota durante la ventana activa
- **ENTONCES** la API responde HTTP 429 y no valida ni emite token

### Requirement: Renovación anticipada coordinada del Agent
El Agent SHALL renovar por reautenticación antes del margen de expiración configurado y SHALL compartir una única renovación activa entre solicitudes concurrentes. No SHALL usar refresh tokens ni persistir tokens o credenciales.

#### Scenario: Renovación antes del vencimiento
- **CUANDO** el token entra en su margen de renovación
- **ENTONCES** el Agent solicita uno nuevo mediante HTTPS antes de vencer

#### Scenario: Solicitudes concurrentes de renovación
- **CUANDO** varias operaciones requieren token mientras falta o vence el mismo token
- **ENTONCES** el Agent realiza una sola solicitud y las operaciones usan su resultado

### Requirement: Recuperación del Agent ante fallos transitorios
El Agent SHALL reintentar red, timeout y HTTP 429 con espera exponencial limitada y jitter, y SHALL permanecer operativo para recuperarse cuando vuelva la API.

#### Scenario: API temporalmente indisponible
- **CUANDO** la solicitud de token falla transitoriamente por red o timeout
- **ENTONCES** el Agent registra un error seguro, espera según su política y reintenta sin terminar definitivamente el Worker

#### Scenario: Rate limit temporal
- **CUANDO** la API responde HTTP 429, con o sin `Retry-After` válido
- **ENTONCES** el Agent espera el intervalo aplicable con jitter y reintenta sin exponer credenciales

### Requirement: Credenciales inválidas del Agent
El Agent SHALL tratar HTTP 401 de token como credencial inválida y SHALL evitar reintentos automáticos continuos hasta cambio de configuración o reinicio controlado.

#### Scenario: API rechaza credenciales del Agent
- **CUANDO** la API responde HTTP 401 a la solicitud de token
- **ENTONCES** el Agent registra un error seguro y no repite automáticamente la solicitud con las mismas credenciales
