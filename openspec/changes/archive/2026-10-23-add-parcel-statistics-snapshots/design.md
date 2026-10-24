# Design

## Context

Ver `proposal.md` y las dos especificaciones del cambio. El SP legado `INS_ESTADISTICAS_PARCELAS` inserta una fila por propuesta que intersecta el mes, por lo que una parcela reasignada dentro del período puede aparecer dos veces. La tabla existente tiene aproximadamente diez mil filas por mes y un índice simple por `PERIODO`. No existe un cubo legado que funcione como fuente de contraste.

## Goals / Non-Goals

**Goals:**

- Materializar hechos atómicos de cobranza por parcela al corte, no KPI ni tarjetas del frontend.
- Conservar los eventos de una propuesta que se da de baja sin duplicar la cartera vigente de su parcela.
- Hacer idempotente la carga nocturna y habilitar backfill por mes.
- Resolver resumen y evolución con agregaciones Firebird acotadas por período y producto.

**Non-Goals:**

- Reemplazar las fuentes transaccionales de propuestas, cuotas, créditos o configuraciones.
- Publicar detalle de parcelas, reportes/exportaciones o una interfaz de administración de batches.
- Cambiar la semántica de los estados de deuda sin validación de negocio en B1.
- Mantener compatibilidad de contrato con artefactos históricos que no existen en este backend.

## Decisions

### 1. Separar snapshot vigente, eventos y control de carga

Se crearán tablas nuevas y se mantendrá `EST_PARCELAS` sin cambios durante la migración:

| Tabla | Granularidad | Propósito |
|---|---|---|
| `EST_PARCELAS_CORTE` | `PERIODO`, `COD_PARCELA` | Una propuesta vigente y sus hechos de cobranza para cada parcela al corte vigente del período. |
| `EST_EVENTOS_PARCELAS` | propuesta, tipo y fecha efectiva de evento | Hechos de alta, baja y exhumación, incluidos los meses pendientes al evento. |
| `EST_CARGAS_PARCELAS` | `PERIODO` | Cobertura, fecha de corte, actualización, estado y cantidades de la última carga exitosa. |

`EST_PARCELAS_CORTE` tendrá `COD_PARCELA`, `COD_PROPUESTA`, `PERIODO`, `FECHA_CORTE`, flags de cartera y producto (`ES_ACTIVO`, `ES_AL_DIA`, `ES_MOROSO`, `ES_RECUPERADO`, `ES_DEUDOR_RECIENTE`, `MESES_PENDIENTES`, `TIENE_DPC`, `TIENE_FLORES_SIEMPRE`) y `ACTUALIZADO_EN`. La restricción única `(PERIODO, COD_PARCELA)` evita duplicar cartera por reasignaciones.

`EST_EVENTOS_PARCELAS` tendrá `COD_PARCELA`, `COD_PROPUESTA`, `PERIODO`, `FECHA_EVENTO`, `TIPO_EVENTO` y `MESES_PENDIENTES`. La clave única será `(COD_PROPUESTA, TIPO_EVENTO, FECHA_EVENTO)`. Los importes de capital, interés y total pendientes no se persistirán en esta etapa.

Alternativa descartada: reutilizar `EST_PARCELAS` para ambos fines. No permite imponer una fila por parcela y período sin eliminar o reinterpretar su historia por propuesta.

### 2. Corte explícito y captura de eventos

El proceso de refresco recibirá el período y una fecha de corte. Para el snapshot seleccionará únicamente la propuesta vigente en esa fecha: alta en o antes del corte y baja posterior al corte o inexistente. Los eventos se detectarán por su fecha efectiva dentro del período y capturarán los meses pendientes calculados al evento. No calculará ni persistirá importes monetarios de deuda.

El mes en curso se refresca cada noche y actualiza su único snapshot por parcela. Los meses cerrados sólo se recalculan con una solicitud explícita. De esta forma el estado actual del mes puede evolucionar sin alterar cierres anteriores.

Las reglas B1 aprobadas son: una parcela está al día si debe como máximo el mes en curso y un mes adicional; está morosa si debe tres meses o más contando el mes en curso; y está recuperada si antes era morosa y durante el mes en curso paga hasta ese mes o uno posterior. Estas reglas se materializarán con `MESES_PENDIENTES`, `ES_AL_DIA`, `ES_MOROSO` y `ES_RECUPERADO` en el snapshot.

### 3. Stored procedures con responsabilidades acotadas

