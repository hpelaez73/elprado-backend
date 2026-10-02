## 1. Base de autenticación y contexto de MCP

- [x] 1.1 Agregar a `ElPrado.McpApi` las referencias de proyectos y paquetes necesarios para `LoginService`, DTOs y autenticación JWT Bearer, y verificar con `dotnet restore ElPrado.McpApi/ElPrado.McpApi.csproj`.
- [x] 1.2 Incorporar y validar la configuración JWT requerida por entorno (`Jwt:Key`, `Jwt:Issuer` y duración opcional), asegurando que no se agregan valores secretos versionados y verificando que el arranque falla con un mensaje claro si faltan los valores obligatorios.
- [x] 1.3 Implementar el adaptador de `IUserContextService` para `ElPrado.McpApi`, basado en los claims del usuario autenticado, y verificar que resuelve `CodUsuario` para un principal JWT de empleado y devuelve valores seguros cuando no hay principal.

## 2. Reutilización segura del login de empleado

- [x] 2.1 Extraer de `LoginService` una operación reutilizable de validación de alias y clave que conserve la auditoría de acceso y no cree refresh tokens, y verificar que una credencial válida e inválida producen los mismos resultados que el login de empleado actual.
- [x] 2.2 Adaptar el login de empleado de `ElPrado.WebApi` para reutilizar la validación extraída sin alterar su respuesta ni la emisión de su refresh token, y verificar con la compilación de `ElPrado.WebApi` y una prueba de regresión o llamada de integración existente.

## 3. Login y protección JWT en la fachada MCP

- [x] 3.1 Crear el contrato propio y `POST /api/login/usuario` en `ElPrado.McpApi`; verificar que alias y clave válidos devuelven la identidad mínima y un JWT, y que credenciales inválidas devuelven HTTP 401 sin distinguir el dato fallido.
- [x] 3.2 Implementar la emisión de JWT HMAC SHA-256 con emisor, audiencia, vencimiento y los claims compatibles (`NameIdentifier`, `Name`, `CodUsuario`, `CodCliente`, `CodPropuesta`, `EsTesting`), y verificar al decodificar un token de prueba que los claims y la expiración son correctos.
- [x] 3.3 Configurar `UseAuthentication`, `UseAuthorization` y la política Bearer para proteger las rutas de negocio de `/api` excepto login; verificar con pruebas de integración o HTTP que un token válido permite el acceso y que token ausente, manipulado o vencido recibe HTTP 401.
- [x] 3.4 Mantener `GET /health` fuera de la política de autorización y verificar que responde correctamente sin encabezado Authorization.

## 4. Integración y entrega

- [x] 4.1 Actualizar `ElPrado.McpApi/README.md` con el endpoint, encabezado Bearer y los nombres de configuración segura, y verificar que no contiene claves, credenciales ni JWT reales.
- [x] 4.2 En el repositorio hermano `elprado-mcp`, configurar el cliente para iniciar sesión con credenciales de entorno, reutilizar el JWT vigente y reenviarlo como Bearer; verificar una llamada extremo a extremo autenticada contra ElPrado.McpApi y el re-login controlado después de un HTTP 401.
- [x] 4.3 Ejecutar `dotnet build ElPradoWeb.sln --configuration Release` y las pruebas agregadas o existentes; verificar manualmente login correcto, login inválido, token vencido y health check anónimo antes del despliegue.
