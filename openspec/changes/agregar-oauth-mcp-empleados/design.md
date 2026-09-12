## Context

Véase `proposal.md` y `specs/mcp-oauth-authentication/spec.md`. `elprado-mcp` ya atiende Streamable HTTP en `/mcp`, pero su cliente REST exige `ELPRADO_API_TOKEN`. `ElPrado.McpApi` es un host .NET 7 sin rutas de negocio todavía; el login de empleados existente está encapsulado en `LoginService`, cuyo método actual también crea refresh tokens propios de la aplicación.

## Goals / Non-Goals

**Goals:**

- Implementar una frontera OAuth interoperable con ChatGPT y con la especificación de autorización MCP.
- Hacer que las APIs para MCP reciban y apliquen identidad individual verificable.
- Mantener la autenticación del frontend y sus refresh tokens actuales sin cambios de contrato.
- Diseñar claims y persistencia que admitan clientes en una entrega posterior.

**Non-Goals:**

- Exponer el protocolo MCP desde `ElPrado.McpApi`.
- Habilitar login de clientes, mutaciones por MCP, ni scopes de escritura.
- Conceder acceso mediante client credentials, API keys de usuario o JWT fijo.
- Mover el adaptador HTTP `UserContextService` a `ElPrado.Services`.

## Decisions

- `elprado-mcp` será el resource server y `ElPrado.McpApi` el authorization server y API de negocio protegida.
  - El resource metadata y el desafío `WWW-Authenticate` residen en el dominio MCP, ya que ChatGPT se conecta allí. Discovery, autorización, token y JWKS residen en la API .NET.
  - Se descarta concentrar todo en `ElPrado.McpApi`: dejaría al endpoint MCP sin el descubrimiento y validación requeridos por el cliente.

- Usar Authorization Code con PKCE S256, `resource` obligatorio y un cliente público identificado por metadata de cliente o registro dinámico compatible con ChatGPT.
  - El access token será JWT firmado asimétricamente, de vida corta, con `iss`, `aud` igual a la URL canónica MCP, `sub=empleado:<codUsuario>`, `scope=elprado:read`, `jti` y claims de compatibilidad `CodUsuario`, `CodCliente=0`, `CodPropuesta=0`.
  - El MCP verificará firma y claims contra JWKS y reenviará el mismo Bearer token a `ElPrado.McpApi` por solicitud. Se descarta token opaco con introspección porque añade una llamada crítica por tool y acopla la disponibilidad del MCP al authorization server.

- Reutilizar `LoginService` para validar alias y clave, pero separar esa validación de la emisión de `DtoLogin` y de los refresh tokens de frontend.
  - La extracción conservará el registro de acceso y devolverá la identidad mínima que necesita OAuth. Los tokens OAuth se emitirán y almacenarán por su ciclo de vida específico.
  - Se descarta llamar al método de login actual y descartar su refresh token: generaría credenciales huérfanas y mezclaría dos contratos de sesión.

- Crear persistencia OAuth propia en Firebird para autorizaciones/códigos y refresh tokens, con hashes de secretos, cliente, sujeto, recurso, scopes, expiración, consumo, rotación y revocación.
  - Los códigos y refresh tokens no se almacenarán en texto plano. La revocación por reutilización invalida la familia de refresh tokens correspondiente.
  - Se descarta adaptar `RefreshTokens`: su esquema actual no almacena datos OAuth indispensables y debe seguir siendo compatible con el frontend.

- Crear una implementación de `IUserContextService` en `ElPrado.McpApi` que lea el principal autenticado por OAuth, y compartir solo constantes o un parser de claims cuando resulte necesario.
  - Cada host mantiene su adaptador de transporte; `ElPrado.Services` conserva la abstracción, sin depender de `HttpContext` ni de un formato JWT concreto.
  - Se descarta mover la implementación existente desde `ElPrado.WebApi` a servicios por acoplar la capa de negocio a ASP.NET.

- Seleccionar en un spike inicial un servidor OAuth mantenido que sea compatible con .NET 7, PKCE, JWKS y almacenamiento Firebird/Dapper o stores personalizados; no se implementarán endpoints OAuth criptográficos ad hoc.
  - La elección debe documentar soporte de los requisitos MCP/ChatGPT, rotación de claves y viabilidad real con Firebird antes de integrar la dependencia.

## Risks / Trade-offs

- [La biblioteca OAuth no ofrece store Firebird compatible] → completar el spike antes de desarrollar el flujo; elegir stores personalizados soportados o un proveedor de identidad separado, sin degradar requisitos OAuth.
- [Los tokens OAuth y de frontend se confunden] → usar endpoints, entidades, nombres de configuración y claims claramente separados; no aceptar tokens de frontend en `/mcp`.
- [La URL canónica del MCP cambia entre entornos] → configurar el identificador de recurso explícitamente y validar coincidencia exacta de `aud` y `resource`.
- [Un token queda expuesto en logs] → redacción obligatoria de Authorization, códigos, claves y refresh tokens en ambos repositorios.
- [Las tools existentes todavía consumen `ElPrado.WebApi`] → migrar cada operación a su adaptador en `ElPrado.McpApi`; no propagar tokens OAuth a endpoints del frontend como solución permanente.
- [La futura identidad de cliente requiere más datos] → usar prefijos de sujeto y claims opcionales sin emitir ni aceptar sujetos cliente hasta su change dedicada.

## Migration Plan

1. Resolver proveedor OAuth, modelo Firebird y configuración de claves en desarrollo sin secretos versionados.
2. Implementar y probar authorization server, login de empleados, JWKS y validación de rutas protegidas en `ElPrado.McpApi`.
3. Implementar metadata, desafío y validación de token por solicitud en `elprado-mcp`; retirar `ELPRADO_API_TOKEN` de configuración y documentación.
4. Migrar una primera operación MCP a una ruta equivalente de `ElPrado.McpApi` y comprobar la identidad extremo a extremo desde un cliente OAuth de prueba antes de registrar ChatGPT.
5. Desplegar con HTTPS y claves administradas, registrar el MCP en ChatGPT y verificar el flujo real.
6. Rollback: retirar el registro/conexión MCP y rechazar tokens OAuth; el login y JWT existentes de `ElPrado.WebApi` permanecen sin cambios. Conservar las tablas OAuth para auditoría o ejecutar una migración de reversión aprobada si el despliegue no llega a usarse.

## Open Questions

- Qué biblioteca/proveedor OAuth satisface los requisitos con Firebird/Dapper y la política de claves del despliegue. El spike de la primera tarea debe decidirlo antes de integrar dependencias.
- Cuál será la URL HTTPS canónica definitiva de producción para el recurso MCP y el emisor OAuth; debe fijarse antes del registro de ChatGPT, sin alterar el contrato funcional.
