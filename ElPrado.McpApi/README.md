# ElPrado.McpApi

`ElPrado.McpApi` es la fachada HTTP REST interna que consume `elprado-mcp`.
Expone contratos adaptados a flujos de agentes y reutiliza los servicios de
dominio compartidos de El Prado mediante adaptadores delgados a medida que se
incorporan operaciones.

`elprado-mcp` conserva la responsabilidad sobre el protocolo MCP, las
definiciones de tools, el transporte y OAuth. Este proyecto no expone un
endpoint MCP ni implementa responsabilidades de ese protocolo.

## Ejecución

Desde la raíz de la solución, ejecute en modo Debug:

```powershell
dotnet run --project ElPrado.McpApi --launch-profile ElPrado.McpApi
```

El perfil de desarrollo escucha en `http://localhost:53570` y
`https://localhost:53569`.

Para generar y ejecutar la aplicación en modo Release:

```powershell
dotnet build ElPrado.McpApi/ElPrado.McpApi.csproj --configuration Release
dotnet ElPrado.McpApi/bin/Release/net7.0/ElPrado.McpApi.dll --urls "http://localhost:53570"
```

En un despliegue, el puerto o las URLs se configuran con la variable de entorno
`ASPNETCORE_URLS`; por ejemplo, `http://*:8080`.

## Configuración

La configuración se carga desde `appsettings.json`, luego desde el archivo
opcional `appsettings.{Environment}.json` y, por último, desde variables de
entorno. Las cadenas de conexión y otros valores sensibles deben mantenerse en
el entorno de despliegue; intencionalmente no se almacenan en estos archivos.
CORS está deshabilitado de forma predeterminada. Configure
`Cors__AllowedOrigins__0` (y valores indexados adicionales) solo para orígenes
web explícitos que requieran acceso desde un navegador.

## Disponibilidad

`GET /health` informa que el proceso está disponible y no consulta Firebird.
