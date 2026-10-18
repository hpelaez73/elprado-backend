# Roadmap — Estadísticas de cobranzas (backend)

**Rama:** `feature-estadisticas` · **Estado:** pendiente · **Objetivo:** servir indicadores gerenciales consistentes desde Firebird al frontend existente.

## Alcance y principios
Reutilizar ASP.NET Core Web API, Dapper, UnitOfWork/repositorios, JWT y permisos existentes; **sin servicio BI nuevo**. Priorizar fotografías mensuales precalculadas. Investigar primero los SP `INS_ESTADISTICAS_PARCELAS` y el procedimiento de consulta y las tablas relacionadas; verificar nombres, parámetros y semántica en código/DB antes de modificarlos.

## Fases (orden de implementación)
- [ ] **B0 — Inventario:** identificar SP, esquema, período de fotografía, periodicidad de carga, índices, permisos y patrones existentes de endpoints/repositorios. Verificar que los datos estén disponibles para 12 meses y filtros de producto.
- [ ] **B1 — Diccionario de métricas:** documentar fórmula, numerador/denominador, unidad y corte de activas, al día, morosas y recuperadas; estados no necesariamente excluyentes. Definir qué significa «recuperada» y evitar sumas dobles. Diferenciar variación porcentual de puntos porcentuales. Validar reglas con negocio.
- [ ] **B2 — Contrato JSON y endpoints de lectura:** `GET /api/estadisticas/cobranzas/resumen?periodo=YYYY-MM&producto=...` y `GET /api/estadisticas/cobranzas/evolucion?desde=YYYY-MM&hasta=YYYY-MM&producto=...`. Son rutas propuestas: ajustar a convenciones del proyecto. Incluir `periodoCorte`, `actualizadoEn`, `filtrosAplicados`, KPI con valor/unidad/comparación y series mensuales. Definir respuesta explícita para períodos sin datos.
- [ ] **B3 — Implementación:** consultas parametrizadas Dapper a estadísticas precalculadas, agregaciones en Firebird cuando corresponda; DTO tipados; validación de períodos/productos; autorización por permisos existentes; evitar SQL dinámico libre. No transferir decenas de miles de filas para graficar.
- [ ] **B4 — Calidad:** pruebas de fórmulas y estados (ACTIVA vs AL_DIA), períodos faltantes, cero denominador, comparaciones, seguridad, tiempos de respuesta y consistencia con consultas manuales.
- [ ] **B5 — Después de validar MVP:** endpoint de detalle con filtros, ordenamiento en lista permitida y paginación server-side; exportación CSV/Excel/PDF reutilizando infraestructura existente. Considerar caché local solo si las mediciones lo justifican.

## Decisiones de datos
- El «estado actual» del MVP es el **último cierre estadístico**, no el saldo vivo al momento de abrir la pantalla. Exponer siempre período y timestamp de actualización; si no existe timestamp confiable, declarar solo período y no inventar fecha.
- El backend es fuente de verdad de las reglas; el frontend no recalcula KPI desde registros individuales.
- Separar snapshots mensuales de transacciones ocurridas durante el mes; especificar denominadores de tasas.
- Revisar la vigencia de los snapshots antes de mostrar una tendencia; señalar faltantes, no completar con ceros falsos.

## Definición de terminado del MVP
Dos endpoints autenticados y documentados, respuestas agregadas estables para filtros y 12 meses, fórmulas validadas con negocio, fechas de corte claras y pruebas de errores/datos faltantes. No introducir un servicio nuevo ni cambios incompatibles en API existente.

**Dependencia:** [roadmap frontend](https://github.com/hpelaez73/elprado-frontend/blob/feature-estadisticas/docs/roadmap-estadisticas.md). Coordinar B1/B2 antes de conectar F2; prototipo F1 puede avanzar en paralelo.
