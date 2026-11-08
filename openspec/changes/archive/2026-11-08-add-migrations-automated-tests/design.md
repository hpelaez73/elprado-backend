# Diseño

## Contexto

La API de Migraciones expone `GET /health` desde un host minimal y el Agent registra `AgentWorker` como servicio hospedado. Ambos proyectos están orientados a .NET 7 y su configuración puede incluir Firebird y Seq, que no deben formar parte de una prueba automatizada.

## Objetivos / No objetivos

**Objetivos:**

- Crear dos proyectos xUnit independientes, uno por ejecutable de Migraciones.
- Ejecutar la API en memoria con `WebApplicationFactory` y comprobar que `/health` es público y saludable.
- Crear un host en memoria para comprobar el ciclo de vida del Worker con señales asíncronas deterministas.
- Mantener las dependencias de prueba y cambios de producción en el mínimo necesario.

**No objetivos:**

- No probar Firebird, Seq, red, la API remota ni migraciones reales.
- No cambiar lógica de negocio, frameworks ni crear capas de abstracción generales.
- No levantar servidores externos ni modificar la configuración productiva para los tests.

## Decisiones

### API: host de integración en memoria

El proyecto de pruebas referenciará la API y usará `Microsoft.AspNetCore.Mvc.Testing` con `WebApplicationFactory<Program>`. `Program.cs` expondrá solamente la declaración parcial pública requerida por el framework de pruebas.

Una fábrica especializada establecerá un entorno de prueba y suprimirá o reemplazará la configuración que active dependencias externas, para que el host pueda iniciarse sin Firebird ni Seq. Se probará con `HttpClient` el HTTP 200 de `/health` y la ausencia de necesidad de autenticación.

Se prefiere `WebApplicationFactory` a un servidor de proceso porque verifica el pipeline real sin depender de puertos, procesos ni infraestructura local.

### Agent: host en memoria con señales de ciclo de vida

El proyecto de pruebas referenciará el Agent y construirá un host de `Microsoft.Extensions.Hosting` en memoria que registre el Worker. Un Worker de prueba o una señal de coordinación inyectada observará de forma determinista el inicio, la cancelación y la detención mediante `TaskCompletionSource` y tokens de cancelación.

Las pruebas esperarán esas señales en lugar de usar `Thread.Sleep` o límites temporales arbitrarios. Se prefiere esta estrategia a iniciar el ejecutable completo, ya que evita extensiones de servicio, API remota y efectos de proceso.

### Dependencias y solución

Cada proyecto de pruebas se mantendrá en `net7.0` y declarará solo xUnit, el SDK de pruebas y las referencias imprescindibles del proyecto probado; la suite de API agregará el paquete de pruebas ASP.NET Core compatible. Ambos se incluirán en `ElPradoWeb.sln` y la guía existente de Migraciones documentará los comandos de ejecución.

## Riesgos / Compensaciones

- [Configuración de observabilidad inicializada durante pruebas] → La fábrica configurará explícitamente el entorno y la configuración de pruebas para que Seq sea opcional y no haya conexiones remotas.
- [Servicios hospedados que continúan ejecutándose al finalizar una prueba] → Cada prueba administrará el host con `await using` o `try/finally` y solicitará su detención con un token cancelado.
- [Pruebas frágiles por temporización] → Las aserciones se basarán en tareas de señalización de inicio/detención, sin pausas arbitrarias.

## Plan de migración

Los cambios se agregan como proyectos nuevos sin modificar datos ni contratos. Si alguna dependencia de pruebas provoca incompatibilidad, se puede retirar ambos proyectos y sus entradas de la solución sin afectar el arranque de API o Agent.
