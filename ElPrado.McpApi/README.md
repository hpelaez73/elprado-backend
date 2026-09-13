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
`Jwt__AccessTokenLifetimeHours` es opcional; el valor predeterminado del codigo
es de 24 horas, pero `appsettings.json` lo fija en 168 (7 dias) de forma
transitoria, hasta que se implemente la renovacion de token en `elprado-mcp`.
Una vez disponible el refresh, ese valor debe volver a una vida corta.
Asimismo, `elprado-mcp` debe recibir sus credenciales mediante su
propio almacen de secretos. Ninguna de esas variables debe versionarse ni
registrarse.

## Consultas de propuestas

Las siete consultas que usa `elprado-mcp` se exponen mediante `POST` bajo el
grupo autenticado `/api/propuestas`. Todas responden el sobre estable
`{ succeeded, data, error }`; los errores funcionales no incluyen trazas,
credenciales ni detalles de Firebird.

| Tool MCP | Ruta | Solicitud MCP |
| --- | --- | --- |
| `consultar_propuesta` | `/api/propuestas/detalle` | `propuesta`, `incluirBaja` |
| `consultar_titulares_propuesta` | `/api/propuestas/titulares` | `propuesta`, `incluirBaja` |
| `consultar_cuenta_corriente` | `/api/propuestas/cuenta-corriente` | `codPropuesta`, opciones de fecha/estado |
| `consultar_servicios_propuesta` | `/api/propuestas/servicios` | `codPropuesta` |
| `consultar_comprobantes_propuesta` | `/api/propuestas/comprobantes` | `propuesta`, `pagina`, `filasPagina` |
| `consultar_contratos_propuesta` | `/api/propuestas/contratos` | `propuesta`, `incluirBaja` |
| `consultar_historial_titulares` | `/api/propuestas/historial-titulares` | `propuesta`, `incluirBaja` |

Los contratos de la fachada estan en `Contracts/ProposalContracts.cs`. Cada
adaptador convierte los DTOs de dominio campo a campo y no devuelve la
envoltura HTTP de `ElPrado.WebApi`. Una solicitud mal formada devuelve `400`
con `INVALID_REQUEST`; una propuesta no disponible devuelve `404` con
`PROPOSAL_NOT_FOUND`.

## Migracion y rollback de base URL

Configure `elprado-mcp` con `ELPRADO_MCP_API_BASE_URL` apuntando a esta
fachada. El cliente debe pedir el JWT con sus credenciales de entorno y usarlo
en las rutas anteriores; no debe conservar rutas de negocio ni contratos de
`ElPrado.WebApi`.

Para rollback, restaure la anterior base URL y la version del cliente MCP que
consume esa fachada. `ElPrado.WebApi` no se modifica durante esta migracion,
por lo que permanece disponible. Nunca incluya alias, claves ni JWT reales en
archivos versionados, comandos de despliegue o registros.

## Disponibilidad

`GET /health` informa que el proceso está disponible y no consulta Firebird.
