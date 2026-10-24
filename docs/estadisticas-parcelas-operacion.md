# Estadísticas de parcelas: reglas y operación

## Reglas B1

- **Al día:** `MESES_PENDIENTES <= 2`; comprende el mes de corte y un mes adicional.
- **Morosa:** `MESES_PENDIENTES >= 3`, contando el mes de corte.
- **Recuperada:** al inicio del mes debía tres o más cuotas, durante ese mes registró un pago y al corte quedó al día.

`MESES_PENDIENTES` toma el mayor conteo entre cuotas periódicas pendientes (`CONFIG_CUOTAS_PERIODICAS` + `CUOTAS_PERIODICAS`) y cuotas de créditos (`CREDITOS` + `CUOTAS`). Las cuotas anuladas, inactivas o rendidas al corte no se consideran pendientes.

La consulta de contraste usada para bajas debe probar una propuesta con deuda y otra sin deuda, evaluando las cuotas pendientes a `PROPUESTA.FECHA_BAJA`.

En el relevamiento del ERP de desarrollo se encontraron estos casos el 19/10/2026:

| Propuesta | Fecha de baja | Cuotas pendientes |
| --- | --- | --- |
| 1204 | 2026-02-11 | 1 periódica, 0 de crédito |
| 597116 | 2026-01-30 | 0 periódicas, 0 de crédito |

## Backfill y tarea programada

Ejecutar un período por corrida, en horario de baja carga:

```sql
EXECUTE PROCEDURE REFRESCAR_ESTADISTICAS_PARCELAS('2026-09-01', '2026-09-30', 1);
COMMIT;
```

Para Windows Task Scheduler, ejecutar `isql` con un archivo SQL que use el mes corriente, fecha de corte del día y `P_RECONSTRUIR = 0`. El job debe registrar duración, período y el conteo de `EST_CARGAS_PARCELAS`; no debe ejecutar más de un período histórico por corrida.

## Validación en desarrollo (20/10/2026)

Se reconstruyeron los períodos 2025-10 a 2026-09, un período por ejecución: 12 cargas exitosas y 122.716 snapshots. La recarga de 2026-09 conservó 10.234 parcelas distintas, sin duplicados. La evolución de doce meses devolvió 12 observaciones ordenadas; su latencia media en cinco ejecuciones fue 196 ms. El MCP no expone el plan de Firebird, por lo que el plan se debe revisar con `isql` antes de agregar otro índice.

La validación posterior del 20/10/2026 confirmó que el resumen de 2026-09 devuelve datos para `todos`, `dpc` y `flores-siempre`; 111 parcelas poseen ambas banderas y por lo tanto participan en ambos filtros. En cinco ejecuciones, el resumen midió entre 7 y 18 ms y la evolución de doce meses entre 60 y 200 ms, por debajo del umbral de 500 ms. La evaluación del plan continúa pendiente porque el driver expuesto por MCP no lo informa.

El 22/10/2026 se aplicó la migración incremental de auditoría de fallos. Un refresco controlado con fecha de corte inválida para 2024-01 produjo `ESTADO = 'FALLIDA'`, cero snapshots y cero eventos; por lo tanto el período no aparece en evolución ni cobertura. El análisis de índices no recomendó índices adicionales para la evolución de doce meses.

La comparación manual contra ERP para 2025-12 y 2026-09 coincidió con el resumen y la evolución materializados en cantidad de parcelas, al día y morosas. No se detectaron diferencias que requieran justificar por semántica de corte; no existe cubo legado para contrastar.

## API

Las rutas requieren JWT y sólo devuelven agregados:

```text
GET /api/estadisticas/parcelas/resumen?periodo=2026-09&producto=dpc
GET /api/estadisticas/parcelas/evolucion?desde=2025-10&hasta=2026-09&producto=flores-siempre
```

`producto` admite `todos`, `dpc` y `flores-siempre`; si se omite usa `todos`.
Un resumen sin carga responde HTTP 200 con `datosDisponibles: false` y `kpi: null`. La evolución devuelve sólo observaciones disponibles y enumera `periodosFaltantes`.

Para validar JWT, autorización, respuestas agregadas y el umbral de 500 ms, copiar `.env.example` como `.env.local`, completar las credenciales locales y ejecutar:

```powershell
powershell -NoProfile -ExecutionPolicy Bypass -File .\scripts\Test-EstadisticasApi.ps1
```

El archivo `.env.local` está ignorado por Git y el script no muestra sus valores.

El runner se ejecutó contra la API de desarrollo el 22/10/2026: validó JWT, rechazo 401 sin token, validación 400 de período y producto, filtros `dpc` y `flores-siempre`, período faltante y ausencia de identificadores individuales. El resumen tomó 20,6 ms y la evolución de doce meses 197,32 ms, ambos por debajo de 500 ms.
