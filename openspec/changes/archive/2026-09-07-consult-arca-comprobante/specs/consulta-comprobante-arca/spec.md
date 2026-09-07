## ADDED Requirements

### Requirement: Respuesta con datos tipados de forma generica
El contrato `Afip.Data.Models.ApiResponse` SHALL aceptar un parametro de tipo `T` y exponer `Data` como `T?`, sin depender de un DTO especifico de ARCA. Las respuestas de la consulta de comprobante SHALL utilizar `ApiResponse<DtoAfipWsfeConsultaDetalle>`.

#### Scenario: Respuesta de consulta con detalle de ARCA
- **WHEN** Afip.WebApi devuelve el resultado de `FECompConsultar`
- **THEN** `Data` se deserializa como `DtoAfipWsfeConsultaDetalle` mediante `ApiResponse<DtoAfipWsfeConsultaDetalle>`

#### Scenario: Respuesta futura con otro tipo de datos
- **WHEN** una operacion utiliza el mismo contrato con un DTO diferente
- **THEN** `Data` conserva ese tipo mediante `ApiResponse<T>` sin modificar la clase de respuesta

### Requirement: Consulta publica de comprobante en ARCA
ElPrado.WebApi SHALL exponer un endpoint GET autenticado en `ComprobantesController` para consultar un comprobante de ARCA usando `codTalonario` y `nroComprobante`. El endpoint SHALL delegar la consulta a Afip.WebApi y no SHALL exponer Afip.WebApi directamente al frontend. La respuesta SHALL usar un DTO nuevo de detalle de consulta y no SHALL modificar el DTO actual utilizado por otros flujos de comprobantes.

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
