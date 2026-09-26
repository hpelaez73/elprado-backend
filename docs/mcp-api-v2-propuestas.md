# Contrato MCP API v2 — Propuestas

## 1. Objetivo

`ElPrado.McpApi` expone una API REST orientada al consumo por agentes y LLM.

No pretende reproducir los DTO utilizados por `ElPrado.WebApi` ni satisfacer necesidades de presentación del frontend. Sus contratos son **Agent Read Models**: proyecciones semánticas de los datos y reglas de negocio que facilitan que un agente pueda interpretar correctamente la información.

Arquitectura:

```text
ChatGPT / Agente
       │
       ▼
   elprado-mcp
       │
       ▼
 ElPrado.McpApi
       │
       ▼
 ElPrado.Services
       │
       ▼
    Firebird
```

Principio fundamental:

> Los nombres configurables se conservan para aportar contexto humano, pero las decisiones del agente deben poder basarse en propiedades funcionales explícitas suministradas por el dominio. `McpApi` proyecta las reglas de negocio; no debe reinventarlas.

---

# 2. Convenciones generales

Base URL:

```text
/api/v2/propuestas
```

Las operaciones de consulta utilizan `GET`.

El identificador público utilizado en las rutas es el número de propuesta:

```text
propuesta
```

`CodPropuesta` es un identificador interno. `McpApi` debe resolverlo internamente cuando sea necesario.

Puede devolverse como `codigo` cuando sea útil para correlacionar información, pero el consumidor no debería necesitar conocerlo para realizar consultas.

## 2.1 Fechas

Todas las fechas se expresan en ISO 8601:

```json
"fecha": "2026-09-25"
```

No utilizar formatos regionales como:

```text
25/09/2026
```

excepto cuando formen parte de un texto descriptivo heredado.

## 2.2 Importes

Los importes monetarios deben utilizar `decimal` en la implementación.

Ejemplo:

```json
"total": 40500.00
```

## 2.3 Valores inexistentes

Utilizar:

```json
"fechaBaja": null
```

para valores individuales inexistentes y:

```json
"alertas": []
```

para colecciones sin elementos.

No utilizar valores especiales como `0`, `""` o fechas ficticias para representar ausencia de información.

---

# 3. Envelope de respuesta

Todos los endpoints mantienen un envelope estable.

## Respuesta exitosa

```json
{
  "succeeded": true,
  "data": {},
  "error": null
}
```

## Error

Ejemplo para una propuesta inexistente:

```json
{
  "succeeded": false,
  "data": null,
  "error": {
    "code": "PROPOSAL_NOT_FOUND",
    "message": "No se encontró la propuesta 41251."
  }
}
```

El campo `code` debe ser estable y apto para procesamiento automático.

`message` está destinado a humanos y agentes y puede ser descriptivo.

---

# 4. Resumen de endpoints

```text
GET /api/v2/propuestas/{propuesta}

GET /api/v2/propuestas/{propuesta}/titulares

GET /api/v2/propuestas/{propuesta}/deuda

GET /api/v2/propuestas/{propuesta}/servicios

GET /api/v2/propuestas/{propuesta}/contratos

GET /api/v2/propuestas/{propuesta}/comprobantes

GET /api/v2/propuestas/{propuesta}/historial-titulares
```

---

# 5. Detalle general de propuesta

```text
GET /api/v2/propuestas/{propuesta}
```

## Propósito

Proporcionar la información general de una propuesta.

Debe permitir responder preguntas como:

- ¿Qué tipo de propuesta es?
- ¿Está activa?
- ¿Qué parcela tiene?
- ¿Se puede vender?
- ¿Se puede inhumar?
- ¿Quiénes están inhumados?
- ¿Qué lugares tiene la parcela?
- ¿Hay lugares ocupados?
- ¿Tiene propuestas relacionadas?

La información detallada de deuda, servicios, contratos, comprobantes y titulares pertenece a sus endpoints específicos.

## Ejemplo

