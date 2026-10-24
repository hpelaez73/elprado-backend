# Spec Delta

## Purpose

Conservar hechos mensuales de cobranza por parcela con un corte explícito y preservar los eventos históricos de propuestas sin duplicar la cartera vigente. Los eventos no incluyen importes monetarios de deuda en esta etapa.

## ADDED Requirements

### Requirement: Snapshot de cartera por parcela al corte

El sistema SHALL mantener para cada período y parcela un único snapshot de la propuesta vigente en la fecha de corte. Una propuesta será vigente cuando inició en o antes del corte y no tiene baja anterior o igual al corte; la parcela SHALL conservar los hechos de cobranza y producto correspondientes a esa propuesta.

#### Scenario: Reasignación de parcela dentro del mes
- **WHEN** una propuesta se da de baja y otra propuesta de la misma parcela se da de alta antes del mismo corte
- **THEN** el snapshot conserva una única fila para la parcela asociada a la propuesta vigente en dicho corte

#### Scenario: Parcela sin propuesta vigente al corte
- **WHEN** ninguna propuesta de una parcela está vigente en la fecha de corte
- **THEN** la parcela no integra la cartera vigente de ese período

### Requirement: Eventos históricos de propuestas por parcela

El sistema SHALL registrar los eventos de alta, baja y exhumación relevantes para una parcela y propuesta, incluyendo la fecha efectiva y los meses pendientes disponibles al momento del evento. Los eventos SHALL permanecer consultables aunque la propuesta ya no integre el snapshot vigente. Los importes de capital, interés y total pendientes quedan fuera de alcance.

#### Scenario: Baja con meses pendientes
- **WHEN** una propuesta se da de baja con meses pendientes
- **THEN** se registra una baja asociada a la parcela, la propuesta, su fecha efectiva y los meses pendientes calculados a ese momento

#### Scenario: Reejecución del proceso nocturno
- **WHEN** se recalcula un período que ya contiene un evento de la misma propuesta, tipo y fecha efectiva
- **THEN** el evento se actualiza o conserva sin duplicarse

### Requirement: Corte y recalculo mensual controlado

El sistema SHALL distinguir el período estadístico de la fecha de corte. El proceso de carga SHALL permitir recalcular el mes en curso y reconstruir explícitamente períodos históricos; los períodos cerrados no se modificarán por una ejecución ordinaria.

#### Scenario: Carga nocturna del mes en curso
- **WHEN** el batch se ejecuta durante el mes en curso
- **THEN** actualiza el snapshot de ese período con la fecha de corte de la ejecución y registra los eventos detectados

#### Scenario: Reconstrucción de un período cerrado
- **WHEN** un operador solicita una reconstrucción explícita de un mes histórico
- **THEN** el sistema recalcula únicamente el período solicitado sin modificar otros snapshots

### Requirement: Trazabilidad y cobertura de snapshots

Cada snapshot y ejecución de carga SHALL conservar el período, fecha de corte y momento lógico de actualización necesarios para determinar la frescura de los indicadores. La ausencia de un período SHALL poder distinguirse de un período con valores cero.

#### Scenario: Período sin carga disponible
- **WHEN** no existe un snapshot para un período solicitado
- **THEN** el sistema identifica el período como faltante y no lo representa como una cartera con cero parcelas
