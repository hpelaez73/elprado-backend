## Context

El flujo actual de clientes vive en `LoginController` y `LoginService`: `RegistrarCliente` valida email y clave, guarda `Clientes.ClaveAcceso` con `FunUtils.SHA1`, y `LoginCliente` autentica contra propuesta, DNI/CUIT y clave. No existe una entidad de recuperacion ni un canal de email transaccional en ElPrado.WebApi; el envio de facturas usa configuracion SMTP existente desde workers.

La recuperacion toca API publica, reglas de seguridad, persistencia Firebird, email y auditoria, por lo que conviene separar el contrato del endpoint de la generacion/validacion de tokens y de la actualizacion de clave.

## Goals / Non-Goals

**Goals:**

- Mantener `LoginController` como fachada HTTP fina y mover el flujo a servicios/repositorios.
- Reutilizar la validacion y hash de clave que ya usa el alta de clientes.
- Guardar solo hashes de tokens y hacer que el token plano exista solo en memoria hasta construir el link del email.
- Garantizar que el consumo de token y el cambio de clave compartan transaccion.
- Evitar enumeracion de cuentas en la solicitud inicial.
- Dejar la persistencia preparada para auditoria, rate limiting e invalidacion de tokens pendientes.

**Non-Goals:**

- Cambiar el mecanismo actual de autenticacion o el contrato de `POST /api/Login/Cliente`.
- Migrar el hash historico de clientes a otro algoritmo.
- Permitir recuperacion automatica para clientes sin email registrado.
- Exponer endpoints anonimos que devuelvan datos de cliente, propuesta o email.
- Implementar cambios en el frontend; solo se configura la URL base del enlace.

## Decisions

- Agregar DTOs especificos para `SolicitarRecuperacionCliente` y `RestablecerClaveCliente`.
  - La solicitud inicial aceptara propuesta y DNI/CUIT, alineada con `DtoLoginCliente`.
  - El restablecimiento aceptara solo token y nueva clave, porque el token identifica la recuperacion pendiente.
  - Se descarta reutilizar `DtoLoginClienteAlta`, porque incluye email y confirmacion de clave propios del registro y forzaria datos que el segundo paso no necesita.

- Extraer reglas comunes de clave desde `LoginService.Registrar` a un helper o servicio interno reutilizable.
  - El alta y el restablecimiento deben validar longitud minima y normalizacion de la misma forma.
  - El hash seguira usando el mecanismo existente para no romper compatibilidad con `LoginCliente`.
  - Se descarta crear una politica nueva para recuperacion, porque produciria claves aceptadas en un flujo y rechazadas en otro.

- Crear una tabla Firebird para recuperaciones de clave con identificador, propuesta, cliente, token hash, fechas de creacion/vencimiento/utilizacion, IP de solicitud y datos de auditoria basicos.
  - El hash del token sera el indice de busqueda para el segundo paso.
  - La tabla necesita indices para `TOKEN_HASH`, tokens pendientes por cliente/propuesta y ventanas de rate limiting por propuesta/IP.
  - Se descarta almacenar el token plano, porque un acceso a base permitiria restablecer claves.

- Generar tokens con `RandomNumberGenerator`, al menos 32 bytes, y codificarlos con formato URL-safe.
  - Para validar se calcula SHA-256 del token recibido y se busca el hash.
  - La expiracion sera de 30 minutos desde la creacion.
  - Se descarta usar GUIDs o strings previsibles por insuficiente entropia.

- Implementar `SolicitarRecuperacionCliente` como endpoint anonimo con respuesta generica.
  - El servicio buscara internamente cliente por propuesta y DNI/CUIT, verificara que pueda recuperar y que tenga email.
  - Si no puede recuperar, devolvera igualmente exito generico sin crear token ni enviar email.
  - Si se excede rate limiting, tambien mantendra una respuesta externa indistinguible y evitara crear otro token.

- Implementar `RestablecerClaveCliente` como endpoint anonimo que valida token y nueva clave.
  - Token invalido, vencido o usado devolvera un error controlado sin revelar datos del cliente.
  - Nueva clave invalida devolvera errores de validacion y no consumira el token, para permitir corregir dentro de la ventana.
  - Reset exitoso actualizara `ClaveAcceso`, marcara el token usado e invalidara pendientes del mismo cliente en una misma transaccion.

- Enviar emails desde ElPrado.WebApi usando una abstraccion compartible con la configuracion SMTP existente.
  - El email inicial incluira `Frontend:PasswordResetUrl` o configuracion equivalente con el token como query string.
  - El email posterior al cambio no incluira token ni datos sensibles.
  - Se descarta depender del `EmailWorker` de facturas si su cola actual no modela emails transaccionales de autenticacion.

- Registrar auditoria con los mecanismos de logs existentes primero, y agregar persistencia dedicada solo si ya existe un patron de auditoria funcional en el repo.
  - Los eventos minimos son `CLIENTE_PASSWORD_RESET_REQUESTED` cuando se crea token y `CLIENTE_PASSWORD_RESET_COMPLETED` cuando el reset confirma la clave.
  - La IP debe guardarse en la recuperacion y en el log cuando este disponible.

## Risks / Trade-offs

- SMTP no disponible durante la solicitud -> Mantener respuesta generica al cliente, registrar el fallo y no filtrar si la cuenta existe.
- Rate limiting en memoria se pierde al reiniciar la API -> Preferir ventanas calculadas desde la tabla de recuperaciones para propuesta/IP; usar rate limiter del framework solo como defensa adicional.
- `FunUtils.SHA1` es debil criptograficamente -> Mantenerlo para compatibilidad porque el issue exige reutilizar el mecanismo actual; documentar como deuda separada si se decide migrar hashes.
- Clientes con email desactualizado no podran recuperar acceso -> Es intencional; deben comunicarse con El Prado para validar identidad fuera del flujo automatico.
- Multiples solicitudes validas generan varios emails -> El reset exitoso invalidara todos los tokens pendientes del mismo cliente; opcionalmente se puede invalidar pendientes al crear un token nuevo si producto prefiere un unico link activo.

## Migration Plan

- Crear la migracion pendiente de Firebird para `RECUPERACIONES_CLAVE` con sus indices.
- Desplegar backend con configuracion de URL frontend para recuperacion y credenciales SMTP disponibles.
- Verificar en staging solicitud recuperable, solicitud no recuperable, token vencido, token usado y nueva clave invalida.
- Rollback: retirar endpoints/configuracion y dejar la tabla sin uso; los tokens pendientes vencen sin afectar el login existente.
