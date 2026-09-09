## 1. Persistencia y contratos

- [x] 1.1 Crear la migracion Firebird pendiente para `RECUPERACIONES_CLAVE` con columnas de cliente/propuesta, `TOKEN_HASH`, fechas, IP e indices, y verificar que el script aplica en una base de prueba.
- [x] 1.2 Agregar el modelo de recuperacion de clave en `ElPrado.Data` y verificar que compila con los nombres reales de columnas.
- [x] 1.3 Agregar el repositorio e interfaz para crear tokens, buscar token activo por hash, consumir token, invalidar pendientes y contar solicitudes por propuesta/IP, y verificar con consultas Dapper contra una transaccion.
- [x] 1.4 Exponer el repositorio desde `IUnitOfWork` y su implementacion, y verificar que las dependencias existentes siguen compilando.
- [x] 1.5 Agregar DTOs para solicitud y restablecimiento en `ElPrado.Dto`, y verificar que Swagger refleja `propuesta + dniCuit` y `token + nuevaClave`.

## 2. Servicio de recuperacion

- [x] 2.1 Extraer la validacion y hashing de clave de `LoginService.Registrar` a una funcion reutilizable, y verificar que el registro de cliente conserva las mismas validaciones y hash.
- [x] 2.2 Implementar generacion de tokens de al menos 32 bytes con codificacion URL-safe y hash SHA-256 para persistencia, y verificar que el token plano no se guarda en base.
- [x] 2.3 Implementar `SolicitarRecuperacionCliente` en el servicio con busqueda interna por propuesta/DNI-CUIT, validacion de email registrado, respuesta generica y registro de IP, y verificar que datos no recuperables no crean token.
- [x] 2.4 Implementar controles de frecuencia por propuesta e IP usando ventanas persistidas, y verificar que al exceder limites no se genera un token adicional.
- [x] 2.5 Implementar `RestablecerClaveCliente` en el servicio con validacion de token activo, nueva clave, cambio de `ClaveAcceso`, consumo del token e invalidacion de pendientes en una sola transaccion, y verificar rollback ante error.

## 3. API, email y auditoria

- [x] 3.1 Agregar `POST /api/Login/SolicitarRecuperacionCliente` en `LoginController`, y verificar que devuelve siempre el mensaje generico para solicitudes validas a nivel de formato.
- [x] 3.2 Agregar `POST /api/Login/RestablecerClaveCliente` en `LoginController`, y verificar que acepta solo `token + nuevaClave`.
- [x] 3.3 Agregar/configurar el servicio de email transaccional para enviar el enlace de recuperacion con la URL frontend configurada, y verificar que el email no incluye claves ni hashes.
- [x] 3.4 Enviar el email de aviso despues de un restablecimiento exitoso, y verificar que no se envia cuando el token o la clave son invalidos.
- [x] 3.5 Registrar auditoria o logs estructurados para `CLIENTE_PASSWORD_RESET_REQUESTED` y `CLIENTE_PASSWORD_RESET_COMPLETED`, y verificar que incluyen cliente/propuesta cuando corresponda e IP cuando este disponible.

## 4. Verificacion

- [ ] 4.1 Agregar pruebas unitarias o de servicio para token valido, token vencido, token usado, nueva clave invalida y cliente sin email, y verificar que pasan.
- [ ] 4.2 Agregar pruebas de endpoint o integracion para respuesta generica de solicitud y restablecimiento exitoso, y verificar los codigos y cuerpos esperados.
- [x] 4.3 Compilar `ElPrado.WebApi` y los proyectos afectados, y verificar que no hay errores de DI, contratos ni namespaces.
- [x] 4.4 Validar la change con `openspec validate implementar-recuperacion-clave-clientes --strict` y corregir cualquier error reportado.
