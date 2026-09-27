## ADDED Requirements

### Requirement: Búsqueda compacta de propuestas por criterios de dominio
ElPrado.McpApi SHALL exponer, con JWT de empleado válido, `GET /api/v2/propuestas/buscar` para localizar candidatas activas cuando no se conoce el número público. La operación SHALL aceptar los parámetros opcionales `nombre`, `documento` y `parcela`, y los parámetros de paginación `page` y `pageSize`; deberá recibirse al menos un criterio no vacío. Si se suministran varios criterios, la propuesta SHALL coincidir con todos ellos. `nombre` SHALL buscar titulares actuales por coincidencia parcial sin distinguir mayúsculas/minúsculas; `documento` y `parcela` SHALL cotejarse contra sus números normalizados, sin requerir que el consumidor conozca identificadores internos.

La respuesta exitosa SHALL conservar el envelope v2 y devolver `data` con `items`, `pagination` (`page`, `pageSize`, `totalItems`, `totalPages`, `hasNext`, `hasPrevious`). Cada elemento SHALL incluir únicamente información de desambiguación: `propuesta`, `tipo`, `estado`, `parcela` (o `null` si no corresponde) y `titulares` actuales con `nombre` y `documento` cuando exista. SHALL excluir el detalle de propuesta, importes, relaciones y códigos internos. Las colecciones sin resultados SHALL ser `[]` y una búsqueda sin coincidencias SHALL responder exitosamente, no como propuesta inexistente. Los valores por defecto SHALL ser `page=1` y `pageSize=20`, y `pageSize` SHALL admitir como máximo 100.

#### Scenario: Búsqueda por nombre de titular
- **WHEN** un consumidor autenticado solicita `/api/v2/propuestas/buscar?nombre=asturzzi`
- **THEN** recibe una página de propuestas activas cuyos titulares actuales coinciden parcialmente con el nombre, con la información compacta necesaria para desambiguarlas

#### Scenario: Búsqueda sin autenticación
- **WHEN** un consumidor solicita la búsqueda sin un JWT válido
- **THEN** la API responde HTTP 401 sin ejecutar la consulta de negocio

#### Scenario: Búsqueda por documento y parcela
- **WHEN** un consumidor autenticado solicita la búsqueda con `documento`, `parcela` o ambos
- **THEN** recibe sólo propuestas activas que cumplen cada criterio indicado y no necesita proporcionar `CodPropuesta`

#### Scenario: Búsqueda sin coincidencias
- **WHEN** los criterios válidos no identifican ninguna propuesta activa
- **THEN** la API responde HTTP 200 con el envelope exitoso, `items: []` y metadatos de paginación consistentes

#### Scenario: Criterio o paginación inválidos
- **WHEN** el consumidor no envía ningún criterio no vacío, usa parámetros de paginación inválidos o solicita `pageSize` mayor que 100
- **THEN** la API responde HTTP 400 con el envelope de error y el código `INVALID_REQUEST`
