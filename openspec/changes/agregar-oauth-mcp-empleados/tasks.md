## 1. Definiciones de seguridad y base técnica

- [ ] 1.1 Evaluar y seleccionar un servidor OAuth mantenido compatible con .NET 7, Authorization Code + PKCE S256, JWKS y Firebird/Dapper o stores personalizados; documentar la decisión y verificar un canje PKCE mínimo contra Firebird de desarrollo.
- [ ] 1.2 Definir la configuración por entorno de URLs canónicas de emisor y recurso MCP, claves de firma, tiempos de vida y `elprado:read`; verificar que `appsettings*.json`, logs y `.env.example` no contengan claves privadas, tokens ni contraseñas.
- [ ] 1.3 Definir el contrato de claims OAuth (`sub`, `iss`, `aud`, `scope`, `jti`, `CodUsuario`, `CodCliente`, `CodPropuesta`) y sus reglas de validación; verificarlo con pruebas de tokens válidos, vencidos, de audiencia errónea y sin scope.

## 2. Identidad y persistencia OAuth en el backend

- [ ] 2.1 Ajustar `ElPrado.Services/LoginService.cs` para exponer la validación de empleado que necesita OAuth sin emitir `DtoLogin` ni refresh tokens del frontend; agregar pruebas que preserven los flujos existentes de `ElPrado.WebApi`.
- [ ] 2.2 Implementar en `ElPrado.McpApi` un `IUserContextService` basado en el principal OAuth y pruebas para empleados y claims ausentes; verificar que el proyecto no dependa de `ElPrado.WebApi`.
- [ ] 2.3 Crear una migración pendiente de Firebird y repositorios/modelos para códigos de autorización, grants y refresh tokens OAuth con hashes, expiración, consumo, cliente, recurso, scopes, rotación y revocación; verificar la migración en una base de desarrollo y su reversión aprobada.
- [ ] 2.4 Implementar servicios de persistencia OAuth para emitir, consumir una vez, rotar y revocar credenciales; agregar pruebas para código repetido, refresh token rotado y revocación de familia.

## 3. Authorization server de ElPrado.McpApi

- [ ] 3.1 Registrar las dependencias y el pipeline de autenticación/autorización OAuth en `ElPrado.McpApi/Program.cs`; verificar que `/health` siga siendo anónimo y que una ruta MCP protegida rechace solicitudes sin Bearer.
- [ ] 3.2 Publicar discovery OAuth/OIDC y JWKS con issuer, endpoints, métodos de cliente y PKCE S256; verificar los documentos con un cliente OAuth y los valores de URL canónica.
- [ ] 3.3 Implementar autorización de empleados con pantalla/flujo seguro de alias y clave, validación de cliente, redirect URI, `state`, consentimiento y parámetro `resource`; verificar éxito y respuestas de error sin enumeración de cuentas.
- [ ] 3.4 Implementar el endpoint de token para canjear código PKCE y renovar tokens OAuth, emitiendo JWT asimétricos con audiencia MCP y scope `elprado:read`; verificar firma contra JWKS, expiración, audiencia y rotación de refresh tokens.
- [ ] 3.5 Agregar una ruta interna protegida de diagnóstico de contexto OAuth, sin exponer datos sensibles, para probar la propagación de identidad MCP a API; verificar que devuelve únicamente el sujeto y scopes del token autenticado.

## 4. Integración del resource server MCP

- [ ] 4.1 En `D:\Desarrollo\ElPrado\elprado-mcp\master`, publicar `/.well-known/oauth-protected-resource` y el desafío `WWW-Authenticate` para `/mcp`; verificar que un cliente sin token recibe `401` y descubre el authorization server.
- [ ] 4.2 Validar por solicitud en `src/server.ts` los JWT OAuth contra JWKS, issuer, audiencia MCP y scope `elprado:read`, conservando el token asociado a la sesión Streamable HTTP; verificar con pruebas de token válido, vencido, inválido y de audiencia distinta.
- [ ] 4.3 Adaptar `src/client.ts`, las tools y sus pruebas para usar el Bearer token de la sesión autenticada al invocar `ElPrado.McpApi`; verificar llamadas concurrentes de dos empleados sin mezclar Authorization headers.
- [ ] 4.4 Eliminar `ELPRADO_API_TOKEN` de `src/config.ts`, `.env.example`, README y pruebas del MCP; verificar `npm run typecheck`, `npm test` y `npm run build` sin un JWT de usuario en el entorno.

## 5. Verificación extremo a extremo y operación

- [ ] 5.1 Agregar pruebas de integración que recorran discovery, autorización PKCE de un empleado de prueba, canje, llamada MCP autenticada y propagación a la ruta protegida de `ElPrado.McpApi`; verificar que no se registren tokens, códigos, claves ni contraseñas.
- [ ] 5.2 Documentar HTTPS, callback/metadata de cliente de ChatGPT, URLs canónicas, variables de entorno, rotación de claves y procedimiento de revocación; verificar la guía con un registro de prueba en ChatGPT o un cliente OAuth equivalente.
- [ ] 5.3 Ejecutar `dotnet build ElPrado.McpApi/ElPrado.McpApi.csproj`, las pruebas .NET agregadas y, en `elprado-mcp`, `npm run typecheck`, `npm test` y `npm run build`; registrar los resultados y cualquier verificación que requiera infraestructura de despliegue.
