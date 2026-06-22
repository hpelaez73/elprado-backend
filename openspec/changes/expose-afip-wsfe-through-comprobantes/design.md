## Context

Afip.WebApi is a separate service that owns the AFIP WSFE integration and currently exposes minimal API routes under `/api/wsfe`. ElPrado.WebApi is the backend consumed by the Next.js frontend and already exposes comprobante-related operations through `ComprobantesController` under `/api/Comprobantes`.

The frontend must not call Afip.WebApi directly. ElPrado.WebApi will therefore expose equivalent comprobante endpoints and forward those calls to Afip.WebApi, which is expected to listen on port 5000.

## Goals / Non-Goals

**Goals:**
- Add `ComprobantesController` endpoints for all current Afip.WebApi WSFE operations.
- Keep AFIP authorization, CAE updates, and WSFE business logic inside Afip.WebApi.
- Make Afip.WebApi's base URL configurable in ElPrado.WebApi, with `http://localhost:5000` as the development default.
- Preserve existing authentication behavior from `ControladorBase` for the new bridge endpoints.
- Return Afip.WebApi success/error payloads and status semantics to the frontend.

**Non-Goals:**
- Reimplement WSFE logic inside ElPrado.WebApi.
- Expose Afip.WebApi directly to the frontend.
- Change existing comprobante, PDF, email, or listing behavior.
- Expand AFIP functionality beyond the four routes currently present in `Afip.WebApi/Program.cs`.

## Decisions

- Use `ComprobantesController` as the public facade for WSFE operations.
  - Rationale: comprobante workflows are already centralized there, and the frontend can continue using ElPrado.WebApi as its single backend.
  - Alternative considered: create a new `WsfeController`; this would mirror Afip.WebApi more literally but split comprobante operations across controllers.

- Expose routes under `/api/Comprobantes/Afip/...`.
  - Rationale: prefixing with `Afip` keeps the new bridge endpoints distinct from existing factura/comprobante actions while staying inside the comprobantes API surface.
  - Alternative considered: reuse `/api/wsfe/...` in ElPrado.WebApi; this would hide that the operation belongs to comprobantes and conflicts with the explicit request to add them in `ComprobantesController`.

- Register an `HttpClient`-based AFIP gateway in ElPrado.WebApi.
  - Rationale: typed/named `HttpClient` centralizes base URL, timeouts, and JSON handling without adding a project reference from ElPrado.WebApi to Afip.Data.
  - Alternative considered: inject `Wsfe` directly from Afip.Services; this would couple ElPrado.WebApi to AFIP data repositories and duplicate the purpose of Afip.WebApi.

- Preserve Afip.WebApi response contracts at the bridge boundary.
  - Rationale: Afip.WebApi currently returns `200 OK` when `Success` is true and `400 Bad Request` otherwise. The frontend should be able to consume the same result data through ElPrado.WebApi.
  - Alternative considered: wrap responses in ElPrado.Core `ApiResponse<T>`; that would normalize local API style but could hide AFIP-specific fields such as `NroCAE`.

## Risks / Trade-offs

- Afip.WebApi unavailable -> return a controlled error from ElPrado.WebApi, log the failure, and avoid exposing transport exceptions to the frontend.
- Afip.WebApi response shape changes -> keep a small local DTO or JSON pass-through and cover the four bridge routes with tests.
- Duplicate route intent between services -> document the ElPrado routes as frontend-facing and Afip.WebApi routes as internal service-facing.
- Longer request latency during AFIP operations -> configure reasonable `HttpClient` timeout and let the frontend handle pending/error states.

## Migration Plan

1. Deploy or run Afip.WebApi on port 5000.
2. Configure ElPrado.WebApi with `AfipWebApi:BaseUrl`, defaulting to `http://localhost:5000` for local development.
3. Deploy ElPrado.WebApi with the new bridge endpoints.
4. Update the frontend to call `/api/Comprobantes/Afip/...` endpoints only.
5. Rollback by restoring frontend calls to previous behavior or disabling the new ElPrado routes; no database migration is required.

## Open Questions

- The final production host name for Afip.WebApi must be provided by deployment configuration.
