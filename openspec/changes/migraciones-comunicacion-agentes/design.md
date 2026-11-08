# Diseño

## Contexto

La API Minimal .NET 7 ya valida JWT Bearer, separa `AdminOnly` de `AgentOnly` y consulta el registro de agentes vigente mediante `IOptionsMonitor` en cada autorización. El Agent ya tiene `IAgentTokenProvider`: conserva el JWT sólo en memoria, coordina la renovación y distingue HTTP 401 de fallos transitorios. Véanse `proposal.md` y `specs/migraciones-comunicacion-agentes/spec.md` para la motivación y el contrato.

El registro de agentes de la Etapa 2 es configuración externa: una fuente con reload aplica revocaciones dinámicamente; una fuente sin reload necesita reinicio o redespliegue. La etapa queda deliberadamente limitada a un proceso de API; no hay backplane ni persistencia.

## Objetivos / no objetivos

**Objetivos:**

- Crear un canal SignalR WSS/HTTPS de Agent a API que derive siempre la identidad del JWT autorizado.
- Mantener presencia local correcta bajo concurrencia, desconexiones tardías y conexiones duplicadas.
- Reutilizar la obtención de token existente para que la conexión sobreviva fallos transitorios y vencimientos sin persistir secretos.
- Ofrecer una comprobación Ping/Pong puntual, observable y determinista para pruebas y operación inicial.
- Hacer efectiva la revocación tanto al conectar como durante una conexión existente.

**No objetivos:**

- Sin comandos genéricos, migraciones, transferencia de archivos, monitoreo periódico, persistencia, Firebird, Redis, colas ni múltiples instancias.
- Sin inventario de instalaciones ni historial de conexiones; el registro sólo expresa la presencia de este proceso.
- Sin frontend administrativo ni cambios a otros proyectos.

## Decisiones

### Hub delgado, autenticado y con identidad derivada

La API agregará servicios SignalR y mapeará `AgentsHub` en `/hubs/agents` con `RequireAuthorization("AgentOnly")`. `OnConnectedAsync` obtiene `AgentId` exclusivamente de `Context.User` (`sub`/NameIdentifier), registra la conexión mediante un servicio y sólo continúa si sigue siendo la conexión vigente. `OnDisconnectedAsync` delega la baja condicional en el mismo servicio; cancelación normal se trata como desconexión esperada y errores se registran sin credenciales.

Los métodos expuestos por el Hub se limitarán al flujo Pong; los endpoints administrativos y la coordinación de Ping usarán servicios, no lógica de Hub. Esta separación permite probar concurrencia y presencia sin un servidor SignalR real.

Alternativa considerada: que el cliente envíe `AgentId` al conectar o usar grupos como fuente de identidad. Se descarta porque permite discrepancias con el JWT y deja la presencia acoplada al transporte.

### JWT para negociación y WebSocket, con query string acotada

Se conserva el esquema Bearer de la Etapa 2. La configuración `JwtBearerOptions.Events.OnMessageReceived` sólo toma `access_token` si la ruta comienza exactamente por `/hubs/agents`; conserva el encabezado `Authorization` para negociación y otros transportes. El cliente empleará `HubConnectionBuilder.WithUrl(..., options => options.AccessTokenProvider = tokenProvider.GetAccessTokenAsync)`; no almacena el token ni lo agrega manualmente a URL.

En navegadores y algunos transportes WebSocket SignalR puede colocar el token en la query string porque no puede fijar el encabezado de actualización. Por ello el middleware de request logging debe excluir o sanitizar `access_token` antes de registrar la solicitud, y no se registrarán excepciones HTTP con query completa. La validación HTTPS existente deberá cubrir también `/hubs/agents` antes de negociación o actualización; Development/Testing controlado conserva su escape explícito.

Alternativa considerada: aceptar `access_token` en cualquier endpoint o crear un esquema JWT exclusivo. Se descarta por exposición innecesaria y duplicación de la seguridad ya validada.

### AgentConnectionRegistry local y compare-and-remove

`IAgentConnectionRegistry` será singleton, thread-safe y encapsulará un `ConcurrentDictionary<string, ActiveAgentConnection>`. Cada entrada conserva `AgentId`, `ConnectedAtUtc`, estado operativo y `ConnectionId` privado. El DTO de listado no contiene ConnectionId.