```json
{
  "succeeded": true,
  "data": {
    "propuesta": 41251,
    "codigo": 536871234,
    "tipo": "PARCELA",
    "fechaAlta": "2018-03-15",
    "fechaBaja": null,

    "estado": "ACTIVA",

    "deuda": {
      "estado": {
        "nombre": "MOROSO 1",
        "activa": true,
        "alDia": false,
        "permiteImputar": true
      }
    },

    "parcela": {
      "codigo": 20750,
      "numero": "20750",
      "manzana": "MANZANA 16",

      "estado": {
        "nombre": "Vendida / Ocupada",
        "permanente": true,
        "disponibleVenta": false,
        "disponibleInhumar": true,
        "inhabilitada": false,
        "conInhumado": true
      },

      "zonas": [
        "División General - Sector 3"
      ],

      "lugares": [
        {
          "nivel": 1,
          "posicion": 1,
          "estado": {
            "nombre": "Ocupado",
            "disponibleVenta": false,
            "disponibleInhumar": false
          },
          "inhumado": {
            "codigo": 536871068,
            "nombre": "SPEZZI ANTONIA JUDITH"
          }
        },
        {
          "nivel": 1,
          "posicion": 2,
          "estado": {
            "nombre": "Ocupado",
            "disponibleVenta": false,
            "disponibleInhumar": false
          },
          "inhumado": {
            "codigo": 536871069,
            "nombre": "SPEZZI CARLOS ALBERTO"
          }
        },
        {
          "nivel": 2,
          "posicion": 1,
          "estado": {
            "nombre": "Asignado",
            "disponibleVenta": false,
            "disponibleInhumar": true
          },
          "inhumado": null
        },
        {
          "nivel": 3,
          "posicion": 1,
          "estado": {
            "nombre": "Vendido",
            "disponibleVenta": false,
            "disponibleInhumar": true
          },
          "inhumado": null
        },
        {
          "nivel": 3,
          "posicion": 2,
          "estado": {
            "nombre": "Libre",
            "disponibleVenta": true,
            "disponibleInhumar": false
          },
          "inhumado": null
        }
      ]
    },

    "inhumados": [
      {
        "codigo": 536871068,
        "nombre": "SPEZZI ANTONIA JUDITH",
        "documento": {
          "tipo": "DNI",
          "numero": "6651775"
        },
        "fechaNacimiento": "1935-06-12",
        "fechaFallecimiento": "2018-04-08",
        "fechaInhumacion": "2018-04-09",
        "fechaExhumacion": null
      },
      {
        "codigo": 536871069,
        "nombre": "SPEZZI CARLOS ALBERTO",
        "documento": {
          "tipo": "DNI",
          "numero": "10234567"
        },
        "fechaNacimiento": "1948-11-20",
        "fechaFallecimiento": "2021-07-13",
        "fechaInhumacion": "2021-07-14",
        "fechaExhumacion": null
      }
    ],

    "propuestasAsociadas": [
      {
        "propuesta": 41250,
        "tipo": "PARCELA"
      },
      {
        "propuesta": 13068,
        "tipo": "SERVICIO"
      }
    ],

    "alertas": []
  },
  "error": null
}
```

## Reglas

Los estados de parcela son configurables. No crear un enum basado en nombres como `LIBRE`, `VENDIDA`, etc.

Debe conservarse el nombre configurado junto con sus flags funcionales:

```json
{
  "nombre": "Vendida / Ocupada",
  "permanente": true,
  "disponibleVenta": false,
  "disponibleInhumar": true,
  "inhabilitada": false,
  "conInhumado": true
}
```

Los colores asociados a estados son información exclusivamente visual y no forman parte del contrato MCP.

Los lugares deben normalizarse como una colección. No exponer estructuras como:

```text
nombreInhumado1
estado1
nombreInhumado2
estado2
...
```

Los estados de lugares también son configurables y se acompañan de:

```json
{
  "nombre": "Vendido",
  "disponibleVenta": false,
  "disponibleInhumar": true
}
```

Si el origen de datos no proporciona un identificador inequívoco del inhumado asociado al lugar, `McpApi` no debe intentar relacionarlo utilizando el nombre de la persona.

