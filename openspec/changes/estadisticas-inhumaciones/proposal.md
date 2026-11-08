# Propuesta

## Motivo

La gerencia no dispone de una vista agregada y consistente para seguir las primeras inhumaciones, las exhumaciones definitivas y la población actualmente inhumada. Los movimientos operativos de un mismo fallecido pueden incluir traslados y cambios de ataúd, por lo que contar filas de detalle produciría indicadores duplicados o engañosos.

## Cambios propuestos

- Incorporar estadísticas de inhumaciones calculadas desde `DET_INHUMADOS`, agrupadas por fallecido y sin utilizar `INHUMACION` ni `DET_INHUMADOS_LUGARES`.
- Definir procedimientos Firebird de resumen por período y evolución mensual que expongan primeras inhumaciones, exhumaciones definitivas, variación neta y fallecidos actualmente inhumados, sin persistir snapshots ni ejecutar procesos programados.
- Exponer consultas autenticadas y agregadas en `ElPrado.WebApi`, siguiendo las capas, DTOs y validaciones del módulo de estadísticas de parcelas.
- Diseñar el consumo de un dashboard Next.js de gerencia, con filtros de período, cuatro KPI, evolución mensual y comparación interanual, reutilizando Ant Design, Recharts y los patrones de estadísticas de parcelas.
- Documentar pruebas de clasificación, no duplicación, meses sin movimientos y empates de fecha/hora; el desempate no se implementará hasta validar el criterio disponible en el esquema y los datos.

## Capacidades

### Nuevas capacidades

- `estadisticas-inhumaciones`: Define la clasificación agregada por fallecido de primeras inhumaciones, exhumaciones definitivas y estado actualmente inhumado.
- `estadisticas-inhumaciones-api`: Expone a consumidores autenticados el resumen y la evolución mensual agregados de inhumaciones.

### Capacidades modificadas

- Ninguna.

## Impacto

- Firebird: dos procedimientos de lectura sobre `DET_INHUMADOS` y, sólo si el análisis de plan lo justifica, índices compatibles con las consultas agregadas.
- Backend: DTOs, repositorio Dapper, servicio, registro en `IUnitOfWork`/`UnitOfWork`, controlador y pruebas en los proyectos existentes de ElPrado.
- Frontend: una página protegida, servicios y componentes bajo el patrón de `EstadisticasParcelas`; no se crean dependencias nuevas ni se reciben detalles individuales.
- No modifica tablas operativas, el módulo de estadísticas de parcelas ni funcionalidades operativas de inhumación o exhumación.
