# mcp-proposal-v2-contracts Specification

## Purpose

Proporcionar a agentes y al servidor MCP consultas v2 de propuestas con hechos de negocio estructurados, sin exponer convenciones ni DTO de la fachada web.

## Requirements

### Requirement: Superficie v2 autenticada y compatible
ElPrado.McpApi SHALL exponer, con JWT de empleado vÃ¡lido, los recursos `GET /api/v2/propuestas/{propuesta}`, `/titulares`, `/deuda`, `/servicios`, `/contratos`, `/comprobantes` e `/historial-titulares`. La ruta SHALL usar el nÃºmero pÃºblico `propuesta`; la API SHALL resolver internamente cualquier `CodPropuesta` necesario. Las rutas heredadas bajo `/api/propuestas` SHALL NOT estar disponibles.

#### Scenario: Consulta v2 autenticada por nÃºmero pÃºblico
- **WHEN** un consumidor autenticado solicita un recurso v2 con una propuesta existente
- **THEN** recibe el recurso solicitado sin requerir conocer el identificador interno de la propuesta

#### Scenario: Consulta v2 sin autenticaciÃ³n
- **WHEN** un consumidor solicita un recurso v2 sin un JWT vÃ¡lido
- **THEN** la API responde HTTP 401 sin ejecutar la consulta de negocio

