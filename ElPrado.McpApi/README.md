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

## Autenticacion JWT

Las rutas de negocio bajo `/api` requieren `Authorization: Bearer <token>`.
El consumidor `elprado-mcp` debe solicitar el token mediante
`POST /api/login/usuario`, enviando un JSON con `alias` y `clave`, y reutilizar
el token hasta su vencimiento. No se aceptan API keys ni tokens fijos.

La configuracion del entorno debe proporcionar `Jwt__Key` y `Jwt__Issuer`.
`Jwt__AccessTokenLifetimeHours` es opcional y tiene un valor predeterminado de
24 horas. Asimismo, `elprado-mcp` debe recibir sus credenciales mediante su
propio almacen de secretos. Ninguna de esas variables debe versionarse ni
registrarse.

## Disponibilidad

`GET /health` informa que el proceso está disponible y no consulta Firebird.
