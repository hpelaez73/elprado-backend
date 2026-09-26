## Context

Ver `proposal.md` para la motivación y `specs/mcp-api-observability-documentation/spec.md` para el contrato. `ElPrado.McpApi` ya construye Serilog desde su configuración y contiene una sección `Serilog` con un destino Seq, pero su proyecto no referencia el paquete que implementa ese destino. La API Minimal actual no registra servicios de exploración de endpoints ni Swagger. `ElPrado.WebApi` es el patrón hermano: usa Serilog configurado, el sink Seq, Swagger con JWT Bearer y habilita la UI sólo en Development/Staging.

## Goals / Non-Goals

**Goals:**

- Completar la cadena de logging ya configurada para que los eventos de la API MCP lleguen a Seq.
- Documentar las rutas Minimal API actuales con OpenAPI/Swagger y permitir ingresar un JWT Bearer en la UI.
- Mantener la misma política de exposición por ambiente que `ElPrado.WebApi`.

**Non-Goals:**

- No modificar contratos, rutas, políticas de autorización, CORS ni la lógica de negocio de la API MCP.
- No cambiar la URL, credenciales, niveles o filtros de Seq ya administrados por configuración.
- No exponer Swagger en Production ni introducir un portal de documentación externo.
- No incorporar logging de solicitudes HTTP adicional ni rediseñar el formato de eventos existente.

## Decisions

- Agregar las referencias de paquetes de Seq y Swagger/OpenAPI compatibles con .NET 7, alineadas con las versiones ya utilizadas por `ElPrado.WebApi`.
  - Esto permite que `ReadFrom.Configuration` resuelva el destino `Seq` declarado sin código de logging específico y conserva la consistencia de mantenimiento entre fachadas.
  - Se descarta configurar el sink mediante código: duplicaría valores que ya pertenecen a `appsettings` y a variables de entorno.

- Registrar el explorador de endpoints y el generador Swagger durante la configuración de servicios; declarar un esquema de seguridad HTTP `bearer` con formato JWT y aplicarlo a la documentación.
  - La UI podrá enviar el token para ejercitar las rutas agrupadas bajo `/api`, que actualmente requieren autorización, sin alterar el comportamiento de autenticación del pipeline.
  - Se descarta documentar un mecanismo de autenticación propio o cambiar los endpoints para satisfacer Swagger: la documentación debe reflejar el contrato existente.

- Activar `UseSwagger` y `UseSwaggerUI` sólo cuando el ambiente sea Development o Staging.
  - Replica el comportamiento de `ElPrado.WebApi` y evita incorporar superficies de exploración en Production.
  - Se descarta habilitar Swagger de forma permanente o mediante una bandera nueva, porque ampliaría la política de despliegue solicitada.

## Risks / Trade-offs

- [Seq no está disponible o su URL es inválida] → Mantener la configuración por ambiente y validar conectividad/ingesta durante el despliegue; la API conserva sus rutas funcionales sin depender de Swagger.
- [La descripción automática de Minimal APIs puede ser limitada para respuestas tipadas] → Verificar en Swagger las operaciones actuales y añadir metadata sólo si hace falta para representar el contrato existente, sin cambiarlo.
- [Un token real usado en Swagger UI puede quedar expuesto en una estación compartida] → Restringir la UI a Development/Staging y operar con credenciales de prueba o de privilegio mínimo.

## Migration Plan

1. Incorporar las dependencias y la configuración de servicios/pipeline en `ElPrado.McpApi`.
2. Restaurar paquetes y compilar la solución en .NET 7.
3. En Development, iniciar la API, comprobar la llegada del evento de inicio a Seq y abrir Swagger UI; autenticar una ruta protegida con un JWT válido.
4. En Production, verificar que las rutas de documentación no se exponen y que el arranque conserva la configuración actual de Kestrel.
5. Rollback: revertir las referencias de paquetes y el registro de Swagger; no hay migraciones de datos ni cambios de contratos que revertir.
