## Context

La fachada actual ya ofrece siete operaciones autenticadas bajo `/api/propuestas`, pero usa `POST`, mezcla el número de propuesta con `CodPropuesta` y expone proyecciones cercanas a DTO de WebApi (lugares posicionales, colores, mensajes y filtros genéricos). Ver `proposal.md` para la motivación y `specs/mcp-proposal-v2-contracts/spec.md` para el contrato observable.

## Goals / Non-Goals

**Goals:**

- Añadir v2 como una superficie REST de sólo lectura y mantener v1 aislada.
- Implementar sin desviaciones los JSON canónicos de `docs/mcp-api-v2-propuestas.md` para cada endpoint.
- Construir contratos de fachada propios y mappers explícitos que preserven hechos del ERP.
- Cerrar mediante cambios en Services, DTOs, repositorios y consultas Firebird cualquier brecha de datos que impida cumplir un campo canónico.
- Cubrir serialización, errores, mapeos y rutas con pruebas de contrato.

**Non-Goals:**

- Migrar tools del servidor `elprado-mcp`, retirar v1 o modificar ElPrado.WebApi.
- Deducir reglas de negocio desde textos configurables, nombres, colores o posiciones de listas.
- Implementar todavía la búsqueda `/buscar`, operaciones de escritura o protocolo MCP.

## Decisions

### Grupo de rutas v2 separado

Se agregará un grupo autenticado `/api/v2/propuestas` con handlers `GET` y parámetros tipados. V1 no se reutilizará como contrato ni se modificará: ambos grupos podrán compartir adaptadores de dominio, pero tendrán contratos y mappers distintos. Esto habilita una transición gradual y rollback por configuración del consumidor. La alternativa de cambiar v1 rompería a `elprado-mcp` y descartaría la convivencia requerida.

### JSON diseñados como fuente normativa de los contratos

Los ejemplos JSON de `docs/mcp-api-v2-propuestas.md`, secciones 3 y 5 a 11, son parte del diseño del change: fijan el contrato público de cada endpoint. Los records v2, serialización, Swagger y pruebas de contrato deberán coincidir con ellos en nombres, anidamiento, nulabilidad, colecciones y tipos. No se permitirá sustituir un campo por una aproximación proveniente de los DTO actuales. Mantener los JSON sólo como documentación de referencia se descarta porque vuelve ambigua la aceptación de la implementación.

### Contratos de lectura propios y envelope común

Se crearán records v2 específicos, con tipos `decimal`, `DateOnly`/nulos y colecciones normalizadas; el envelope existente se conservará como forma común de éxito/error. Los nombres JSON se validarán contra el contrato v2 mediante pruebas. Reutilizar directamente DTO de `ElPrado.Dto` se descarta porque acopla el contrato de agentes a necesidades del frontend y arrastra datos visuales.

### Resolución de propuesta antes de dependencias por código interno

Un adaptador o servicio de consulta v2 resolverá el número público una vez y suministrará el código interno sólo a servicios de dominio que lo necesiten. Los handlers no expondrán ese paso ni pedirán `CodPropuesta`. Invocar cada operación con inputs distintos conservaría la inconsistencia actual y dificulta el manejo uniforme de `PROPOSAL_NOT_FOUND`.

### Hechos estructurados provienen del dominio y se completan en origen

Los mappers v2 sólo renombran, agrupan, normalizan y convierten tipos. Cuando los servicios actuales no provean flags de estado, motivos de habilitación, relaciones de lugar-inhumado, vínculo contrato-plan o cualquier otro campo canónico, se extenderán Services, DTOs, repositorios y consultas Firebird para entregarlo desde el ERP. Un endpoint no se considerará terminado si omite un campo canónico por una brecha del origen. El fallback de analizar textos, nombres o índices se rechaza porque sería frágil ante configuración y produciría inferencias incorrectas.

### Comprobantes con filtro de borde y traducción encapsulada

La API v2 aceptará filtros de negocio y paginación, validará rangos y límites en el borde, y los traducirá internamente al mecanismo existente mientras éste sea necesario. El mecanismo `Campo`/`TipoComparacion` no formará parte del contrato. Se documentarán defaults y un máximo de `pageSize`; cualquier filtro que no pueda traducirse sin pérdida requerirá una extensión explícita del servicio antes de publicarse.

### Contratos y planes preservan relaciones verificables

El mapper anidará `planVenta` sólo si la fuente trae una clave de relación comprobable. Sin ella retornará colecciones separadas. Asociar por orden reduce trabajo inicial, pero viola la semántica del contrato y se descarta.

## Risks / Trade-offs

- [Los servicios actuales sólo exponen textos o formatos de frontend] → inventariar cada campo v2 y extender Services, repositorios o consultas Firebird hasta disponer de la fuente autoritativa; no publicar una variante incompleta del contrato.
- [Los procedimientos Firebird no admiten todos los filtros de comprobantes] → validar capacidades antes de publicar filtros; aplicar paginación/filtros sólo donde el origen mantenga totales correctos.
- [La normalización de lugares no conserva un identificador del inhumado] → publicar `inhumado: null` en lugar de asociar por nombre y registrar la brecha del dominio.
- [Divergencia entre v1 y v2 durante la transición] → pruebas de regresión de v1 y pruebas de contrato v2 independientes con casos reales anonimizados.
- [Cambio accidental de formato monetario o fecha] → configurar y probar la serialización HTTP, incluyendo nulos, decimales y colecciones vacías.

## Migration Plan

1. Construir una matriz campo a campo de cada JSON canónico contra Services, DTOs, repositorios y consultas Firebird, y extender el origen para cada brecha.
2. Incorporar contratos, resolución de propuesta, mappers y rutas v2 protegidas, sin modificar las rutas v1.
3. Ejecutar pruebas unitarias/de integración contra casos representativos del ERP y publicar Swagger/documentación de parámetros y errores.
4. Habilitar el consumo de v2 en un entorno de prueba y comparar respuestas con el ERP antes de migrar tools MCP en un cambio posterior.
5. Para rollback, restaurar el consumidor a v1 o deshabilitar el uso de v2; v1 permanece desplegada e intacta.
