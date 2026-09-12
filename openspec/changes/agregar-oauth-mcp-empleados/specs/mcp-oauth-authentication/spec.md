## Purpose

Permitir que cada empleado conecte ChatGPT al MCP de El Prado mediante OAuth 2.1, con permisos e identidad verificables y sin secretos fijos compartidos.

## ADDED Requirements

### Requirement: Descubrimiento OAuth para el recurso MCP
El servidor MCP SHALL publicar metadata de recurso protegido en HTTPS y SHALL anunciar el servidor de autorización de El Prado, su identificador canónico de recurso y los scopes disponibles. Cuando una solicitud a MCP no presente un access token válido, el servidor SHALL responder `401 Unauthorized` con un desafío Bearer que permita al cliente descubrir esa metadata.

#### Scenario: ChatGPT descubre una conexión protegida
- **WHEN** ChatGPT intenta conectarse al endpoint MCP sin un access token válido
- **THEN** recibe una respuesta de desafío que referencia la metadata del recurso protegido y puede iniciar el flujo OAuth sin una API key o JWT configurado manualmente

### Requirement: Metadatos y autorización OAuth de empleados
`ElPrado.McpApi` SHALL publicar metadatos OAuth 2.1 en HTTPS que identifiquen de forma canónica al emisor y sus endpoints de autorización, token y claves públicas. SHALL aceptar Authorization Code con PKCE `S256` y el parámetro `resource`, y SHALL permitir que un empleado se autentique únicamente con alias y clave mediante la lógica de credenciales vigente.

#### Scenario: Empleado concede acceso desde ChatGPT
- **WHEN** ChatGPT inicia Authorization Code con PKCE `S256`, un `redirect_uri` permitido y el recurso MCP anunciado
- **THEN** el empleado puede iniciar sesión con alias y clave y, tras otorgar consentimiento al scope solicitado, recibe una redirección con un código de autorización de un solo uso

#### Scenario: Credenciales o solicitud OAuth inválidas
- **WHEN** el alias o clave no son válidos, el código PKCE no coincide, el redirect URI no está registrado, o el recurso solicitado no corresponde al MCP
- **THEN** el servidor rechaza la autorización o el canje sin revelar si existe la cuenta, la clave ni detalles internos

### Requirement: Tokens OAuth vinculados al empleado y al recurso
El servidor de autorización SHALL emitir access tokens verificables para empleados con emisor, audiencia igual al recurso MCP, vencimiento, identidad de sujeto estable y scopes concedidos. El scope inicial SHALL ser `elprado:read`; un token SHALL no incluir permisos no concedidos. Las claves públicas necesarias para verificarlos SHALL estar disponibles mediante JWKS.

#### Scenario: El MCP recibe un token válido de empleado
- **WHEN** ChatGPT invoca una tool con un access token no vencido cuyo emisor, audiencia y scope son válidos
- **THEN** el MCP puede asociar la solicitud al empleado autenticado y ejecutar únicamente capacidades de lectura autorizadas

#### Scenario: Token vencido o destinado a otro recurso
- **WHEN** se presenta un token vencido, alterado, emitido por otro emisor, sin `elprado:read` o con una audiencia diferente
- **THEN** el MCP rechaza la solicitud antes de ejecutar una tool y desafía al cliente a reautorizarse

### Requirement: Renovación y revocación de la autorización OAuth
El sistema SHALL administrar códigos de autorización y refresh tokens OAuth como credenciales distintas de los refresh tokens actuales de la aplicación. Los códigos SHALL expirar y poder usarse una sola vez; los refresh tokens SHALL estar sujetos a expiración, rotación y revocación. El almacenamiento SHALL no conservar secretos de OAuth en texto plano recuperable.

#### Scenario: Renovación válida de acceso
- **WHEN** ChatGPT presenta un refresh token OAuth vigente asociado al mismo cliente, empleado y recurso
- **THEN** recibe un nuevo access token con los scopes previamente concedidos y el refresh token anterior deja de ser reutilizable

#### Scenario: Reutilización o revocación de credencial
- **WHEN** se reutiliza un código de autorización, se presenta un refresh token revocado o se detecta reutilización de un refresh token rotado
- **THEN** el canje se rechaza y las credenciales afectadas no otorgan acceso posterior

### Requirement: Propagación de identidad hacia la API MCP
El servidor MCP SHALL eliminar la dependencia de un token fijo de entorno y SHALL validar el Bearer token por solicitud antes de propagarlo a las rutas protegidas de `ElPrado.McpApi`. La fachada SHALL derivar el contexto de empleado de los claims del token sin depender de `ElPrado.WebApi`.

#### Scenario: Dos empleados usan el MCP de forma independiente
- **WHEN** dos empleados autorizados usan herramientas MCP en sesiones separadas
- **THEN** cada llamada de backend se procesa con la identidad y permisos del token de la sesión correspondiente, sin compartir credenciales entre empleados

#### Scenario: Configuración de despliegue sin JWT de usuario
- **WHEN** se inicia `elprado-mcp` con configuración válida de URL y OAuth
- **THEN** puede atender conexiones protegidas sin requerir `ELPRADO_API_TOKEN` ni otro JWT de usuario almacenado en `.env`

### Requirement: Preparación de identidad para clientes
El contrato de tokens SHALL distinguir el tipo de sujeto y conservar la posibilidad de asociar un cliente con su propuesta sin cambiar el significado de la identidad de empleado. Esta entrega SHALL no habilitar el inicio de sesión de clientes por DNI, propuesta y clave.

#### Scenario: Solicitud de login de cliente durante la primera entrega
- **WHEN** una persona intenta usar el flujo OAuth inicial con credenciales de cliente
- **THEN** el flujo no autentica al cliente y no emite un token de cliente
