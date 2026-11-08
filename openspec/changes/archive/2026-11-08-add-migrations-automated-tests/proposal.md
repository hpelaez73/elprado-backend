# Propuesta

## Motivo

Los proyectos de Migraciones no cuentan con una base de pruebas automatizadas que compruebe el arranque de la API y el ciclo de vida del Agent de forma aislada. Incorporarla ahora permite detectar regresiones sin requerir Firebird, Seq ni otros servicios externos.

## Cambios

- Crear proyectos de pruebas xUnit para la API y el Agent de Migraciones bajo `tests/` y agregarlos a la solución.
- Cubrir el health check de la API mediante `WebApplicationFactory`, sin autenticación ni conexiones externas.
- Cubrir el registro, inicio, cancelación y detención controlada de `AgentWorker` mediante un host en memoria y sincronización determinista.
- Documentar la ejecución de ambas suites desde la terminal y VS Code.

## Capacidades

No se modifican requisitos funcionales del sistema; el cambio incorpora infraestructura de verificación automatizada. Por ello, se omiten deltas de especificación.

## Impacto

Se modificarán `ElPradoWeb.sln`, el punto de entrada de la API (solo si requiere el ajuste mínimo de visibilidad para pruebas), la documentación de Migraciones y se agregarán las dependencias de prueba mínimas compatibles con .NET 7. No se alterarán los proyectos de producción ni se añadirán conexiones a infraestructura externa.
