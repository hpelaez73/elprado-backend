# Diseño

## Contexto

La Minimal API .NET 7 tiene `/health` anónimo, Swagger sólo en Development y pruebas con `WebApplicationFactory`; el Agent es un Worker sin cliente HTTP. Esta etapa no usa Firebird, OAuth Server, ASP.NET Identity ni almacén persistente para usuarios, agentes o revocaciones. Véanse `proposal.md` y `specs/migraciones-autenticacion/spec.md` para el alcance funcional.

## Objetivos / no objetivos

**Objetivos:**

- Obtener credenciales y configuración de seguridad desde fuentes externas, validadas fail-closed.
- Mantener identidades admin y agent separadas mediante JWT Bearer y autorización basada en configuración vigente.
- Renovar el token del Agent antes de vencer, sin solicitudes duplicadas, y mantener recuperable el Worker ante indisponibilidad temporal de la API.
- Dejar explícitos los límites criptográficos, de transporte y de reintentos.

**No objetivos:**

- Refresh tokens, roles, tablas de usuarios/agentes, Firebird, SignalR, OAuth/OIDC o ASP.NET Identity.
- Persistir tokens, secretos o una caché de revocación en disco.
- Administración de credenciales mediante UI o API, ni compatibilidad con JWT de otras fachadas.

## Decisiones

### Configuración externa validada al inicio

`MigrationAuthentication` contendrá `Jwt`, `Admin`, `Agents`, límites de autenticación y parámetros PBKDF2. `Jwt` usará clave HMAC Base64 de al menos 256 bits, issuer, audience y vidas de token. Los valores iniciales serán 30 minutos para administrador y 15 para agente. Serán configurables y se validarán: vida admin 5–60 minutos, agent 5–30 minutos, y margen de renovación 30 segundos–5 minutos, siempre inferior a la vida agent.

Se validarán textos obligatorios, Base64, IDs únicos y hashes; `ValidateOnStart` impedirá iniciar con configuración requerida inválida. Un registro vacío es válido y no concede acceso. Los secretos no irán a `appsettings*.json` versionados; Development usará User Secrets o variables de entorno y Production secretos del host. Fuentes sin recarga, como variables de entorno, requerirán reinicio o redespliegue documentado.

Alternativa considerada: archivo JSON local con credenciales. Se descarta porque facilita versionar o desplegar secretos accidentalmente.

### PBKDF2 autocontenido y aprovisionamiento sin exposición

Contraseñas administrativas y secretos de agente usarán PBKDF2-HMAC-SHA512 versionado que serializa versión, iteraciones, sal aleatoria de 16 bytes y hash derivado de 32 bytes. El valor inicial será 210.000 iteraciones y se validará entre 210.000 y 1.000.000. La comparación será de tiempo constante; hashes malformados fallarán sin revelar causa.

La documentación definirá una utilidad CLI local, fuera del proceso hospedado de la API, que recibe el secreto por entrada estándar sin eco y escribe sólo el hash para copiar al almacén de secretos. No aceptará secretos como argumentos, no los registrará y los ejemplos usarán marcadores. La API sólo verificará hashes.

Alternativa considerada: BCrypt. Se descarta para usar criptografía disponible en .NET 7, menos dependencias y coste explícito.

### JWT Bearer de corta vida por clase de identidad

La API emitirá JWT firmados con `issuer`, `audience`, `sub`, `iat`, `nbf`, `exp` y el claim privado `migration_identity` con valores exactos `admin` o `agent`. El token agent sólo llevará `AgentId` como sujeto; ningún token incluirá contraseña, secreto ni hash. Bearer validará firma, issuer, audience y vida. `AdminOnly` exigirá `migration_identity=admin`; `AgentOnly`, `migration_identity=agent` y agente habilitado en la configuración vigente.

Alternativas consideradas: claves/esquemas separados o roles. Añaden configuración o un modelo fuera del alcance sin aportar una frontera necesaria.

### Revocación y rotación son operaciones distintas

`AgentOnly` consultará `IOptionsMonitor` en cada autorización para comprobar que el `AgentId` existe y está habilitado. Deshabilitar o eliminar es revocación: tras reload o reinicio se deniegan nuevos tokens y tokens previos, aunque no hayan vencido. Cambiar sólo `SecretHash` es rotación: el secreto anterior deja de obtener tokens, pero JWT ya emitidos siguen válidos hasta `exp` mientras el agente esté habilitado. Para invalidarlos antes se debe revocar; rotar la clave JWT invalida todos los JWT y se reserva para emergencia.

