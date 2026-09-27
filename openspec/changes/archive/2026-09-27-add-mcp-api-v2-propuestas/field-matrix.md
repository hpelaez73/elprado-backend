# Matriz de procedencia de campos — MCP API v2 Propuestas

Esta matriz releva las secciones 5 a 11 de `docs/mcp-api-v2-propuestas.md` contra el código disponible. Una fila marcada como **extender** tiene una tarea de extensión explícita; no puede ser completada mediante inferencia en `ElPrado.McpApi`.

| Recurso y propiedades canónicas | Fuente actual | Estado / tarea asignada |
|---|---|---|
| Detalle: `propuesta`, `codigo`, `tipo`, `fechaAlta`, `fechaBaja` | `DtoPropuestaDetalleResp`; `PropuestasRepository.BuscarPropuestaDetalle`; `GET_CONSULTA_PROPUESTA` | Disponible; 1.2 adapta tipos y nulos. |
| Detalle: `estado`, `deuda.estado.{nombre,activa,alDia,permiteImputar}` | `PropuestasRepository.BuscarEstadoDeuda`; tabla `ESTADOS_DEUDAS` (`ESTADO`, `ACTIVA`, `AL_DIA`, `PERMITIR_IMPUTAR`) | Disponible para 1.2 mediante consulta estructurada al estado autoritativo; no se usa el nombre para deducir flags. |
| Detalle: `parcela.{codigo,numero,manzana}` y `zonas` | `DtoPropuestaDetalleResp`; `BuscarZonasParcelas`; `GET_CONSULTA_PROPUESTA`, `GET_DATOS_PARCELAS_ZONAS` | Disponible; 1.2 adapta tipos y nulos. |
| Detalle: `parcela.estado.{nombre,permanente,disponibleVenta,disponibleInhumar,inhabilitada,conInhumado}` | tabla `ESTADOS_PARCELA`; la parcela referencia `COD_ESTADO` | **Extender 1.2**: proyectar flags desde la tabla autoritativa. |
| Detalle: `parcela.lugares[].{nivel,posicion,estado,inhumado}` | `GET_DATOS_PARCELAS_LUGARES`; `ESTADOS_LUGARES`; `DET_INHUMADOS_LUGARES` | **Extender 1.4**: usar esta salida normalizada en lugar de `GET_DATOS_PARCELAS_NIVELES`; ya contiene flags y claves de inhumado. |
| Detalle: `inhumados[]` y `propuestasAsociadas[]` | `DtoInhumadosPropuestas`, `DtoPropuestasAsociadas`; `InhumadosRepository.BuscarInhumados`, `BuscarPropuestasAsociadas`; `GET_DATOS_*` | Disponible en parte; **1.2/1.4** completa documento y claves inequívocas. |
| Detalle: `alertas[]` | `MuestraMensajeAlerta`, `MensajeAlerta` de `GET_CONSULTA_PROPUESTA` | **Extender 1.2**: colección semántica, no mensaje de presentación. |
| Titulares: `cantidad`, `codigo`, `orden`, identidad, contacto, domicilio y fechas | `DtoClientesPropuestas`; `ClientesRepository.BuscarTitulares` | Disponible excepto regla formal de principal; **1.2** confirma que `orden == 1` es la regla de dominio. |
| Historial: cliente, altas/bajas y usuarios | `DtoClientesPropuestasHistorial`; `PropuestasRepository.DetalleHistorialTitulares`; `GET_DATOS_HISTORIAL_TITULARES` | Disponible; **1.2** normaliza cliente con código y nulos. |
| Deuda: `totales`, cuenta, período, importes, cliente y cobranza | `DtoCuentasCorrientesResumen`; `CuentasCorrientesRepository.ResumenCuentas`; `CONSULTA_RESUMEN_CUENTAS`; `CuentasCorrientesRepository.EstadosDeuda` | Los importes por cuenta proceden del ERP; **1.2** los convierte a `decimal` y `CuentasCorrientesService.ResumenDeuda` calcula el total de propuesta mediante suma, por decisión explícita del usuario. La sección CIBA fue retirada del procedimiento aplicado por no tener datos vigentes. |
| Servicios: habilitaciones, motivo estructurado y cliente | `CONSULTA_SERVICIOS_PROP`; `ServiciosModelosRepository.ServiciosPropuesta`; `GET_SERVICIOS_PROPUESTA` | **Extender 1.3**: proyectar la fuente fila a fila; no interpretar `TituloServicioNN`/`MensajeServicioNN`; falta motivo estructurado. |
| Servicios: cupos | `DtoServiciosUtilizados`; `ServiciosUtilizadosPropuesta` | **Extender 1.3**: proveer producto, servicio, total, utilizados y disponibles. |
| Servicios: utilizaciones, beneficiario, comprobante y propuestas | `DtoServiciosBeneficiarios`; `BeneficiariosPropuesta` | **Extender 1.3**: documento tipado, nulos y las dos propuestas públicas. |
| Contratos y plan de venta | `DtoContratosPropuestas`, `DtoPlanesVentasPropuestas`; repositorios `Contratos` y `PlanesVentas` | Disponible: `CodContrato` vincula el plan; `Estado` se determina por `FECHA_BAJA` (`ACTIVO` si es nula, `BAJA` en otro caso). |
| Facturación: titulares habilitados | `DtoClientesPropuestasFacturasPagos`; `ClientesRepository.BuscarTitularesFacturasPagos` | Disponible; 1.4 normaliza la proyección. |
| Comprobantes: item y paginación | `GET_COMPROBANTES_PROPUESTA_V2`; `DtoComprobantesPropuestaV2`; `ComprobantesRepository.ComprobantesPropuestaV2` | Disponible: filtros tipados `desde/hasta/tipo/estado`, categoría de tipo, cliente con código y `anulado` booleano. La paginación y sus totales se calculan sobre esta colección filtrada en el servicio. |
| Resolución pública `propuesta` → `CodPropuesta` | `PropuestasService.BuscarPropuesta` (privado) y `PropuestasRepository.BuscarPropuesta` | **Extender 1.6**: adaptador reutilizable con resultado no encontrado. |

## Conclusión

No hay campos sin fuente ni tarea asignada. Las fuentes disponibles deben conservarse para los datos ya provistos; las filas marcadas **extender** requieren cambios de DTO, servicio, repositorio y, cuando corresponda, de los procedimientos Firebird indicados antes de exponer v2.
