## Context

Véanse `proposal.md` y `specs/mcp-proposal-business-operations/spec.md`.
`elprado-mcp` contiene siete tools que hoy consumen rutas de ElPrado.WebApi.
ElPrado.McpApi ya posee autenticación JWT y un grupo `/api` protegido, pero no
tiene adaptadores de negocio.

## Goals / Non-Goals

**Goals:**

- Completar una frontera MCP autenticada de extremo a extremo para las tools
  de propuestas existentes.
- Reutilizar servicios y repositorios de dominio, transformando únicamente los
  contratos de transporte.

**Non-Goals:**

- No modificar rutas ni contratos del frontend.
- No agregar escrituras, protocolo MCP ni nuevas familias de negocio.

## Decisions

- Crear un grupo de endpoints de propuestas en ElPrado.McpApi, protegido por
  la autorización Bearer ya configurada, con DTOs locales por operación.
  Se descarta reenviar HTTP a ElPrado.WebApi, porque mantendría la dependencia
  de contratos de frontend y no aplicaría el contexto MCP directamente.

- Cada endpoint será un adaptador delgado a los servicios existentes. Las
  validaciones y consultas Firebird permanecen en las capas de dominio.

- Migrar `elprado-mcp` una tool por vez a la nueva base URL, preservando sus
  esquemas Zod y sus respuestas públicas. Se descartará el uso de
  `ELPRADO_API_BASE_URL` cuando todas las tools estén migradas.

## Risks / Trade-offs

- [Un DTO MCP omite campos requeridos por una tool] → Comparar cada respuesta
  real con los esquemas Zod y cubrirla con pruebas de integración.
- [Cambios de servicios afectan frontend] → Reutilizar APIs de dominio ya
  existentes y ejecutar regresiones de WebApi.
- [Migración parcial mezcla dos backends] → Habilitar la configuración MCP
  solo al finalizar las siete rutas y verificar cada tool extremo a extremo.

## Migration Plan

1. Implementar y probar las rutas MCP protegidas contra Firebird de prueba.
2. Cambiar las tools una por una y validar sus contratos MCP.
3. Desplegar ambas versiones, configurar la nueva base URL y monitorear
   errores de autenticación y contratos.
4. Rollback: restaurar la base URL anterior de elprado-mcp; ElPrado.WebApi
   permanece intacta.