---

# 6. Titulares

```text
GET /api/v2/propuestas/{propuesta}/titulares
```

## Propósito

Obtener los titulares actuales de una propuesta y sus datos básicos de identificación y contacto.

## Ejemplo

```json
{
  "succeeded": true,
  "data": {
    "propuesta": 41251,
    "cantidad": 2,

    "titulares": [
      {
        "codigo": 536873944,
        "orden": 1,
        "esPrincipal": true,

        "nombre": "ASTURZZI ALFONSO LUIS",

        "documento": {
          "tipo": "DNI",
          "numero": "12345678"
        },

        "contacto": {
          "telefono": "0341-4000000",
          "movil": "3416000000",
          "email": "alfonso@example.com"
        },

        "domicilio": {
          "direccion": "SAN MARTIN 1234",
          "localidad": "ROSARIO",
          "provincia": "SANTA FE"
        },

        "fechaNacimiento": "1940-05-18",
        "fechaAlta": "2018-03-15"
      },

      {
        "codigo": 551922,
        "orden": 2,
        "esPrincipal": false,

        "nombre": "ASTURZZI RUBEN OSVALDO",

        "documento": {
          "tipo": "DNI",
          "numero": "20123456"
        },

        "contacto": {
          "telefono": null,
          "movil": "3416111111",
          "email": "ruben@example.com"
        },

        "domicilio": {
          "direccion": "CORDOBA 2500",
          "localidad": "ROSARIO",
          "provincia": "SANTA FE"
        },

        "fechaNacimiento": "1965-08-21",
        "fechaAlta": "2018-03-15"
      }
    ]
  },
  "error": null
}
```

## Reglas

`esPrincipal` puede derivarse de `orden == 1` siempre que esa sea una regla oficial del dominio.

Campos puramente técnicos o relacionados con presentación web no deben exponerse salvo que posean semántica útil para el agente.

---

# 7. Deuda

```text
GET /api/v2/propuestas/{propuesta}/deuda
```

## Propósito

Proporcionar la situación financiera de la propuesta y el detalle de sus cuentas corrientes.

Debe permitir distinguir entre:

- deuda reclamable;
- situación al día;
- deuda en mora;
- deuda que puede cobrarse por canales normales;
- deuda que requiere un tratamiento especial, por ejemplo un proceso judicial.

`McpApi` no determina estas situaciones analizando fechas o importes. Utiliza los estados y flags calculados por el ERP.

## Ejemplo

```json
{
  "succeeded": true,
  "data": {
    "propuesta": 41251,

    "estado": {
      "nombre": "MOROSO 1",
      "activa": true,
      "alDia": false,
      "permiteImputar": true
    },

    "totales": {
      "vencido": 38000.00,
      "intereses": 2800.00,
      "aVencer": 45000.00,
      "descuentoVencido": 0.00,
      "descuentoAVencer": 4500.00,
      "deuda": 81300.00,
      "total": 81300.00
    },

    "cuentas": [
      {
        "codigo": 14745,
        "tipo": "CP",
        "categoria": "Derecho Periódico por la Cesión",

        "estado": {
          "nombre": "MOROSO 1",
          "activa": true,
          "alDia": false,
          "permiteImputar": true
        },

        "cliente": {
          "codigo": 551922,
          "nombre": "ASTURZZI RUBEN OSVALDO"
        },

        "importe": 45000.00,

        "periodo": {
          "inicio": "2018-03-15",
          "primeraCuota": "2018-04-01",
          "cuotaDesde": "2026-08-01",
          "cuotaHasta": "2026-09-01"
        },

        "importes": {
          "vencido": 38000.00,
          "intereses": 2800.00,
          "aVencer": 45000.00,
          "descuentoVencido": 0.00,
          "descuentoAVencer": 4500.00,
          "deuda": 81300.00,
          "total": 81300.00
        },

        "cobranza": {
          "cobrador": "TARJETA - ORIGEN",
          "zona": "TARJETA - ORIGEN",
          "comercializadora": "ORIGEN S.A."
        }
      },

      {
        "codigo": 15980,
        "tipo": "FIN",
        "categoria": "Financiación",

        "estado": {
          "nombre": "CANCELADO",
          "activa": false,
          "alDia": false,
          "permiteImputar": false
        },

        "cliente": {
          "codigo": 536873944,
          "nombre": "ASTURZZI ALFONSO LUIS"
        },

        "importe": 120000.00,

        "periodo": {
          "inicio": "2018-03-15",
          "primeraCuota": "2018-04-01",
          "cuotaDesde": null,
          "cuotaHasta": null
        },

        "importes": {
          "vencido": 0.00,
          "intereses": 0.00,
          "aVencer": 0.00,
          "descuentoVencido": 0.00,
          "descuentoAVencer": 0.00,
          "deuda": 0.00,
          "total": 0.00
        },

        "cobranza": {
          "cobrador": null,
          "zona": null,
          "comercializadora": null
        }
      }
    ]
  },
  "error": null
}
```

