# Migraciones: autenticación y credenciales

La configuración sensible se entrega fuera del repositorio mediante User Secrets, variables de entorno o el proveedor de secretos del host. `appsettings*.json` no contiene claves, hashes ni credenciales.

## Generar un hash

Ejecutar localmente y escribir la credencial cuando se solicite; no pasarla por argumentos ni guardarla en archivos:

```powershell
dotnet run --project tools/ElPrado.Migraciones.HashTool
```

La salida única es el hash PBKDF2-HMAC-SHA512 versionado. Cárguelo como `MigrationAuthentication__Admin__PasswordHash` o `MigrationAuthentication__Agents__0__SecretHash` en su proveedor de secretos.

## Configuración de API

Además de los hashes, la API requiere `MigrationAuthentication__Jwt__Issuer`, `Audience`, `SigningKey` (Base64 de al menos 32 bytes), `Admin__Username` y los registros `Agents`. Los cambios provenientes de un proveedor con recarga dinámica aplican la revocación y la rotación inmediatamente. Variables de entorno y proveedores sin recarga requieren reiniciar o redesplegar la API.

Deshabilitar o eliminar un agente revoca sus JWT aún vigentes una vez aplicada la configuración. Cambiar únicamente `SecretHash` bloquea el secreto anterior para nuevos tokens, pero los JWT existentes siguen válidos hasta expirar. Rotar `Jwt__SigningKey` invalida todos los JWT y es una operación de emergencia.

## Configuración del Agent

El Agent recibe `MigrationAgentAuthentication__ApiUrl` (HTTPS), `AgentId` y `AgentSecret` desde una fuente externa. El token sólo se conserva en memoria; renueva antes de expirar. Los errores de red y 429 se reintentan con backoff y jitter; un 401 suspende nuevos intentos automáticos hasta actualizar la configuración o reiniciar.
