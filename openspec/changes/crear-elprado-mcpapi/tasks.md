## 1. Estructura y dependencias

- [x] 1.1 Crear `ElPrado.McpApi` como proyecto web `net7.0` con Minimal APIs, nullable e implicit usings, y verificar que `dotnet build ElPrado.McpApi/ElPrado.McpApi.csproj` compila el proyecto.
- [x] 1.2 Agregar `ElPrado.McpApi` y sus configuraciones Debug/Release a `ElPradoWeb.sln`, y verificar que `dotnet sln ElPradoWeb.sln list` lo incluye.
- [x] 1.3 Agregar solo las referencias de proyectos compartidos necesarias para la fachada (`ElPrado.Services`, `ElPrado.Data`, `ElPrado.DataFactory`, `ElPrado.Dto` y `ElPrado.Core` cuando corresponda), y verificar que el restore no incorpora SDKs o paquetes de protocolo MCP.

## 2. Host, infraestructura y contratos base

- [x] 2.1 Configurar `Program.cs` de `ElPrado.McpApi` para cargar `appsettings.json`, `appsettings.{Environment}.json` y variables de entorno en ese orden de precedencia, y verificar que una variable de entorno reemplaza un valor de prueba.
- [x] 2.2 Crear los archivos de configuracion de `ElPrado.McpApi` con logging, origenes CORS opcionales y placeholders no sensibles, y verificar que no contienen cadenas de conexion, claves ni secretos reales.
- [x] 2.3 Configurar Serilog desde el host siguiendo el patron de `ElPrado.WebApi`, y verificar al iniciar que se emite un evento de arranque sin exponer valores sensibles.
- [x] 2.4 Registrar de forma incremental la infraestructura Firebird y los servicios de dominio compartidos que usaran las rutas de esta fachada, sin copiar controllers, autenticacion JWT, Swagger ni Data Protection innecesarios, y verificar que el contenedor inicia sin errores de DI.
- [x] 2.5 Definir los modelos base de respuesta y error de `ElPrado.McpApi` para que las futuras rutas entreguen datos o errores con codigo y mensaje, y verificar con una respuesta de prueba que no reutilizan el contrato HTTP de `ElPrado.WebApi`.
- [x] 2.6 Mantener CORS deshabilitado si no hay origenes configurados y aplicar una politica restrictiva solo cuando existan origenes permitidos, y verificar que una solicitud cross-origin no configurada no recibe acceso abierto.

## 3. Endpoints de la fachada

- [x] 3.1 Mapear `GET /health` como health check de proceso sin consulta a Firebird, y verificar que devuelve una respuesta HTTP exitosa mientras el servicio esta iniciado.
- [x] 3.2 Establecer la estructura de grupos Minimal API y adaptadores delgados para futuras operaciones de negocio, y verificar que no exponen `/mcp` ni rutas de protocolo MCP.
- [x] 3.3 Documentar junto al proyecto que `elprado-mcp` consume esta API y conserva la responsabilidad de MCP, tools, transporte y OAuth, y verificar que la documentacion no prescribe `ModelContextProtocol.AspNetCore` ni un JWT fijo en `.env`.

## 4. Validacion y entrega operativa

- [ ] 4.1 Compilar la solucion con `dotnet build ElPradoWeb.sln` y corregir errores de referencias, framework o DI sin modificar contratos de `ElPrado.WebApi`, `Afip.WebApi` ni `ElPrado.Workers`.
- [x] 4.2 Ejecutar `ElPrado.McpApi` con configuracion de prueba y verificar extremo a extremo `GET /health`, el formato de respuesta base y la ausencia de rutas MCP.
- [ ] 4.3 Ejecutar `openspec validate crear-elprado-mcpapi --strict` y corregir todos los errores de validacion reportados.
