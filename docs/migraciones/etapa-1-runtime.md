# Migraciones: etapa 1 — ejecución y diagnóstico

Los proyectos de Migraciones se ejecutan con .NET 7 y no requieren Firebird, una API remota ni Seq para iniciar.

## Restaurar y compilar

Desde la raíz del repositorio, ejecutar:

```powershell
dotnet restore ElPrado.Migraciones.Contracts/ElPrado.Migraciones.Contracts.csproj
dotnet build ElPrado.Migraciones.Contracts/ElPrado.Migraciones.Contracts.csproj --no-restore
dotnet restore ElPrado.Migraciones.Api/ElPrado.Migraciones.Api.csproj
dotnet build ElPrado.Migraciones.Api/ElPrado.Migraciones.Api.csproj --no-restore
dotnet restore ElPrado.Migraciones.Agent/ElPrado.Migraciones.Agent.csproj
dotnet build ElPrado.Migraciones.Agent/ElPrado.Migraciones.Agent.csproj --no-restore
```

Contracts permanece como una biblioteca vacía de infraestructura y sin DTOs. API y Agent lo referencian para conservar el límite de proyectos de la etapa.

## API

Iniciar la API en Development:

```powershell
$env:ASPNETCORE_ENVIRONMENT = 'Development'
dotnet run --project ElPrado.Migraciones.Api -- --urls http://127.0.0.1:5187
```

Comprobar los endpoints:

```powershell
Invoke-WebRequest http://127.0.0.1:5187/health
Invoke-WebRequest http://127.0.0.1:5187/swagger/index.html
Invoke-WebRequest http://127.0.0.1:5187/swagger/v1/swagger.json
```

`/health` se implementa con Health Checks de ASP.NET Core: devuelve 200 cuando está saludable y devuelve 503 si el estado informado es unhealthy. Swagger y el documento OpenAPI sólo se publican en Development; en otro entorno, por ejemplo Production, `/swagger/index.html` devuelve 404.

La API registra a consola el arranque, la detención, fallos no controlados y solicitudes HTTP mediante Serilog. No incluir secretos, cabeceras de autorización ni credenciales en mensajes de log.

## Agent

Iniciar el Agent como proceso de consola:

```powershell
$env:DOTNET_ENVIRONMENT = 'Development'
dotnet run --project ElPrado.Migraciones.Agent
```

El Worker registra su inicio, queda a la espera del token de cancelación del host y registra una detención controlada. Para detenerlo, usar `Ctrl+C`. También se mantienen las integraciones de host para Windows Service y systemd; no se conecta a la API ni ejecuta migraciones en esta etapa.

## Serilog y Seq

Los archivos `appsettings.json` configuran consola y los archivos `appsettings.Development.json` aumentan el nivel de diagnóstico. Seq es opcional: configurar su URL sin versionar credenciales.

```powershell
$env:Seq__ServerUrl = 'http://localhost:5341'
dotnet run --project ElPrado.Migraciones.Api
# o
dotnet run --project ElPrado.Migraciones.Agent
```

Al omitir `Seq__ServerUrl`, ambos procesos conservan el log de consola e inician normalmente. La jerarquía de configuración .NET hace que `Seq__ServerUrl` sobrescriba `Seq:ServerUrl` de los archivos JSON.

## Pruebas automatizadas

Desde la raíz del repositorio, ejecutar las suites de Migraciones:

```powershell
dotnet test tests/ElPrado.Migraciones.Api.Tests
dotnet test tests/ElPrado.Migraciones.Agent.Tests
```

En VS Code, abrir la carpeta del repositorio, instalar la extensión C# si todavía no está disponible y usar el panel Testing para ejecutar cada proyecto o sus pruebas individuales. Las pruebas de API usan un host en memoria y las del Agent usan señales asíncronas de ciclo de vida; no requieren Firebird, Seq, una API remota ni servidores externos.

## Resultados de validación

Validado localmente el 2026-11-07:

- `dotnet test tests/ElPrado.Migraciones.Api.Tests`: 3 pruebas superadas, 0 errores.
- `dotnet test tests/ElPrado.Migraciones.Agent.Tests`: 4 pruebas superadas, 0 errores.
- `dotnet restore` y `dotnet build --no-restore` de Contracts, API y Agent: correctos, sin errores. API y Agent emiten únicamente la advertencia `NETSDK1138` por el destino requerido `net7.0`.
- API en Development, URL `http://127.0.0.1:5187`: `/health`, Swagger UI y el documento OpenAPI respondieron HTTP 200.
- API en Production, URL `http://127.0.0.1:5188`: `/health` respondió HTTP 200 y Swagger UI respondió HTTP 404.
- Agent en Development: compiló, inició como consola y fue detenido de forma controlada. La captura no retransmite la consola del proceso hijo; los mensajes esperados son los registrados por `AgentWorker` al iniciar y detenerse.
- `dotnet build ElPradoWeb.sln --no-restore`: correcto, 0 errores y 130 advertencias existentes de compatibilidad de dependencias y nulabilidad fuera de Migraciones.
