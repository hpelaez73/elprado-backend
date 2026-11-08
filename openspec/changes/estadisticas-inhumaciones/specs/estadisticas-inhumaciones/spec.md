# Spec Delta

## Purpose

Proporcionar indicadores agregados y reproducibles de inhumaciones para gerencia, sin exponer movimientos ni datos personales individuales.

## ADDED Requirements

### Requirement: Primera inhumación única por fallecido

El sistema SHALL clasificar a cada fallecido por su primera fecha válida de inhumación, definida como el mínimo de `FECHA_INHUMACION` entre sus movimientos de `DET_INHUMADOS`. Un fallecido SHALL aportar como máximo una primera inhumación, incluso si luego fue trasladado, exhumado o reinhumado.

#### Scenario: Reinhumación posterior

- **WHEN** un fallecido tiene dos o más movimientos de inhumación
- **THEN** sólo integra el mes de su fecha mínima de inhumación

#### Scenario: Movimiento sin fecha válida

- **WHEN** un fallecido no tiene una fecha de inhumación válida en sus movimientos
- **THEN** no integra el indicador de primeras inhumaciones

### Requirement: Exhumación definitiva por último estado temporal

El sistema SHALL contar una exhumación definitiva sólo cuando la última fecha válida de exhumación de un fallecido es posterior a su última fecha válida de inhumación. Las exhumaciones con una inhumación posterior SHALL considerarse movimientos intermedios y no integrarán el indicador.

#### Scenario: Exhumación seguida de reinhumación

- **WHEN** un fallecido es exhumado y existe una fecha de inhumación posterior
- **THEN** la exhumación no se cuenta como definitiva

#### Scenario: Última acción de exhumación

- **WHEN** la última fecha de exhumación es posterior a la última fecha de inhumación
- **THEN** el fallecido integra una única exhumación definitiva en el mes de esa exhumación

### Requirement: Estado actual de fallecido inhumado

El sistema SHALL informar como actualmente inhumado a cada fallecido con primera inhumación válida cuya última acción no sea una exhumación definitiva. El valor SHALL reflejar el historial disponible al momento de consultar y no un snapshot histórico.

#### Scenario: Nueva inhumación luego de exhumación definitiva previa

- **WHEN** se registra una inhumación posterior para un fallecido antes considerado exhumado definitivamente
- **THEN** deja de integrar exhumaciones definitivas y vuelve a integrar el total actual de fallecidos inhumados

### Requirement: Agregación sin duplicación de movimientos ni ubicaciones

Los indicadores SHALL agrupar primero por `COD_INHUMADO` y no SHALL usar `INHUMACION` ni `DET_INHUMADOS_LUGARES` como fuentes de conteo. Los cambios de parcela, receptáculo o ubicación física no SHALL crear observaciones adicionales para un mismo fallecido.

#### Scenario: Traslado con varias ubicaciones físicas

- **WHEN** un movimiento posee varias filas de ubicación física o el fallecido fue trasladado entre parcelas
- **THEN** los indicadores conservan un único aporte del fallecido según sus fechas agregadas

### Requirement: Empates de fecha y hora explícitamente verificables

El sistema SHALL conservar identificables los fallecidos cuyo último registro de inhumación y de exhumación empate exactamente en fecha y hora, sin inferir un orden a partir de `COD_DET_INHUMADO`. Su clasificación definitiva SHALL depender de un criterio de orden validado antes de habilitarla.

#### Scenario: Inhumación y exhumación con la misma marca temporal

- **WHEN** las últimas fechas válidas de inhumación y exhumación de un fallecido son iguales
- **THEN** la consulta no atribuye un orden cronológico por el identificador de detalle y deja el caso sujeto al criterio de desempate validado
