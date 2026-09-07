## Context

`Afip.Services.Wsfe` ya invoca `FECompConsultar` dentro de la operacion que actualiza CAE, pero esa ruta persiste el resultado en Firebird. El nuevo flujo necesita la misma consulta remota, expuesta desde `Afip.WebApi` y reenviada por `ElPrado.WebApi`, sin ejecutar las operaciones de actualizacion de comprobantes ni de errores.

## Goals / Non-Goals

**Goals:**

- Consultar el comprobante actual en ARCA usando el talonario y numero existentes en ElPrado.
- Exponer el detalle a usuarios autenticados mediante `ComprobantesController`.
- Mantener el acceso SOAP y las credenciales dentro de Afip.WebApi.
- Reutilizar el gateway HTTP tipado y el formato `ApiResponse` existente.
- Introducir un DTO nuevo para el detalle completo de `FECompConsultarResponse` sin modificar el DTO actual que consume otros endpoints.

**Non-Goals:**

- Solicitar o actualizar CAE, modificar el comprobante o registrar el resultado de la consulta.
- Crear migraciones, tablas, stored procedures o configuraciones nuevas.
- Exponer Afip.WebApi al frontend ni cambiar las rutas WSFE existentes.

## Decisions

- Afip.WebApi agregara un `GET /api/wsfe/comprobante/{codTalonario}/{nroComprobante}` y ElPrado.WebApi lo publicara como `GET /api/Comprobantes/Afip/Comprobante/{codTalonario}/{nroComprobante}`.
  - El numero de comprobante puede contener guiones y se codificara al cruzar el gateway. Usar GET expresa que el flujo es solo lectura.
  - Se descarta reutilizar `actualizar-cae`, porque su efecto de persistencia contradice el objetivo.

- La consulta leera la configuracion existente del comprobante para obtener tipo WSFE y punto de venta, y luego llamara `FECompConsultar`.
  - Evita que el frontend deba conocer codigos fiscales internos.
  - Se descarta pedir tipo y punto de venta al cliente, ya que permitiria consultas inconsistentes con el comprobante local.

- `Afip.Data.Models.ApiResponse` se convertira en `ApiResponse<T>` y su propiedad `Data` sera `T?`.
  - La consulta de comprobante utilizara `ApiResponse<DtoAfipWsfeConsultaDetalle>`.
  - Se descarta mantener `Data` ligado a `DtoAfipWsfeConsultaDetalle`, porque impediria reutilizar el contrato con respuestas de otros tipos.

- Se agregara un DTO de detalle que represente resultado, CAE, vencimiento, fecha de proceso y observaciones devueltos por ARCA, junto con el talonario y numero consultados.
  - Ese DTO nuevo tambien modelara los bloques repetibles de la respuesta SOAP: comprobantes asociados, tributos, Iva, opcionales, compradores, periodo asociado, observaciones, errores y eventos.
  - El gateway deserializara ese contrato y conservara los errores controlados actuales de transporte y respuesta invalida.
  - Se descarta devolver los tipos SOAP generados para no acoplar el contrato publico a la integracion.

- La operacion no llamara a los metodos que actualizan respuesta CAE, errores o ultimo comprobante.
  - La renovacion normal del ticket de autenticacion permanece bajo la responsabilidad existente de Afip.WebApi y no forma parte del resultado de la consulta.

## Risks / Trade-offs

- ARCA no encuentra el comprobante o devuelve un error WSFE -> devolver una respuesta no exitosa sin persistirla.
- El comprobante local no permite resolver tipo o punto de venta -> devolver un error funcional antes de invocar ARCA.
- Afip.WebApi no esta disponible -> el gateway devolvera el error controlado existente, sin exponer excepciones de red.
- Los datos de ARCA pueden diferir de ElPrado -> se mostraran como respuesta remota, sin sincronizacion automatica.
