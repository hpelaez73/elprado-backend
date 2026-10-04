## 1. Confirmar y modelar equivalencias locales

- [x] 1.1 Inspeccionar con consultas Firebird acotadas o DDL las columnas y descripciones de las tablas maestras AFIP necesarias para concepto, tipo de documento, tipo de comprobante, condicion IVA, moneda, resultado, tipo de emision, IVA y tributos; verificar que cada lectura se pueda filtrar por el comprobante solicitado.
- [x] 1.2 Agregar en `ElPrado.Data.Models` los modelos de lectura locales necesarios para el detalle enriquecido; verificar que representan solo datos usados por la respuesta.

## 2. Extender el contrato de detalle en ElPrado

- [x] 2.1 Agregar a `ElPrado.Dto.Dtos.DtoAfipWsfeConsultaDetalle` y sus DTOs anidados los campos descriptivos aditivos requeridos, incluida la condicion IVA del receptor; verificar que los campos tecnicos existentes no cambien de nombre ni tipo.
- [x] 2.2 Actualizar la documentacion OpenAPI o los contratos de prueba disponibles en ElPrado y verificar que la respuesta serializada incluya los nuevos campos sin eliminar los actuales.

## 3. Enriquecer la respuesta en ElPrado

- [x] 3.1 Incorporar en `ElPrado.Data.Interfaces` y `ElPrado.Data.Repositories.ComprobantesRepository` lecturas parametrizadas y acotadas de comprobante y equivalencias; verificar que un codigo sin equivalencia no genere error ni escritura en Firebird.
- [x] 3.2 Actualizar `ElPrado.Services.ComprobantesService` para solicitar el detalle tecnico mediante `IAfipWsfeGateway` y enriquecerlo con los datos locales; verificar que conserva codigos y completa descripciones cuando existen.
- [x] 3.3 Aplicar en `ElPrado.Services.ComprobantesService` la precedencia CUIT, documento alternativo y consumidor sin documento; verificar que el tipo y numero expuestos coincidan con los usados para emitir.
- [x] 3.4 Actualizar `ElPrado.WebApi.Controllers.ComprobantesController` para delegar la consulta enriquecida a `ComprobantesService`; verificar que Afip.WebApi siga encapsulada y no requiera cambios.

## 4. Validar comportamiento y compatibilidad

- [x] 4.1 Agregar o actualizar pruebas de ElPrado para equivalencia encontrada, equivalencia inexistente, receptor con CUIT y descripciones de IVA disponibles; verificar que las pruebas pasen.
- [x] 4.2 Ejecutar la compilacion de la solucion y las pruebas relevantes; verificar una consulta exitosa y una respuesta controlada ante error de Afip.WebApi sin cambios persistentes en el comprobante.
