# Diseño

## Contexto

Ver `proposal.md` y las especificaciones del cambio. El backend ya implementa estadísticas de parcelas con `EstadisticasController`, `EstadisticasService`, `EstadisticasRepository`, DTOs en `ElPrado.Dto`, `IUnitOfWork`/`UnitOfWork` y Dapper sobre procedimientos Firebird. El frontend de la rama `feature-estadisticas` consume ese patrón desde servicios `fetchGet`/`safeApiCall`, y renderiza componentes client-side con Ant Design, Day.js y Recharts.

El esquema real confirma que `DET_INHUMADOS` contiene una fila por detalle de movimiento, con `COD_INHUMADO`, `FECHA_INHUMACION` y `FECHA_EXHUMACION`; su clave primaria es `COD_DET_INHUMADO` y está relacionado con `INHUMADOS`. `DET_INHUMADOS_LUGARES` puede tener varias filas por detalle y por eso no puede integrar la fuente de conteo. No se identificaron índices declarados adicionales para las fechas o la combinación de consulta en el repositorio de esquema. `INHUMACION` queda excluida por ser legado.

## Objetivos y no objetivos

**Objetivos:**

- Calcular los indicadores a partir de un conjunto canónico de una fila por `COD_INHUMADO`.
- Publicar resumen y evolución mensual autenticados, estables y sin detalle individual.
- Diseñar una pantalla protegida, responsiva y coherente con `EstadisticasParcelasDashboard`.
- Mantener las consultas de lectura agregadas, sin snapshots ni procesos en segundo plano.

**No objetivos:**

- Cambiar tablas operativas, procedimientos existentes de parcelas, flujo de inhumación/exhumación o menú/permisos sin validación previa.
- Implementar mapas, cortes por parcela/manzana, detalle individual, exportaciones o métricas de duración de movimientos.
- Tratar movimientos inferiores a siete días con una regla especial.
- Resolver por heurística el orden de acciones que comparten exactamente fecha y hora.

## Decisiones

### Fuente y conjunto canónico por fallecido

Los dos procedimientos consultarán exclusivamente `DET_INHUMADOS`, sin unirse a `DET_INHUMADOS_LUGARES`. Una consulta común conceptual agrupará por `COD_INHUMADO` y calculará:

- `primera_inhumacion = MIN(FECHA_INHUMACION)`;
- `ultima_inhumacion = MAX(FECHA_INHUMACION)`;
- `ultima_exhumacion = MAX(FECHA_EXHUMACION)`.

El filtrado de fechas nulas se realizará antes de las agregaciones pertinentes. Desde este conjunto se deriva una única contribución por fallecido: primera inhumación en el mes de `primera_inhumacion`; exhumación definitiva en el mes de `ultima_exhumacion` cuando sea estrictamente posterior a `ultima_inhumacion`; estado actual inhumado cuando existe la primera fecha y no se cumple esa condición. Así se eliminan duplicaciones por traslado, cambio de ataúd, nueva parcela, receptáculo o múltiples ubicaciones físicas.

Se prefiere esta agregación sobre contar filas o intentar emparejar cada exhumación con la inhumación del mismo detalle: el modelo permite múltiples detalles para una misma persona y el indicador solicitado es por fallecido, no por acción física. Los procedimientos repetirán una CTE o tabla derivada equivalente mientras no exista una abstracción Firebird reutilizable que no aumente complejidad; se extraerá una pieza común sólo si el dialecto, plan y mantenibilidad la justifican.

### Procedimientos y rango inclusivo de meses

Se crearán en una migración versionada los procedimientos:

- `EVOLUCION_ESTADISTICAS_INHUMACIONES(P_DESDE, P_HASTA)`, con límites mensuales normalizados y una fila por cada mes del rango: `ANIO`, `MES`, `INHUMACIONES`, `EXHUMACIONES`.
- `RESUMEN_ESTADISTICAS_INHUMACIONES(P_DESDE, P_HASTA)`, que devuelve `INHUMACIONES`, `EXHUMACIONES`, `VARIACION_NETA` e `INHUMADOS_ACTUALES`.

El rango abarcará meses completos: desde el primer instante de `P_DESDE` hasta antes del primer instante del mes posterior a `P_HASTA`. La evolución generará explícitamente el calendario mensual, de modo que un mes sin movimientos devuelva cero y pueda compararse visualmente con otro año. El resumen suma las dos clasificaciones dentro del rango y calcula la variación neta como inhumaciones menos exhumaciones; el total actual no queda limitado por el rango, porque representa el historial disponible al momento de la consulta.

Se prefiere no almacenar snapshots: la semántica de una exhumación definitiva puede cambiar al registrarse una nueva inhumación, y el alcance pide reflejar el historial actual. La alternativa de materializar cierres requeriría reconciliación y jobs explícitamente fuera de alcance.

### Empates de fecha y hora

El uso de `MAX` resuelve el orden sólo cuando las marcas temporales difieren. `COD_DET_INHUMADO` es una clave, pero no hay evidencia en el esquema de que establezca el orden cronológico de acciones, por lo que no se usará como desempate. La implementación deberá primero medir los empates reales, revisar si existe una marca de auditoría o regla de negocio confiable y acordar el comportamiento. Hasta esa validación, el diseño no prescribe clasificar un empate como exhumación definitiva.

### Capas backend y contratos

