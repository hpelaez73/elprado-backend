## Why

Los clientes registrados necesitan recuperar el acceso web sin que ElPrado revele, recupere o envie la clave existente. El flujo actual solo permite registrar, iniciar sesion o borrar el acceso, por lo que la recuperacion debe agregar una validacion de posesion del email mediante token temporal.

## What Changes

- Agregar un endpoint para solicitar recuperacion de clave mediante propuesta y DNI/CUIT, con respuesta generica para evitar enumeracion de cuentas.
- Generar un token criptograficamente seguro, URL-safe, de un solo uso y con vencimiento aproximado de 30 minutos.
- Persistir solo el hash del token, junto con datos de auditoria como propuesta, fechas e IP de solicitud.
- Enviar al email registrado un enlace de recuperacion hacia el frontend configurado.
- Agregar un endpoint para restablecer la clave usando `token + nuevaClave`, sin requerir propuesta ni DNI/CUIT en esta segunda etapa.
- Reutilizar la misma validacion y hashing de clave que usa el alta de cliente, evitando duplicar reglas en `LoginController`.
- Consumir el token y cambiar la clave dentro de una misma transaccion, invalidando otros tokens pendientes del cliente.
- Aplicar limites de frecuencia por propuesta y por IP para reducir abuso.
- Registrar eventos de auditoria de solicitud y finalizacion, y enviar un aviso posterior al cambio de clave.

## Capabilities

### New Capabilities

- `recuperacion-clave-clientes`: Recuperacion y restablecimiento seguro de clave web de clientes mediante token temporal enviado al email registrado.

### Modified Capabilities

Ninguna.

## Impact

- `ElPrado.WebApi/Controllers/LoginController.cs` incorporara los endpoints `SolicitarRecuperacionCliente` y `RestablecerClaveCliente`.
- `ElPrado.Dto/Dtos/DtoLogin.cs` o archivos DTO equivalentes incorporaran los contratos de solicitud y restablecimiento.
- `ElPrado.Services/Services/LoginService.cs` concentrara el flujo de recuperacion, reutilizando o extrayendo reglas comunes de validacion y hashing de clave.
- `ElPrado.Data` incorporara persistencia para recuperaciones de clave, consultas por hash de token, consumo atomico e invalidacion de tokens pendientes.
- La base Firebird requerira una migracion para la tabla de recuperaciones y los indices necesarios para token, propuesta, vencimiento y uso.
- La infraestructura de email/configuracion debera permitir enviar el enlace de recuperacion y el aviso de cambio de clave desde el backend.
- `ElPrado.WebApi/Program.cs` o configuracion equivalente incorporara rate limiting para las rutas de recuperacion.
- No cambia el contrato de login existente ni permite cambiar clave solo con propuesta y DNI/CUIT.