## Estados

Los nombres son configurables.

Ejemplos actuales:

```text
ACTIVO
ACTIVADO
MOROSO RECIENTE
MOROSO 1
MOROSO 2
MOROSO 3
GESTION JUDICIAL
PROCESO JUDICIAL
CANCELADO
...
```

El agente no debe deducir el comportamiento a partir del nombre.

Los flags tienen la siguiente semántica:

### `activa`

```text
true  → cuenta vigente y deuda reclamable
false → deuda no reclamable
```

### `alDia`

```text
true  → según las reglas configuradas del ERP, la deuda no genera intereses
false → no está considerada al día
```

La regla temporal utilizada para determinar `alDia` pertenece al ERP y puede cambiar.

### `permiteImputar`

```text
true  → puede cobrarse mediante los canales normales
false → no debe cobrarse mediante los canales normales
```

Una cuenta puede simultáneamente tener:

```json
{
  "activa": true,
  "alDia": false,
  "permiteImputar": false
}
```

por ejemplo cuando se encuentra en gestión judicial.

El estado general de deuda de la propuesta es determinado por el ERP a partir de los estados de sus cuentas corrientes.

`McpApi` no debe recalcularlo.

---

# 8. Servicios

```text
GET /api/v2/propuestas/{propuesta}/servicios
```

## Propósito

Proporcionar tres dimensiones independientes:

1. habilitación de cada persona para utilizar los servicios incluidos en sus productos;
2. cupos contratados y disponibles de la propuesta;
3. prestaciones efectivamente utilizadas.

## Ejemplo completo