El alta usa una sustitución atómica por `AgentId`, devuelve el registro desplazado y marca o remueve su capacidad de operar antes de solicitar `Clients.Client(oldConnectionId).SendAsync("Disconnect", ...)`/`Context.Abort` por un mecanismo coordinado. Las operaciones que requieren una conexión vuelven a comprobar que su `ConnectionId` coincide con la entrada activa. La baja usa `TryRemove(KeyValuePair<...>)` o una comparación equivalente: una desconexión vieja no puede borrar la nueva.

SignalR no ofrece desde `IHubContext` un `Abort` remoto directo para otro cliente. Por ello se usará una señal de desconexión de protocolo mínima que el Agent atiende cerrando su `HubConnection`; la invalidación inmediata en el registro y en los servicios de Ping es la barrera de seguridad, incluso si el socket viejo tarda en cerrarse.

Alternativa considerada: permitir múltiples ConnectionId por AgentId o guardar la presencia en base/Redis. Se descarta por la política de una sola sesión y el alcance local de esta etapa.

### Revocación activa y defensa por operación

`AgentOnly` ya impide nuevos handshakes para agentes deshabilitados. Para sesiones existentes, un servicio suscrito a `IOptionsMonitor.OnChange` calcula los AgentId que dejan de estar habilitados, los elimina/invalida del registry y envía su señal de cierre. En paralelo, cada Ping y cada callback Pong comprueba que la conexión sigue siendo la entrada activa y que el agente sigue habilitado; si no, no procesa la operación. Una fuente sin reload conserva la limitación ya documentada: el cambio sólo surte efecto al recargar configuración o reiniciar.

Alternativa considerada: esperar al vencimiento del JWT o renovar sólo al reconectar. Se descarta porque no satisface la revocación de conexiones activas de esta etapa.

### Cliente SignalR y estados mínimos de recuperación

Se añadirá el paquete oficial `Microsoft.AspNetCore.SignalR.Client` 7.x al Agent. Un servicio dedicado será dueño de una única `HubConnection`; `AgentWorker` inicia y detiene ese servicio, sin poner reintentos dentro de un handler ni crear conexiones concurrentes. Sus estados internos mínimos serán `Disconnected`, `Connecting`, `Connected`, `WaitingToRetry` y `CredentialsRejected`, expuestos sólo a logs/diagnóstico local.

`WithAutomaticReconnect` cubrirá pérdidas después de una conexión establecida con una secuencia corta y acotada. Los eventos `Closed` y el fallo de `StartAsync` activan un único bucle externo, protegido por un lock/semaforo, que aplica la misma configuración de backoff exponencial y jitter ya validada para el token. El bucle respeta el `CancellationToken` del Worker; al detenerse llama `StopAsync`/`DisposeAsync` con cancelación controlada.

`AccessTokenProvider` consulta `IAgentTokenProvider` por cada negociación o reconexión. Su renovación anticipada cubre tokens expirados; un 401/403 o `InvalidOperationException` de credenciales pasa a `CredentialsRejected`, registra un mensaje seguro y espera cambio de opciones o reinicio controlado en vez de reintentar agresivamente. Red, timeout, 5xx y desconexiones son transitorios. Al agotarse los reintentos internos de SignalR, el bucle externo retoma con backoff.

Alternativa considerada: dejar toda la recuperación en `WithAutomaticReconnect` o modificar `AgentTokenProvider` para que posea SignalR. La primera no cubre el fallo inicial ni el agotamiento; la segunda mezcla autenticación HTTP con ciclo de vida de conexión.

### Ping/Pong puntual y contratos mínimos

Contracts añadirá DTOs públicos mínimos, por ejemplo `AgentPingRequest(CorrelationId, RequestedAtUtc)` y `AgentPong(CorrelationId, RespondedAtUtc)`, y un resultado administrativo sin ConnectionId. El servicio de Ping genera un `Guid` de correlación y guarda un `TaskCompletionSource` por correlación con timeout configurado y acotado. Envía `Ping` a la ConnectionId interna vigente a través de `IHubContext`; el Agent registra un handler `Ping` que retorna/envía `Pong` con el mismo identificador. El Hub delega `Pong` al servicio, que acepta sólo correlaciones pendientes de la conexión activa correspondiente.