Alternativa considerada: blacklist de JWT en memoria. Se descarta porque se pierde al reiniciar y no mejora la fuente de verdad.

### HTTPS estricto y forwarded headers de confianza explícita

Fuera de pruebas locales controladas, middleware temprano rechazará HTTP para endpoints de autenticación o protegidos antes de enlazar cuerpo, verificar credenciales o autenticar. `UseHttpsRedirection` puede conservarse para rutas públicas, pero no lo sustituye. Detrás de proxy, forwarded headers se aplicarán antes de esa comprobación y sólo si la dirección remota pertenece a `KnownProxies` o `KnownNetworks` configurados; encabezados no autorizados no cambiarán el esquema efectivo.

Alternativa considerada: confiar en cualquier `X-Forwarded-Proto`. Se descarta porque permite que un cliente declare HTTPS artificialmente.

### Renovación coordinada y tolerancia a fallos del Agent

El Agent guardará el token sólo en memoria y lo renovará si falta o entra en el margen configurado; el valor inicial será dos minutos antes de `exp`. Un mecanismo single-flight compartirá una sola renovación entre llamadas concurrentes.

Red, timeouts y HTTP 429 se reintentarán con backoff exponencial de 1 segundo, multiplicador 2, tope 60 segundos y jitter uniforme ±20 %. Valores configurables y validados: base 1–10 segundos, tope 30–300 segundos, jitter 0–25 %. En 429 se respetará `Retry-After` válido, limitado por el tope, antes del jitter. Se continuará con espera limitada hasta recuperar la API; el Worker no terminará definitivamente. HTTP 401 en `/auth/agent/token` se tratará como credencial inválida: log seguro, sin reintentos automáticos hasta cambio de configuración o reinicio controlado.

Alternativa considerada: renovar sólo al vencer o por cada consumidor. Puede interrumpir operaciones o duplicar carga y exposición del secreto.

### Endpoints, límites y observabilidad segura

Los endpoints de token serán anónimos, JSON y limitados por origen; al excederse responderán 429 sin validar ni emitir token. Las rutas `me` requerirán sus policies. `/health` seguirá anónimo y Swagger sólo en Development. Serilog no registrará cuerpos, `Authorization`, contraseñas, `AgentSecret` ni tokens; errores usarán mensajes genéricos.

### Contratos mínimos

`ElPrado.Migraciones.Contracts` contendrá sólo DTOs de solicitud/respuesta de token agent. Opciones, hashes, emisión JWT y DTOs administrativos serán privados. El Agent recibirá URL HTTPS, `AgentId` y `AgentSecret` desde configuración externa; no iniciará SignalR ni migraciones.

## Riesgos / compensaciones

- [Rotar la clave JWT invalida todos los tokens] → se documentará como operación de emergencia y las vidas serán cortas.
- [Una fuente no recarga configuración] → revocación/rotación exige reinicio o redespliegue controlado.
- [Rotar secreto no revoca JWT previos] → se documentará y se indicará deshabilitar al agente si se requiere invalidación inmediata.
- [Rate limiting por IP agrupa agentes detrás de NAT] → cuotas conservadoras y configurables.
- [Proxy mal configurado altera el esquema] → sólo proxies/redes autorizados aportan forwarded headers.
- [PBKDF2 aumenta coste] → parámetros acotados, rate limiting y pruebas sin valores productivos.
- [API temporalmente indisponible retrasa al Agent] → backoff limitado, jitter y Worker persistente.

## Plan de migración

1. Agregar paquetes, opciones, validadores, generador PBKDF2 local y autenticación sin tocar Firebird.
2. Crear endpoints, policies, HTTPS/forwarded headers y rate limiting; configurar secretos externos.
3. Actualizar Agent con renovación anticipada coordinada y reintentos, sin SignalR.
4. Ejecutar pruebas de API y Agent con configuración en memoria y escenarios de independencia, rotación, revocación, 401, red/429 y renovación.
5. Desplegar con un administrador y dos agentes de prueba; verificar renovación, revocación y rotación. Rollback: restaurar la versión previa; no hay datos que migrar.
