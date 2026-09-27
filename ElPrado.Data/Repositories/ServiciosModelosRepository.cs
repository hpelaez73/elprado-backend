using Dapper;
using ElPrado.Dto.Dtos;

namespace ElPrado.Data.Repositories 
{
    public class ServiciosModelosRepository : RepositoryBase
    {
        public ServiciosModelosRepository(DbContext dbContext) : base(dbContext)
        {
        }

        public List<DtoServiciosHabilitados> ServiciosPropuesta(int codPropuesta)
        {
            string sql = "SELECT * FROM GET_SERVICIOS_PROPUESTA(@codPropuesta)";
            return _connection.Query<DtoServiciosHabilitados>(sql, new { codPropuesta }, _transaction).ToList();
        }

        public List<DtoServicioPropuestaMcpFuente> ServiciosPropuestaMcp(int codPropuesta)
        {
            const string sql = "SELECT * FROM GET_SERVICIOS_PROPUESTA_V2(@codPropuesta)";
            return _connection.Query<DtoServicioPropuestaMcpFuente>(sql, new { codPropuesta }, _transaction).ToList();
        }

        public List<DtoUtilizacionServicioMcp> UtilizacionesPropuestaMcp(int codPropuesta)
        {
            const string sql = @"SELECT DISTINCT COALESCE(H.FECHA_UTILIZACION, C.FECHA) AS FECHA,
                                        COALESCE(PM.NOMBRE, SM.DESCRIPCION) AS PRODUCTO,
                                        TS.TIPO_SERVICIO AS SERVICIO,
                                        CL.COD_CLIENTE, CL.NOMBRE AS BENEFICIARIO,
                                        CL.TIPO_DOCUMENTO, CL.NRO_DOCUMENTO,
                                        H.NRO_COMPROBANTE,
                                        PO.LEGAJO AS PROPUESTA_ORIGEN,
                                        PA.LEGAJO AS PROPUESTA_APLICACION
                                 FROM HIST_SERVICIOS_UTILIZADOS H
                                 INNER JOIN TIPOS_SERVICIOS TS ON TS.COD_TIPO_SERVICIO = H.COD_TIPO_SERVICIO
                                 INNER JOIN COMPROBANTES C ON C.COD_TALONARIO = H.COD_TALONARIO
                                                          AND C.NRO_COMPROBANTE = H.NRO_COMPROBANTE
                                 LEFT JOIN PLANES_VENTAS PV ON PV.COD_PLAN_VENTA = H.COD_PLAN_VENTA
                                 LEFT JOIN PLANES_MODELOS PM ON PM.COD_PLAN_MODELO = PV.COD_PLAN_MODELO
                                 LEFT JOIN SERVICIOS_MODELOS SM ON SM.COD_SERVICIO_MODELO = H.COD_SERVICIO_MODELO
                                 LEFT JOIN INHUMADOS I ON I.COD_INHUMADO = H.COD_INHUMADO
                                 LEFT JOIN CLIENTES CL ON CL.COD_CLIENTE = COALESCE(H.COD_CLIENTE_BENEFICIADO, I.COD_CLIENTE_INHUMADO)
                                 LEFT JOIN PROPUESTA PO ON PO.COD_PROPUESTA = H.COD_PROPUESTA_ORIGINAL
                                 LEFT JOIN PROPUESTA PA ON PA.COD_PROPUESTA = H.COD_PROPUESTA
                                 WHERE H.COD_PROPUESTA = @codPropuesta
                                    OR H.COD_PROPUESTA_ORIGINAL = @codPropuesta
                                 ORDER BY 1, 8";
            return _connection.Query<DtoUtilizacionServicioMcp>(sql, new { codPropuesta }, _transaction).ToList();
        }

        public List<DtoServiciosUtilizados> ServiciosUtilizadosPropuesta(int codPropuesta)
        {
            string sql = @" SELECT DISTINCT C.MODELO || ' - ' || C.SERVICIO AS SERVICIO, C.SERVICIOS_REALIZADOS, C.SERVICIOS_PENDIENTES, C.COD_SERVICIO_MODELO
                            FROM CONSULTA_SERVICIOS_PROP(@codPropuesta, 0) C
                            WHERE C.LIMITE_SERVICIOS > 0 AND COALESCE(C.VIGENCIA_HASTA, CURRENT_DATE) >= CURRENT_DATE";
            return _connection.Query<DtoServiciosUtilizados>(sql, new { codPropuesta }, _transaction).ToList();
        }

        public List<DtoServiciosBeneficiarios> BeneficiariosPropuesta(int codPropuesta)
        {
            string sql = "SELECT * FROM GET_BENEFICIARIOS_PROPUESTA(@codPropuesta)";
            return _connection.Query<DtoServiciosBeneficiarios>(sql, new { codPropuesta }, _transaction).ToList();
        }
    }
}
