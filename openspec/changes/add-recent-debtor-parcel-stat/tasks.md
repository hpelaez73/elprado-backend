# Tasks

## 1. Agregación en Firebird

- [x] 1.1 Crear una migración versionada que extienda `RESUMEN_ESTADISTICAS_PARCELAS` y `EVOLUCION_ESTADISTICAS_PARCELAS` con el conteo de `ES_DEUDOR_RECIENTE`, usando el filtro de producto existente; verificar ambas salidas para `todos`, `dpc` y `flores-siempre`.

## 2. Contrato y proyección del backend

- [x] 2.1 Extender `DtoEstadisticasFila` y `DtoKpiParcelas` con la cantidad de deudores recientes; verificar que Dapper materializa la nueva columna desde ambos procedimientos.
- [x] 2.2 Mapear el nuevo valor en `EstadisticasService.CrearKpi`; verificar que aparece en las respuestas de resumen y en cada observación de evolución sin alterar los campos existentes.

## 3. Pruebas

- [x] 3.1 Actualizar `EstadisticasServiceTests` con datos de deudores recientes y verificar que `dotnet test ElPrado.Tests` pasa, cubriendo resumen, evolución y filtros de producto.
