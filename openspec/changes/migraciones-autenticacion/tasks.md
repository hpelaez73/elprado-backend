# Tareas

## 1. Base de configuración y criptografía de la API

- [ ] 1.1 Agregar a `ElPrado.Migraciones.Api` las referencias JWT Bearer necesarias y la configuración de rate limiting de .NET 7; verificar `dotnet restore ElPrado.Migraciones.Api/ElPrado.Migraciones.Api.csproj`.
- [ ] 1.2 Crear las opciones `MigrationAuthentication` para JWT, administrador, agentes y límites de intentos, con validación de clave Base64 de 256 bits, issuer, audience, duración, hashes e IDs únicos; verificar que `ValidateOnStart` rechace configuración incompleta en una prueba de API.
- [ ] 1.3 Implementar el codificador/verificador PBKDF2-HMAC-SHA512 versionado, con sal aleatoria y comparación de tiempo constante; verificar hashes válidos, secreto incorrecto y formatos malformados con pruebas unitarias.
- [ ] 1.4 Registrar las opciones desde fuentes externas sin agregar secretos a `ElPrado.Migraciones.Api/appsettings*.json`; documentar el esquema y comprobar que la fábrica de pruebas pueda inyectar una configuración efímera válida.

## 2. Emisión y validación de tokens administrativos

- [ ] 2.1 Implementar el servicio privado de emisión JWT con los claims, firma, issuer, audience y expiración de `MigrationAuthentication`; verificar mediante pruebas que un token válido contiene `migration_identity=admin` o `agent` y nunca secretos.
- [ ] 2.2 Configurar autenticación Bearer y la policy `AdminOnly` en `ElPrado.Migraciones.Api/Infrastructure/DependencyInjection.cs`; verificar rechazo HTTP 401 para token vencido, manipulado, issuer inválido y audience inválida.
- [ ] 2.3 Crear `POST /auth/admin/login` y `GET /auth/me` bajo `ElPrado.Migraciones.Api/Features/Authentication`; verificar login válido, credenciales inválidas, respuesta sin secretos y acceso 200/401/403 según corresponda.
- [ ] 2.4 Configurar el límite de solicitudes de los endpoints de login por origen y respuestas 429 seguras; verificar con una prueba que exceder la cuota no emite tokens.
- [ ] 2.5 Documentar en `docs/migraciones/autenticacion.md` el aprovisionamiento inicial de administrador, la generación de hash y la rotación sin valores reales; verificar que los comandos y ejemplos usan marcadores o secretos del host.

## 3. Autenticación y revocación de agentes en la API

- [ ] 3.1 Añadir a `ElPrado.Migraciones.Contracts` únicamente los DTOs de solicitud y respuesta de token de agente; verificar que API y Agent compilan con la referencia y que Contracts no contiene opciones, hashes ni claves.
- [ ] 3.2 Implementar el registro configurado de agentes y su verificación por `AgentId`, hash y estado habilitado; verificar con pruebas que agentes inexistentes, deshabilitados o con secreto incorrecto reciben 401 sin información adicional.
- [ ] 3.3 Crear `POST /auth/agent/token`, emitir el token de agente y configurar `AgentOnly` para requerir el claim de agente; verificar 200 para agente autorizado, 403 al usar token administrativo y que la respuesta no devuelve secretos.
- [ ] 3.4 Incorporar el requisito de autorización que consulta el registro de agentes vigente para `AgentOnly` y crear `GET /auth/agent/me`; verificar que un token vigente pasa antes de revocar y recibe 403 tras deshabilitar o retirar su `AgentId` de la configuración activa.
- [ ] 3.5 Documentar el aprovisionamiento de `AgentId`/`AgentSecret`, hash, rotación, revocación y comportamiento de reload/reinicio; verificar que la documentación no incluye secretos reales ni usa el `AgentId` como prueba de autenticación.

## 4. Protección HTTP y observabilidad segura

- [ ] 4.1 Actualizar `ElPrado.Migraciones.Api/Program.cs` para usar autenticación antes de autorización, HTTPS redirection y las policies de endpoints, conservando `GET /health` anónimo y Swagger sólo en Development; verificar los estados 200, 401 y 403 con `MigrationApiFactory`.
- [ ] 4.2 Configurar la aceptación de HTTPS detrás de proxy sólo con forwarded headers de proxies confiables y el perfil HTTP de pruebas locales controladas; verificar que una solicitud HTTP no procesa credenciales como autenticadas fuera de dicho perfil.
- [ ] 4.3 Revisar el logging de API y los manejadores de error para excluir cuerpos de autenticación, encabezado `Authorization`, contraseñas, secretos y tokens; verificar con un sink de prueba que no aparecen en logs ni `ProblemDetails`.
- [ ] 4.4 Extender `tests/ElPrado.Migraciones.Api.Tests` para cubrir autenticación ausente, separación AdminOnly/AgentOnly, expiración/firma/issuer/audience, configuración vacía, rate limiting y revocación; verificar `dotnet test tests/ElPrado.Migraciones.Api.Tests/ElPrado.Migraciones.Api.Tests.csproj`.

## 5. Cliente de token del Agent

- [ ] 5.1 Agregar opciones externas del Agent para URL HTTPS de API, `AgentId` y `AgentSecret`, con validación fail-closed y sin valores secretos en `ElPrado.Migraciones.Agent/appsettings*.json`; verificar el rechazo de configuración ausente con pruebas unitarias.
- [ ] 5.2 Implementar y registrar un cliente HTTP tipado que solicite `POST /auth/agent/token`, conserve el token sólo en memoria hasta su expiración y reautentique sin refresh token; verificar solicitudes y renovación contra un `HttpMessageHandler` simulado.
- [ ] 5.3 Integrar el cliente de token en el ciclo de vida de `AgentWorker` sin iniciar SignalR ni migraciones; verificar que el Worker maneja 401/429/error de red y registra mensajes sin `AgentSecret` ni access token.
- [ ] 5.4 Ampliar `tests/ElPrado.Migraciones.Agent.Tests` con configuración válida/inválida, token simulado, renovación y logs seguros; verificar `dotnet test tests/ElPrado.Migraciones.Agent.Tests/ElPrado.Migraciones.Agent.Tests.csproj`.
- [ ] 5.5 Completar `docs/migraciones/autenticacion.md` con la configuración del Agent en Development y Production, renovación y rotación; verificar que las instrucciones no requieren Firebird, Seq ni servicios externos.

## 6. Validación integral

- [ ] 6.1 Ejecutar restore y build de Contracts, API, Agent y ambos proyectos de pruebas; verificar `dotnet build ElPradoWeb.sln` o registrar por separado fallos ajenos a Migraciones.
- [ ] 6.2 Ejecutar la batería de pruebas de Migraciones con configuración aislada y revisar cobertura de los escenarios de `specs/migraciones-autenticacion/spec.md`; verificar que no se conecta a Firebird, Seq ni SignalR.
- [ ] 6.3 Ejecutar la API con secretos de Development efímeros y comprobar `/health`, Swagger en Development, login administrativo, token de agente y revocación; verificar manualmente que Production no expone Swagger y requiere HTTPS.

## Seguimiento del flujo

- Revisar y confirmar las decisiones listadas en `design.md` antes de aplicar el cambio.
- Archivar el cambio tras implementar y aprobar las validaciones del proyecto.
