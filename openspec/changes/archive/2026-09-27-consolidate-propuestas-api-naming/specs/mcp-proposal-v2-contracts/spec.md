## MODIFIED Requirements

### Requirement: Superficie v2 autenticada y compatible
ElPrado.McpApi SHALL exponer, con JWT de empleado válido, los recursos `GET /api/v2/propuestas/{propuesta}`, `/titulares`, `/deuda`, `/servicios`, `/contratos`, `/comprobantes` e `/historial-titulares`. La ruta SHALL usar el número público `propuesta`; la API SHALL resolver internamente cualquier `CodPropuesta` necesario. Las rutas heredadas bajo `/api/propuestas` SHALL NOT estar disponibles.

#### Scenario: Consulta v2 autenticada por nÃºmero pÃºblico
- **WHEN** un consumidor autenticado solicita un recurso v2 con una propuesta existente
- **THEN** recibe el recurso solicitado sin requerir conocer el identificador interno de la propuesta

#### Scenario: Consulta v2 sin autenticaciÃ³n
- **WHEN** un consumidor solicita un recurso v2 sin un JWT válido
- **THEN** la API responde HTTP 401 sin ejecutar la consulta de negocio

#### Scenario: Ruta heredada retirada
- **WHEN** un consumidor invoca una ruta de propuestas bajo `/api/propuestas`
- **THEN** la API no publica esa operación heredada
