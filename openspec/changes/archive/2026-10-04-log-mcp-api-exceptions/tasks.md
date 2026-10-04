## 1. Manejo y registro de excepciones de MCP

- [x] 1.1 Incorporar en `ElPrado.McpApi` un límite central para excepciones no controladas que emita un evento Error de Serilog con excepción, método, ruta y estado HTTP; verificar con una prueba de integración que la respuesta pública sea segura y el evento sea capturado.
- [x] 1.2 Actualizar `PropuestasEndpointExtensions` para conservar y registrar las excepciones que hoy se traducen a `BUSINESS_OPERATION_FAILED`, sin cambiar el código, mensaje ni estado HTTP de la respuesta; verificar con una prueba que simule la falla y compruebe ambos resultados.
- [x] 1.3 Revisar los demás adaptadores de `ElPrado.McpApi` que puedan traducir excepciones a respuestas y aplicar el mismo registro una sola vez; verificar que las rutas existentes continúen devolviendo su contrato actual.

## 2. Seguridad y validación

- [x] 2.1 Asegurar que las propiedades de los eventos no incluyan cuerpo de solicitud, cabecera Authorization, contraseñas, tokens ni claves; verificarlo con una prueba de captura de eventos que envíe una credencial de prueba.
- [x] 2.2 Ejecutar `dotnet test ElPrado.Tests/ElPrado.Tests.csproj` y verificar los escenarios de fallo controlado y no controlado, la preservación de las respuestas públicas y la emisión de los eventos Error.
