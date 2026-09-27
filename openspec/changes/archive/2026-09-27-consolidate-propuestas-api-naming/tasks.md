## 1. Retiro de la fachada heredada

- [x] 1.1 Eliminar `ElPrado.McpApi/Endpoints/PropuestasEndpointExtensions.cs` y `ElPrado.McpApi/Contracts/PropuestasContracts.cs` heredados, y verificar que no queden rutas POST bajo `/api/propuestas`.
- [x] 1.2 Actualizar `BusinessEndpointExtensions` para registrar sólo la extensión de propuestas consolidada y verificar por prueba de integración que `/api/propuestas` no está publicada.

## 2. Consolidación de nombres

- [x] 2.1 Renombrar el archivo, clase y método de extensión de endpoints vigentes desde `PropuestasV2...` a `Propuestas...`, manteniendo las rutas `/api/v2/propuestas`, y verificar que todos sus recursos GET continúen registrados.
- [x] 2.2 Renombrar `PropuestasV2Contracts.cs` y todos sus records públicos sin el sufijo o segmento `V2`, actualizar los mapeos y verificar que las respuestas serializadas conservan exactamente los nombres JSON canónicos.
- [x] 2.3 Renombrar los DTOs, métodos de repositorio y servicio, y helpers asociados a la fachada vigente que contienen `V2`, actualizando todos sus consumidores; conservar sin cambios los identificadores de ruta y procedimientos Firebird que incluyen `V2`, y verificar con una búsqueda de referencias que no quedan símbolos C# de esta fachada con esa distinción.

## 3. Documentación y pruebas

- [x] 3.1 Actualizar `ElPrado.McpApi/README.md` y la documentación de propuestas para retirar la superficie heredada y conservar como vigente `/api/v2/propuestas`; verificar que no se documenten rutas `/api/propuestas` retiradas.
- [x] 3.2 Renombrar fixtures, archivos y casos de `ElPrado.Tests` relacionados con propuestas para reflejar los tipos consolidados; verificar que cubren autenticación, envelope, búsqueda y ausencia de las rutas heredadas.

## 4. Verificación integrada

- [x] 4.1 Ejecutar `dotnet test ElPrado.Tests/ElPrado.Tests.csproj` y verificar que finaliza correctamente.
- [x] 4.2 Ejecutar `openspec validate consolidate-propuestas-api-naming --strict` y verificar que la propuesta, las deltas, el diseño y las tareas son coherentes.
