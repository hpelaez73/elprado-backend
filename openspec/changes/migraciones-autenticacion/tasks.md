# Tareas

## 1. Configuración, criptografía y aprovisionamiento

- [x] 1.1 Agregar JWT Bearer y Rate Limiting compatibles con .NET 7 a la API; verificar `dotnet restore ElPrado.Migraciones.Api/ElPrado.Migraciones.Api.csproj`.
- [x] 1.2 Crear opciones para JWT, admin, agentes, PBKDF2, rate limiting y proxies confiables; validar HMAC Base64 ≥256 bits, vidas iniciales admin 30 min/agent 15 min y rangos; verificar `ValidateOnStart` contra configuración inválida.
- [x] 1.3 Implementar PBKDF2-HMAC-SHA512 versionado con sal 16 bytes, hash 32 bytes, 210.000 iteraciones iniciales y rango 210.000–1.000.000; verificar hash válido, secreto incorrecto, comparación constante y formato malformado.
- [x] 1.4 Crear la utilidad local que lea credencial por entrada estándar sin eco y produzca sólo el hash; verificar que ejemplos no pasen secretos por argumentos, logs ni archivos versionados.
- [x] 1.5 Registrar fuentes externas sin secretos en `appsettings*.json`; documentar reload/reinicio por proveedor y verificar configuración efímera en pruebas.

## 2. Emisión, validación y transporte seguro de la API

- [x] 2.1 Implementar emisor JWT con firma, issuer, audience, expiración y `migration_identity=admin|agent`; verificar vidas iniciales y ausencia de secretos.
- [x] 2.2 Configurar Bearer, `AdminOnly` y `AgentOnly`; verificar 401 para token vencido/manipulado/issuer-audience inválidos y 403 entre identidades.
- [x] 2.3 Crear login admin y consulta `me`; verificar login válido/inválido y respuestas sin secretos.
- [x] 2.4 Configurar rate limiting de endpoints token; verificar HTTP 429 sin validación ni emisión.
- [x] 2.5 Rechazar HTTP antes de enlazar o validar credenciales y aceptar forwarded headers sólo de `KnownProxies`/`KnownNetworks`; verificar que un cliente no autorizado no habilita HTTPS.
- [x] 2.6 Excluir cuerpos, Authorization, contraseñas, secretos, hashes y tokens de logs/errores; verificar con sink de prueba.

## 3. Agentes, revocación y rotación en la API

- [x] 3.1 Añadir a Contracts sólo DTOs de token agent; verificar que no contiene opciones, hashes ni claves.
- [x] 3.2 Implementar registro por `AgentId`, hash y estado; verificar 401 genérico para agente inexistente, deshabilitado o secreto incorrecto.
- [x] 3.3 Crear endpoint token agent, `GET /auth/agent/me` y `AgentOnly`; verificar independencia de dos agentes, respuesta sin secretos y rechazo de token admin.
- [x] 3.4 Consultar el registro vigente con `IOptionsMonitor` en `AgentOnly`; verificar que revocar un agente da 403 para su token vigente, no afecta a otro y que una fuente sin reload exige reinicio.
- [x] 3.5 Implementar/documentar rotación de `SecretHash`; verificar que el secreto anterior no emite tokens y JWT previos siguen válidos hasta exp si el agente está habilitado.
- [x] 3.6 Documentar aprovisionamiento, revocación, rotación y rotación de clave JWT de emergencia, distinguiendo efectos sobre tokens emitidos.

## 4. Cliente resiliente de token del Agent

- [x] 4.1 Agregar opciones externas para URL HTTPS, `AgentId`, `AgentSecret`, margen y reintentos; validar margen 30 s–5 min menor al JWT agent y límites de backoff/jitter.
- [x] 4.2 Implementar cliente HTTP tipado con token sólo en memoria y renovación dos minutos antes de exp; verificar renovación anticipada con `HttpMessageHandler` simulado.
- [x] 4.3 Coordinar renovación concurrente en una solicitud en curso; verificar que múltiples consumidores causan una sola llamada al endpoint.
- [x] 4.4 Implementar reintentos de red, timeout y 429: base 1 s, multiplicador 2, tope 60 s, jitter ±20 % y `Retry-After` acotado; verificar demora y recuperación.
- [x] 4.5 Tratar 401 como credencial inválida sin reintento continuo y mantener Worker vivo ante fallos transitorios; verificar logs seguros, suspensión tras 401 y recuperación de API.
- [x] 4.6 Ampliar pruebas Agent para configuración válida/inválida, renovación, concurrencia, 429, red, 401 y logs; verificar `dotnet test tests/ElPrado.Migraciones.Agent.Tests/ElPrado.Migraciones.Agent.Tests.csproj`.

## 5. Validación integral

- [x] 5.1 Ampliar pruebas API para identidad ausente, policies, expiración/firma/issuer/audience, HTTPS/proxy, configuración inválida, rate limit, independencia, rotación y revocación; verificar `dotnet test tests/ElPrado.Migraciones.Api.Tests/ElPrado.Migraciones.Api.Tests.csproj`.
- [x] 5.2 Ejecutar restore/build de Contracts, API, Agent y pruebas; verificar `dotnet build ElPradoWeb.sln` o registrar fallos ajenos.
- [x] 5.3 Ejecutar pruebas de Migraciones con configuración aislada y revisar escenarios del spec; verificar ausencia de Firebird, Seq y SignalR.
- [x] 5.4 Ejecutar API con secretos Development efímeros y dos agentes de prueba; comprobar login, renovación, rotación, revocación, `/health`, Swagger Development y HTTPS Production.

## Seguimiento del flujo

- Aplicar con `$openspec-apply-change migraciones-autenticacion` sólo tras aprobar esta planificación.
- Archivar tras implementar y aprobar validaciones del proyecto.
