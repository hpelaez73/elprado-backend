# Architecture Decision Records

Esta carpeta contiene las decisiones arquitectónicas (ADR) de `elprado-backend`.

Un ADR registra **qué decisión arquitectónica se tomó y por qué**. No debe duplicar Issues ni OpenSpec.

## Cuándo crear un ADR

Crear un ADR cuando una decisión:

- afecta la arquitectura o varios componentes;
- establece una convención que deberá mantenerse;
- selecciona una tecnología, patrón o estrategia relevante;
- tiene alternativas razonables;
- modifica una decisión arquitectónica anterior.

Regla práctica: si en el futuro alguien puede preguntarse **"¿por qué se hizo así?"**, probablemente merece un ADR.

## Convenciones

Los ADR son globales al repositorio y usan numeración secuencial:

```text id="tpr13b"
0001-mcp-como-servicio-independiente.md
0002-identificacion-consumidor-api.md
```

El campo `Ámbito` indica los componentes afectados, por ejemplo:

```text id="3zlyyj"
Global
ElPrado.McpApi
ElPrado.WebApi, ElPrado.McpApi
```

Estados permitidos:

- `Propuesto`
- `Aceptado`
- `Descartado`
- `Reemplazado por ADR-NNNN`

Los ADR no se eliminan ni se reutilizan sus números.

## Relación con el proceso

```text id="id1mdz"
Issue      → problema / necesidad
ADR        → decisión arquitectónica y motivo
OpenSpec   → cambio a implementar
```

Referenciar entre ellos cuando corresponda.

## Agentes de IA

Los ADR `Aceptado` deben tratarse como restricciones arquitectónicas.

Antes de realizar un cambio arquitectónico, revisar los ADR relacionados. Si un cambio contradice uno existente, proponer una nueva decisión en lugar de ignorarlo.

## Índice

| ADR | Decisión | Ámbito | Estado |
|---|---|---|---|
| — | — | — | — |

Para crear un ADR usar `0000-template.md`.