# Propuesta

## Motivo

La API y el Agent de Migraciones necesitan identidades verificables para administradores y para cada instalación de agente, sin incorporar persistencia de usuarios ni un proveedor de identidad. La autenticación debe mantener al Worker operativo ante fallos transitorios, revocar agentes de forma controlada y no exponer credenciales durante su aprovisionamiento.

## Cambios

- Incorporar JWT Bearer de corta duración para identidades excluyentes, con el claim privado `migration_identity` y valores `admin` y `agent`.
- Exponer los inicios de sesión y consultas de identidad protegidas por `AdminOnly` y `AgentOnly`.
- Usar hashes PBKDF2-HMAC-SHA512 y documentar una utilidad local que genere hashes sin mostrar, registrar ni persistir el secreto de entrada.
- Distinguir revocación de agentes y rotación de su secreto, incluido el efecto sobre tokens emitidos.
- Renovar anticipadamente el token del Agent con una única solicitud concurrente y recuperarse de red/HTTP 429 con backoff exponencial y jitter; no reintentar continuamente credenciales rechazadas.
- Exigir HTTPS antes de procesar credenciales o tokens y aceptar forwarded headers sólo de proxies autorizados.
- Validar configuración y límites al inicio; admitir que algunas fuentes requieren reiniciar la API.
- Ampliar pruebas para independencia entre agentes, rotación, revocación, configuración inválida y renovación, sin Firebird, OAuth Server, ASP.NET Identity ni servicios externos.

## Capacidades

### Nuevas capacidades

- `migraciones-autenticacion`: Autenticación, emisión y validación de tokens, autorización por identidad, protección de transporte y gestión configurada de credenciales para Migraciones.

### Capacidades modificadas

- Ninguna.

## Impacto

Se modificarán sólo `ElPrado.Migraciones.Api`, `ElPrado.Migraciones.Agent`, `ElPrado.Migraciones.Contracts`, sus proyectos xUnit y la documentación de Migraciones. No se modificarán Firebird, SignalR, otros proyectos del repositorio ni se versionarán secretos reales.
