## Purpose

Permitir que los operadores diagnostiquen fallas de la fachada MCP sin alterar ni exponer detalles internos en las respuestas a los consumidores.

## ADDED Requirements

### Requirement: Registro de excepciones que producen respuestas de error
El sistema SHALL registrar en Serilog toda excepción que haga que `ElPrado.McpApi` devuelva una respuesta de error al consumidor. El evento SHALL conservar la excepción original y contener el método HTTP, la ruta solicitada y el código de estado de la respuesta.

#### Scenario: Falla de operación de propuestas controlada
- **WHEN** una operación de propuestas captura una excepción y devuelve el error público `BUSINESS_OPERATION_FAILED`
- **THEN** Serilog recibe un evento de nivel Error con la excepción y el contexto de la solicitud, mientras el consumidor recibe el mismo código, mensaje y estado HTTP definidos por el contrato actual

#### Scenario: Excepción no manejada por un adaptador
- **WHEN** una excepción alcanza el límite de la tubería HTTP de `ElPrado.McpApi`
- **THEN** Serilog recibe un evento de nivel Error con la excepción y el contexto de la solicitud antes de devolver la respuesta de error aplicable

### Requirement: Protección de la información expuesta en errores
El sistema SHALL mantener fuera de las respuestas de error al consumidor el detalle técnico de las excepciones y SHALL evitar registrar credenciales, tokens o claves secretas como parte del contexto de la solicitud.

#### Scenario: Excepción con detalle interno
- **WHEN** una excepción incluye detalles técnicos o sensibles
- **THEN** dichos detalles se conservan sólo en el evento de Serilog y no se agregan al mensaje o código público de la respuesta
