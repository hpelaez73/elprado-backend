## Purpose

Proporcionar a agentes y al servidor MCP consultas v2 de propuestas con hechos de negocio estructurados, sin exponer convenciones ni DTO de la fachada web.

## ADDED Requirements

### Requirement: Superficie v2 autenticada y compatible
ElPrado.McpApi SHALL exponer, con JWT de empleado válido, los recursos `GET /api/v2/propuestas/{propuesta}`, `/titulares`, `/deuda`, `/servicios`, `/contratos`, `/comprobantes` e `/historial-titulares`. La ruta SHALL usar el número público `propuesta`; la API SHALL resolver internamente cualquier `CodPropuesta` necesario. Las rutas existentes bajo `/api/propuestas` SHALL permanecer disponibles sin cambios de contrato.

#### Scenario: Consulta v2 autenticada por número público
- **WHEN** un consumidor autenticado solicita un recurso v2 con una propuesta existente
- **THEN** recibe el recurso solicitado sin requerir conocer el identificador interno de la propuesta

#### Scenario: Consulta v2 sin autenticación
- **WHEN** un consumidor solicita un recurso v2 sin un JWT válido
- **THEN** la API responde HTTP 401 sin ejecutar la consulta de negocio

### Requirement: Envelope y convenciones de datos v2
Toda respuesta v2 SHALL usar el envelope `{ succeeded, data, error }`. Las respuestas exitosas SHALL contener `succeeded: true`, datos no nulos y `error: null`; los errores SHALL contener `succeeded: false`, `data: null` y un `error.code` estable. Fechas SHALL serializarse en ISO 8601, importes monetarios SHALL conservar precisión decimal, campos individuales inexistentes SHALL ser `null` y colecciones sin elementos SHALL ser `[]`.

#### Scenario: Propuesta inexistente
- **WHEN** se consulta cualquier recurso v2 de una propuesta que no existe o no está disponible
- **THEN** la API responde HTTP 404 con el envelope de error y el código `PROPOSAL_NOT_FOUND`

#### Scenario: Parámetros inválidos
- **WHEN** una ruta v2 recibe un número de propuesta o parámetros de consulta inválidos
- **THEN** la API responde HTTP 400 con el envelope de error y el código `INVALID_REQUEST`

### Requirement: JSON canónicos por endpoint
Los JSON de respuesta de `docs/mcp-api-v2-propuestas.md`, sección 3 y secciones 5 a 11, son el contrato canónico y vinculante de v2. Cada recurso SHALL preservar exactamente los nombres de propiedades, su anidamiento, cardinalidad, nulabilidad y semántica allí definidos; los ejemplos no son ilustrativos ni podrán sustituirse por DTO existentes. Si la fuente actual no proporciona un campo o hecho necesario, el sistema SHALL extender Services, DTOs, repositorios o consultas Firebird para obtenerlo desde una fuente autoritativa.

El contrato canónico exige como mínimo la siguiente forma de `data`:

- Detalle (`GET /{propuesta}`): `propuesta`, `codigo`, `tipo`, `fechaAlta`, `fechaBaja`, `estado`, `deuda.estado`, `parcela` (`codigo`, `numero`, `manzana`, `estado`, `zonas`, `lugares`), `inhumados`, `propuestasAsociadas` y `alertas`.
- Titulares (`GET /{propuesta}/titulares`): `propuesta`, `cantidad` y `titulares[]` con `codigo`, `orden`, `esPrincipal`, `nombre`, `documento`, `contacto`, `domicilio`, `fechaNacimiento` y `fechaAlta`.
- Deuda (`GET /{propuesta}/deuda`): `propuesta`, `estado`, `totales` y `cuentas[]`, donde cada cuenta contiene `codigo`, `tipo`, `categoria`, `estado`, `cliente`, `importe`, `periodo`, `importes` y `cobranza`.
- Servicios (`GET /{propuesta}/servicios`): `propuesta`, `habilitaciones[]`, `cupos[]` y `utilizaciones[]`, con motivo, beneficiario, comprobante y propuestas origen/aplicación cuando corresponda.
- Contratos (`GET /{propuesta}/contratos`): `propuesta`, `contratos[]` y `facturacion.titularesHabilitados[]`; cuando no exista vínculo verificable contrato-plan, se aplicará la variante de colecciones separadas establecida en la sección 9 del documento canónico.
- Comprobantes (`GET /{propuesta}/comprobantes`): `propuesta`, `items[]` y `pagination`, con filtros opcionales `desde`, `hasta`, `tipo`, `estado`, `page` y `pageSize` definidos en la sección 10.
- Historial (`GET /{propuesta}/historial-titulares`): `propuesta` e `historial[]` con `cliente`, `fechaAlta`, `fechaBaja`, `usuarioAlta` y `usuarioBaja`.

#### Scenario: Respuesta conforme al JSON diseñado
- **WHEN** un consumidor consulta cualquiera de los siete recursos v2 con datos disponibles
- **THEN** el cuerpo `data` coincide con el JSON canónico de ese endpoint, incluidos los campos nulos, las colecciones y la estructura anidada

#### Scenario: Dato canónico ausente en el origen actual
- **WHEN** un DTO, servicio o repositorio existente no provee un campo o hecho requerido por el JSON canónico
- **THEN** se amplía la capa de dominio o persistencia para obtenerlo de una fuente autoritativa y McpApi no lo inventa, omite ni deriva de texto, color o posición

