## Context

`ElPrado.McpApi` ya configura Serilog desde configuración y tiene adaptadores mínimos que traducen fallas de negocio a respuestas `ApiResponse<T>`. En las rutas de propuestas, las excepciones capturadas se convierten en respuestas controladas sin conservar la excepción; otras rutas pueden dejar excepciones sin un punto uniforme que las registre. Véanse `proposal.md` y las especificaciones de esta modificación para la motivación y el contrato.

## Goals / Non-Goals

**Goals:**

- Registrar una única vez cada excepción que cause una respuesta de error, con excepción, método, ruta y estado HTTP.
- Preservar los códigos, mensajes y estados HTTP que hoy reciben los consumidores.
- Asegurar que una excepción no controlada también sea registrada antes de emitir una respuesta de error segura.

**Non-Goals:**

- Cambiar contratos de rutas, autenticación, reglas de negocio o la configuración de sinks existente.
- Registrar cuerpos completos, encabezados de autorización, tokens, contraseñas o claves.
- Extender este comportamiento a `ElPrado.WebApi` o `Afip.WebApi`.

## Decisions

### Centralizar el límite de excepciones HTTP

Se incorporará un manejador de excepciones en la tubería de `ElPrado.McpApi` para registrar las no controladas y devolver una respuesta segura y consistente. Es el límite común que cubre las rutas actuales y futuras, sin duplicar `try/catch` de infraestructura en cada endpoint.

Se descarta depender exclusivamente del registro de excepciones no controladas del host: no permite controlar de forma explícita la respuesta ni garantizar las propiedades de contexto requeridas.

### Registrar al traducir excepciones ya controladas

Los adaptadores que convierten excepciones en errores de negocio conservarán explícitamente la excepción y la registrarán en el momento de la traducción, junto con el contexto de la solicitud. Esto evita que la excepción quede absorbida antes de alcanzar el manejador central y evita volver a lanzarla sólo para registrarla.

Se descarta registrar únicamente en el manejador central porque las excepciones capturadas en los adaptadores nunca llegarían a él.

### Contexto mínimo y seguro

Los eventos incluirán método HTTP, ruta y estado HTTP; cuando esté disponible, se asociará la operación o identificador de correlación de la solicitud. No se registrarán cuerpos, cabeceras de autorización ni datos de credenciales. El detalle técnico permanece en el objeto de excepción de Serilog, no en la respuesta pública.

## Risks / Trade-offs

- [Doble registro de una misma excepción] → Las excepciones capturadas por adaptadores se registrarán allí y no se volverán a propagar; el manejador central registrará sólo las que lo alcancen.
- [Información sensible en propiedades de log] → Se limitarán las propiedades al contexto técnico mínimo y se excluirán cuerpos y cabeceras sensibles.
- [Respuesta ya iniciada] → El manejador central comprobará si la respuesta ya comenzó antes de intentar escribir una respuesta de error y conservará el registro de la excepción en todos los casos.

## Migration Plan

1. Desplegar el cambio con la configuración actual de Serilog/Seq.
2. Provocar en un ambiente no productivo una falla controlada y otra no controlada, verificando evento Error y respuesta pública.
3. Si se detecta impacto en respuestas, revertir el manejo central y los registros de adaptadores; los sinks y la configuración existente permanecen sin cambios.
