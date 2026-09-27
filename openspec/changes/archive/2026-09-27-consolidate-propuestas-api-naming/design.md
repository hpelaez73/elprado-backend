## Context

La aplicación registra hoy dos extensiones de rutas de propuestas en el mismo grupo `/api`: una heredada con contratos MCP propios y otra vigente bajo `/v2/propuestas`. Sus contratos, DTOs de soporte y pruebas incorporan `V2` en el nombre aunque ya representan la única fachada de propuestas que quedará publicada. Véanse `proposal.md` y las deltas de especificación para el cambio de superficie externa.

## Goals / Non-Goals

**Goals:**

- Dejar una única implementación y un único conjunto de contratos de propuestas en `ElPrado.McpApi`.
- Renombrar los tipos C# y sus referencias asociados a la fachada vigente sin cambiar el JSON ni los recursos HTTP de esa fachada.
- Eliminar el registro, archivos, contratos y pruebas exclusivos de la fachada heredada.

**Non-Goals:**

- No mover `/api/v2/propuestas` a otra URL ni cambiar su autenticación, envelope o payloads.
- No cambiar procedimientos, tablas o datos Firebird por una decisión de nomenclatura de C#.
- No mantener adaptadores, aliases ni rutas de compatibilidad para la fachada retirada.

## Decisions

### Consolidar la fachada vigente en los nombres sin versión

El archivo y la clase de endpoint vigentes pasarán a ser `PropuestasEndpointExtensions`; el conjunto de contratos vigente pasará a ser `PropuestasContracts`. Sus records y clases auxiliares perderán el segmento `V2`, y se actualizarán sus referencias, mapeos, pruebas y documentación interna.

Esto deja al código expresar el dominio actual en lugar de una transición histórica. Se descarta conservar aliases o wrappers `V2`, porque perpetuarían los objetos duplicados que el cambio busca retirar.

### Retirar completamente la fachada heredada

Se eliminarán el endpoint, contratos y pruebas que sólo daban soporte a `/api/propuestas`, y el registro principal invocará únicamente la extensión consolidada. El resultado intencional es que esas rutas ya no se publiquen.

Se descarta redirigirlas o conservarlas en paralelo: ambas alternativas preservan un contrato que el cambio declara obsoleto y pueden ocultar consumidores sin migrar.

### Mantener la versión como contrato HTTP, no como nombre C#

La ruta `/api/v2/propuestas` y la documentación que describe su contrato se conservan. Los nombres de tipos C#, archivos y pruebas no forman parte del contrato JSON ni de la URL y se limpian; las referencias a versiones externas (ruta y objetos de base de datos existentes) permanecen cuando son necesarias para compatibilidad operativa.

## Risks / Trade-offs

- [Consumidores que invocan `/api/propuestas`] → Documentar el cambio incompatible, retirar las rutas en una entrega coordinada y comprobar mediante pruebas de integración que ya no se registran.
- [Renombrado incompleto deja referencias `V2` o rompe compilación] → Buscar en toda la solución, actualizar referencias atómicas y compilar con la suite de contratos e integración.
- [Confundir nombres C# con contrato HTTP] → Mantener pruebas que ejerciten exactamente `/api/v2/propuestas` y comparar su envelope y JSON canónico.

## Migration Plan

1. Renombrar y consolidar el código de la fachada vigente; eliminar el código heredado y actualizar el registro.
2. Actualizar consumidores, documentación y pruebas al nombre consolidado, conservando las URLs vigentes.
3. Ejecutar compilación y pruebas de `ElPrado.Tests`; verificar que las rutas v2 responden y que las heredadas no están registradas.
4. Revertir mediante el commit previo si se detecta un consumidor no migrado; no habrá compatibilidad parcial dentro de la versión resultante.
