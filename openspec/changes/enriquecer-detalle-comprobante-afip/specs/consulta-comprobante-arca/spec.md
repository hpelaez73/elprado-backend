## MODIFIED Requirements

### Requirement: Consulta publica de comprobante en ARCA
ElPrado.WebApi SHALL exponer un endpoint GET autenticado en `ComprobantesController` para consultar un comprobante de ARCA usando `codTalonario` y `nroComprobante`. El endpoint SHALL delegar la consulta tecnica a Afip.WebApi y no SHALL exponer Afip.WebApi directamente al frontend. ElPrado.WebApi SHALL enriquecer la respuesta tecnica con informacion local antes de devolverla al frontend, sin requerir cambios en Afip.WebApi. La respuesta SHALL usar un DTO nuevo de detalle de consulta y no SHALL modificar el DTO actual utilizado por otros flujos de comprobantes. El detalle SHALL conservar los codigos tecnicos existentes y agregar la informacion operativa descripta para cada codigo que pueda resolverse.

#### Scenario: Consulta exitosa desde el frontend
- **WHEN** un usuario autenticado solicita un talonario y numero de comprobante validos
- **THEN** ElPrado.WebApi devuelve una respuesta exitosa con el detalle completo informado por ARCA, incluyendo datos del comprobante, importes, comprobantes asociados, tributos, Iva, opcionales, compradores, periodo asociado, observaciones, errores y eventos cuando existan

#### Scenario: Afip.WebApi no esta disponible
- **WHEN** ElPrado.WebApi no puede comunicarse con Afip.WebApi
- **THEN** devuelve una respuesta no exitosa controlada sin exponer detalles de transporte

## ADDED Requirements

### Requirement: Descripciones operativas de codigos fiscales
ElPrado.WebApi SHALL conservar los codigos tecnicos recibidos de Afip.WebApi y SHALL incluir sus descripciones legibles cuando exista una equivalencia local para el concepto, tipo de documento, tipo de comprobante, condicion frente al IVA del receptor, moneda, resultado, tipo de emision y los elementos anidados que correspondan a IVA, tributos, comprobantes asociados y compradores.

#### Scenario: Equivalencias disponibles para el comprobante
- **WHEN** las tablas maestras locales contienen una equivalencia para un codigo fiscal del detalle
- **THEN** la respuesta conserva el codigo original y devuelve su descripcion asociada para que el frontend pueda mostrarla al operador

#### Scenario: Equivalencia local inexistente
- **WHEN** un codigo fiscal informado por el detalle no tiene una equivalencia local
- **THEN** la consulta se completa exitosamente, conserva el codigo y devuelve la descripcion vacia o no informada para ese dato

#### Scenario: Detalle con elementos anidados
- **WHEN** el comprobante contiene alicuotas de IVA, tributos, comprobantes asociados o compradores
- **THEN** cada elemento conserva su codigo y contiene la descripcion operativa correspondiente cuando la equivalencia exista

### Requirement: Consistencia del documento fiscal consultado
ElPrado.WebApi SHALL informar en el detalle el mismo tipo y numero de documento que se emplearon al enviar o reconstruir la solicitud de CAE del comprobante a partir de la informacion local. Cuando se utiliza CUIT del receptor, el detalle SHALL informar el codigo de tipo de documento CUIT y ese mismo numero.

#### Scenario: Receptor identificado por CUIT
- **WHEN** el comprobante se emite utilizando el CUIT del receptor
- **THEN** la respuesta informa el tipo de documento CUIT y el CUIT como numero de documento, junto con la descripcion del tipo

#### Scenario: Receptor sin CUIT utilizado para la emision
- **WHEN** el comprobante se emite con otro tipo de documento o como consumidor sin documento
- **THEN** la respuesta informa el mismo tipo y numero de documento usados para esa emision