```json
{
  "succeeded": true,
  "data": {
    "propuesta": 41251,

    "habilitaciones": [
      {
        "cliente": {
          "codigo": 536873944,
          "nombre": "ASTURZZI ALFONSO LUIS"
        },

        "producto": "Plan Exclusivo Cremación",
        "servicio": "Cremación",

        "habilitado": false,

        "motivo": {
          "codigo": "EDAD",
          "descripcion": "Sin servicio por edad",
          "hasta": null
        }
      },

      {
        "cliente": {
          "codigo": 551922,
          "nombre": "ASTURZZI RUBEN OSVALDO"
        },

        "producto": "Plan Exclusivo Cremación",
        "servicio": "Cremación",

        "habilitado": true,
        "motivo": null
      },

      {
        "cliente": {
          "codigo": 536873944,
          "nombre": "ASTURZZI ALFONSO LUIS"
        },

        "producto": "Plan Plus 1º Periodo",
        "servicio": "Introducción",

        "habilitado": false,

        "motivo": {
          "codigo": "CARENCIA",
          "descripcion": "Sin servicio hasta 15/10/2026",
          "hasta": "2026-10-15"
        }
      },

      {
        "cliente": {
          "codigo": 551922,
          "nombre": "ASTURZZI RUBEN OSVALDO"
        },

        "producto": "Plan Plus 1º Periodo",
        "servicio": "Introducción",

        "habilitado": false,

        "motivo": {
          "codigo": "MORA",
          "descripcion": "Sin servicio por mora",
          "hasta": null
        }
      },

      {
        "cliente": {
          "codigo": 551922,
          "nombre": "ASTURZZI RUBEN OSVALDO"
        },

        "producto": "Plan Plus 1º Periodo",
        "servicio": "Sepelio",

        "habilitado": false,

        "motivo": {
          "codigo": "MORA",
          "descripcion": "Sin servicio por mora",
          "hasta": null
        }
      },

      {
        "cliente": {
          "codigo": 551922,
          "nombre": "ASTURZZI RUBEN OSVALDO"
        },

        "producto": "Plan Plus 1º Periodo",
        "servicio": "Servicio Médico",

        "habilitado": false,

        "motivo": {
          "codigo": "MORA",
          "descripcion": "Sin servicio por mora",
          "hasta": null
        }
      }
    ],

    "cupos": [
      {
        "producto": "Plan Exclusivo Cremación",
        "servicio": "Cremación",
        "total": 3,
        "utilizados": 0,
        "disponibles": 3
      },

      {
        "producto": "Plan Plus 1º Periodo",
        "servicio": "Introducción",
        "total": 3,
        "utilizados": 1,
        "disponibles": 2
      },

      {
        "producto": "Plan Plus 1º Periodo",
        "servicio": "Sepelio",
        "total": 3,
        "utilizados": 0,
        "disponibles": 3
      }
    ],

    "utilizaciones": [
      {
        "fecha": "2018-04-09",

        "producto": "Plan Plus 1º Periodo",
        "servicio": "Introducción",

        "beneficiario": {
          "nombre": "SPEZZI ANTONIA JUDITH",
          "documento": {
            "tipo": "DNI",
            "numero": "6651775"
          }
        },

        "comprobante": {
          "numero": "B-0006-00006207"
        },

        "propuestaOrigen": 41251,
        "propuestaAplicacion": 41250
      }
    ]
  },
  "error": null
}
```

## Habilitaciones

Cada elemento responde a:

> ¿Esta persona puede utilizar este servicio de este producto?

Un `mensajeServicioXX` vacío en el modelo actual significa que ese servicio **no forma parte del producto**.

Por lo tanto, esa combinación no debe aparecer en `habilitaciones`.

Por ejemplo:

```text
Plan Exclusivo Cremación
Farmacia
mensajeServicio = ""
```

no debe transformarse en:

```json
{
  "servicio": "Farmacia",
  "habilitado": false
}
```

Debe omitirse.

## Estados/motivos de habilitación

Normalización propuesta:

| Valor de dominio | habilitado | motivo.codigo |
|---|---:|---|
| Habilitado | true | null |
| Sin servicio - máximo utilizado | false | MAXIMO_UTILIZADO |
| Sin servicio hasta dd/mm/yyyy | false | CARENCIA |
| Sin servicio | false | NO_HABILITADO |
| Sin servicio por mora | false | MORA |
| Sin servicio por edad | false | EDAD |
| Servicio utilizado | false | SERVICIO_UTILIZADO |
| Sin servicio - el producto ha caducado | false | PRODUCTO_CADUCADO |

Idealmente esta información debe evolucionar para ser proporcionada estructuradamente por la capa de dominio.

Debe evitarse que `McpApi` dependa permanentemente de comparar textos para descubrir reglas de negocio.

## Cupos

Los cupos pertenecen a la propuesta/producto y no a cada titular.

```text
total = utilizados + disponibles
```

siempre que esta igualdad sea garantizada por el dominio.

Los nombres actuales:

```text
serviciosRealizados
serviciosPendientes
```

se transforman conceptualmente en:

```text
utilizados
disponibles
```

`pendientes` no se utiliza porque podría interpretarse erróneamente como prestaciones ya solicitadas pendientes de realización.

## Utilizaciones entre propuestas

Una prestación puede consumir un beneficio perteneciente a una propuesta pero aplicarse sobre otra.

Ejemplo:

```json
{
  "propuestaOrigen": 41251,
  "propuestaAplicacion": 41250
}
```

significa:

