## Why

El servidor MCP usa actualmente un JWT fijo configurado en su entorno para invocar El Prado, lo que impide identificar y limitar el acceso por persona cuando se conecta desde ChatGPT. Se necesita un inicio de sesión OAuth para empleados que reutilice la autenticación existente y deje una identidad extensible a clientes.

## What Changes

- Incorporar en `ElPrado.McpApi` el servidor de autorización OAuth 2.1 para el flujo Authorization Code con PKCE (`S256`), comenzando por empleados autenticados con alias y clave mediante `LoginService`.
- Publicar discovery OAuth/OIDC, autorización, token y JWKS; emitir access tokens con identidad, audiencia, vencimiento y scopes verificables.
- Agregar el almacenamiento y ciclo de vida propios de códigos de autorización y refresh tokens OAuth; no reutilizar los refresh tokens actuales como contrato OAuth.
- Proteger las rutas de negocio de `ElPrado.McpApi` y proveer un contexto de usuario propio para esa fachada, sin trasladar el adaptador HTTP actual a `ElPrado.Services`.
- Adaptar `elprado-mcp` para anunciar metadata de recurso protegido, desafiar solicitudes no autenticadas y validar/propagar el Bearer token individual hacia `ElPrado.McpApi`; retirar la dependencia de `ELPRADO_API_TOKEN`.
- Definir la base de identidad para una futura autenticación de clientes por DNI, propuesta y clave, fuera del alcance de esta entrega.

## Capabilities

### New Capabilities

- `mcp-oauth-authentication`: Autenticación OAuth 2.1 de empleados para el servidor MCP y las rutas de `ElPrado.McpApi`.

### Modified Capabilities

Ninguna.

## Impact

- `ElPrado.McpApi`: host, configuración, dependencias de autenticación, persistencia OAuth, endpoints de autorización y protección de rutas.
- `ElPrado.Services/LoginService.cs` y contratos compartidos: reutilización o extracción de validación de credenciales de empleados, sin cambiar el login del frontend.
- `elprado-mcp`: transporte Streamable HTTP, cliente REST, configuración y pruebas de autenticación por solicitud.
- Firebird: nuevas estructuras para autorizaciones OAuth y su revocación/expiración.
- Despliegue: HTTPS público, claves de firma y configuración de OAuth administradas fuera de archivos versionados.
