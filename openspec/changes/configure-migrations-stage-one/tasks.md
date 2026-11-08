# Tareas

## 1. Revisión y contratos compartidos

- [x] 1.1 Revisar las referencias, `Program.cs`, extensiones de infraestructura, configuraciones y paquetes de API, Agent y Contracts; verificar que los tres proyectos permanezcan en `net7.0` con `dotnet build` individual.
- [x] 1.2 Confirmar que API y Agent referencian `ElPrado.Migraciones.Contracts` y que Contracts no incorpora DTOs, infraestructura ni dependencias nuevas; verificar mediante revisión de `.csproj` y compilación de Contracts.

## 2. Infraestructura de la API

- [x] 2.1 Organizar los registros de Swagger y Health Checks en `ElPrado.Migraciones.Api/Infrastructure/DependencyInjection.cs`; verificar que el proyecto compile sin introducir dependencias innecesarias.
- [x] 2.2 Reemplazar la respuesta manual de `GET /health` por el mapeo estándar de Health Checks; verificar HTTP 200 al ejecutar la API sana en Development.
- [x] 2.3 Mantener Swagger y Swagger UI condicionados a Development; verificar que la UI y el documento OpenAPI estén disponibles en Development y no se expongan al iniciar fuera de Development.
- [x] 2.4 Ajustar el arranque y Serilog de la API para registrar solicitudes HTTP, inicio, detención y fallos críticos sin duplicar providers ni información sensible; verificar logs por consola al iniciar y al solicitar `/health`.
- [x] 2.5 Mantener `appsettings.json` y `appsettings.Development.json` de la API con niveles por entorno y Seq opcional; verificar que `Seq__ServerUrl` sea reconocida y que la API inicie sin esa variable.
- [x] 2.6 Documentar en `docs/migraciones/` la ejecución de API, Swagger, Health Check y configuración de Serilog/Seq; verificar los comandos documentados contra la API local.

## 3. Infraestructura del Agent

- [x] 3.1 Mantener el registro del Worker y de servicios de host en `ElPrado.Migraciones.Agent/Infrastructure/DependencyInjection.cs` sin abstracciones prematuras; verificar la compilación del proyecto.
- [x] 3.2 Confirmar que `AgentWorker` respeta el `CancellationToken` y registra inicio y detención controlados sin conectar a la API ni ejecutar migraciones; verificar al iniciar el Agent y finalizarlo de forma controlada.
- [x] 3.3 Mantener la compatibilidad de host con Windows Service y systemd; verificar que las extensiones correspondientes continúan registradas y que el Agent inicia como proceso de consola.
- [x] 3.4 Ajustar Serilog y `appsettings*.json` del Agent para consola, niveles por entorno y Seq opcional; verificar logs de inicio sin Seq y el reconocimiento de `Seq__ServerUrl` cuando se configure.
- [x] 3.5 Completar la documentación de Agent, variables de entorno y procedimiento de detención en `docs/migraciones/`; verificar que los comandos de inicio y detención descritos reproduzcan los logs esperados.

## 4. Validación integral

- [x] 4.1 Ejecutar `dotnet restore` y `dotnet build` para Contracts, API y Agent, y compilar `ElPradoWeb.sln` si el entorno lo permite; registrar resultados y separar fallos ajenos a Migraciones.
- [x] 4.2 Ejecutar la API en Development y comprobar el estado HTTP de `/health`, el acceso a Swagger y los logs de solicitud; registrar URLs, comandos y resultados reales en la documentación.
- [x] 4.3 Ejecutar y detener el Agent de forma controlada; comprobar los logs de ciclo de vida y registrar el resultado real en la documentación.