> El derecho al servicio pertenece a la propuesta 41251 y fue utilizado en la propuesta 41250.

Esto reemplaza la semántica menos explícita de:

```text
usadoDe
usadoEn
```

Los valores `0` utilizados actualmente para indicar ausencia de relación no deben exponerse en v2.

## `cantServicios`

Se elimina completamente.

Es información exclusivamente visual utilizada por el frontend para determinar la cantidad de columnas a renderizar.

No representa una cantidad de servicios habilitados ni contratados.

---

# 9. Contratos

```text
GET /api/v2/propuestas/{propuesta}/contratos
```

## Propósito

Obtener los contratos comerciales relacionados con la propuesta, los planes de venta asociados y la configuración relevante para facturación/pagos.

## Ejemplo

```json
{
  "succeeded": true,
  "data": {
    "propuesta": 41251,

    "contratos": [
      {
        "codigo": 2,
        "fecha": "2018-03-15",

        "tipo": "VENTA",
        "modelo": "CONTRATO PARQUE",

        "estado": "ACTIVO",

        "total": 130000.00,

        "vendedor": {
          "nombre": "JUAN PEREZ"
        },

        "planVenta": {
          "codigo": 536873227,
          "nombre": "PLAN 24 CUOTAS",
          "concepto": "CESIÓN PARCELA"
        }
      }
    ],

    "facturacion": {
      "titularesHabilitados": [
        {
          "codigo": 536873944,
          "nombre": "ASTURZZI ALFONSO LUIS",
          "factura": true,
          "pago": true
        },
        {
          "codigo": 551922,
          "nombre": "ASTURZZI RUBEN OSVALDO",
          "factura": false,
          "pago": false
        }
      ]
    }
  },
  "error": null
}
```

## Regla importante

El contrato v1 devuelve actualmente contratos y planes de venta en colecciones independientes.

No debe suponerse que:

```text
contratos[0]
```

corresponde a:

```text
planesVentas[0]
```

simplemente por posición.

`planVenta` sólo debe anidarse dentro del contrato cuando exista una clave o relación inequívoca suministrada por el dominio.

Si esa relación no está disponible, v2 debe mantener ambas colecciones separadas hasta que el servicio de dominio pueda proporcionarla.

---

# 10. Comprobantes

```text
GET /api/v2/propuestas/{propuesta}/comprobantes
```

## Query params

Ejemplos:

```text
?desde=2026-01-01
&hasta=2026-09-30
&tipo=FACTURA
&estado=CONTADO
&page=1
&pageSize=20
```

Todos son opcionales salvo los parámetros de paginación que tengan defaults definidos.

No debe exponerse al agente el mecanismo genérico de filtros del frontend basado en estructuras como:

```text
Campo
TipoComparacion
Valor
```

Los filtros de MCP API deben representar conceptos del dominio.

## Ejemplo

```json
{
  "succeeded": true,
  "data": {
    "propuesta": 41251,

    "items": [
      {
        "fecha": "2026-08-10",
        "tipo": "FACTURA",
        "numero": "B-0006-00124567",

        "cliente": {
          "codigo": 551922,
          "nombre": "ASTURZZI RUBEN OSVALDO"
        },

        "total": 45000.00,
        "pago": 45000.00,

        "estado": "CONTADO",
        "anulado": false
      },

      {
        "fecha": "2026-09-10",
        "tipo": "FACTURA",
        "numero": "B-0006-00126789",

        "cliente": {
          "codigo": 551922,
          "nombre": "ASTURZZI RUBEN OSVALDO"
        },

        "total": 45000.00,
        "pago": 0.00,

        "estado": "PENDIENTE",
        "anulado": false
      },

      {
        "fecha": "2026-09-12",
        "tipo": "NOTA DE CRÉDITO",
        "numero": "B-0006-00004567",

        "cliente": {
          "codigo": 551922,
          "nombre": "ASTURZZI RUBEN OSVALDO"
        },

        "total": 5000.00,
        "pago": 0.00,

        "estado": "APLICADO",
        "anulado": false
      }
    ],

    "pagination": {
      "page": 1,
      "pageSize": 20,
      "totalItems": 3,
      "totalPages": 1,
      "hasNext": false,
      "hasPrevious": false
    }
  },
  "error": null
}
```

