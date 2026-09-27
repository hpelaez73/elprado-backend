## 1. Relevamiento y extensión de hechos de dominio

- [x] 1.1 Construir una matriz, campo por campo, contra los JSON canónicos de `docs/mcp-api-v2-propuestas.md` (secciones 5 a 11), identificando para cada propiedad su DTO, servicio, repositorio y consulta Firebird de origen; verificar que la matriz no tenga campos sin fuente o tarea de extensión asignada. (ver `field-matrix.md`)
- [x] 1.2 Extender `ElPrado.Services`, DTOs, repositorios y consultas Firebird para completar detalle, titulares, historial y deuda, incluidos flags funcionales de deuda, parcela y lugar, totales, períodos y cobranza; verificar con casos del ERP que los valores se reciben estructurados y no se deducen de nombres configurables.
- [x] 1.3 Extender el origen de servicios para entregar todas las propiedades canónicas de habilitaciones, motivo, cupos, beneficiario, comprobante y propuestas origen/aplicación; verificar que no se publiquen servicios ausentes del producto ni se interpreten mensajes de presentación en McpApi.
- [x] 1.4 Extender el origen de contratos y lugares para completar todos los campos canónicos y proveer o confirmar claves inequívocas para lugar-inhumado y contrato-plan de venta; verificar que, cuando no existan, se aplique la variante canónica `null` o colección separada sin asociar por nombre, índice o posición.
- [x] 1.5 Extender el origen de comprobantes para soportar todos los filtros de dominio, campos de item y totales de paginación exigidos por el JSON canónico; verificar con una consulta real que filtros y metadatos no dependen de la estructura genérica del frontend.
- [x] 1.6 Incorporar una resolución reutilizable desde número público de propuesta a identificador interno y resultado no encontrado; verificar que las consultas v2 no reciban `CodPropuesta` como parámetro público.

## 2. Contratos y soporte HTTP v2

- [x] 2.1 Crear contratos de lectura v2 en `ElPrado.McpApi/Contracts` que reproduzcan campo por campo los siete JSON canónicos, incluidos envelope, objetos anidados, referencias de persona, estados funcionales, fechas opcionales e importes `decimal`; verificar mediante serialización que respetan nombres JSON, ISO 8601, `null` y colecciones vacías. (ver `ElPrado.Tests/PropuestasV2ContractsTests.cs`)
- [x] 2.2 Implementar el manejo común de validación, propuesta inexistente y fallos controlados para v2; verificar HTTP 400/`INVALID_REQUEST`, HTTP 404/`PROPOSAL_NOT_FOUND` y que no se filtren detalles internos.
- [x] 2.3 Agregar el grupo autenticado `GET /api/v2/propuestas/{propuesta}` sin modificar el grupo v1; verificar en Swagger y con una solicitud autenticada que la ruta pública no exige `CodPropuesta`.

## 3. Recursos semánticos de propuestas

- [x] 3.1 Implementar el mapper y endpoint de detalle v2 conforme al JSON canónico de la sección 5, con propuesta, deuda resumida, parcela, zonas, lugares normalizados, inhumados, asociadas y alertas; verificar una respuesta contra el JSON canónico y que no contiene colores, campos posicionales ni vínculos inferidos.
- [x] 3.2 Implementar el endpoint de titulares v2 conforme al JSON canónico de la sección 6, incluidos `cantidad`, documento, contacto y domicilio; verificar una respuesta contra el JSON canónico y que el indicador principal sigue la regla de dominio confirmada.
- [x] 3.3 Implementar el endpoint de deuda v2 conforme al JSON canónico de la sección 7, incluidos totales, períodos, importes y cobranza; verificar una respuesta contra el JSON canónico y, para una cuenta judicial, que `activa`, `alDia` y `permiteImputar` no dependen del nombre del estado.
- [x] 3.4 Implementar el endpoint de servicios v2 conforme al JSON canónico de la sección 8, con habilitaciones, motivos, cupos y utilizaciones; verificar respuestas para servicio no incluido, cupo disponible y uso entre propuesta origen/aplicación.
- [x] 3.5 Implementar el endpoint de contratos v2 conforme al JSON canónico de la sección 9 y la salida de titulares habilitados para factura/pago; verificar una respuesta contra el JSON canónico y que plan de venta sólo se anida con relación comprobable.
- [x] 3.6 Implementar el endpoint de historial de titulares v2 conforme al JSON canónico de la sección 11; verificar una respuesta contra el JSON canónico y que devuelve hechos registrados sin inferir transferencias a partir de las fechas.

## 4. Comprobantes y documentación

- [x] 4.1 Implementar el endpoint v2 de comprobantes conforme al JSON canónico de la sección 10, con parámetros `desde`, `hasta`, `tipo`, `estado`, `page` y `pageSize`, defaults y límites documentados; verificar validación de rango, filtros de dominio y traducción encapsulada al servicio existente.
- [x] 4.2 Mapear todos los campos canónicos de comprobantes, incluido `anulado` booleano y los metadatos de paginación; verificar una respuesta contra el JSON canónico y que no se exponen filtros genéricos ni identificadores internos innecesarios.
- [x] 4.3 Actualizar Swagger y `ElPrado.McpApi/README.md` con las siete rutas v2, autenticación, parámetros, defaults, envelope y códigos de error; verificar que la documentación no anuncie eliminación de v1.

## 5. Pruebas y validación de transición

- [x] 5.1 Agregar pruebas unitarias de mappers y serialización contra snapshots de los siete JSON canónicos, incluyendo nulos, colecciones vacías, decimales, fechas, estados configurables y ausencia de relaciones inequívocas; verificar que todas pasen con `dotnet test`.
- [x] 5.2 Agregar pruebas de integración HTTP para autorización, rutas GET v2, éxito, propuesta inexistente, parámetros inválidos y paginación; verificar status, envelope y contenido JSON de cada escenario contra el contrato canónico correspondiente.
- [x] 5.3 Ejecutar regresión de las siete rutas v1 y comparar muestras anonimizadas de v2 con el ERP; verificar que v1 conserva su contrato y que v2 representa los hechos autoritativos esperados.
- [x] 5.4 Ejecutar `dotnet build ElPrado.McpApi/ElPrado.McpApi.csproj` y `openspec validate add-mcp-api-v2-propuestas --strict`; verificar compilación y validación del change sin errores.
