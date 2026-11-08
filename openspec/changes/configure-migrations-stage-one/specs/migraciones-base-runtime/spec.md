# Spec Delta

## Purpose

Proporcionar a los procesos de Migraciones una ejecución diagnosticable y consistente, con controles de salud, documentación de desarrollo y telemetría configurable.

## ADDED Requirements

### Requirement: Diagnóstico de salud de la API
La API de Migraciones MUST exponer `GET /health` mediante el mecanismo estándar de Health Checks. MUST responder HTTP 200 cuando la aplicación esté saludable y HTTP 503 cuando su estado sea unhealthy, sin comprobar Firebird, SignalR ni servicios externos en esta etapa.

#### Scenario: Aplicación saludable
- **WHEN** la API se encuentra operativa y se solicita `GET /health`
- **THEN** responde HTTP 200 mediante el endpoint estándar de Health Checks

#### Scenario: Estado unhealthy
- **WHEN** el mecanismo de Health Checks informa un estado unhealthy
- **THEN** `GET /health` responde HTTP 503

### Requirement: Documentación de API en desarrollo
La API de Migraciones MUST publicar la documentación Swagger/OpenAPI únicamente cuando el entorno activo sea Development. Los endpoints públicos de la API MUST aparecer documentados sin exigir mecanismos de autenticación en esta etapa.

#### Scenario: Swagger en Development
- **WHEN** la API se inicia con el entorno Development
- **THEN** la interfaz Swagger queda disponible y documenta los endpoints registrados

#### Scenario: Swagger fuera de Development
- **WHEN** la API se inicia en un entorno distinto de Development
- **THEN** la interfaz Swagger no queda expuesta

### Requirement: Observabilidad configurable
La API y el Agent MUST registrar a consola sus eventos de inicio, detención y fallos críticos. MUST permitir habilitar Seq configurando su URL, sin requerirlo para iniciar, y MUST aplicar niveles de log según el entorno sin registrar secretos o credenciales.

#### Scenario: Ejecución sin Seq
- **WHEN** la URL de Seq no está configurada
- **THEN** cada proceso inicia y conserva el registro por consola

#### Scenario: Ejecución con Seq configurado
- **WHEN** la URL de Seq está configurada mediante configuración o variables de entorno
- **THEN** cada proceso envía sus eventos a Seq además de la salida por consola

### Requirement: Ciclo de vida controlado del Agent
El Agent MUST ejecutarse como servicio en segundo plano compatible con Windows Service y systemd. MUST registrar el inicio y la detención, respetar la señal de cancelación del host y finalizar de forma controlada sin iniciar conexiones a la API ni tareas de migración.

#### Scenario: Inicio del agente
- **WHEN** el host del Agent inicia
- **THEN** el servicio en segundo plano registra su inicio y queda a la espera de cancelación

#### Scenario: Detención del agente
- **WHEN** el host solicita la cancelación del Agent
- **THEN** el servicio finaliza sin error y registra su detención

### Requirement: Configuración y contratos base
Los proyectos API, Agent y Contracts MUST mantenerse en .NET 7. API y Agent MUST conservar su referencia a Contracts, que MUST permanecer libre de contratos de comunicación e infraestructura. La configuración MUST admitir valores compartidos, valores por entorno y sobrescritura mediante variables de entorno sin versionar secretos reales.

#### Scenario: Sobrescritura de configuración
- **WHEN** se proporciona una variable de entorno para una opción de configuración
- **THEN** el proceso utiliza ese valor en lugar del valor equivalente de archivos de configuración

#### Scenario: Compilación independiente
- **WHEN** se restaura y compila cualquiera de los tres proyectos de Migraciones
- **THEN** el proyecto compila con destino .NET 7 y conserva sus dependencias permitidas
