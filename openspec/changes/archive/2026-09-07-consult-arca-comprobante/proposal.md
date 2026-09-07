## Why

El sistema necesita contrastar un comprobante con la informacion actualmente registrada por ARCA, sin alterar el comprobante local ni volver a solicitar su CAE. Esta consulta debe estar disponible para el frontend a traves de ElPrado.WebApi, manteniendo Afip.WebApi como integracion interna con WSFE.

## What Changes

- Agregar una consulta de solo lectura de un comprobante en ARCA mediante WSFE `FECompConsultar`.
- Exponer la consulta internamente desde Afip.WebApi y publicamente desde `ComprobantesController` mediante el gateway existente.
- Devolver el detalle informado por ARCA, incluidos resultado, CAE, vencimiento, fecha de proceso y observaciones cuando existan.
- Informar errores controlados cuando el comprobante no pueda identificarse, ARCA no responda o WSFE devuelva errores.
- Hacer generico el tipo de `Data` en `Afip.Data.Models.ApiResponse<T>` para permitir respuestas con distintos contratos.
- Garantizar que la consulta no actualice comprobantes, CAE, configuracion ni registros de error en la base de datos.

## Capabilities

### New Capabilities

- `consulta-comprobante-arca`: Consulta autenticada y de solo lectura del detalle de un comprobante autorizado en ARCA a traves de ElPrado.WebApi.

### Modified Capabilities

Ninguna.

## Impact

- `ElPrado.WebApi/Controllers/ComprobantesController.cs` incorporara el endpoint consumido por el frontend.
- `ElPrado.Services` ampliara el contrato y la implementacion de `IAfipWsfeGateway`.
- `Afip.WebApi/Program.cs` y `Afip.Services/Wsfe.cs` incorporaran la ruta interna y la consulta WSFE.
- `ElPrado.Dto` o el contrato equivalente incorporara el detalle de respuesta de ARCA.
- `Afip.Data.Models.ApiResponse` pasara a ser generico para no fijar el tipo de `Data` al detalle de consulta de ARCA.
- No requiere migraciones, cambios de esquema ni actualizaciones de datos.
