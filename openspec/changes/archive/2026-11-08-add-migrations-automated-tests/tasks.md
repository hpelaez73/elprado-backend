# Tareas

## 1. Proyecto de pruebas de API

- [x] 1.1 Crear `tests/ElPrado.Migraciones.Api.Tests` para `net7.0`, con xUnit, `Microsoft.NET.Test.Sdk`, `Microsoft.AspNetCore.Mvc.Testing` compatible y referencia a `ElPrado.Migraciones.Api`; verificar que `dotnet test tests/ElPrado.Migraciones.Api.Tests` restaure y compile.
- [x] 1.2 Ajustar el punto de entrada de `ElPrado.Migraciones.Api` solo lo necesario para que `WebApplicationFactory<Program>` pueda crear el host; verificar que la API continúa compilando en `net7.0`.
- [x] 1.3 Implementar una fábrica de pruebas en memoria que no inicialice servicios externos y pruebas de `GET /health` para HTTP 200, acceso sin autenticación e inicialización sin Firebird ni Seq; verificar la ejecución completa de la suite de API.

## 2. Proyecto de pruebas de Agent

- [x] 2.1 Crear `tests/ElPrado.Migraciones.Agent.Tests` para `net7.0`, con xUnit, `Microsoft.NET.Test.Sdk` y referencia a `ElPrado.Migraciones.Agent`; verificar que `dotnet test tests/ElPrado.Migraciones.Agent.Tests` restaure y compile.
- [x] 2.2 Implementar pruebas con host en memoria que comprueben el registro de `AgentWorker`, su inicio, respuesta a cancelación y detención sin errores, usando señales asíncronas deterministas y sin API remota; verificar la ejecución completa de la suite de Agent sin `Thread.Sleep`.

## 3. Integración y documentación

- [x] 3.1 Agregar ambos proyectos de pruebas a `ElPradoWeb.sln`; verificar que la solución reconoce los dos proyectos de test.
- [x] 3.2 Documentar en `docs/migraciones/` los comandos para ejecutar las suites desde terminal y VS Code; verificar que los comandos documentados son `dotnet test tests/ElPrado.Migraciones.Api.Tests` y `dotnet test tests/ElPrado.Migraciones.Agent.Tests`.
- [x] 3.3 Ejecutar ambas suites con los comandos documentados y registrar que todas las pruebas pasan sin depender de Firebird, Seq ni servicios externos.