### Requirement: Detalle semántico de propuesta y parcela
El recurso de detalle SHALL implementar el JSON canónico de la sección 5, incluidos `estado`, `deuda.estado`, `parcela.estado`, `parcela.zonas`, `parcela.lugares`, `inhumados`, `propuestasAsociadas` y `alertas`. Los estados configurables de deuda, parcela y lugar SHALL incluir su nombre configurado y todos los flags funcionales definidos en dicho JSON. Lugares SHALL ser una colección normalizada e incluirán inhumado únicamente cuando exista una relación inequívoca. El contrato SHALL excluir colores y datos de presentación.

#### Scenario: Parcela con lugares configurables
- **WHEN** una propuesta tiene parcela y lugares con estados configurables
- **THEN** el detalle devuelve cada lugar como elemento de colección y permite decidir venta o inhumación mediante flags funcionales, no mediante el texto del estado

#### Scenario: Lugar sin vínculo inequívoco al inhumado
- **WHEN** el origen sólo provee el nombre de una persona para un lugar y no su relación identificable
- **THEN** el lugar devuelve `inhumado: null` y la API no infiere la relación por nombre

### Requirement: Titulares actuales e historial factual
Los recursos de titulares e historial SHALL implementar respectivamente los JSON canónicos de las secciones 6 y 11. Titulares SHALL incluir la cantidad, orden, indicador principal cuando sea una regla oficial del dominio, identificación, contacto, domicilio y fechas; historial SHALL incluir cliente, altas y bajas registradas y usuarios asociados. Ambos SHALL omitir campos técnicos o visuales sin semántica para el agente y SHALL no inferir transferencias ni reemplazos de titulares.

#### Scenario: Titular principal actual
- **WHEN** el dominio identifica el orden de los titulares actuales
- **THEN** la respuesta indica cuál es principal conforme a la regla oficial y preserva el orden

#### Scenario: Historial con fechas próximas
- **WHEN** dos hechos de titularidad tienen fechas consecutivas
- **THEN** el historial expone ambos hechos registrados sin declarar una transferencia no proporcionada por el ERP

### Requirement: Situación de deuda autorizada por el ERP
El recurso de deuda SHALL implementar el JSON canónico de la sección 7, incluidos todos los campos de `totales`, `cuentas[].periodo`, `cuentas[].importes` y `cuentas[].cobranza`. Cada estado SHALL incluir `nombre`, `activa`, `alDia` y `permiteImputar`; cada cuenta SHALL incluir importes, período, cliente y datos de cobranza aplicables. La API SHALL usar los estados, flags e importes calculados por el ERP y SHALL NOT recalcular mora, intereses, condición al día ni cobrabilidad normal.

#### Scenario: Cuenta judicial activa no imputable
- **WHEN** el ERP informa una cuenta activa, no al día y no imputable por los canales normales
- **THEN** la respuesta conserva los tres flags con esos valores sin deducirlos del nombre del estado

### Requirement: Servicios por habilitación, cupo y utilización
El recurso de servicios SHALL implementar el JSON canónico de la sección 8, incluidas las tres colecciones independientes y todos sus objetos anidados. Una habilitación no disponible SHALL incluir `motivo.codigo`, `motivo.descripcion` y `motivo.hasta` con la normalización establecida; combinaciones que no forman parte de un producto SHALL omitirse. Los cupos SHALL expresar `total`, `utilizados` y `disponibles`. Cada utilización SHALL identificar beneficiario, comprobante y propuesta de origen/aplicación, usando `null` cuando una relación no exista.

#### Scenario: Servicio ausente del producto
- **WHEN** el dominio indica que un servicio no está incluido en un producto
- **THEN** la respuesta no publica una habilitación deshabilitada para esa combinación

#### Scenario: Beneficio aplicado a otra propuesta
- **WHEN** una utilización consume un beneficio de una propuesta y se aplica en otra
- **THEN** la respuesta identifica ambas como `propuestaOrigen` y `propuestaAplicacion`

### Requirement: Contratos y planes sin asociaciones inferidas
El recurso de contratos SHALL implementar el JSON canónico de la sección 9, incluidos contrato, vendedor, total, plan de venta y `facturacion.titularesHabilitados`. Un plan de venta SHALL anidarse en un contrato sólo cuando el dominio provea una clave o relación inequívoca; de lo contrario, los contratos y planes de venta SHALL devolverse como colecciones independientes según la variante canónica.

#### Scenario: Plan sin relación verificable
- **WHEN** el dominio devuelve contratos y planes de venta sin una relación inequívoca entre ellos
- **THEN** la respuesta conserva ambas colecciones separadas y no las asocia por índice

### Requirement: Comprobantes filtrables y paginados por conceptos de dominio
El recurso de comprobantes SHALL implementar el JSON canónico de la sección 10 y admitir opcionalmente `desde`, `hasta`, `tipo`, `estado`, `page` y `pageSize`, con sus defaults documentados. La respuesta SHALL devolver todos los campos de `items[]` y metadatos `page`, `pageSize`, `totalItems`, `totalPages`, `hasNext` y `hasPrevious`. El contrato SHALL exponer anulaciones como booleano y SHALL NOT exponer la estructura genérica de filtros del frontend ni identificadores internos que no habiliten una operación posterior.

#### Scenario: Consulta paginada y filtrada
- **WHEN** un consumidor solicita comprobantes con rango de fechas, tipo, estado y paginación válidos
- **THEN** recibe sólo los comprobantes coincidentes y metadatos consistentes con la página solicitada

#### Scenario: Filtro de comprobantes inválido
- **WHEN** un consumidor envía una fecha inválida, un rango inconsistente o valores de paginación fuera de los límites documentados
- **THEN** la API responde `INVALID_REQUEST` sin traducir esos valores al mecanismo interno de filtros del frontend
