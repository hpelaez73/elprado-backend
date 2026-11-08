# Design

## Context

Ver `proposal.md`. `EST_PARCELAS_CORTE` ya persiste `ES_DEUDOR_RECIENTE`, pero los procedimientos de resumen y evolución devuelven únicamente los conteos de al día, morosas y recuperadas. La proyección C# y el contrato de KPI reflejan exactamente esas salidas actuales.

## Goals / Non-Goals

**Goals:**

- Propagar el conteo filtrado de `ES_DEUDOR_RECIENTE` desde Firebird hasta los KPI de resumen y evolución.
- Conservar las rutas, parámetros, autorización y comportamiento de cobertura existentes.

**Non-Goals:**

- No modificar la definición, cálculo ni persistencia del flag en el proceso de carga.
- No publicar el estado individual de una parcela ni crear endpoints nuevos.

## Decisions

### Agregación en los procedimientos existentes

Se añadirá un valor agregado a las salidas de `RESUMEN_ESTADISTICAS_PARCELAS` y `EVOLUCION_ESTADISTICAS_PARCELAS`, calculado sobre el mismo `LEFT JOIN` y predicado de producto usado por los demás KPI. Esto asegura que el nuevo conteo use el mismo universo de parcelas que el resto de la respuesta.

Alternativa considerada: consultar `EST_PARCELAS_CORTE` por separado desde el servicio. Se descarta porque duplicaría la lógica de filtros y podría producir resultados inconsistentes con los otros indicadores.

### Proyección tipada extremo a extremo

La columna de los procedimientos se incorporará al DTO de transporte y se mapeará a una nueva propiedad del objeto de KPI, que será usada tanto por el resumen como por cada observación de evolución. La adición es compatible para consumidores JSON existentes, al sumar un campo sin cambiar los actuales.

Alternativa considerada: exponer el nombre de columna de base de datos en el JSON. Se descarta para mantener el contrato de KPI desacoplado de la nomenclatura de Firebird.

## Risks / Trade-offs

- [Desalineación entre la columna SQL y el DTO de Dapper] → cubrir resumen y evolución con pruebas de servicio que incluyan un valor de deudores recientes.
- [Filtro de producto aplicado de forma distinta al nuevo KPI] → reutilizar la misma consulta y condición de join ya usada para los demás conteos.

## Migration Plan

1. Aplicar una migración versionada que reemplace las dos salidas de procedimientos con la columna agregada.
2. Desplegar el backend con los DTO y el mapeo del nuevo KPI.
3. Verificar ambos endpoints con y sin filtro de producto para un período cargado.
4. Para revertir, desplegar la versión previa del backend y restaurar las definiciones previas de los procedimientos mediante una migración compensatoria si fuera necesario.
