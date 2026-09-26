## 1. Dependencias y logging centralizado

- [x] 1.1 Agregar a `ElPrado.McpApi.csproj` las referencias compatibles con .NET 7 para `Serilog.Sinks.Seq` y Swagger/OpenAPI, alineadas con `ElPrado.WebApi`, y verificar que `dotnet restore ElPrado.McpApi/ElPrado.McpApi.csproj` finaliza correctamente.
- [x] 1.2 Conservar la carga de configuración Serilog existente para resolver el destino Seq declarado en `appsettings` y verificar en Development que el evento estructurado de inicio de `ElPrado.McpApi` aparece en la instancia Seq configurada.

## 2. OpenAPI y Swagger UI

- [x] 2.1 Registrar el explorador de endpoints y el generador Swagger en `ElPrado.McpApi/Program.cs`, con un esquema de seguridad HTTP Bearer JWT equivalente al de `ElPrado.WebApi`, y verificar que el documento OpenAPI incluye las rutas de login, health y negocio.
- [x] 2.2 Incorporar al pipeline Swagger y Swagger UI condicionados a Development o Staging, sin cambiar autenticación, autorización ni CORS, y verificar que la UI permite autorizar una operación protegida con un JWT válido.
- [x] 2.3 Verificar al iniciar con ambiente Production que `/swagger` y el documento OpenAPI no se exponen, mientras `/health` y las rutas existentes mantienen su comportamiento.

## 3. Validación de integración

- [x] 3.1 Compilar `ElPrado.McpApi` con `dotnet build ElPrado.McpApi/ElPrado.McpApi.csproj --no-restore` y corregir errores o advertencias introducidos por las dependencias y la configuración nuevas.
- [x] 3.2 Ejecutar las pruebas automatizadas disponibles para `ElPrado.McpApi` (o documentar que no existen) y realizar una comprobación manual de Swagger en Development y de la ausencia de Swagger en Production.