## Reglas

Valores heredados como:

```text
"SI"
"NO"
```

deben transformarse a booleanos cuando su semántica sea inequívoca:

```json
"anulado": true
```

Los identificadores técnicos internos deben omitirse salvo que sean necesarios para realizar una operación posterior.

---

# 11. Historial de titulares

```text
GET /api/v2/propuestas/{propuesta}/historial-titulares
```

## Propósito

Mostrar la evolución histórica de los titulares de una propuesta.

No reemplaza `/titulares`, que representa el estado actual.

## Ejemplo

```json
{
  "succeeded": true,
  "data": {
    "propuesta": 41251,

    "historial": [
      {
        "cliente": {
          "codigo": 536873944,
          "nombre": "ASTURZZI ALFONSO LUIS"
        },

        "fechaAlta": "2018-03-15",
        "fechaBaja": null,

        "usuarioAlta": "ADMIN",
        "usuarioBaja": null
      },

      {
        "cliente": {
          "codigo": 541758,
          "nombre": "CUASSOLO CARINA ANDREA"
        },

        "fechaAlta": "2018-03-15",
        "fechaBaja": "2021-06-20",

        "usuarioAlta": "ADMIN",
        "usuarioBaja": "HPelaez"
      },

      {
        "cliente": {
          "codigo": 551922,
          "nombre": "ASTURZZI RUBEN OSVALDO"
        },

        "fechaAlta": "2021-06-20",
        "fechaBaja": null,

        "usuarioAlta": "HPelaez",
        "usuarioBaja": null
      }
    ]
  },
  "error": null
}
```

El historial debe representar hechos registrados por el ERP. `McpApi` no debe inferir transferencias, reemplazos o relaciones entre titulares simplemente por proximidad entre fechas.

---

# 12. Búsqueda de propuestas

La búsqueda no forma parte de los siete endpoints iniciales, pero debe contemplarse como siguiente extensión:

```text
GET /api/v2/propuestas/buscar
```

Ejemplos:

```text
/api/v2/propuestas/buscar?nombre=asturzzi

/api/v2/propuestas/buscar?documento=20123456

/api/v2/propuestas/buscar?parcela=20750
```

Su función será permitir que un agente encuentre candidatos cuando el usuario no conoce el número de propuesta.

Debe devolver resultados compactos y suficientes para desambiguar, no el detalle completo de cada propuesta.

---

# 13. Transformaciones principales respecto de v1

## Eliminar

Información exclusivamente visual o estructural del frontend:

```text
cantServicios
colores
cantidad de columnas
metadatos de grillas
estructuras genéricas de filtrado
```

## Transformar

```text
CodPropuesta
    → codigo interno; la ruta utiliza propuesta

lugares con campos 1..6
    → lugares[]

tituloServicio01..15
mensajeServicio01..15
    → habilitaciones[]

serviciosRealizados
    → utilizados

serviciosPendientes
    → disponibles

usadoDe / usadoEn
    → propuestaOrigen / propuestaAplicacion

"SI" / "NO"
    → boolean cuando corresponda

estado de parcela
    → nombre + flags funcionales

estado de lugar
    → nombre + flags funcionales

estado de cuenta corriente
    → nombre + flags funcionales
```

## Conservar como fuente autoritativa

Los cálculos realizados por el ERP:

```text
estado de deuda de la propuesta
estado de las cuentas corrientes
importes de deuda
intereses
descuentos
habilitación de servicios
cupos utilizados/disponibles
estado funcional de parcelas y lugares
```

`McpApi` no debe reproducir estas reglas.

---

# 14. Regla de diseño de DTOs

Los DTO existentes en:

```text
ElPrado.Dto
```

no se convierten automáticamente en contratos públicos de:

```text
ElPrado.McpApi
```

El flujo debe ser:

