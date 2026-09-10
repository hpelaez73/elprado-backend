# ElPrado.McpApi

`ElPrado.McpApi` is the internal HTTP REST facade consumed by `elprado-mcp`.
It provides contracts tailored to agent workflows and reuses shared ElPrado
domain services through thin adapters as operations are added.

`elprado-mcp` remains responsible for the MCP protocol, tool definitions,
transport, and OAuth. This project does not expose an MCP endpoint or implement
MCP protocol concerns.

## Configuration

Configuration is loaded from `appsettings.json`, then the optional
`appsettings.{Environment}.json`, and finally environment variables. Keep
connection strings and other sensitive values in the deployment environment;
they are intentionally not stored in these files. CORS is disabled by default.
Set `Cors__AllowedOrigins__0` (and additional indexed values) only for explicit
web origins that need browser access.

## Liveness

`GET /health` reports process availability and does not query Firebird.
