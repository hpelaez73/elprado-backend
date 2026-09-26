## Context

See `proposal.md` for motivation and the delta to `consulta-comprobante-arca` for the observable contract. `ElPrado.WebApi` currently forwards the technical detail received from `Afip.WebApi` directly to the frontend. `ElPrado` already has the authenticated API boundary, the WSFE gateway, the unit of work, and repository access to the local Firebird data needed to enrich that response.

## Goals / Non-Goals

**Goals:**

- Extend the response additively with operator-readable descriptions.
- Resolve Firebird equivalences and fiscal-document display rules exclusively in `ElPrado.*`.
- Keep the request to `Afip.WebApi` and its existing technical response unchanged.
- Make equivalence reads bounded to the requested comprobante and its present nested elements.

**Non-Goals:**

- Do not modify any source, DTO, repository, service, route, or behavior in `Afip.*`.
- Do not alter the public route, authentication, HTTP method, or existing technical fields.
- Do not update AFIP master data, comprobantes, CAE data, or add database migrations.
- Do not refactor unrelated WSFE operations.

## Decisions

### Enrich the detail after the gateway response in ElPrado.Services

`ElPrado.Services` will receive the existing technical response through `IAfipWsfeGateway`, then enrich its `Data` using a dedicated method in the local comprobantes data path. `ComprobantesController` will delegate this orchestration to `ComprobantesService` and continue to expose only ElPrado.WebApi to the frontend.

This leaves the Afip.WebApi contract unchanged and keeps controllers free of data-access logic. Enriching the response in `Afip.Services` is rejected because the change is explicitly limited to `ElPrado.*`.

### Keep additive presentation fields in ElPrado.Dto

`DtoAfipWsfeConsultaDetalle` and its existing nested DTOs belong to `ElPrado.Dto`; they will retain all current code fields and receive nullable or empty-safe description fields. The local detail model will additionally carry the recipient VAT-condition code and description needed to populate the response.

An isolated frontend-only DTO is rejected because it would duplicate the complete technical detail and need an additional gateway mapping with no compatibility benefit.

### Resolve only local equivalences referenced by the response

`ElPrado.Data` will add parameterized, bounded reads to the comprobantes repository for the requested talonario and comprobante. These reads will obtain the local source data and the applicable master descriptions. Missing mappings will leave the description unset and will not change a successful technical AFIP response into an error.

Known mapping relations are `AFIP_TIPOS_COMPROBANTES`, `AFIP_TIPOS_DOCUMENTOS`, `AFIP_CONDICION_FRENTE_AL_IVA`, `CAT_IVA`, `ALICUOTAS`, and `TALONARIOS`; the implementation will verify their precise descriptive columns before finalizing queries. Static in-code catalogs are rejected because descriptions must reflect persisted equivalences.

### Derive the displayed document with the emission precedence

`ElPrado.Services` will use one local rule for the display document: usable CUIT takes precedence and maps to AFIP document type 80; otherwise use the mapped local document; otherwise use the consumer/no-document representation. The same local data used to reconstruct the detail supports the condition IVA and its description.

Maintaining the technical response's independent document fields without correction is rejected because they can differ from the fiscal identity used for emission.

### Preserve incomplete nested detail honestly

The local presentation mapper enriches only elements present in the technical detail or supported by the requested comprobante's local data. It will not fabricate tributes, associated documents, optional data, buyers, or periods that the existing technical response does not expose.

## Risks / Trade-offs

- [Missing or undocumented master table/column] -> Inspect schema with bounded Firebird queries or DDL before writing the final repository query; leave descriptions unset when a runtime lookup is absent.
- [Master data differs from a historic emission] -> Preserve technical codes and treat descriptions as explanatory local metadata.
- [Additional Firebird reads] -> Filter by the comprobante primary key and only fetch nested equivalences that are needed.
- [Consumer compatibility] -> Add properties only; do not rename or remove the current technical fields.

## Migration Plan

1. Deploy the additive DTO, ElPrado repository, service, and controller changes together.
2. Verify successful, unmapped, CUIT, and nested-IVA responses through the ElPrado.WebApi endpoint.
3. Roll back by deploying the prior ElPrado application version; no data migration or cleanup is required.

## Open Questions

- Exact table structures and persisted descriptions for concept, currency, result, emission type, and tributes must be confirmed from Firebird before their bounded `ElPrado.Data` lookups are finalized.
