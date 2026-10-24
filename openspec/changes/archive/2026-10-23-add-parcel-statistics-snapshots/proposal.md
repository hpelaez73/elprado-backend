# Proposal

## Why

Las estadísticas de parcelas existentes quedaron inconclusas y mezclan en una misma fotografía mensual propuestas históricas y vigentes de una parcela. El backend necesita una fuente histórica confiable para indicadores por parcela, que preserve los eventos de propuestas y permita una lectura rápida desde la aplicación web.

## What Changes

- Incorporar un modelo de estadísticas con un snapshot al corte por parcela y un registro separado de eventos de propuestas, incluidas sus altas, bajas y exhumaciones. Los importes históricos de deuda quedan fuera de alcance por ahora.
- Definir procesos Firebird idempotentes para recalcular el mes en curso durante el batch nocturno y para reconstruir períodos históricos de manera controlada.
- Incorporar consultas agregadas y parametrizadas para resumen y evolución mensual por producto, sin persistir tarjetas ni tasas específicas del frontend.
- Exponer endpoints autenticados de estadísticas de parcelas en `ElPrado.WebApi`, con período de corte, frescura y una respuesta explícita ante datos faltantes.
- Cubrir el flujo con validaciones de reglas, períodos, filtros y rendimiento, y documentar la operación necesaria para completar los últimos doce meses.

## Capabilities

### New Capabilities

- `estadisticas-parcelas-snapshots`: Mantiene snapshots por parcela y eventos de propuestas con corte y meses pendientes verificables.
- `estadisticas-parcelas-api`: Expone indicadores agregados y evolución mensual de parcelas para consumidores autenticados de ElPrado.WebApi.

### Modified Capabilities

- Ninguna.

## Impact

- Firebird: nuevas tablas, restricciones, índices y stored procedures; el procedimiento legado `INS_ESTADISTICAS_PARCELAS` se reemplazará o migrará de forma controlada tras validar resultados.
- Backend: nuevos DTOs, repositorio, servicio, controlador y pruebas en `ElPrado.WebApi`, `ElPrado.Services`, `ElPrado.Data`, `ElPrado.Dto` y `ElPrado.Tests`.
- Operación: batch nocturno y backfill inicial de doce meses, con monitoreo de ejecución y de períodos faltantes.
- Frontend: consume una API agregada nueva; no recibe filas individuales de parcelas ni recalcula KPI.