Se crearán los siguientes SP Firebird:

- `REFRESCAR_ESTADISTICAS_PARCELAS(P_PERIODO, P_FECHA_CORTE, P_RECONSTRUIR)`: valida el corte, regenera sólo el período solicitado, hace upsert de snapshots/eventos y marca la carga exitosa. `P_RECONSTRUIR` habilita períodos cerrados.
- `RESUMEN_ESTADISTICAS_PARCELAS(P_PERIODO, P_PRODUCTO)`: devuelve hechos agregados del período disponible, su corte y actualización.
- `EVOLUCION_ESTADISTICAS_PARCELAS(P_DESDE, P_HASTA, P_PRODUCTO)`: devuelve sólo períodos presentes, ordenados, para construir la serie.
- `COBERTURA_ESTADISTICAS_PARCELAS(P_DESDE, P_HASTA)`: devuelve la cobertura por período para que la API informe faltantes sin deducirlos de filas de cartera.

Los SP de lectura agruparán la tabla de snapshot y no recorrerán cuotas, créditos ni configuraciones transaccionales. Los eventos se consultarán sólo para métricas de movimientos que B1 apruebe; no integrarán denominadores de cartera vigente.

Alternativa descartada: calcular indicadores directamente contra cuotas en cada request. Repetiría joins y subconsultas costosas, y la respuesta dependería del estado vivo en vez del cierre estadístico.

### 4. Índices iniciales y evaluación posterior

Además de las PK y FK necesarias, se crearán:

- único `(PERIODO, COD_PARCELA)` en `EST_PARCELAS_CORTE`;
- acceso por `(PERIODO, TIPO_EVENTO)` en `EST_EVENTOS_PARCELAS`;
- acceso por período en `EST_CARGAS_PARCELAS`.

No se creará inicialmente un índice aislado de `TIENE_DPC`: en los datos inspeccionados tiene baja selectividad. Se medirán planes antes de agregar un índice compuesto para filtros específicos, en particular `FLORES_SIEMPRE`.

### 5. Contrato de API y capas existentes

Se agregará un controlador de estadísticas bajo `ElPrado.WebApi`, autenticado por el patrón de `ControladorBase`. Controller, Service, UnitOfWork y Repository mantendrán la separación actual; Dapper invocará SP parametrizados. Los DTOs tipados describirán resumen, evolución, filtros y disponibilidad.

El resumen tendrá `datosDisponibles`, `periodoCorte`, `actualizadoEn`, `filtrosAplicados` y KPI. La evolución tendrá `observaciones`, `periodosFaltantes`, filtros y límites efectivos. Para ausencia de snapshot se devolverá HTTP 200 con `datosDisponibles: false`, no un cero ni un error de validación. Los rangos/formato/producto inválidos devuelven HTTP 400.

## Risks / Trade-offs

- [Una baja corregida retroactivamente puede dejar un evento desactualizado] → el refresco explícito del período reconstruye snapshots y eventos de forma transaccional.
- [El batch falla luego de limpiar datos del período] → ejecutar el refresco en una transacción y publicar `EST_CARGAS_PARCELAS` sólo al finalizar correctamente.
- [La carga de meses pasados impacta el ERP] → limitar el backfill a un período por ejecución, medir tiempos y ejecutarlo fuera de horario.
- [Producto puede crecer más allá de DPC y Flores Siempre] → usar un parámetro validado en API y centralizar su traducción en Firebird, sin SQL dinámico.

## Migration Plan

1. Crear las nuevas tablas, secuencias/generadores, restricciones e índices mediante migraciones versionadas.
2. Implementar y probar los SP de refresco, consulta y cobertura sin cambiar consumidores existentes.
3. Ejecutar un backfill controlado de al menos los últimos doce meses y comparar muestras con consultas manuales del ERP.
4. Habilitar el batch nocturno para el mes corriente y alertar cargas fallidas o períodos faltantes.
5. Publicar la API autenticada, medir plan/latencia con un máximo de 500 ms por endpoint y conectar el frontend.
6. Mantener `EST_PARCELAS` y los SP legados durante una ventana de validación; el rollback consiste en deshabilitar el batch y retirar las rutas nuevas, sin borrar las nuevas tablas históricas.

La validación B1 de fórmulas y denominadores queda fijada en las reglas de al día, morosa y recuperada documentadas arriba. El alcance de datos de baja conserva fecha, tipo y meses pendientes; los importes de capital, interés y total quedan diferidos.
