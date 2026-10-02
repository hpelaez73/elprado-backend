## Purpose

Define el flujo seguro para que un cliente registrado recupere y restablezca su clave web mediante un token temporal enviado al email asociado, sin exponer si la cuenta existe ni revelar la clave actual.

## ADDED Requirements

### Requirement: Solicitud de recuperacion con respuesta generica
ElPrado.WebApi SHALL expose `POST /api/Login/SolicitarRecuperacionCliente` to accept a customer recovery request with `propuesta` and `dniCuit`. The endpoint SHALL always return a generic successful message for syntactically valid requests, regardless of whether the proposal, customer account, web registration, or email exists.

#### Scenario: Cuenta identificable con email registrado
- **WHEN** a customer submits a valid `propuesta` and `dniCuit` that match a recoverable web account with registered email
- **THEN** the system accepts the request and returns only the generic recovery message

#### Scenario: Datos no recuperables
- **WHEN** a request uses a proposal, document number, account state, web registration, or email condition that cannot start automatic recovery
- **THEN** the system returns the same generic recovery message without indicating which condition failed

### Requirement: Token temporal de recuperacion
The system SHALL create a cryptographically random recovery token of at least 256 bits for recoverable requests. The token SHALL be encoded safely for URLs, SHALL expire approximately 30 minutes after creation, SHALL be single-use, and SHALL never be stored in plain text.

#### Scenario: Token generado para solicitud recuperable
- **WHEN** a recoverable request is accepted
- **THEN** the system stores only a hash of the token with creation, expiration, proposal, customer identity, and request IP metadata

#### Scenario: Token vencido o ya usado
- **WHEN** a reset attempt presents a token that is expired, already used, or unknown
- **THEN** the system rejects the reset without changing the customer's clave

### Requirement: Envio de enlace de recuperacion
For recoverable requests with registered email, the system SHALL send an email containing a recovery link to the configured frontend URL with the generated token as a query parameter. The email SHALL NOT include the current password or any password hash.

#### Scenario: Email de recuperacion enviado
- **WHEN** a recoverable request creates a token
- **THEN** the registered email receives a recovery link that allows submitting a new password using that token

#### Scenario: Cliente sin email registrado
- **WHEN** the customer account has no registered email suitable for recovery
- **THEN** automatic recovery is not initiated and no password change is possible through the recovery request alone

### Requirement: Restablecimiento mediante token
ElPrado.WebApi SHALL expose `POST /api/Login/RestablecerClaveCliente` to accept `token` and `nuevaClave`. The token SHALL identify the pending recovery, so the client SHALL NOT need to send proposal or DNI/CUIT during reset.

#### Scenario: Restablecimiento exitoso
- **WHEN** a customer submits a valid unused token before expiration and a valid new password
- **THEN** the system updates the customer's clave using the same password rules as customer registration and consumes the token

#### Scenario: Token no requiere datos adicionales
- **WHEN** a reset request is submitted with only `token` and `nuevaClave`
- **THEN** the system can identify the pending recovery from the token and does not require proposal or DNI/CUIT

### Requirement: Validacion y almacenamiento de clave
The new password SHALL be validated and stored using the same customer password validation and hashing behavior used by customer registration. The recovery flow SHALL NOT introduce a different password policy or hashing format.

#### Scenario: Nueva clave invalida
- **WHEN** a reset request includes a password that would be rejected during customer registration
- **THEN** the system rejects the reset and leaves the token available until it expires or is consumed by a valid reset

#### Scenario: Nueva clave valida
- **WHEN** a reset request includes a password accepted by the customer registration rules
- **THEN** the stored customer clave is updated using the same hashing mechanism as registration

### Requirement: Consumo transaccional e invalidacion de pendientes
The password update, token consumption, and invalidation of other pending recovery tokens for the same customer SHALL occur atomically. A token SHALL be valid only when it exists, is unused, and has an expiration later than the current time.

#### Scenario: Cambio e invalidacion atomicos
- **WHEN** a reset succeeds
- **THEN** the used token is marked consumed, other pending tokens for the same customer are invalidated, and the password change is committed in the same transaction

#### Scenario: Fallo durante el restablecimiento
- **WHEN** the system cannot complete any part of a reset transaction
- **THEN** it rolls back the password change and token state changes together

### Requirement: Rate limiting y auditoria
The recovery request flow SHALL apply rate limits equivalent to at most 3 requests every 15 minutes per proposal and at most 10 requests per hour per IP. The system SHALL record audit events for recovery request creation and successful password reset.

#### Scenario: Limite por propuesta excedido
- **WHEN** requests for the same proposal exceed the configured recovery limit within the time window
- **THEN** the system does not create another recovery token for that proposal during the limited window

#### Scenario: Eventos auditados
- **WHEN** a recoverable request creates a recovery token or a reset completes successfully
- **THEN** the system records an audit event equivalent to `CLIENTE_PASSWORD_RESET_REQUESTED` or `CLIENTE_PASSWORD_RESET_COMPLETED`

### Requirement: Notificacion posterior al cambio
After a successful password reset, the system SHALL send a notification email to the registered email indicating that the password was changed and that the customer should contact El Prado if they did not perform the operation.

#### Scenario: Aviso enviado tras cambio exitoso
- **WHEN** the customer's password is reset successfully
- **THEN** the registered email receives a notification that the password was changed
