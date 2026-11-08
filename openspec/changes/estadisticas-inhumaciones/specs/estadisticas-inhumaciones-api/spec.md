# Spec Delta

## Purpose

Exponer al dashboard de gerencia estadísticas agregadas de inhumaciones mediante contratos autenticados, sin entregar movimientos ni identificadores individuales.

## ADDED Requirements

### Requirement: Resumen autenticado de inhumaciones por rango

ElPrado.WebApi SHALL exponer `GET /api/estadisticas/inhumaciones/resumen` para empleados autenticados. La operación SHALL aceptar `desde` y `hasta` con formato `YYYY-MM` y devolver primeras inhumaciones, exhumaciones definitivas, variación neta y fallecidos actualmente inhumados para el rango solicitado.

#### Scenario: Resumen de período válido

- **WHEN** un empleado autenticado solicita un rango mensual válido
- **THEN** recibe los cuatro indicadores agregados y los límites normalizados del rango

#### Scenario: Solicitud sin autenticación

- **WHEN** un consumidor solicita el resumen sin un JWT válido
- **THEN** la API responde HTTP 401 sin ejecutar consultas estadísticas

### Requirement: Evolución mensual autenticada de inhumaciones

ElPrado.WebApi SHALL exponer `GET /api/estadisticas/inhumaciones/evolucion` para empleados autenticados. La operación SHALL aceptar `desde` y `hasta` con formato `YYYY-MM` y devolver observaciones ordenadas por mes con las cantidades agregadas de primeras inhumaciones y exhumaciones definitivas.

#### Scenario: Evolución con meses sin movimientos

- **WHEN** el rango contiene un mes sin primeras inhumaciones ni exhumaciones definitivas
- **THEN** la respuesta incluye ese mes con ambos valores en cero

#### Scenario: Comparación con año anterior

- **WHEN** el consumidor consulta un rango y el rango homólogo del año anterior
- **THEN** ambas series contienen observaciones mensuales comparables sin identificadores individuales

### Requirement: Validación de período estadístico

Las operaciones SHALL rechazar con HTTP 400 los valores `desde` o `hasta` que no respeten `YYYY-MM`, los rangos invertidos y cualquier rango que exceda el límite de consulta definido por la implementación. La respuesta de error SHALL identificar que el período es inválido.

#### Scenario: Rango invertido

- **WHEN** `desde` es posterior a `hasta`
- **THEN** la API responde HTTP 400 sin consultar datos

### Requirement: Respuesta sin detalle individual

Las respuestas de estadísticas SHALL contener únicamente períodos, indicadores agregados y filtros aplicados. No SHALL incluir `COD_INHUMADO`, `COD_DET_INHUMADO`, ubicación, parcela, receptáculo ni datos personales de fallecidos.

#### Scenario: Consumo para dashboard

- **WHEN** el dashboard consulta resumen y evolución
- **THEN** puede mostrar KPI, evolución y comparación interanual sin requerir una consulta de detalle
