## 1. Configuration and HTTP Gateway

- [x] 1.1 Add Afip.WebApi base URL configuration to ElPrado.WebApi appsettings, using `http://localhost:5000` as the local default.
- [x] 1.2 Register an `HttpClient`-based AFIP gateway in ElPrado.WebApi dependency injection.
- [x] 1.3 Implement the gateway methods for service status, last authorized comprobante, CAE update, and CAE request.
- [x] 1.4 Map Afip.WebApi transport errors and invalid responses to controlled non-success API responses.

## 2. ComprobantesController Endpoints

- [x] 2.1 Inject the AFIP gateway into `ComprobantesController`.
- [x] 2.2 Add an authenticated GET endpoint for AFIP service status under `/api/Comprobantes/Afip`.
- [x] 2.3 Add an authenticated GET endpoint for last authorized comprobante under `/api/Comprobantes/Afip`.
- [x] 2.4 Add an authenticated POST endpoint for CAE update under `/api/Comprobantes/Afip`.
- [x] 2.5 Add an authenticated POST endpoint for CAE request under `/api/Comprobantes/Afip`.
- [x] 2.6 Preserve Afip.WebApi success payloads with HTTP 200 and non-success payloads with non-success HTTP status codes.

## 3. Verification

- [x] 3.1 Build ElPrado.WebApi and fix compile errors.
- [x] 3.2 Verify each new endpoint forwards to the matching Afip.WebApi route from `Afip.WebApi/Program.cs`.
- [x] 3.3 Verify behavior when Afip.WebApi is unavailable returns a controlled error.
- [x] 3.4 Run the relevant automated tests if a test project is available, or document the manual verification performed.