#### Scenario: Ruta heredada retirada
- **WHEN** un consumidor invoca una ruta de propuestas bajo /api/propuestas`r
- **THEN** la API no publica esa operación heredada

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

### Requirement: Envelope y convenciones de datos v2
Toda respuesta v2 SHALL usar el envelope `{ succeeded, data, error }`. Las respuestas exitosas SHALL contener `succeeded: true`, datos no nulos y `error: null`; los errores SHALL contener `succeeded: false`, `data: null` y un `error.code` estable. Fechas SHALL serializarse en ISO 8601, importes monetarios SHALL conservar precisiÃ³n decimal, campos individuales inexistentes SHALL ser `null` y colecciones sin elementos SHALL ser `[]`.

#### Scenario: Propuesta inexistente
- **WHEN** se consulta cualquier recurso v2 de una propuesta que no existe o no estÃ¡ disponible
- **THEN** la API responde HTTP 404 con el envelope de error y el cÃ³digo `PROPOSAL_NOT_FOUND`

#### Scenario: ParÃ¡metros invÃ¡lidos
- **WHEN** una ruta v2 recibe un nÃºmero de propuesta o parÃ¡metros de consulta invÃ¡lidos
- **THEN** la API responde HTTP 400 con el envelope de error y el cÃ³digo `INVALID_REQUEST`

### Requirement: JSON canÃ³nicos por endpoint
Los JSON de respuesta de `docs/mcp-api-v2-propuestas.md`, secciÃ³n 3 y secciones 5 a 11, son el contrato canÃ³nico y vinculante de v2. Cada recurso SHALL preservar exactamente los nombres de propiedades, su anidamiento, cardinalidad, nulabilidad y semÃ¡ntica allÃ­ definidos; los ejemplos no son ilustrativos ni podrÃ¡n sustituirse por DTO existentes. Si la fuente actual no proporciona un campo o hecho necesario, el sistema SHALL extender Services, DTOs, repositorios o consultas Firebird para obtenerlo desde una fuente autoritativa.

El contrato canÃ³nico exige como mÃ­nimo la siguiente forma de `data`:

- Detalle (`GET /{propuesta}`): `propuesta`, `codigo`, `tipo`, `fechaAlta`, `fechaBaja`, `estado`, `deuda.estado`, `parcela` (`codigo`, `numero`, `manzana`, `estado`, `zonas`, `lugares`), `inhumados`, `propuestasAsociadas` y `alertas`.
- Titulares (`GET /{propuesta}/titulares`): `propuesta`, `cantidad` y `titulares[]` con `codigo`, `orden`, `esPrincipal`, `nombre`, `documento`, `contacto`, `domicilio`, `fechaNacimiento` y `fechaAlta`.
- Deuda (`GET /{propuesta}/deuda`): `propuesta`, `estado`, `totales` y `cuentas[]`, donde cada cuenta contiene `codigo`, `tipo`, `categoria`, `estado`, `cliente`, `importe`, `periodo`, `importes` y `cobranza`.
- Servicios (`GET /{propuesta}/servicios`): `propuesta`, `habilitaciones[]`, `cupos[]` y `utilizaciones[]`, con motivo, beneficiario, comprobante y propuestas origen/aplicaciÃ³n cuando corresponda.
- Contratos (`GET /{propuesta}/contratos`): `propuesta`, `contratos[]` y `facturacion.titularesHabilitados[]`; cuando no exista vÃ­nculo verificable contrato-plan, se aplicarÃ¡ la variante de colecciones separadas establecida en la secciÃ³n 9 del documento canÃ³nico.
- Comprobantes (`GET /{propuesta}/comprobantes`): `propuesta`, `items[]` y `pagination`, con filtros opcionales `desde`, `hasta`, `tipo`, `estado`, `page` y `pageSize` definidos en la secciÃ³n 10.
- Historial (`GET /{propuesta}/historial-titulares`): `propuesta` e `historial[]` con `cliente`, `fechaAlta`, `fechaBaja`, `usuarioAlta` y `usuarioBaja`.

#### Scenario: Respuesta conforme al JSON diseÃ±ado
- **WHEN** un consumidor consulta cualquiera de los siete recursos v2 con datos disponibles
- **THEN** el cuerpo `data` coincide con el JSON canÃ³nico de ese endpoint, incluidos los campos nulos, las colecciones y la estructura anidada

#### Scenario: Dato canÃ³nico ausente en el origen actual
- **WHEN** un DTO, servicio o repositorio existente no provee un campo o hecho requerido por el JSON canÃ³nico
- **THEN** se amplÃ­a la capa de dominio o persistencia para obtenerlo de una fuente autoritativa y McpApi no lo inventa, omite ni deriva de texto, color o posiciÃ³n

### Requirement: Detalle semÃ¡ntico de propuesta y parcela
El recurso de detalle SHALL implementar el JSON canÃ³nico de la secciÃ³n 5, incluidos `estado`, `deuda.estado`, `parcela.estado`, `parcela.zonas`, `parcela.lugares`, `inhumados`, `propuestasAsociadas` y `alertas`. Los estados configurables de deuda, parcela y lugar SHALL incluir su nombre configurado y todos los flags funcionales definidos en dicho JSON. Lugares SHALL ser una colecciÃ³n normalizada e incluirÃ¡n inhumado Ãºnicamente cuando exista una relaciÃ³n inequÃ­voca. El contrato SHALL excluir colores y datos de presentaciÃ³n.

#### Scenario: Parcela con lugares configurables
- **WHEN** una propuesta tiene parcela y lugares con estados configurables
- **THEN** el detalle devuelve cada lugar como elemento de colecciÃ³n y permite decidir venta o inhumaciÃ³n mediante flags funcionales, no mediante el texto del estado

#### Scenario: Lugar sin vÃ­nculo inequÃ­voco al inhumado
- **WHEN** el origen sÃ³lo provee el nombre de una persona para un lugar y no su relaciÃ³n identificable
- **THEN** el lugar devuelve `inhumado: null` y la API no infiere la relaciÃ³n por nombre

### Requirement: Titulares actuales e historial factual
Los recursos de titulares e historial SHALL implementar respectivamente los JSON canÃ³nicos de las secciones 6 y 11. Titulares SHALL incluir la cantidad, orden, indicador principal cuando sea una regla oficial del dominio, identificaciÃ³n, contacto, domicilio y fechas; historial SHALL incluir cliente, altas y bajas registradas y usuarios asociados. Ambos SHALL omitir campos tÃ©cnicos o visuales sin semÃ¡ntica para el agente y SHALL no inferir transferencias ni reemplazos de titulares.

#### Scenario: Titular principal actual
- **WHEN** el dominio identifica el orden de los titulares actuales
- **THEN** la respuesta indica cuÃ¡l es principal conforme a la regla oficial y preserva el orden

#### Scenario: Historial con fechas prÃ³ximas
- **WHEN** dos hechos de titularidad tienen fechas consecutivas
- **THEN** el historial expone ambos hechos registrados sin declarar una transferencia no proporcionada por el ERP

### Requirement: SituaciÃ³n de deuda autorizada por el ERP
El recurso de deuda SHALL implementar el JSON canÃ³nico de la secciÃ³n 7, incluidos todos los campos de `totales`, `cuentas[].periodo`, `cuentas[].importes` y `cuentas[].cobranza`. Cada estado SHALL incluir `nombre`, `activa`, `alDia` y `permiteImputar`; cada cuenta SHALL incluir importes, perÃ­odo, cliente y datos de cobranza aplicables. La API SHALL usar los estados, flags e importes calculados por el ERP y SHALL NOT recalcular mora, intereses, condiciÃ³n al dÃ­a ni cobrabilidad normal.

#### Scenario: Cuenta judicial activa no imputable
- **WHEN** el ERP informa una cuenta activa, no al dÃ­a y no imputable por los canales normales
- **THEN** la respuesta conserva los tres flags con esos valores sin deducirlos del nombre del estado

### Requirement: Servicios por habilitaciÃ³n, cupo y utilizaciÃ³n
El recurso de servicios SHALL implementar el JSON canÃ³nico de la secciÃ³n 8, incluidas las tres colecciones independientes y todos sus objetos anidados. Una habilitaciÃ³n no disponible SHALL incluir `motivo.codigo`, `motivo.descripcion` y `motivo.hasta` con la normalizaciÃ³n establecida; combinaciones que no forman parte de un producto SHALL omitirse. Los cupos SHALL expresar `total`, `utilizados` y `disponibles`. Cada utilizaciÃ³n SHALL identificar beneficiario, comprobante y propuesta de origen/aplicaciÃ³n, usando `null` cuando una relaciÃ³n no exista.

#### Scenario: Servicio ausente del producto
- **WHEN** el dominio indica que un servicio no estÃ¡ incluido en un producto
- **THEN** la respuesta no publica una habilitaciÃ³n deshabilitada para esa combinaciÃ³n

#### Scenario: Beneficio aplicado a otra propuesta
- **WHEN** una utilizaciÃ³n consume un beneficio de una propuesta y se aplica en otra
- **THEN** la respuesta identifica ambas como `propuestaOrigen` y `propuestaAplicacion`

### Requirement: Contratos y planes sin asociaciones inferidas
El recurso de contratos SHALL implementar el JSON canÃ³nico de la secciÃ³n 9, incluidos contrato, vendedor, total, plan de venta y `facturacion.titularesHabilitados`. Un plan de venta SHALL anidarse en un contrato sÃ³lo cuando el dominio provea una clave o relaciÃ³n inequÃ­voca; de lo contrario, los contratos y planes de venta SHALL devolverse como colecciones independientes segÃºn la variante canÃ³nica.

#### Scenario: Plan sin relaciÃ³n verificable
- **WHEN** el dominio devuelve contratos y planes de venta sin una relaciÃ³n inequÃ­voca entre ellos
- **THEN** la respuesta conserva ambas colecciones separadas y no las asocia por Ã­ndice

### Requirement: Comprobantes filtrables y paginados por conceptos de dominio
El recurso de comprobantes SHALL implementar el JSON canÃ³nico de la secciÃ³n 10 y admitir opcionalmente `desde`, `hasta`, `tipo`, `estado`, `page` y `pageSize`, con sus defaults documentados. La respuesta SHALL devolver todos los campos de `items[]` y metadatos `page`, `pageSize`, `totalItems`, `totalPages`, `hasNext` y `hasPrevious`. El contrato SHALL exponer anulaciones como booleano y SHALL NOT exponer la estructura genÃ©rica de filtros del frontend ni identificadores internos que no habiliten una operaciÃ³n posterior.

#### Scenario: Consulta paginada y filtrada
- **WHEN** un consumidor solicita comprobantes con rango de fechas, tipo, estado y paginaciÃ³n vÃ¡lidos
- **THEN** recibe sÃ³lo los comprobantes coincidentes y metadatos consistentes con la pÃ¡gina solicitada

#### Scenario: Filtro de comprobantes invÃ¡lido
- **WHEN** un consumidor envÃ­a una fecha invÃ¡lida, un rango inconsistente o valores de paginaciÃ³n fuera de los lÃ­mites documentados
- **THEN** la API responde `INVALID_REQUEST` sin traducir esos valores al mecanismo interno de filtros del frontend


