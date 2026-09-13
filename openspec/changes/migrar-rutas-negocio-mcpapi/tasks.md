## 1. Contratos y adaptadores de propuestas

- [x] 1.1 Relevar las siete tools y sus esquemas Zod actuales, definir DTOs MCP para detalle, titulares, cuenta corriente, servicios, comprobantes, contratos e historial, y verificar correspondencia campo a campo.
- [x] 1.2 Registrar los servicios de dominio necesarios en `ElPrado.McpApi` y crear el grupo protegido de endpoints de propuestas; verificar que una solicitud sin Bearer recibe HTTP 401.
- [ ] 1.3 Implementar detalle, titulares y cuenta corriente como adaptadores delgados a servicios existentes, y verificar respuestas exitosas, datos inválidos y propuestas inexistentes contra Firebird de prueba.
- [ ] 1.4 Implementar servicios, comprobantes, contratos e historial como adaptadores delgados a servicios existentes, y verificar sus contratos y errores controlados contra Firebird de prueba.

## 2. Integración del servidor MCP

- [x] 2.1 Actualizar `elprado-mcp` para usar las rutas y DTOs de ElPrado.McpApi en cada tool, preservando los esquemas y respuestas MCP, y verificar sus pruebas Vitest.
- [x] 2.2 Eliminar el uso restante de contratos y URL de negocio de ElPrado.WebApi del cliente MCP, y verificar que la configuración solo requiere ElPrado.McpApi y credenciales de entorno.

## 3. Verificación y despliegue

- [ ] 3.1 Agregar pruebas de integración para JWT ausente, JWT válido y error estructurado por cada familia de consulta, y verificar que pasan.
- [ ] 3.2 Ejecutar `dotnet build ElPradoWeb.sln --configuration Release`, `npm run typecheck`, `npm test` y `npm run build`, y verificar las siete tools extremo a extremo contra el entorno de prueba.
- [x] 3.3 Actualizar la documentación de despliegue y rollback para la migración de base URL, y verificar que no incluye secretos ni tokens reales.