```text
DTO / modelo de dominio
        │
        ▼
Mapper / Projection
        │
        ▼
MCP API Contract
```

Esto permite que el dominio y el frontend evolucionen independientemente del contrato destinado a agentes.

---

# 15. Responsabilidad de cada capa

## ElPrado.Services

Debe determinar hechos de negocio:

```text
qué estado tiene una cuenta
si está al día
si permite imputar
si una parcela permite inhumar
si un servicio está habilitado
por qué está bloqueado
cuántos servicios quedan disponibles
```

## ElPrado.McpApi

Debe transformar esos hechos en contratos semánticos, estables y fáciles de consumir.

Puede:

```text
renombrar campos
agrupar datos
normalizar colecciones
convertir flags 0/1 a boolean
convertir fechas a ISO
eliminar datos de presentación
```

No debe:

```text
recalcular mora
decidir si alguien está al día
interpretar nombres de estados configurables
decidir si una parcela está disponible mirando el texto
recalcular habilitaciones de servicios
hacer joins basados en nombres o posición de arrays
```

## MCP Server

Debe definir tools orientadas a intenciones del usuario.

No existe obligación de mantener una relación 1:1 entre:

```text
REST endpoint ↔ MCP tool
```

Una tool puede utilizar uno o varios endpoints.

---

# 16. Preguntas que el contrato debe permitir responder

Sin que el LLM tenga que reconstruir reglas del ERP, la API debe proporcionar información suficiente para preguntas como:

```text
¿Qué parcela tiene la propuesta 41251?

¿Se puede vender esa parcela?

¿Se puede realizar una inhumación?

¿Qué lugares están disponibles?

¿Quiénes están inhumados?

¿Quién es el titular principal?

¿Cuáles son los titulares actuales?

¿Quiénes fueron titulares anteriormente?

¿La propuesta está al día?

¿Cuánto debe?

¿Tiene deuda vencida?

¿Se puede cobrar normalmente?

¿Por qué no se puede cobrar por mostrador?

¿Qué servicios tiene Rubén?

¿Rubén puede usar cremación?

¿Por qué no puede usar sepelio?

¿Cuántas cremaciones quedan disponibles?

¿Cuántas introducciones se utilizaron?

¿Quién utilizó una introducción?

¿De qué propuesta salió ese beneficio?

¿En qué propuesta se utilizó?

¿Qué contratos tiene?

¿Quién puede recibir las facturas?

¿Cuáles son los últimos comprobantes?

¿Hay comprobantes anulados?
```

Si para contestar una de estas preguntas el agente necesita interpretar una convención técnica del frontend o reconstruir una regla del ERP, debe evaluarse si falta información semántica en el contrato.

---

# 17. Versionado y transición

Los endpoints actuales deben mantenerse durante la construcción de v2.

```text
v1 → consumidores actuales

v2 → ElPrado.McpApi / agentes
```

La migración sugerida es:

```text
1. Implementar contratos v2.

2. Probar los endpoints independientemente.

3. Adaptar las tools del servidor MCP.

4. Probar preguntas reales desde ChatGPT.

5. Comparar las respuestas con el ERP.

6. Retirar v1 únicamente cuando deje de tener consumidores.
```

---

# 18. Principio final

El contrato debe favorecer **hechos estructurados sobre textos interpretables**.

Preferir:

```json
{
  "nombre": "PROCESO JUDICIAL",
  "activa": true,
  "alDia": false,
  "permiteImputar": false
}
```

sobre:

```json
{
  "estado": "PROCESO JUDICIAL"
}
```

y:

```json
{
  "servicio": "Sepelio",
  "habilitado": false,
  "motivo": {
    "codigo": "MORA",
    "descripcion": "Sin servicio por mora"
  }
}
```

sobre:

```json
{
  "servicio": "Sepelio",
  "mensaje": "Sin servicio por mora"
}
```

El objetivo no es que el LLM conozca las convenciones internas de El Prado.

El objetivo es que **El Prado le entregue al agente los hechos de negocio que ya conoce**, de una forma estable, explícita y semánticamente inequívoca.