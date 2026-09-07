## 1. Contratos de consulta

- [x] 1.1 Agregar en `ElPrado.Dto` un DTO nuevo para devolver el detalle completo de comprobante ARCA, incluyendo campos escalares y colecciones anidadas de `FECompConsultarResponse`, sin modificar el DTO actual de comprobantes.
- [x] 1.2 Ajustar el contrato de respuesta de `Afip.WebApi` para incluir el nuevo DTO de consulta completo sin romper las rutas WSFE existentes.
- [x] 1.3 Convertir `Afip.Data.Models.ApiResponse` en `ApiResponse<T>` con `Data` de tipo `T?`, y actualizar la consulta de comprobante para usar `ApiResponse<DtoAfipWsfeConsultaDetalle>`.

## 2. Consulta interna WSFE

- [x] 2.1 Incorporar en `Afip.Services/Wsfe.cs` una operacion de solo lectura que resuelva el comprobante existente y ejecute `FECompConsultar`.
- [x] 2.2 Mapear la respuesta y los errores de WSFE al nuevo contrato completo sin invocar metodos de actualizacion de CAE, ultimo comprobante o errores.
- [x] 2.3 Agregar en `Afip.WebApi/Program.cs` la ruta GET interna para consultar un comprobante por talonario y numero.

## 3. Fachada publica

- [x] 3.1 Extender `IAfipWsfeGateway` y `AfipWsfeGateway` con la llamada GET a la nueva ruta, incluyendo codificacion segura del numero de comprobante.
- [x] 3.2 Agregar en `ElPrado.WebApi/Controllers/ComprobantesController.cs` el endpoint GET autenticado `Afip/Comprobante/{codTalonario}/{nroComprobante}` que delegue al gateway.
- [x] 3.3 Conservar respuestas controladas para indisponibilidad de Afip.WebApi y respuestas invalidas del servicio interno.

## 4. Verificacion

- [x] 4.1 Compilar `Afip.WebApi` y `ElPrado.WebApi` y resolver errores de contrato, incluyendo el uso generico de `ApiResponse<T>`, o inyeccion de dependencias.
- [ ] 4.2 Verificar que una consulta exitosa devuelve los datos de ARCA y que un comprobante no identificable o un error WSFE devuelve una respuesta no exitosa.
- [x] 4.3 Verificar que el flujo de consulta no ejecuta actualizaciones de CAE, ultimo comprobante, observaciones ni errores en Firebird.
