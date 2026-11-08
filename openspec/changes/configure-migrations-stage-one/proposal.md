# Propuesta

## Motivo

Los proyectos de Migraciones ya contienen una base en .NET 7, pero requieren una validación y ordenamiento explícitos para contar con observabilidad, diagnóstico y ejecución consistentes antes de incorporar autenticación o comunicación en tiempo real. Esta etapa establece ese punto de partida sin adelantar componentes del circuito de migraciones.

## Cambios

- Consolidar el registro de dependencias y el arranque de la API Minimal API y del Worker Service con las extensiones nativas de ASP.NET Core.
- Configurar Serilog para consola, niveles por entorno y Seq opcional, registrando ciclo de vida y fallos críticos sin exponer datos sensibles ni duplicar proveedores.
- Restringir Swagger de la API al entorno Development y conservar una configuración de OpenAPI simple.
- Sustituir la respuesta manual de `/health` por los Health Checks estándar de ASP.NET Core, sin dependencias externas.
- Mantener configuración separada por entorno y sobrescribible mediante variables de entorno, sin secretos versionados.
- Verificar que `Contracts` permanezca como una biblioteca liviana y que API y Agent mantengan sus referencias.
- Añadir documentación reproducible de ejecución, configuración y validaciones de la Etapa 1 en `docs/migraciones/`.

## Capacidades

### Nuevas capacidades

- `migraciones-base-runtime`: Define el comportamiento base de ejecución, observabilidad, diagnóstico y configuración de los proyectos API y Agent de Migraciones.

### Capacidades modificadas

- Ninguna.

## Impacto

Se verán afectados `Program.cs`, `Infrastructure/DependencyInjection.cs`, `appsettings*.json`, el endpoint de salud y la documentación de los proyectos `ElPrado.Migraciones.Api`, `ElPrado.Migraciones.Agent` y `ElPrado.Migraciones.Contracts`. Se mantendrán .NET 7 y los paquetes actuales; no se modificará código ajeno a Migraciones ni se incorporarán JWT, SignalR, Firebird o lógica de migraciones.
