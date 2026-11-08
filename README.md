# El Prado Backend

## Ejecutar `ElPrado.WebApi` en Development

Desde la raíz del repositorio:

```powershell
dotnet run --project .\ElPrado.WebApi\ElPrado.WebApi.csproj --launch-profile http
```

El perfil `http` establece `ASPNETCORE_ENVIRONMENT=Development`, por lo que ASP.NET Core carga `appsettings.json` y luego `appsettings.Development.json`, cuyos valores prevalecen.

La API queda disponible en `http://localhost:5286` y Swagger en `http://localhost:5286/swagger`.

También se puede ejecutar desde el directorio del proyecto:

```powershell
cd .\ElPrado.WebApi
dotnet run --launch-profile http
```
