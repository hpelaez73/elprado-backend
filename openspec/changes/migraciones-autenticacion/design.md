# Diseño

## Contexto

La Etapa 1 dejó una Minimal API .NET 7 con `/health` anónimo, Swagger sólo en Development y pruebas con `WebApplicationFactory`; el Agent es un Worker sin cliente HTTP. Véanse `proposal.md` y `specs/migraciones-autenticacion/spec.md` para el alcance y comportamiento. Esta etapa no usa Firebird ni tiene un almacén persistente para usuarios, agentes o revocaciones.

## Objetivos / no objetivos

**Objetivos:**

- Agregar una única configuración de seguridad validada al arranque, proveniente de variables de entorno, User Secrets en Development o el proveedor de secretos del host.
- Mantener dos identidades con tokens verificables y policies mutuamente excluyentes.
- Permitir una revocación de agente efectiva al recargar la configuración o reiniciar el proceso, sin lista de revocación persistente.
- Preparar al Agent para pedir y renovar su propio token antes de futuros clientes SignalR.

**No objetivos:**

- ASP.NET Identity, OAuth/OIDC, refresh tokens, roles, tablas de usuarios/agentes, Firebird, SignalR, registro de conexiones ni Ping/Pong.
- Administración de credenciales mediante UI o API, ni compatibilidad con los JWT de las otras fachadas de ElPrado.
- Persistir tokens, secretos o una caché de revocación en disco.

## Decisiones

### Configuración externa con validación fail-closed

La API tendrá una sección `MigrationAuthentication` enlazada a opciones: `Jwt` (clave simétrica codificada en Base64, issuer, audience y vida útil), `Admin` (usuario y hash) y `Agents` (entradas `AgentId`, `SecretHash`, `Enabled`). También incluirá los límites de rate limiting. Se validará que los textos obligatorios no estén vacíos, que la clave tenga al menos 256 bits y que los IDs sean únicos; `ValidateOnStart` detendrá el host si la configuración requerida es inválida. Un registro de agentes vacío es válido como estado sin agentes autorizados, no como acceso abierto.

Los valores secretos no se agregarán a `appsettings*.json` versionados. Development usará `dotnet user-secrets` o variables de entorno; Production usará variables inyectadas por el host, secretos montados o un gestor equivalente. `docs/migraciones/autenticacion.md` documentará el esquema, ejemplos con marcadores y la generación local de valores, sin valores reales.

Alternativa considerada: archivo JSON local con credenciales. Se descarta porque facilita versionar o desplegar secretos accidentalmente.

### Hashes PBKDF2 con formato autocontenido

Contraseñas administrativas y secretos de agente se guardarán como hashes PBKDF2-HMAC-SHA512 con sal aleatoria, iteraciones y versión serializadas junto al hash. La comparación será de tiempo constante. La utilidad de generación documentada producirá el valor que se carga en el almacén de secretos; la API sólo verifica hashes y nunca transforma un secreto en un dato persistente.

La rotación se resuelve sustituyendo el hash en la fuente de configuración y recargando o reiniciando: para el administrador invalida credenciales anteriores; para un agente, se reemplaza su secreto o se marca `Enabled=false`. No se conservará más de un secreto activo por agente en esta etapa.

Alternativa considerada: BCrypt mediante dependencia adicional. Se descarta para mantener una dependencia menos y usar criptografía disponible en .NET 7; el coste PBKDF2 será explícito y testeado.

### JWT único de corta vida con claim de clase de identidad

La API emitirá JWT Bearer firmados con la clave configurada, issuer, audience, `sub`, `iat`, `nbf`, `exp` y el claim privado `migration_identity` con valores exactos `admin` o `agent`. Un token de agente incluirá sólo el `AgentId` como sujeto; ninguno incluirá contraseña, secreto ni hash. Los valores de expiración se expresarán en configuración, con un máximo seguro validado.

`AddAuthentication().AddJwtBearer()` validará firma, issuer, audience y vida. `AdminOnly` exigirá `migration_identity=admin`; `AgentOnly` exigirá `migration_identity=agent` y una comprobación adicional contra el registro actual de agentes habilitados. Esto evita que un JWT administrativo satisfaga una ruta de agente y permite que un JWT de un agente revocado deje de autorizarse aunque aún no expire.

Alternativas consideradas: claves y esquemas Bearer separados por tipo, o roles. Dos claves/esquemas añaden configuración, rotación y pruebas sin necesidad actual; roles introducen un modelo de autorización que el alcance excluye. El claim explícito más las policies separadas conservan la frontera necesaria y puede coexistir después con SignalR.

### Revocación basada en configuración vigente

