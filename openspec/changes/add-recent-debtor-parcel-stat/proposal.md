# Proposal

## Why

El snapshot de parcelas ya conserva el indicador `ES_DEUDOR_RECIENTE`, pero las estadísticas publicadas no lo exponen. Incorporarlo permite que los consumidores consulten esa clasificación sin recalcularla ni acceder a registros individuales.

## What Changes

- Agregar a los KPI del resumen y de cada observación de evolución de estadísticas de parcelas la cantidad agregada de parcelas con `ES_DEUDOR_RECIENTE` activo.
- Propagar el dato existente desde los procedimientos de estadísticas de Firebird hasta el contrato JSON de `ElPrado.WebApi`, respetando los filtros de producto actuales.
- Mantener la semántica existente del campo en `EST_PARCELAS_CORTE`; el cambio no redefine ni recalcula su clasificación.

## Capabilities

### New Capabilities

_Ninguna._

### Modified Capabilities

- `estadisticas-parcelas-api`: Incluir el indicador agregado de deudores recientes en el resumen y la evolución de parcelas.

## Impact

- Firebird: salidas de `RESUMEN_ESTADISTICAS_PARCELAS` y `EVOLUCION_ESTADISTICAS_PARCELAS`.
- Backend: DTO interno de consulta, DTO de KPI, servicio de estadísticas y pruebas asociadas.
- API pública: se añade un campo no disruptivo a los KPI de los endpoints existentes.
