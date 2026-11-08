# Propuesta

## Motivo

La API central no dispone aún de un canal autenticado y bidireccional para conocer qué instalaciones de Agent están disponibles ni para comprobar su comunicación. Esta etapa extiende la autenticación individual de la Etapa 2 con SignalR, manteniendo las credenciales y los tokens fuera de persistencia y de los logs.

## Cambios

- Incorporar el Hub SignalR `/hubs/agents`, exclusivo de identidades `AgentOnly`, cuya identidad se deriva exclusivamente del JWT validado.
- Incorporar un registro local, singleton y thread-safe de conexiones activas por `AgentId`, con reemplazo seguro de conexiones duplicadas y sin persistencia ni historial.
- Agregar al Agent un cliente SignalR oficial para .NET 7 que reutilice `IAgentTokenProvider`, gestione su ciclo de vida con el Worker y se recupere de fallos transitorios mediante reconexión acotada.
- Definir contratos mínimos de Ping/Pong con correlación y exponer la consulta administrativa `GET /agents`; planificar también un Ping administrativo puntual para validar la vía bidireccional.
- Extender las garantías de autorización y revocación de la Etapa 2 al establecimiento y uso de conexiones SignalR, incluido el cierre de conexiones ya establecidas cuando un agente se revoca.
- Ampliar las pruebas unitarias e integrar una prueba local real Hub–Agent sin Firebird, Seq ni servicios externos.

## Capacidades

### Nuevas capacidades

- `migraciones-comunicacion-agentes`: Canal SignalR autenticado entre la API de Migraciones y sus agentes, presencia local de conexiones, Ping/Pong y consulta administrativa de disponibilidad.

### Capacidades modificadas

- Ninguna. La especificación de autenticación de la Etapa 2 aún está en su change y no figura como capacidad principal sincronizada; este change reutiliza su comportamiento implementado sin redefinirlo.

## Impacto

Se modificarán únicamente `ElPrado.Migraciones.Api`, `ElPrado.Migraciones.Agent`, `ElPrado.Migraciones.Contracts` y sus proyectos xUnit, añadiendo los paquetes oficiales de SignalR necesarios para .NET 7. Se añaden `/hubs/agents`, `GET /agents` y, si se confirma, `POST /agents/{agentId}/ping`; no se crean tablas Firebird, colas, Redis, backplanes, frontend ni soporte multiinstancia.
