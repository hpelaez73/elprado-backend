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

## Propuestas

La única superficie de propuestas es de solo lectura y usa `GET` bajo
`/api/propuestas`. Todas sus rutas requieren el JWT de empleado y responden
`{ succeeded, data, error }`. Los contratos se encuentran en
`Contracts/PropuestasContracts.cs`; las fechas son ISO 8601, los importes son
decimales, los valores ausentes son `null` y las colecciones vacías son `[]`.

| Ruta | Descripción |
| --- | --- |
| `/api/propuestas/buscar` | Búsqueda compacta de propuestas activas. |
| `/api/propuestas/{propuesta}` | Detalle de propuesta y parcela. |
| `/api/propuestas/{propuesta}/titulares` | Titulares actuales. |
| `/api/propuestas/{propuesta}/deuda` | Estado, totales y cuentas. |
| `/api/propuestas/{propuesta}/servicios` | Habilitaciones, cupos y utilizaciones. |
| `/api/propuestas/{propuesta}/contratos` | Contratos y titulares habilitados para facturación/pago. |
| `/api/propuestas/{propuesta}/comprobantes` | Comprobantes filtrables y paginados. |
| `/api/propuestas/{propuesta}/historial-titulares` | Hechos históricos de titularidad. |

`propuesta` es siempre el número público; el código interno se resuelve dentro
de la API. En comprobantes son opcionales `desde`, `hasta` (formato
`yyyy-MM-dd`), `tipo`, `estado`, `page` y `pageSize`; los defaults son `page=1`
y `pageSize=20`, con máximo `pageSize=100`. Un rango de fechas inválido o una
paginación fuera de límites devuelve `400`/`INVALID_REQUEST`. Una propuesta no
disponible devuelve `404`/`PROPOSAL_NOT_FOUND`; un fallo controlado de negocio
devuelve `422`/`BUSINESS_OPERATION_FAILED`, sin detalles internos.

La búsqueda usa `GET /api/propuestas/buscar` con al menos uno de `nombre`,
`documento` o `parcela`. Cuando se combinan, todos los criterios deben
coincidir. `nombre` busca parcialmente entre titulares actuales; `documento` y
`parcela` aceptan sus números normalizados. La respuesta contiene una página de
`items` con `propuesta`, `tipo`, `estado`, parcela opcional y titulares actuales
para desambiguar, sin detalle, importes ni códigos internos. Usa los mismos
defaults de paginación (`page=1`, `pageSize=20`, máximo 100); una búsqueda sin
coincidencias devuelve `200` con `items: []`.

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
