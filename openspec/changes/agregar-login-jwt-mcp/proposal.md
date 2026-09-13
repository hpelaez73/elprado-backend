## Why

`ElPrado.McpApi` no tiene actualmente una forma de autenticar al consumidor
`elprado-mcp`; por lo tanto no puede asociar las solicitudes a una identidad
del sistema. Se necesita un inicio de sesión directo compatible con las
credenciales de empleados existentes que emita un JWT verificable en cada
llamada posterior.

## What Changes

- Incorporar un endpoint de login de empleados en `ElPrado.McpApi`, basado en
  alias y clave y reutilizando la validación de credenciales de
  `ElPrado.WebApi`.
- Emitir JWT firmados con la configuración JWT existente, con la identidad y
  los claims de compatibilidad que necesita el dominio.
- Configurar autenticación y autorización Bearer en `ElPrado.McpApi`, dejando
  el health check anónimo y exigiendo un token válido en las rutas de negocio
  de la fachada.
- Definir el contrato de integración para que `elprado-mcp` obtenga el token y
  lo reenvíe mediante `Authorization: Bearer` al invocar la API.
- Documentar la configuración requerida sin versionar claves JWT ni
  credenciales.

## Capabilities

### New Capabilities

- `mcp-jwt-authentication`: Login de empleados y autenticación JWT de las
  rutas de negocio de `ElPrado.McpApi` para su consumidor `elprado-mcp`.

### Modified Capabilities

Ninguna.

## Impact

- `ElPrado.McpApi`: dependencias de JWT, configuración, endpoint de login,
  pipeline de autenticación/autorización y documentación.
- Proyectos compartidos de dominio: la validación de usuario y clave se
  reutilizará o extraerá desde `LoginService` sin cambiar el contrato del
  frontend.
- `elprado-mcp`: configuración de credenciales de empleado, obtención y
  renovación operativa del JWT, y encabezado Bearer en sus llamadas HTTP.
- No se agrega OAuth, PKCE, refresh tokens nuevos, login de clientes ni
  protocolo MCP a `ElPrado.McpApi`.
