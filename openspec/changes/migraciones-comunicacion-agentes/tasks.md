# Tareas

## 1. Contratos y configuración compartida

- [ ] 1.1 Agregar en `ElPrado.Migraciones.Contracts` los DTOs mínimos e inmutables de Ping, Pong y resultado administrativo, sin ConnectionId, token ni secreto; verificar serialización JSON y pruebas de contrato.
- [ ] 1.2 Agregar opciones validadas para Hub, Ping timeout y reconexión del Agent, reutilizando los límites de backoff/jitter de `AgentAuthenticationOptions`; verificar `ValidateOnStart` para valores fuera de rango y URL no HTTPS fuera de pruebas controladas.
- [ ] 1.3 Agregar las referencias oficiales SignalR compatibles con .NET 7 en API, Agent y proyectos de pruebas que las requieran; verificar `dotnet restore` de los cuatro proyectos en alcance.

## 2. Registro local y ciclo de vida en la API

- [ ] 2.1 Implementar `IAgentConnectionRegistry` singleton thread-safe en `ElPrado.Migraciones.Api` con alta, baja condicional, consulta y DTO público de presencia; verificar prueba unitaria de registro, listado vacío y hora UTC.
- [ ] 2.2 Implementar sustitución atómica por `AgentId`, invalidación interna y compare-and-remove de desconexiones; verificar pruebas deterministas de conexión duplicada y desconexión tardía sin borrar la nueva.
- [ ] 2.3 Implementar coordinación de cierre cooperativo para una conexión reemplazada o invalidada, verificando la conexión vigente antes de toda operación; verificar que una conexión desplazada no recibe ni confirma Ping.
- [ ] 2.4 Suscribir la presencia a cambios de `IOptionsMonitor<MigrationAuthenticationOptions>` para invalidar y cerrar agentes revocados; verificar prueba con monitor mutable y conservar el comportamiento documentado para fuentes sin reload.

## 3. Hub, autenticación y administración de la API

- [ ] 3.1 Configurar `AddSignalR`, autenticación Bearer de SignalR y `OnMessageReceived` para aceptar `access_token` sólo en `/hubs/agents`; verificar rechazo de token inválido, vencido, admin y query-string token fuera del Hub.
- [ ] 3.2 Ajustar protección HTTPS/WSS y Serilog request logging para `/hubs/agents`, redactando `access_token` y evitando query strings sensibles en errores; verificar con sink de prueba que tokens, Authorization, secretos y hashes no aparecen en logs.
- [ ] 3.3 Implementar `AgentsHub` delgado en `/hubs/agents`, protegido con `AgentOnly`, que derive el `AgentId` de `Context.User` y delegue conexión, desconexión, cancelación y excepciones al registry/servicios; verificar que un `AgentId` enviado por cliente no altera la identidad registrada.
- [ ] 3.4 Implementar `GET /agents` protegido con `AdminOnly` y una respuesta independiente de SignalR; verificar HTTP 200 para admin, HTTP 403 para Agent y ausencia de ConnectionId/tokens/secretos.
- [ ] 3.5 Implementar el servicio de Ping/Pong correlacionado y, tras confirmar la decisión de diseño, `POST /agents/{agentId}/ping`; verificar Pong exitoso, timeout, agente ausente, limpieza de correlaciones y que el timeout no desconecta automáticamente.
- [ ] 3.6 Ampliar pruebas API para autorización del Hub, conexión/desconexión, reemplazo, revocación durante conexión, listado administrativo, Ping/Pong, timeout y logs seguros, ejecutando `dotnet test tests/ElPrado.Migraciones.Api.Tests/ElPrado.Migraciones.Api.Tests.csproj`.

## 4. Cliente SignalR y Worker del Agent

- [ ] 4.1 Implementar un servicio de conexión SignalR propietario de una única `HubConnection`, configurado con `AccessTokenProvider` que reutilice `IAgentTokenProvider`; verificar que no persiste token ni duplica la autenticación HTTP.
- [ ] 4.2 Integrar el servicio en `AgentWorker` para inicio, cancelación, `StopAsync` y `DisposeAsync` controlados; verificar pruebas de arranque, detención y ausencia de conexiones pendientes.
- [ ] 4.3 Implementar `WithAutomaticReconnect` y un bucle externo serializado para fallo inicial y reconexiones agotadas, con backoff acotado, jitter y estados mínimos; verificar fallos transitorios, reinicio del servidor, agotamiento, ausencia de intentos simultáneos y respeto de `CancellationToken`.
- [ ] 4.4 Distinguir credenciales inválidas/revocadas de fallos transitorios, detener reintentos agresivos de autenticación y reanudar sólo tras cambio de opciones o reinicio; verificar 401/403, renovación de token y logs sin secretos.
- [ ] 4.5 Registrar handlers de Ping y cierre cooperativo en el Agent para responder Pong con la correlación original y detener conexiones reemplazadas/revocadas; verificar respuesta a Ping y que no se incorpora un sistema genérico de comandos ni sondeo periódico.
- [ ] 4.6 Ampliar pruebas Agent para conexión autenticada, uso/renovación de token, reconexión, fallo inicial, detención, Ping/Pong, credenciales inválidas y redacción de logs; ejecutar `dotnet test tests/ElPrado.Migraciones.Agent.Tests/ElPrado.Migraciones.Agent.Tests.csproj`.

## 5. Integración y verificación final

- [ ] 5.1 Crear una prueba de integración SignalR real con API Kestrel local y cliente .NET 7, configuración efímera y sin Firebird/Seq/red externa; verificar handshake `AgentOnly`, `GET /agents`, Ping/Pong y desconexión.
- [ ] 5.2 Ejecutar `openspec validate migraciones-comunicacion-agentes --strict`, `dotnet build ElPradoWeb.sln` y los proyectos xUnit en alcance; documentar únicamente fallos ajenos al change si los hubiera.
- [ ] 5.3 Verificar manualmente en un entorno local HTTPS controlado un Agent habilitado, reemplazo de conexión, Ping administrativo y revocación; confirmar que el registro es local al proceso y que no se registran credenciales ni access tokens.

## Seguimiento del flujo

- Confirmar las tres decisiones indicadas en `design.md` antes de iniciar Apply.
- Archivar el change sólo después de completar la implementación, revisión y verificaciones del proyecto.
