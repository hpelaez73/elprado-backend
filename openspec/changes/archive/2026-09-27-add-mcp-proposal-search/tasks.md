## 1. Contratos y acceso a datos

- [x] 1.1 Agregar los DTOs internos de criterios, candidatos y página de búsqueda en `ElPrado.Dto`, verificando que contengan los datos necesarios para agrupar titulares sin publicar identificadores internos.
- [x] 1.2 Incorporar en `ElPrado.Data\Repositories\PropuestasRepository` la consulta Firebird parametrizada de conteo y página de propuestas activas por `nombre`, `documento` y `parcela`, verificando con datos del ERP que una propuesta con varios titulares se cuenta una sola vez y las bajas se excluyen.
- [x] 1.3 Incorporar la carga por conjunto de titulares actuales de los candidatos paginados, verificando que cada propuesta devuelve sus titulares con nombre y documento y que no se ejecuta una consulta por candidato.

## 2. Servicio y contrato MCP v2

- [x] 2.1 Agregar en `ElPrado.Services\Services\PropuestasService` la operación de búsqueda que combina los criterios con AND, agrupa los titulares y calcula los metadatos de paginación; verificar resultados vacíos, combinados y páginas fuera del total.
- [x] 2.2 Definir en `ElPrado.McpApi\Contracts\PropuestasV2Contracts.cs` los records de respuesta compacta, item, parcela, titular y paginación de búsqueda; verificar serialización camelCase, nulos y colecciones vacías mediante pruebas de contrato.

## 3. Endpoint y verificación

- [x] 3.1 Registrar `GET /api/v2/propuestas/buscar` en `PropuestasV2EndpointExtensions` antes de `/{propuesta}`, normalizando criterios y validando presencia de al menos uno, `page >= 1` y `1 <= pageSize <= 100`; verificar HTTP 400 y `INVALID_REQUEST` para entradas inválidas.
- [x] 3.2 Mapear la página del servicio al envelope v2 sin `CodPropuesta`, detalle ni importes, y verificar con pruebas de endpoint autenticado los filtros por nombre, documento y parcela, la combinación AND, el resultado vacío HTTP 200 y la prioridad de la ruta literal.
- [x] 3.3 Actualizar la documentación Swagger/OpenAPI y `ElPrado.McpApi\README.md` con parámetros, defaults y contrato compacto; verificar que el endpoint aparece bajo Propuestas v2.
- [x] 3.4 Ejecutar `dotnet test ElPrado.Tests/ElPrado.Tests.csproj` y `openspec validate add-mcp-proposal-search --strict`, corrigiendo cualquier fallo de compilación, contrato o especificación.
