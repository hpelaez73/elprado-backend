# Tareas

## 1. Validación de datos y diseño de consulta

- [ ] 1.1 Inspeccionar una copia representativa de `DET_INHUMADOS`, cuantificar filas sin fechas y empates entre última inhumación/exhumación, e identificar con negocio una fuente de orden auditable; verificar el resultado con una consulta de diagnóstico que no use `COD_DET_INHUMADO` como orden cronológico.
- [ ] 1.2 Definir el límite máximo de meses para consultas interactivas a partir de volumen y plan Firebird, y documentarlo junto con el criterio final de empates; verificar aprobación técnica antes de codificar la clasificación.
- [ ] 1.3 Construir consultas manuales de contraste, agrupadas exclusivamente por `COD_INHUMADO`, para primera inhumación, exhumación definitiva y actualmente inhumados; verificar los diez escenarios funcionales contra un conjunto controlado.

## 2. Estadísticas Firebird

- [ ] 2.1 Crear una migración versionada con `EVOLUCION_ESTADISTICAS_INHUMACIONES(P_DESDE, P_HASTA)` que genere todos los meses del rango y devuelva `ANIO`, `MES`, `INHUMACIONES` y `EXHUMACIONES`; verificar cronología, meses sin movimientos en cero y que no se consulte `INHUMACION` ni `DET_INHUMADOS_LUGARES`.
- [ ] 2.2 Crear `RESUMEN_ESTADISTICAS_INHUMACIONES(P_DESDE, P_HASTA)` con primeras inhumaciones, exhumaciones definitivas, variación neta y total actual; verificar que traslado, cambio de ataúd y reinhumación no dupliquen fallecidos y que una nueva inhumación revierta la clasificación previa.
- [ ] 2.3 Evaluar el plan de ambos procedimientos con datos representativos y añadir sólo el índice mínimo que resulte necesario; verificar que el plan mejora o se mantiene y que no se crean índices redundantes.
- [ ] 2.4 Ejecutar pruebas SQL de límites mensuales, años distintos, fechas nulas, exhumación definitiva, movimientos intermedios y empates según el criterio aprobado; conservar los resultados de contraste como evidencia de migración.

## 3. Contrato y capas de backend

- [ ] 3.1 Crear DTOs públicos de resumen, evolución y observación mensual, además de filas internas de procedimientos, en `ElPrado.Dto`; verificar serialización JSON de `desde`, `hasta`, período, cuatro KPI y colecciones vacías sin campos individuales.
- [ ] 3.2 Implementar `IEstadisticasInhumacionesRepository` y su repositorio Dapper parametrizado, y registrarlos mediante `IUnitOfWork` y `UnitOfWork`; verificar que invoca ambos procedimientos con parámetros y mapea resultados vacíos y meses cero.
- [ ] 3.3 Implementar el servicio de estadísticas de inhumaciones con validación `YYYY-MM`, orden de rango y límite aprobado; verificar errores de formato, rango invertido y rango excesivo sin consultar Firebird.
- [ ] 3.4 Agregar un controlador dedicado bajo `api/estadisticas/inhumaciones` con `GET resumen` y `GET evolucion`, preservando `ApiResponse<T>` y autenticación de `ControladorBase`; verificar HTTP 200 para solicitudes válidas, 400 para filtros inválidos y 401 sin JWT.
- [ ] 3.5 Añadir pruebas unitarias e integración de servicio/controlador para los contratos, los cuatro KPI y la ausencia de claves o datos personales; verificar ejecución exitosa del proyecto de pruebas relevante.

## 4. Dashboard de inhumaciones

- [ ] 4.1 Confirmar la ruta protegida, opción de menú y permiso reutilizables antes de exponer la pantalla; verificar que un usuario autorizado recibe la navegación y uno no autorizado no la visualiza.
- [ ] 4.2 Crear tipos y servicios `EstadisticasInhumaciones` mediante `fetchGet` y `safeApiCall`; verificar que las URLs, query strings y errores coinciden con el contrato del backend y que los componentes no usan `fetch` directo.
- [ ] 4.3 Crear la página `gestion/estadisticas_inhumaciones` y `EstadisticasInhumacionesDashboard` con filtros Desde/Hasta, estados loading/error/vacío, actualización manual y cuatro KPI; verificar renderizado responsivo con datos, error y rango inválido.
- [ ] 4.4 Crear `EvolucionInhumacionesChart` y `ComparacionInhumacionesChart` con Recharts para inhumaciones, exhumaciones y año anterior; verificar alineación mensual, meses cero y legibilidad en pantallas estrechas sin instalar dependencias nuevas.
- [ ] 4.5 Cubrir los servicios y componentes con las pruebas existentes del frontend o pruebas manuales reproducibles, incluyendo comparación interanual y datos vacíos; verificar que `lint` y la comprobación de tipos configurados finalicen correctamente.

## 5. Integración y aceptación

- [ ] 5.1 Contrastar resumen, evolución y dashboard contra las consultas manuales de la sección 1 en una base de prueba; verificar los diez escenarios solicitados y que nunca se duplica un fallecido por detalles o ubicaciones.
- [ ] 5.2 Ejecutar las suites backend y frontend pertinentes y revisar el plan Firebird final; verificar que estadísticas de parcelas y flujos operativos de inhumación/exhumación no cambian.

## Seguimiento del flujo

- Revisar y resolver las preguntas abiertas de `design.md` antes de aplicar la clasificación de empates y de fijar el límite de rango.
- Archivar el change después de la implementación, las validaciones de despliegue y la revisión del equipo.
