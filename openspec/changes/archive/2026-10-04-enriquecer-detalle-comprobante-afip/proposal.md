## Why

La consulta de comprobantes muestra al frontend codigos fiscales que un operador no puede interpretar sin conocer AFIP. Ademas, la representacion actual del documento puede no coincidir con el tipo y numero efectivamente enviados a AFIP.

## What Changes

- Enriquece la respuesta de consulta de comprobante AFIP con descripciones legibles de los codigos fiscales, sin eliminar los codigos tecnicos existentes.
- Agrega la condicion frente al IVA del receptor como codigo y descripcion.
- Resuelve las descripciones desde las equivalencias persistidas en Firebird y conserva una respuesta exitosa aun cuando falte una equivalencia.
- Alinea el documento informado por la consulta con el que se utiliza para solicitar el CAE.
- Completa, cuando existan datos locales, las descripciones de los elementos anidados aplicables, como IVA, tributos, comprobantes asociados y compradores.
- Realiza todos los ajustes en los proyectos `ElPrado.*`; `Afip.*` conserva sin cambios su contrato y comportamiento actuales.

## Capabilities

### New Capabilities

- Ninguna.

### Modified Capabilities

- `consulta-comprobante-arca`: enriquecer el detalle expuesto al frontend con descripciones operativas y garantizar consistencia del documento fiscal mostrado.

## Impact

- Afecta el contrato de respuesta de `POST /api/Comprobantes/Afip/Comprobante` de forma aditiva y compatible.
- Afecta DTOs, servicios y repositorios de `ElPrado.Dto`, `ElPrado.Services`, `ElPrado.Data` y `ElPrado.WebApi`.
- Requiere lecturas acotadas de las tablas maestras AFIP de Firebird; no modifica comprobantes ni datos de equivalencia.
