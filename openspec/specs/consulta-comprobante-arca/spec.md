# consulta-comprobante-arca Specification

## Purpose
Define la consulta de comprobantes ARCA desde las fachadas de ElPrado y Afip, con respuesta tipada completa y sin efectos de persistencia.

## Requirements

### Requirement: Respuesta con datos tipados de forma generica
El contrato `Afip.Data.Models.ApiResponse` SHALL aceptar un parametro de tipo `T` y exponer `Data` como `T?`, sin depender de un DTO especifico de ARCA. Las respuestas de la consulta de comprobante SHALL utilizar `ApiResponse<DtoAfipWsfeConsultaDetalle>`.

#### Scenario: Respuesta de consulta con detalle de ARCA
- **WHEN** Afip.WebApi devuelve el resultado de `FECompConsultar`
- **THEN** `Data` se deserializa como `DtoAfipWsfeConsultaDetalle` mediante `ApiResponse<DtoAfipWsfeConsultaDetalle>`

#### Scenario: Respuesta futura con otro tipo de datos
- **WHEN** una operacion utiliza el mismo contrato con un DTO diferente
- **THEN** `Data` conserva ese tipo mediante `ApiResponse<T>` sin modificar la clase de respuesta

### Requirement: Consulta publica de comprobante en ARCA
ElPrado.WebApi SHALL exponer un endpoint GET autenticado en `ComprobantesController` para consultar un comprobante de ARCA usando `codTalonario` y `nroComprobante`. El endpoint SHALL delegar la consulta tecnica a Afip.WebApi y no SHALL exponer Afip.WebApi directamente al frontend. ElPrado.WebApi SHALL enriquecer la respuesta tecnica con informacion local antes de devolverla al frontend, sin requerir cambios en Afip.WebApi. La respuesta SHALL usar un DTO nuevo de detalle de consulta y no SHALL modificar el DTO actual utilizado por otros flujos de comprobantes. El detalle SHALL conservar los codigos tecnicos existentes y agregar la informacion operativa descripta para cada codigo que pueda resolverse.

#### Scenario: Consulta exitosa desde el frontend
- **WHEN** un usuario autenticado solicita un talonario y numero de comprobante validos
- **THEN** ElPrado.WebApi devuelve una respuesta exitosa con el detalle completo informado por ARCA, incluyendo datos del comprobante, importes, comprobantes asociados, tributos, Iva, opcionales, compradores, periodo asociado, observaciones, errores y eventos cuando existan

#### Scenario: Afip.WebApi no esta disponible
- **WHEN** ElPrado.WebApi no puede comunicarse con Afip.WebApi
- **THEN** devuelve una respuesta no exitosa controlada sin exponer detalles de transporte

### Requirement: Consulta interna de solo lectura a WSFE
Afip.WebApi SHALL exponer una ruta interna GET que consulte WSFE `FECompConsultar` para el comprobante identificado. La consulta SHALL resolver el tipo de comprobante y punto de venta desde la informacion existente y SHALL devolver un DTO nuevo con todos los campos informados por ARCA en `FECompConsultarResponse`, sin reutilizar el contrato de actualizacion de CAE ni persistir cambios.

#### Scenario: ARCA devuelve el detalle del comprobante
- **WHEN** WSFE encuentra el comprobante solicitado
- **THEN** Afip.WebApi devuelve los datos recibidos de ARCA asociados al talonario y numero consultados, incluyendo campos escalares y colecciones anidadas del comprobante

#### Scenario: No se puede identificar el comprobante para WSFE
- **WHEN** no existe informacion local suficiente para resolver el comprobante solicitado
- **THEN** Afip.WebApi devuelve una respuesta no exitosa y no consulta ARCA

#### Scenario: WSFE devuelve un error
- **WHEN** ARCA responde errores para la consulta
- **THEN** Afip.WebApi devuelve una respuesta no exitosa con el mensaje funcional recibido

### Requirement: Ausencia de efectos de persistencia
La consulta de comprobante SHALL NOT actualizar CAE, estado, observaciones, ultimo comprobante ni registros de error en Firebird. La respuesta de ARCA SHALL ser transitoria y no SHALL iniciar una sincronizacion del comprobante local.

#### Scenario: Consulta con respuesta exitosa
- **WHEN** ARCA devuelve datos para el comprobante
- **THEN** el resultado se devuelve al solicitante sin actualizar datos del comprobante en la base

#### Scenario: Consulta con error remoto
- **WHEN** ARCA devuelve un error o no encuentra el comprobante
- **THEN** el error se devuelve al solicitante sin registrar una actualizacion del comprobante ni un error de consulta

### Requirement: Cobertura completa del detalle ARCA
El nuevo DTO de consulta SHALL incluir, como minimo, los campos escalares `Concepto`, `DocTipo`, `DocNro`, `CbteDesde`, `CbteHasta`, `CbteFch`, `ImpTotal`, `ImpTotConc`, `ImpNeto`, `ImpOpEx`, `ImpTrib`, `ImpIVA`, `FchServDesde`, `FchServHasta`, `FchVtoPago`, `MonId`, `MonCotiz`, `Resultado`, `CodAutorizacion`, `EmisionTipo`, `FchVto`, `FchProceso`, `PtoVta` y `CbteTipo`, mas las colecciones `CbtesAsoc`, `Tributos`, `Iva`, `Opcionales`, `Compradores`, `PeriodoAsoc`, `Observaciones`, `Errors` y `Events`.

#### Scenario: El servicio devuelve un comprobante compuesto
- **WHEN** ARCA informa listas de comprobantes asociados, tributos, Iva, opcionales, compradores, observaciones, errores o eventos
- **THEN** ElPrado.WebApi conserva esa estructura en el nuevo DTO de salida sin aplanarla ni eliminar elementos

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
