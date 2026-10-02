## Context

Véanse `proposal.md` y `specs/mcp-jwt-authentication/spec.md`. `ElPrado.McpApi`
es una Minimal API .NET 7 que hoy solo registra Firebird y expone health check;
sus rutas de negocio viven bajo `/api`. `ElPrado.WebApi` ya valida credenciales
de empleados con `LoginService`, firma JWT simétricos y consume los claims de
identidad mediante su adaptador de contexto HTTP.

## Goals / Non-Goals

**Goals:**

- Establecer una autenticación de servicio a servicio simple basada en la
  identidad de un empleado existente.
- Mantener tokens de `ElPrado.McpApi` compatibles con los claims de dominio y
  con la configuración JWT ya desplegada.
- Hacer que cada adaptador de negocio pueda obtener al usuario autenticado sin
  depender de `elprado-mcp`.

**Non-Goals:**

- No crear un servidor OAuth, client registration, PKCE, scopes ni JWKS.
- No aceptar login de clientes ni emitir o gestionar refresh tokens para MCP.
- No trasladar protocolo MCP, tools o transporte Streamable HTTP a esta API.
- No convertir el JWT del frontend en un contrato nuevo ni modificar sus rutas
  públicas.

## Decisions

- Exponer el login como Minimal API `POST /api/login/usuario`, manteniendo los
  campos `alias` y `clave` del contrato de empleado existente, pero con una
  respuesta propia de `ElPrado.McpApi`.
  - El endpoint será anónimo y el resto de las rutas del grupo de negocio
    requerirán autorización por defecto; `/health` queda fuera del grupo.
  - Se descarta copiar el `LoginController` o montar controllers en la nueva
    fachada, ya que su arquitectura usa adaptadores Minimal API.

- Extraer o incorporar a la capa de dominio una operación de validación de
  empleado que preserve la comprobación de alias/clave y su auditoría, pero no
  cree el refresh token del frontend. `ElPrado.WebApi` conservará su login
  actual delegando en esa validación antes de emitir su respuesta actual.
  - Se descarta invocar directamente `LoginService.Login(alias, clave)` desde
    MCP y desechar su resultado, porque genera refresh tokens de frontend que
    MCP no usa.

- Configurar `JwtBearer` en `ElPrado.McpApi` con la misma clave, emisor,
  audiencia, algoritmo HMAC SHA-256 y reglas de vencimiento que el token
  emitido. El emisor de token compondrá los claims actuales de WebApi para
  evitar divergencia de identidad (`CodUsuario`, `CodCliente`, `CodPropuesta`,
  nombre y `EsTesting`).
  - Se descarta usar un token fijo o una API key: no representa al empleado y
    no provee vencimiento ni validación estándar.

- Registrar un adaptador `IUserContextService` propio de `ElPrado.McpApi` que
  traduzca el `ClaimsPrincipal` de la solicitud a la abstracción de dominio.
  Cada host conserva su adaptador HTTP y la capa de servicios continúa sin
  acoplarse a ASP.NET Core.

- Cargar `Jwt:Key`, `Jwt:Issuer` y, si se define, la duración de token desde
  configuración por entorno o variables de entorno. La aplicación fallará al
  iniciar cuando falte una configuración JWT esencial en vez de emitir tokens
  inseguros.
  - Se descarta agregar secretos al repositorio o registrar encabezados
    Authorization; la documentación mostrará únicamente nombres de variables.

## Risks / Trade-offs

- [Una credencial de empleado queda expuesta en la configuración de
  `elprado-mcp`] → Usar un almacén de secretos del entorno, redacción de logs y
  una cuenta de empleado dedicada con los mínimos permisos disponibles.
- [Los dos hosts divergen en claims o parámetros JWT] → Centralizar la
  validación de configuración y cubrir el token con pruebas de integración y
  de claims; mantener los nombres de claims de WebApi.
- [Cambiar `LoginService` altera el login frontend] → Extraer primero una
  operación mínima y cubrir los dos caminos con pruebas de regresión antes de
  eliminar lógica duplicada.
- [El token expira durante una sesión de MCP] → `elprado-mcp` vuelve a iniciar
  sesión de forma controlada al recibir 401; no se introducen refresh tokens
  en esta entrega.

## Migration Plan

1. Configurar en cada entorno una clave JWT compatible, emisor y credenciales
   de una cuenta de empleado destinada al consumidor MCP.
2. Publicar ElPrado.McpApi con el endpoint de login y protección Bearer; probar
   login, token válido, token vencido y health check anónimo.
3. Cambiar `elprado-mcp` para solicitar el token y enviarlo en cada llamada a
   la fachada; eliminar cualquier credencial estática anterior de acceso a la
   API.
4. Observar rechazos de autenticación sin registrar secretos ni tokens.
5. Rollback: retirar la configuración de `elprado-mcp` y el despliegue de la
   nueva versión de MCP API; no hay migraciones de datos ni cambios de contrato
   requeridos en `ElPrado.WebApi`.
