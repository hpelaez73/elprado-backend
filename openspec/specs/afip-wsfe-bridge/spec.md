# afip-wsfe-bridge Specification

## Purpose
Define la fachada autenticada de ElPrado.WebApi para operaciones WSFE delegadas a Afip.WebApi, incluyendo configuracion, errores controlados y preservacion de respuestas.

## Requirements

### Requirement: Expose WSFE service status through ComprobantesController
ElPrado.WebApi SHALL expose an authenticated endpoint under `/api/Comprobantes/Afip` that queries Afip.WebApi's `/api/wsfe/estado-servicio` operation and returns its response to the caller.

#### Scenario: AFIP service status succeeds
- **WHEN** an authenticated frontend client requests the ElPrado.WebApi AFIP service-status endpoint
- **THEN** ElPrado.WebApi calls Afip.WebApi `/api/wsfe/estado-servicio`
- **AND** ElPrado.WebApi returns HTTP 200 with the AFIP response payload when Afip.WebApi reports success

#### Scenario: AFIP service status fails
- **WHEN** Afip.WebApi returns a non-success response for `/api/wsfe/estado-servicio`
- **THEN** ElPrado.WebApi returns a non-success HTTP status with the AFIP response payload

### Requirement: Expose last authorized comprobante query
ElPrado.WebApi SHALL expose an authenticated endpoint under `/api/Comprobantes/Afip` that accepts `codTipoComprobante` and `codTalonario`, forwards them to Afip.WebApi's `/api/wsfe/ultimo-comprobante/{codTipoComprobante}/{codTalonario}` operation, and returns the result.

#### Scenario: Last authorized comprobante succeeds
- **WHEN** an authenticated frontend client requests the last authorized comprobante for a valid comprobante type and talonario
- **THEN** ElPrado.WebApi calls Afip.WebApi `/api/wsfe/ultimo-comprobante/{codTipoComprobante}/{codTalonario}`
- **AND** ElPrado.WebApi returns HTTP 200 with the AFIP response payload when Afip.WebApi reports success

#### Scenario: Last authorized comprobante fails
- **WHEN** Afip.WebApi returns a non-success response for the last authorized comprobante query
- **THEN** ElPrado.WebApi returns a non-success HTTP status with the AFIP response payload

### Requirement: Expose CAE update for emitted comprobantes
ElPrado.WebApi SHALL expose an authenticated endpoint under `/api/Comprobantes/Afip` that accepts `codTalonario` and `nroComprobante`, forwards them to Afip.WebApi's `/api/wsfe/actualizar-cae/{codTalonario}/{nroComprobante}` operation, and returns the result.

#### Scenario: CAE update succeeds
- **WHEN** an authenticated frontend client requests CAE update for an emitted comprobante
- **THEN** ElPrado.WebApi calls Afip.WebApi `/api/wsfe/actualizar-cae/{codTalonario}/{nroComprobante}` using POST
- **AND** ElPrado.WebApi returns HTTP 200 with the AFIP response payload when Afip.WebApi reports success

#### Scenario: CAE update fails
- **WHEN** Afip.WebApi returns a non-success response for the CAE update operation
- **THEN** ElPrado.WebApi returns a non-success HTTP status with the AFIP response payload

### Requirement: Expose CAE request for comprobantes
ElPrado.WebApi SHALL expose an authenticated endpoint under `/api/Comprobantes/Afip` that accepts `codTalonario` and `nroComprobante`, forwards them to Afip.WebApi's `/api/wsfe/solicitar-cae/{codTalonario}/{nroComprobante}` operation, and returns the result.

#### Scenario: CAE request succeeds
- **WHEN** an authenticated frontend client requests CAE authorization for a comprobante
- **THEN** ElPrado.WebApi calls Afip.WebApi `/api/wsfe/solicitar-cae/{codTalonario}/{nroComprobante}` using POST
- **AND** ElPrado.WebApi returns HTTP 200 with the AFIP response payload when Afip.WebApi reports success

#### Scenario: CAE request fails
- **WHEN** Afip.WebApi returns a non-success response for the CAE request operation
- **THEN** ElPrado.WebApi returns a non-success HTTP status with the AFIP response payload

### Requirement: Configure Afip.WebApi base URL
ElPrado.WebApi SHALL read the Afip.WebApi base URL from configuration and SHALL use `http://localhost:5000` as the local default when no override is provided.

#### Scenario: Base URL configured
- **WHEN** ElPrado.WebApi starts with an `AfipWebApi:BaseUrl` value
- **THEN** bridge requests use that base URL for all Afip.WebApi calls

#### Scenario: Base URL not configured
- **WHEN** ElPrado.WebApi starts without an `AfipWebApi:BaseUrl` value
- **THEN** bridge requests use `http://localhost:5000` for Afip.WebApi calls

### Requirement: Handle Afip.WebApi transport failures
ElPrado.WebApi SHALL return a controlled non-success response when Afip.WebApi cannot be reached or returns an invalid response.

#### Scenario: Afip.WebApi unavailable
- **WHEN** an authenticated frontend client calls an AFIP bridge endpoint and Afip.WebApi is unavailable
- **THEN** ElPrado.WebApi returns a non-success HTTP status
- **AND** the response contains a useful error message without exposing raw transport exception details
