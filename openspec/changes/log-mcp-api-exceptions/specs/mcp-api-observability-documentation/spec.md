## MODIFIED Requirements

### Requirement: Registro estructurado centralizado en Seq
El sistema SHALL emitir los eventos de Serilog de `ElPrado.McpApi` al servidor Datalust Seq configurado, conservando los niveles mínimos y filtros definidos por ambiente. La configuración de Seq SHALL poder sobrescribirse mediante la jerarquía de configuración existente, incluidas las variables de entorno. Las excepciones que produzcan respuestas de error SHALL emitirse con nivel Error, la excepción original y contexto operativo de la solicitud que no incluya credenciales, tokens ni claves secretas.

#### Scenario: Inicio de la API con Seq configurado
- **WHEN** `ElPrado.McpApi` inicia con una URL válida de Seq en su configuración
- **THEN** los eventos estructurados emitidos por la aplicación se entregan al servidor Seq configurado

#### Scenario: Nivel de log definido por ambiente
- **WHEN** la API se inicia en un ambiente que define un nivel mínimo de Serilog distinto
- **THEN** sólo se envían a Seq los eventos admitidos por ese nivel y por los filtros configurados

#### Scenario: Excepción que causa una respuesta de error
- **WHEN** una solicitud de `ElPrado.McpApi` produce una excepción que se traduce en una respuesta de error
- **THEN** Seq recibe un evento Error con la excepción original, el método HTTP, la ruta y el estado de respuesta, sin exponer secretos en sus propiedades
