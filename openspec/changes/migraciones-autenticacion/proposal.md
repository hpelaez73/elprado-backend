# Propuesta

## Motivo

La API y el Agent de Migraciones ya poseen infraestructura operativa, pero no una identidad verificable para administradores ni para cada instalación de agente. Antes de exponer operaciones administrativas o de conexión futura, se necesita una base de autenticación simple, configurable externamente y segura, sin introducir Firebird ni un proveedor de identidad.

## Cambios

- Incorporar autenticación JWT Bearer de corta duración para dos clases de identidad excluyentes: administrador y agente.
- Exponer los inicios de sesión `POST /auth/admin/login` y `POST /auth/agent/token`, y las consultas protegidas `GET /auth/me` y `GET /auth/agent/me`.
- Definir las policies `AdminOnly` y `AgentOnly`, que acepten únicamente el tipo de token correspondiente.
- Incorporar configuración externa validada al arranque para el administrador, el registro de agentes autorizados, la clave y parámetros JWT, y los límites de intentos de autenticación.
- Almacenar únicamente hashes de contraseñas y secretos de agente; permitir deshabilitar o quitar un agente de la configuración para revocar su capacidad de obtener nuevos tokens.
- Exigir HTTPS fuera de pruebas locales controladas, mantener `/health` anónimo y Swagger sólo en Development, y evitar secretos y tokens en respuestas de error o logs.
- Añadir los contratos compartidos mínimos para que el Agent solicite y consuma su token, junto con documentación de aprovisionamiento, rotación y configuración segura.
- Ampliar las pruebas unitarias e integrales de API y Agent sin Firebird, Seq ni servicios externos.

## Capacidades

### Nuevas capacidades

- `migraciones-autenticacion`: Autenticación, emisión y validación de tokens, autorización por tipo de identidad, protección HTTP y gestión configurada de credenciales para los proyectos de Migraciones.

### Capacidades modificadas

- Ninguna.

## Impacto

Se modificarán sólo `ElPrado.Migraciones.Api`, `ElPrado.Migraciones.Agent`, `ElPrado.Migraciones.Contracts`, sus proyectos xUnit y la documentación de Migraciones. La API incorporará paquetes JWT/Rate Limiting nativos compatibles con .NET 7 y endpoints de autenticación; el Agent incorporará un cliente HTTP tipado y configuración de identidad. No se modificarán Firebird, SignalR, otros proyectos del repositorio ni se versionarán secretos reales.
