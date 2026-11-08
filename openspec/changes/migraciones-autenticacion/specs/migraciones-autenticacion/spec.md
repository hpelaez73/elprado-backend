# Spec Delta

## Purpose

Proveer identidades administrativas y de agente verificables para la plataforma de Migraciones, sin persistencia de usuarios ni secretos en Firebird.

## ADDED Requirements

### Requirement: Inicio de sesión administrativo externo
La API de Migraciones SHALL exponer `POST /auth/admin/login` para recibir usuario y contraseña, validar la credencial administrativa configurada externamente y devolver un access token y su expiración únicamente cuando ambos datos sean válidos.

#### Scenario: Credenciales administrativas válidas
- **CUANDO** un administrador envía el usuario y la contraseña configurados a `POST /auth/admin/login`
- **ENTONCES** recibe HTTP 200 con un access token administrativo y su instante o duración de expiración

#### Scenario: Credenciales administrativas inválidas
- **CUANDO** se envía un usuario inexistente o una contraseña incorrecta
- **ENTONCES** la API responde HTTP 401 sin token ni indicación de cuál dato fue incorrecto

### Requirement: Identidad administrativa protegida
La API de Migraciones SHALL exponer `GET /auth/me` protegido por `AdminOnly`, devolviendo sólo la identidad administrativa mínima del token autenticado.

#### Scenario: Consulta administrativa autenticada
- **CUANDO** se invoca `GET /auth/me` con un token administrativo válido
- **ENTONCES** la API responde HTTP 200 con la identidad administrativa y sin secretos ni token en el cuerpo

#### Scenario: Token de agente en consulta administrativa
- **CUANDO** se invoca `GET /auth/me` con un token de agente válido
- **ENTONCES** la API responde HTTP 403

### Requirement: Autenticación de instalación de agente
La API de Migraciones SHALL exponer `POST /auth/agent/token` para recibir `AgentId` y `AgentSecret`, verificarlos contra el registro externo de agentes autorizados y devolver un access token de agente y su expiración.

#### Scenario: Agente autorizado solicita token
- **CUANDO** un agente habilitado presenta su `AgentId` y `AgentSecret` correctos mediante HTTPS
- **ENTONCES** recibe HTTP 200 con un token de agente que identifica al `AgentId` y no contiene el secreto

#### Scenario: Credencial de agente inválida
- **CUANDO** el `AgentId` no existe, está deshabilitado o el secreto es incorrecto
- **ENTONCES** la API responde HTTP 401 sin token ni detalle sobre la causa

### Requirement: Identidad de agente protegida
La API de Migraciones SHALL exponer `GET /auth/agent/me` protegido por `AgentOnly`, devolviendo la identidad mínima del agente autenticado y sin secretos.

#### Scenario: Consulta de agente autenticada
- **CUANDO** se invoca `GET /auth/agent/me` con un token de agente válido de un agente aún habilitado
- **ENTONCES** la API responde HTTP 200 con su `AgentId`

#### Scenario: Token administrativo en consulta de agente
- **CUANDO** se invoca `GET /auth/agent/me` con un token administrativo válido
- **ENTONCES** la API responde HTTP 403

### Requirement: Tokens firmados y diferenciados
La API SHALL emitir JWT con firma, issuer, audience y expiración configurados, y una marca de tipo de identidad no ambigua. La validación SHALL rechazar tokens con firma, issuer, audience o expiración no válidos; `AdminOnly` y `AgentOnly` SHALL aceptar exclusivamente su respectivo tipo de identidad.

#### Scenario: Token adulterado o vencido
- **CUANDO** se presenta un token con firma modificada o con vencimiento superado a una ruta protegida
- **ENTONCES** la API responde HTTP 401 y no ejecuta la operación protegida

#### Scenario: Issuer o audience no aceptados
- **CUANDO** se presenta un token firmado pero con issuer o audience distintos de la configuración activa
- **ENTONCES** la API responde HTTP 401

### Requirement: Revocación individual de agentes
La API SHALL impedir que un agente revocado o deshabilitado obtenga tokens nuevos y que sus tokens existentes satisfagan `AgentOnly`, sin requerir una base de datos.

#### Scenario: Agente revocado con token vigente
- **CUANDO** se deshabilita o elimina un agente del registro externo después de emitirle un token aún vigente
- **ENTONCES** una solicitud posterior a una ruta `AgentOnly` responde HTTP 403

### Requirement: Configuración de credenciales segura y completa
La API SHALL obtener hashes de credenciales, la lista de agentes y parámetros JWT desde fuentes externas al código versionado. Si falta, está vacía o es inválida una configuración de seguridad requerida, la aplicación SHALL NOT emitir tokens ni conceder acceso protegido.

#### Scenario: Registro de agentes vacío
- **CUANDO** la configuración activa no contiene agentes autorizados
- **ENTONCES** ninguna solicitud a `POST /auth/agent/token` recibe un token

#### Scenario: Configuración JWT incompleta
- **CUANDO** falta un parámetro requerido para firmar o validar tokens
- **ENTONCES** la API no queda disponible para autenticar ni autorizar solicitudes

### Requirement: Transporte, respuestas y exposición segura
Fuera de pruebas locales controladas, la API SHALL requerir HTTPS para rutas de autenticación y rutas protegidas. SHALL mantener `GET /health` anónimo y Swagger sólo en Development; errores, respuestas y logs SHALL NOT exponer contraseñas, secretos ni access tokens.

#### Scenario: Health check anónimo
- **CUANDO** un monitor solicita `GET /health` sin autenticación
- **ENTONCES** recibe el estado de salud sin requerir token

#### Scenario: Solicitud insegura fuera de entorno local controlado
- **CUANDO** se envían credenciales o un token por HTTP en un entorno que exige HTTPS
- **ENTONCES** la solicitud no se procesa como autenticada

### Requirement: Resistencia a intentos repetidos
La API SHALL limitar intentos repetidos contra ambos endpoints de emisión de token y responder sin revelar credenciales o tokens cuando se exceda el límite configurado.

#### Scenario: Límite de inicio de sesión excedido
- **CUANDO** un origen supera el límite configurado de solicitudes a un endpoint de autenticación durante su ventana activa
- **ENTONCES** la API responde HTTP 429 y no valida ni emite un token para esa solicitud

### Requirement: Renovación del token del agente
El Agent SHALL conservar su access token sólo hasta su expiración y solicitar uno nuevo presentando nuevamente `AgentId` y `AgentSecret`; SHALL NOT usar refresh tokens ni registrar esas credenciales.

#### Scenario: Renovación mediante reautenticación
- **CUANDO** el token de un agente vence o no está disponible
- **ENTONCES** el Agent solicita un token nuevo a `POST /auth/agent/token` con sus credenciales configuradas mediante HTTPS

#### Scenario: Error de autenticación del agente
- **CUANDO** la API rechaza la solicitud de token del Agent
- **ENTONCES** el Agent registra un error seguro sin incluir el secreto, la contraseña ni el token
