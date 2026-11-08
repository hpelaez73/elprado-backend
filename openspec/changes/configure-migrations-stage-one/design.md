# Diseño

## Contexto

La API y el Agent ya usan .NET 7, Serilog, configuración JSON y extensiones de registro propias; la API ya condiciona Swagger a Development y el Agent ya es un `BackgroundService`. El endpoint de salud aún devuelve una respuesta manual. Véase `proposal.md` para la motivación y `specs/migraciones-base-runtime/spec.md` para el contrato de comportamiento.

## Objetivos / no objetivos

**Objetivos:**

- Completar la infraestructura existente con cambios pequeños, idiomáticos y verificables en los tres proyectos de Migraciones.
- Centralizar el registro de servicios por proyecto y preservar arranques concisos.
- Dejar trazables la ejecución, el diagnóstico y las validaciones de la etapa.

**No objetivos:**

- Autenticación, SignalR, registro de agentes, Ping/Pong, conectividad Firebird, DTOs nuevos o lógica del circuito de migraciones.
- Actualizar el framework, crear proyectos, modificar proyectos ajenos o introducir dependencias que no resuelvan una necesidad de esta etapa.

## Decisiones

### Usar extensiones nativas para salud y composición

La API registrará y mapeará Health Checks con las extensiones estándar de ASP.NET Core; el endpoint conservará la ruta `/health` para evitar cambios de consumo. Las altas de Swagger, Health Checks y servicios propios permanecerán en `Infrastructure/DependencyInjection.cs`, y `Program.cs` sólo compondrá el host y el pipeline.

Alternativa considerada: conservar una lambda que devuelve `OK`. Se descarta porque no representa estados unhealthy ni usa el mecanismo extensible requerido.

### Mantener Serilog con consola y Seq opcional

La configuración continuará siendo la fuente de niveles y sink de consola. La extensión existente agregará Seq únicamente cuando exista una URL no vacía; las variables de entorno utilizarán la jerarquía habitual de configuración .NET. El host se inicializará con logger bootstrap y se cerrará en `finally`, registrando inicio, detención y errores fatales una sola vez por flujo.

Alternativa considerada: agregar providers de logging de Microsoft junto a Serilog. Se descarta para evitar mensajes duplicados y configuraciones divergentes.

### Separar configuración común y de Development sin secretos

`appsettings.json` conservará valores seguros y comunes, mientras que `appsettings.Development.json` ajustará el nivel de diagnóstico. Seq se documentará como configuración opcional mediante `Seq__ServerUrl`, sin URL con credenciales en archivos versionados.

Alternativa considerada: requerir un archivo local de secretos o Seq al iniciar. Se descarta porque impediría ejecutar la infraestructura base sin servicios externos.

### Conservar el Worker mínimo y portable

`AgentWorker` seguirá esperando con el token de cancelación del host y registrará sus transiciones. La configuración de Windows Service y systemd se mantendrá en el host, sin abstraer servicios que todavía no tienen comportamiento propio.

Alternativa considerada: crear capas de agente, clientes API y opciones de conexión anticipadas. Se descarta por estar fuera de alcance y aumentar complejidad sin uso real.

## Riesgos / compensaciones

- [Los puertos o perfiles de ejecución pueden variar entre equipos] → La documentación indicará cómo localizar la URL publicada y probar `/health` y Swagger en Development.
- [Seq podría no estar accesible] → La ausencia o falla de configuración no deberá impedir el arranque; la consola permanecerá disponible para diagnóstico.
- [La solución completa puede tener fallos ajenos] → Se compilarán primero los tres proyectos en forma individual y se reportarán separadamente los fallos no relacionados.
- [Los mensajes de ciclo de vida pueden duplicarse] → Se verificará la configuración de providers y se asignará cada evento al host o al Worker de forma deliberada.

## Plan de migración

1. Revisar el estado inicial y aplicar los ajustes mínimos en los proyectos de Migraciones.
2. Restaurar y compilar individualmente Contracts, API y Agent; luego compilar la solución si el entorno lo permite.
3. Ejecutar la API en Development, comprobar Swagger y `/health`; ejecutar y detener el Agent, comprobando sus logs.
4. Documentar comandos, variables disponibles y resultados reales de validación.

Los cambios son compatibles con la infraestructura actual. Para revertirlos, se restauran los archivos de configuración y composición de Migraciones a su revisión previa; no existen migraciones de datos ni cambios externos que revertir.
