# Spec Delta

## MODIFIED Requirements

### Requirement: Métricas calculadas en servidor sobre parcelas

Los KPI y las series SHALL contar parcelas del snapshot, no propuestas ni filas de eventos. Las tasas, comparaciones y variaciones SHALL ser calculadas en el backend a partir de los hechos del snapshot, con manejo explícito de denominadores cero. Para cada período disponible, los KPI SHALL incluir la cantidad de parcelas cuyo indicador `ES_DEUDOR_RECIENTE` esté activo, respetando el filtro de producto aplicado.

#### Scenario: Propuesta dada de baja durante el período

- **WHEN** existe un evento de baja de propuesta en el mes pero la parcela tiene otra propuesta vigente al corte
- **THEN** la propuesta dada de baja no duplica los KPI de cartera vigente

#### Scenario: Tasa sin denominador

- **WHEN** una tasa requiere un denominador cuyo valor es cero
- **THEN** la respuesta expresa la tasa como no disponible y no genera una división inválida

#### Scenario: Deudores recientes en resumen y evolución

- **WHEN** un empleado autenticado consulta un período o una evolución disponibles con un producto válido
- **THEN** cada KPI devuelto incluye la cantidad de parcelas del snapshot filtrado cuyo `ES_DEUDOR_RECIENTE` está activo
