# estadisticas-parcelas-api Specification

## Purpose

Exponer al frontend indicadores consistentes de cobranzas por parcela a partir de snapshots históricos, sin transferir registros individuales ni recalcular reglas de deuda en el cliente.

## Requirements

### Requirement: Resumen autenticado de cobranzas por período

ElPrado.WebApi SHALL exponer `GET /api/estadisticas/parcelas/resumen` para empleados autenticados. La operación SHALL aceptar `periodo` con formato `YYYY-MM` y `producto` opcional, y devolver indicadores agregados por parcela junto con período de corte, actualización disponible y filtros aplicados.

#### Scenario: Resumen disponible

- **WHEN** un empleado autenticado solicita un período cargado con un producto válido
- **THEN** recibe los KPI agregados exclusivamente desde el snapshot de parcelas de ese período y sus filtros normalizados

#### Scenario: Solicitud sin autenticación

- **WHEN** un consumidor solicita el resumen sin un JWT válido
- **THEN** la API responde HTTP 401 sin ejecutar la consulta de estadísticas

### Requirement: Evolución mensual autenticada de cobranzas

ElPrado.WebApi SHALL exponer `GET /api/estadisticas/parcelas/evolucion` para empleados autenticados. La operación SHALL aceptar `desde`, `hasta` y `producto` opcional, todos los períodos en formato `YYYY-MM`, y devolver una serie mensual agregada por parcela dentro del rango solicitado.

#### Scenario: Evolución de doce meses

- **WHEN** un empleado autenticado solicita doce períodos consecutivos disponibles
- **THEN** recibe una serie ordenada cronológicamente con una observación agregada por cada período

#### Scenario: Rango de períodos inválido

- **WHEN** `desde` o `hasta` no tiene formato válido o `desde` es posterior a `hasta`
- **THEN** la API responde HTTP 400 sin consultar datos estadísticos

### Requirement: Filtros de producto permitidos

Las operaciones de estadísticas SHALL admitir los productos `todos`, `dpc` y `flores-siempre`; la ausencia de `producto` SHALL equivaler a `todos`. Una parcela que tenga ambos productos SHALL participar en cada filtro específico que corresponda, sin que los productos se traten como categorías excluyentes.

#### Scenario: Parcela con ambos productos

- **WHEN** una parcela posee DPC y Flores Siempre en el snapshot solicitado
- **THEN** integra el resultado del filtro `dpc` y también el de `flores-siempre`

#### Scenario: Producto desconocido

- **WHEN** el consumidor envía un valor de producto distinto de los permitidos
- **THEN** la API responde HTTP 400 e identifica el filtro como inválido

### Requirement: Períodos faltantes explícitos

Las operaciones SHALL distinguir un período faltante de un valor estadístico cero. El resumen de un período sin snapshot SHALL responder exitosamente con disponibilidad falsa; la evolución SHALL informar los períodos faltantes solicitados y no inventará observaciones con cero.

#### Scenario: Resumen sin snapshot

- **WHEN** el período solicitado no fue cargado
- **THEN** la respuesta indica que los datos no están disponibles y no publica KPI calculados

#### Scenario: Hueco dentro de una evolución

- **WHEN** falta uno o más snapshots dentro del rango solicitado
- **THEN** la respuesta lista los períodos faltantes y conserva sólo las observaciones realmente disponibles

### Requirement: Métricas calculadas en servidor sobre parcelas

Los KPI y las series SHALL contar parcelas del snapshot, no propuestas ni filas de eventos. Las tasas, comparaciones y variaciones SHALL ser calculadas en el backend a partir de los hechos del snapshot, con manejo explícito de denominadores cero.

#### Scenario: Propuesta dada de baja durante el período

- **WHEN** existe un evento de baja de propuesta en el mes pero la parcela tiene otra propuesta vigente al corte
- **THEN** la propuesta dada de baja no duplica los KPI de cartera vigente

#### Scenario: Tasa sin denominador

- **WHEN** una tasa requiere un denominador cuyo valor es cero
- **THEN** la respuesta expresa la tasa como no disponible y no genera una división inválida

### Requirement: Clasificación B1 de estado de cobranza

Las métricas SHALL clasificar cada parcela de cartera vigente por `MESES_PENDIENTES` respecto del mes de corte: una parcela está al día si debe como máximo el mes en curso y un mes adicional; está morosa si debe tres meses o más contando el mes en curso. Una parcela recuperada SHALL ser aquella que, habiendo estado morosa, durante el mes de corte pagó hasta el mes en curso o un período posterior. La clasificación SHALL ser calculada en el servidor y no incluirá importes monetarios de deuda.

#### Scenario: Parcela al día

- **WHEN** una parcela vigente debe como máximo el mes de corte y un mes posterior
- **THEN** integra el indicador de parcelas al día y no el de morosas

#### Scenario: Parcela morosa recuperada

- **WHEN** una parcela que estaba morosa paga durante el mes de corte hasta ese mes o uno posterior
- **THEN** integra el indicador de recuperadas de dicho período