El requisito de `AgentOnly` consultará el registro recibido por `IOptionsMonitor` y comprobará que el `AgentId` del token existe y está habilitado. Los proveedores que admiten reload aplicarán un cambio sin reinicio; las variables de entorno típicamente requieren reiniciar o redesplegar el proceso. El procedimiento operativo de revocación será deshabilitar/eliminar la entrada, confirmar la carga de configuración y, cuando no haya reload, reiniciar la API. Los tokens previos no se aceptarán tras aplicar esa configuración.

Alternativa considerada: blacklist de JWT en memoria. Se descarta porque se perdería al reiniciar y no ofrece una fuente de verdad más confiable que la configuración administrada.

### Endpoints mínimos, HTTPS y límite de intentos

`/auth/admin/login` y `/auth/agent/token` serán anónimos, de cuerpo JSON y limitados por partición de origen con una ventana y cuota configurables; al excederla responderán 429. Las consultas `me` requerirán sus policies. El pipeline incorporará autenticación antes de autorización, `UseHttpsRedirection` y, tras configurar correctamente los forwarded headers del proxy en Production, no considerará HTTPS por un encabezado no confiable. Sólo se admitirá HTTP explícitamente en el perfil de pruebas locales controladas.

La respuesta de autenticación fallida será genérica. Serilog no registrará cuerpos, `Authorization`, contraseñas, `AgentSecret` ni access tokens; la documentación y los ejemplos emplearán marcadores. Swagger conservará su condición Development y `/health` no tendrá policy.

Alternativa considerada: bloqueo persistente por cuenta. Se descarta porque requeriría almacenamiento y recuperación administrativa; rate limiting por origen cumple el alcance inicial y no almacena credenciales.

### Contratos y cliente del Agent acotados

`ElPrado.Migraciones.Contracts` contendrá sólo los DTOs que viajan entre Agent y API para solicitar y devolver el token de agente. Las opciones de API, verificación de hashes, emisión JWT y DTOs administrativos permanecerán privados a sus proyectos. El Agent recibirá URL HTTPS, `AgentId` y `AgentSecret` desde configuración externa; un cliente HTTP tipado pedirá un token al inicio y lo renovará por reautenticación al vencer. No iniciará SignalR ni ejecutará migraciones.

Alternativa considerada: compartir toda la capa de autenticación. Se descarta porque trasladaría configuración y secretos de servidor al contrato compartido.

## Riesgos / compensaciones

- [La rotación de la clave JWT invalida todos los tokens vigentes] → se documentará como procedimiento intencional de emergencia y se usarán vidas cortas.
- [Una configuración de entorno no recarga dinámicamente] → el procedimiento de revocación exige un reinicio controlado cuando el proveedor no soporte reload.
- [Rate limiting por IP puede agrupar agentes detrás de NAT] → cuotas conservadoras y configurables; si resulta insuficiente, una etapa posterior podrá usar una clave compuesta sin cambiar las credenciales.
- [HTTPS terminado por proxy puede ocultar el esquema original] → configurar proxies conocidos y forwarded headers antes de habilitar Production; no confiar en encabezados arbitrarios.
- [PBKDF2 eleva el coste de cada intento] → rate limiting, parámetros configurados dentro de un rango validado y pruebas sin valores de producción.
- [La API no inicia con secretos ausentes] → es un fallo deliberado y visible que evita un arranque inseguro; Development y tests proporcionarán configuración aislada.

## Plan de migración

1. Agregar paquetes, opciones, validadores y servicios de autenticación en API, sin tocar Firebird ni otros proyectos.
2. Crear endpoints, policies, protección HTTPS/rate limiting y contratos mínimos; añadir la configuración segura a secretos de Development y al host de cada ambiente.
3. Actualizar el Agent con su cliente de token y configuración; no conectar todavía ningún hub.
4. Ejecutar pruebas de API y Agent con configuración en memoria y firmas efímeras; verificar que respuestas y logs no expongan secretos.
5. Desplegar primero con un administrador y un agente aprovisionados; probar login, revocación y renovación. Para rollback, retirar los endpoints y configuración de esta etapa o restaurar la versión previa; no hay datos que migrar.

## Confirmaciones requeridas antes de implementar

- Confirmar el claim privado propuesto `migration_identity` y los valores `admin`/`agent` como contrato estable para policies y una futura conexión SignalR.
- Confirmar PBKDF2-HMAC-SHA512 como algoritmo de hash y el rango de coste/límite de vida del token que se fijará en configuración segura.
- Confirmar que revocar por cambio de configuración, con reinicio cuando el proveedor no recarga, satisface el requisito operativo de revocación inmediata para una instalación administrada por un único desarrollador.
