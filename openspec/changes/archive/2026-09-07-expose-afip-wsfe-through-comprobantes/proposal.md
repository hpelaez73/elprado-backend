## Why

El frontend debe operar comprobantes electronicos desde ElPrado.WebApi sin consumir directamente Afip.WebApi. Exponer las operaciones WSFE desde ComprobantesController mantiene a ElPrado.WebApi como fachada unica de la app y permite centralizar autenticacion, CORS y configuracion.

## What Changes

- Add endpoints in `ComprobantesController` that mirror the current `Afip.WebApi` WSFE operations:
  - consultar estado del servicio
  - consultar ultimo comprobante autorizado
  - actualizar CAE de un comprobante emitido
  - solicitar CAE de un comprobante
- Configure ElPrado.WebApi with an HTTP client for Afip.WebApi, defaulting to the service on port 5000 via configuration.
- Preserve Afip.WebApi as the authority for AFIP communication and database-side CAE updates; ElPrado.WebApi acts as a bridge for the frontend.
- Return success and error responses from Afip.WebApi through ElPrado.WebApi with appropriate HTTP status codes.

## Capabilities

### New Capabilities
- `afip-wsfe-bridge`: ElPrado.WebApi exposes authenticated comprobante endpoints that proxy WSFE operations to Afip.WebApi.

### Modified Capabilities

## Impact

- Affected API surface: `ElPrado.WebApi.Controllers.ComprobantesController`.
- Affected configuration: ElPrado.WebApi appsettings/environment settings for Afip.WebApi base URL.
- Affected integrations: ElPrado.WebApi will call Afip.WebApi on port 5000; the frontend will call only ElPrado.WebApi.
- No breaking changes are expected for existing comprobante endpoints.