Se incorporarán DTOs separados del módulo de parcelas, previsiblemente `DtoEstadisticasInhumacionesResumen`, `DtoEstadisticasInhumacionesEvolucion` y `DtoEstadisticasInhumacionesMes`, más DTOs internos de transporte para las filas de procedimientos. El resumen contendrá `desde`, `hasta`, `inhumaciones`, `exhumaciones`, `variacionNeta` e `inhumadosActuales`; cada observación mensual contendrá `anio`, `mes`, `periodo`, `inhumaciones` y `exhumaciones`.

Un repositorio Dapper dedicado invocará ambos procedimientos parametrizados. Su interfaz se registrará en `IUnitOfWork` y `UnitOfWork`; un servicio validará `YYYY-MM`, orden y máximo de meses antes de consultar. Un controlador dedicado con ruta `api/estadisticas/inhumaciones`, derivado del patrón `ControladorBase`, publicará:

- `GET /api/estadisticas/inhumaciones/resumen?desde=YYYY-MM&hasta=YYYY-MM`
- `GET /api/estadisticas/inhumaciones/evolucion?desde=YYYY-MM&hasta=YYYY-MM`

Las respuestas preservarán el envoltorio `ApiResponse<T>`, devolverán HTTP 400 ante filtros inválidos y heredarán la autenticación del controlador base. No se expondrán campos de fallecidos ni claves operativas. Se elige controlador dedicado en lugar de ampliar el actual, cuyo `Route` está fijado a `api/estadisticas/parcelas`, para conservar rutas y responsabilidades existentes.

### Diseño frontend

En `elprado-frontend`, una nueva página protegida bajo `src/app/gestion/estadisticas_inhumaciones/page.tsx` reutilizará `PageContainer` y cargará dinámicamente `EstadisticasInhumacionesDashboard`. Los servicios en `src/services/EstadisticasInhumaciones/` usarán `fetchGet` y `safeApiCall`; no habrá `fetch` directo en componentes.

El dashboard conservará filtros mensuales Desde/Hasta con `DatePicker picker="month"`, estado de carga/error/vacío y botón de actualización. Mostrará cuatro `Statistic`: primeras inhumaciones, exhumaciones definitivas, variación neta y actualmente inhumados. `EvolucionInhumacionesChart` mostrará ambas series mensuales; `ComparacionInhumacionesChart` solicitará o construirá desde observaciones del rango homólogo del año anterior una comparación alineada por mes. Ambos serán componentes client-side con Recharts. La pantalla no deberá añadir dependencia: Ant Design, Day.js y Recharts ya existen.

La ruta, opción de menú y permiso deben verificarse durante implementación contra el mecanismo existente antes de crear una entrada: la inspección actual confirma que parcelas depende de la entrada autorizada entregada por backend, no de un enlace hardcodeado.

### Rendimiento y pruebas

Los procedimientos limitarán el calendario y la clasificación por fechas del rango cuando calculen los eventos mensuales, pero la determinación de última acción y total actual requiere considerar el historial completo por fallecido. Se validará el plan de ejecución y cardinalidad con una base representativa. Antes de añadir índices se medirá el costo; si es necesario, la migración propondrá un índice que soporte la agrupación por `COD_INHUMADO` y las fechas de inhumación/exhumación, sin crear índices redundantes con los ya presentes en la base desplegada.

Las pruebas de servicio/repositorio cubrirán: única inhumación; exhumar-reinhumar; traslado; exhumación definitiva; múltiples movimientos intermedios; reversión de una exhumación definitiva por nueva inhumación; cambios de mes/año; ausencia de fechas válidas; empate temporal; meses sin movimientos y comparación interanual. La validación SQL incluirá que agregar ubicaciones físicas no altere el conteo.

## Riesgos y compensaciones

- [Empates de fecha y hora sin orden verificable] → Medir casos, acordar fuente de orden y añadir prueba de regresión antes de habilitar una clasificación definitiva para ellos.
- [Consulta histórica completa para estado actual] → Medir planes y tiempos con datos reales; agregar índice sólo con evidencia y revisar su impacto de escritura.
- [Exhumación histórica reclasificable] → Documentar que los informes reflejan el historial disponible y no son un cierre contable inmutable.
- [Inconsistencia de contrato entre repositorios] → Fijar DTOs, respuestas ejemplo y pruebas de integración antes de construir los componentes del frontend.
- [Ruta o permiso aún no inventariados] → Reutilizar el mecanismo de menú autorizado de parcelas y confirmar la opción existente o requerida durante la tarea de integración.

## Plan de migración

1. Añadir la migración Firebird con los dos procedimientos de lectura y, si el análisis lo respalda, el índice mínimo necesario.
2. Ejecutar contra un respaldo o entorno de prueba, contrastando resultados con casos manuales y verificando planes.
3. Desplegar backend con las rutas nuevas; el cambio es aditivo y no altera consumidores existentes.
4. Desplegar frontend después del backend. Ante un rollback, retirar la pantalla o las rutas; los procedimientos nuevos no modifican datos operativos y pueden permanecer inactivos o eliminarse mediante una migración inversa controlada.

## Preguntas abiertas

- ¿Qué dato auditable, si existe, define el orden de inhumación y exhumación cuando ambas marcas temporales son idénticas? El identificador de detalle no se asumirá como orden cronológico.
- ¿Cuál es el límite máximo de meses aceptable para una consulta interactiva y cuál es el volumen real de `DET_INHUMADOS` para fijarlo con evidencia?
- ¿Ya existe la opción autorizada de menú para estadísticas de inhumaciones, o debe gestionarse en la configuración de permisos durante la implementación?