Se propone `POST /agents/{agentId}/ping`, protegido por `AdminOnly`, para una prueba operacional directa. La respuesta propuesta es `200 OK` con un resultado explícito `{ correlationId, requestedAtUtc, respondedAtUtc, status }` para Pong o timeout; `404` cuando no hay agente conectado. Esto no usa el ping/keep-alive nativo de SignalR, que sólo detecta salud del transporte y no confirma que el Agent procese código.

Alternativa considerada: `IHubContext.Clients.Client(...).InvokeAsync`. La API no puede invocar métodos cliente con respuesta directa mediante ese mecanismo; el correlacionador Pong mantiene el protocolo simple sin crear un bus de comandos.

### Pruebas deterministas y servidor local

Los tests del registry usarán reloj/control de tiempo inyectable o valores UTC explícitos, sin esperas reales. API tests usarán `WebApplicationFactory` y configuración en memoria para probar autorización, registro, reemplazo, revocación y el endpoint. Los tests Agent inyectarán fábrica/conector y demora aleatoria para controlar fallos iniciales, cierre, token provider, estados y logs.

Una prueba de integración arrancará la API de pruebas en Kestrel local con configuración efímera HTTPS/controlada, conectará un cliente SignalR real usando un token de prueba y verificará Ping/Pong. No requerirá Firebird, Seq, ni servicios de red externos.

## Riesgos / compensaciones

- [Los tokens de WebSocket pueden aparecer en URLs de infraestructura] → limitar `access_token` al Hub, usar WSS y sanitizar logs; documentar que proxies deben redactor query strings.
- [El registro se pierde en reinicio o diverge entre instancias] → declarar presencia local y mantener explícitamente fuera de alcance la escala horizontal.
- [No existe aborto remoto directo desde `IHubContext`] → invalidar primero y cerrar cooperativamente mediante señal; ninguna operación acepta una conexión desplazada.
- [Revocación depende de reload de configuración] → conservar la advertencia y proceso de reinicio controlado de la Etapa 2.
- [Reconexión puede cargar la API] → un solo bucle, backoff exponencial limitado, jitter y cancelación.
- [Ping HTTP espera un cliente lento] → timeout corto configurable, limpieza garantizada de correlaciones y sin sondeo periódico.

## Plan de migración

1. Agregar paquetes y contratos SignalR/Ping compatibles con .NET 7, opciones validadas y registro local sin datos persistentes.
2. Configurar autenticación del Hub, sanitización de logs, mapeo HTTPS/WSS, Hub delgado, registry, revocación activa y endpoints administrativos.
3. Añadir cliente SignalR y servicio de ciclo de vida al Agent reutilizando `IAgentTokenProvider`; conectar Ping/Pong y cierre cooperativo.
4. Ejecutar pruebas unitarias, API y una integración SignalR local; verificar logs sin secretos y escenarios de reemplazo, timeout, token inválido y revocación.
5. Desplegar primero una sola API y un Agent de prueba con WSS; verificar `/agents`, Ping y una revocación. Rollback: desplegar la versión anterior; no hay datos ni migraciones que revertir.

## Decisiones que requieren confirmación antes de implementar

- Confirmar el endpoint recomendado `POST /agents/{agentId}/ping` y su semántica propuesta: `200` tanto para Pong como timeout con `status`, y `404` si el Agent no está conectado. Si se prefiere que timeout sea `504 Gateway Timeout`, se ajustarán contrato, pruebas y clientes administrativos.
- Confirmar el timeout inicial recomendado de Ping (10 segundos, configurable entre 1 y 60) y que esta etapa use cierre cooperativo más invalidación, dado que SignalR no ofrece aborto remoto directo desde `IHubContext`.
- Confirmar que la revocación de una conexión ya establecida se aplica cuando `IOptionsMonitor` notifica el cambio; con proveedores sin reload seguirá requiriendo reinicio/redespliegue, coherente con la Etapa 2.
