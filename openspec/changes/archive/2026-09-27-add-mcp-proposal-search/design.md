## Context

La superficie v2 ya usa `PropuestasV2EndpointExtensions` como adaptador HTTP, `PropuestasService` para coordinar consultas de dominio y `PropuestasRepository` con Dapper/Firebird. Las rutas por propuesta resuelven el número público antes de consultar; este recurso es una colección y no puede usar esa resolución previa. Ver [proposal.md](proposal.md) y el delta de `mcp-proposal-v2-contracts` para la motivación y el contrato.

## Goals / Non-Goals

**Goals:**

- Consultar candidatas activas por los tres criterios de negocio sin reutilizar el filtro genérico de WebApi.
- Proyectar una página estable y compacta en contratos exclusivos de McpApi.
- Reutilizar el envelope, los límites de paginación y la autenticación que ya aplica al grupo v2.

**Non-Goals:**

- No agregar búsquedas por inhumados, beneficiarios, estados, importes ni datos históricos.
- No incluir propuestas dadas de baja, ni exponer `CodPropuesta` o reemplazar los endpoints de detalle.
- No modificar WebApi, Afip.WebApi ni stored procedures existentes si una consulta Dapper parametrizada cubre el contrato.

## Decisions

### Ruta literal y validación en el adaptador MCP

Registrar `/buscar` como ruta literal del grupo `/v2/propuestas` antes de la ruta paramétrica `/{propuesta}`. El endpoint normalizará espacios en los criterios, exigirá al menos uno, y reutilizará `page=1`, `pageSize=20` y máximo 100. Los errores de entrada se devolverán con `INVALID_REQUEST` antes de llamar al servicio.

Se elige una ruta GET con parámetros explícitos en vez del DTO de filtros de grilla de WebApi porque expone conceptos del dominio y produce documentación OpenAPI apta para agentes.

### Consulta de candidatos en dos etapas, parametrizada y determinista

Agregar al repositorio una operación específica que reciba los criterios y paginación. La primera consulta contará propuestas activas distintas; la segunda recuperará sólo la página, ordenada de forma determinista por número público de propuesta. Ambas aplicarán los filtros condicionales con parámetros Dapper: `nombre` sobre `PROPUESTAS_TITULARES` vigente y `CLIENTES`, `documento` sobre el documento del titular actual y `parcela` sobre `PARCELA.LEGAJO`.

Luego se obtendrán los titulares actuales de las propuestas de la página mediante una consulta por conjunto y se agruparán en la capa de servicio. Esto evita duplicados por cotitular, no requiere agregación específica de Firebird y evita el patrón N+1. Se prefiere esta alternativa a adaptar `ListadoTitulares`, ya que éste acepta estructuras genéricas de filtrado y devuelve una fila por titular, no una candidata compacta.

### Proyección separada para la búsqueda

Definir DTOs internos de búsqueda para el repositorio/servicio y records de contrato v2 para la respuesta paginada. El mapper de McpApi publicará el número de propuesta, tipo, estado, referencia de parcela nullable y titulares actuales con nombre/documento; descartará los códigos técnicos empleados para agrupar y consultar.

El estado se obtendrá de la fuente de propuesta/deuda existente y se tratará como dato proporcionado por el ERP, no como una clasificación inferida. Se usa una proyección nueva en lugar de reutilizar `PropuestaV2DetalleResponse` o `TitularV2`, porque estos contienen detalle, datos de contacto o identificadores que no son necesarios para desambiguar.

### Semántica de coincidencias y resultados vacíos

El servicio combinará criterios informados con AND. El nombre se comparará parcialmente sin distinguir mayúsculas/minúsculas; documento y parcela se compararán contra sus valores normalizados. La falta de coincidencias se representará con una página vacía exitosa; no se invocará el flujo `PROPOSAL_NOT_FOUND`, reservado para recursos individuales.

## Risks / Trade-offs

- [El `LIKE '%texto%'` sobre nombres puede ser costoso] → paginar, exigir un criterio y revisar el plan/índices de Firebird con datos reales antes del despliegue.
- [Una propuesta con varios titulares puede duplicarse] → contar y paginar por propuesta distinta, y cargar titulares de la página en una consulta separada.
- [Diferencias de formato en documentos históricos] → normalizar sólo formatos ya soportados por el dominio y comprobar los ejemplos del ERP; no deducir identidad entre registros.
- [La ruta paramétrica puede competir con `buscar`] → registrar y probar explícitamente la ruta literal antes de `/{propuesta}`.

## Migration Plan

1. Incorporar la consulta y DTOs internos, y verificar conteo, filtros combinados y exclusión de bajas contra datos del ERP.
2. Exponer contratos, endpoint y documentación OpenAPI bajo `/api/v2`; los recursos existentes no cambian.
3. Ejecutar pruebas de contrato y de endpoint autenticado, incluyendo resultado vacío e inválidos.
4. Desplegar junto a v2; el rollback consiste en retirar el registro de la nueva ruta, sin migración de datos ni cambios a v1.
