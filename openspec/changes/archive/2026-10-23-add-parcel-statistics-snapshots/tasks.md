# Tasks

## 1. Validación funcional y migración de datos

- [x] 1.1 Documentar las fórmulas B1 aprobadas para al día, morosas y recuperadas: al día hasta el mes en curso más un mes adicional; morosa desde tres meses pendientes contando el mes en curso; recuperada cuando una morosa paga en el mes hasta el mes en curso o posterior; verificar el documento contra casos manuales del ERP.
- [x] 1.2 Relevar las fuentes Firebird de `MESES_PENDIENTES` al momento de una baja y verificar una propuesta dada de baja con meses pendientes y una sin deuda.
- [x] 1.3 Crear migraciones versionadas para `EST_PARCELAS_CORTE`, `EST_EVENTOS_PARCELAS`, `EST_CARGAS_PARCELAS`, sus generadores, PK/FK, restricciones únicas e índices; verificar creación limpia en una base de prueba y unicidad de período/parcela.

## 2. Carga estadística Firebird

- [x] 2.1 Implementar `REFRESCAR_ESTADISTICAS_PARCELAS(P_PERIODO, P_FECHA_CORTE, P_RECONSTRUIR)` con validación de corte, transacción y selección de una propuesta vigente por parcela; verificar una reasignación de parcela dentro del mismo mes sin duplicar el snapshot.
- [x] 2.2 Incorporar en el refresco los eventos de alta, baja y exhumación con `MESES_PENDIENTES`; verificar que una reejecución no duplica eventos ni pierde los meses pendientes de una baja.
- [x] 2.3 Registrar el resultado exitoso o fallido de cada carga en `EST_CARGAS_PARCELAS`; verificar que un fallo revierte datos parciales y no declara el período disponible.
- [x] 2.4 Preparar y ejecutar un backfill controlado de doce meses, un período por corrida; verificar cobertura completa, conteos comparados con consultas manuales y tiempos registrados.
- [x] 2.5 Documentar el comando y parámetros que Windows Task Scheduler debe invocar para el mes corriente; verificar manualmente una ejecución idempotente con la fecha de corte esperada.

## 3. Consultas agregadas y rendimiento

- [x] 3.1 Implementar `RESUMEN_ESTADISTICAS_PARCELAS(P_PERIODO, P_PRODUCTO)` con agregación exclusiva de snapshots; verificar `todos`, `dpc`, `flores-siempre`, ambas banderas y denominadores cero.
- [x] 3.2 Implementar `EVOLUCION_ESTADISTICAS_PARCELAS(P_DESDE, P_HASTA, P_PRODUCTO)` y `COBERTURA_ESTADISTICAS_PARCELAS(P_DESDE, P_HASTA)`; verificar orden cronológico y que los períodos faltantes no se convierten en ceros.
- [ ] 3.3 Medir plan y latencia de las consultas para doce meses y cada producto en datos de volumen representativo; la latencia de cada consulta debe ser de hasta 500 ms; agregar un índice compuesto sólo si el plan demuestra beneficio.

## 4. API de estadísticas

- [x] 4.1 Crear DTOs tipados de resumen, evolución, KPI, observación, disponibilidad y filtros en `ElPrado.Dto`; verificar serialización JSON de períodos, nulos y colecciones vacías.
- [x] 4.2 Agregar repositorio de estadísticas, propiedad en `IUnitOfWork`/`UnitOfWork` y servicio que invoquen los SP con parámetros Dapper; verificar que no se construye SQL dinámico a partir de filtros.
- [x] 4.3 Implementar `EstadisticasController` en `ElPrado.WebApi` con `GET /api/estadisticas/parcelas/resumen` y `GET /api/estadisticas/parcelas/evolucion`; verificar JWT, formatos `YYYY-MM`, rango, productos permitidos y HTTP 400 para solicitudes inválidas.
- [x] 4.4 Implementar la respuesta de período no disponible con HTTP 200 y `datosDisponibles: false`; verificar que resumen y evolución informan cobertura y no publican KPI ficticios.
- [x] 4.5 Documentar los endpoints y ejemplos en Swagger o la documentación de API vigente; verificar que los parámetros y respuestas coinciden con las especificaciones del change.

## 5. Pruebas integradas y salida controlada

- [x] 5.1 Agregar pruebas de Firebird y de integración para bajas con meses pendientes, reasignación de parcela, filtros de producto, períodos faltantes y denominador cero; verificar que la suite relevante pasa.
- [x] 5.2 Comparar una muestra de resumen y evolución contra consultas manuales del ERP; registrar las diferencias justificadas por la nueva semántica de corte.
- [x] 5.3 Ejecutar pruebas de rendimiento y autorización de los dos endpoints en la base de desarrollo que replica la carga de producción; verificar que no se transfieren filas individuales de parcelas y que la latencia de cada endpoint es de hasta 500 ms.

## Workflow follow-up

- Revisar los artefactos y aplicar el change mediante `$openspec-apply-change` cuando se autorice la implementación.
- Archivar el change cuando la implementación, validación de negocio y despliegue estén completos.
