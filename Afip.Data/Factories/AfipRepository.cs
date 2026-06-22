using Afip.Data.Interfaces;
using Afip.Data.Models;
using Dapper;

namespace Afip.Data.Factories;

public class AfipRepository : IAfipRepository
{
    private readonly IDbConnectionFactory _connectionFactory;

    public AfipRepository(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    #region Configuración AFIP
    public async Task<AfipConfiguraciones?> GetAfipConfigurationAsync()
    {
        const string sql = @"
            SELECT 
                AFIP_TOKEN,
                AFIP_SIGN,
                AFIP_GENERACION,
                AFIP_EXPIRACION,
                AFIP_ID_UNICO
            FROM CONFIGURACION_LOCAL";

        using var connection = _connectionFactory.Create();
        return await connection.QueryFirstOrDefaultAsync<AfipConfiguraciones>(sql);
    }

    public async Task UpdateAfipConfigurationAsync(AfipConfiguraciones config)
    {
        const string sql = @"
            UPDATE CONFIGURACION_LOCAL 
            SET 
                AFIP_TOKEN = @AfipToken,
                AFIP_SIGN = @AfipSign,
                AFIP_GENERACION = @AfipGeneracion,
                AFIP_EXPIRACION = @AfipExpiracion,
                AFIP_ID_UNICO = @AfipIdUnico";

        using var connection = _connectionFactory.Create();
        await connection.ExecuteAsync(sql, config);
    }
    #endregion

    #region Comprobantes
    public async Task<AfipComprobante?> GetComprobanteParaCAEAsync(int codTalonario, string nroComprobante)
    {
        const string sql = @"
            SELECT C.COD_TALONARIO, C.NRO_COMPROBANTE,
                ATC.ID AS AFIP_TIPO_COMPROBANTE, T.PUNTO_VENTA, T.LETRA, C.AFIP_CONCEPTO,
                (SELECT ATD.ID FROM AFIP_TIPOS_DOCUMENTOS ATD WHERE ATD.TIPO_DOCUMENTO = CL.TIPO_DOCUMENTO) AS AFIP_TIPO_DOCUMENTO,
                (SELECT ACI.ID FROM AFIP_CONDICION_FRENTE_AL_IVA ACI INNER JOIN CAT_IVA CI ON CI.ID_CONDICION_FRENTE_AL_IVA = ACI.ID WHERE CI.COD_CAT_IVA = CL.COD_CAT_IVA) AS AFIP_CONDICION,
                CL.NRO_DOCUMENTO, CL.CUIT, C.FECHA, C.TOTAL, C.NETO - C.DESCUENTO_NETO + C.RECARGO_NETO + C.AJUSTE_GRAVADO AS NETO,
                C.NO_GRAVADO - C.DESCUENTO_NO_GRAVADO + C.RECARGO_NO_GRAVADO + C.EXENTO - C.DESCUENTO_EXENTO + C.RECARGO_EXENTO + C.IMPUESTO AS NO_GRAVADO,
                C.IVA + C.AJUSTE_IVA AS IVA, C.AFIP_SERVICIO_DESDE, C.AFIP_SERVICIO_HASTA
            FROM COMPROBANTES C
            INNER JOIN TALONARIOS T ON T.COD_TALONARIO = C.COD_TALONARIO
            INNER JOIN AFIP_TIPOS_COMPROBANTES ATC ON ATC.COD_TIPO_COMPROBANTE = C.COD_TIPO_COMPROBANTE AND ATC.LETRA_TALONARIO = T.LETRA
            LEFT OUTER JOIN CLIENTES CL ON CL.COD_CLIENTE = C.COD_CLIENTE
            WHERE C.COD_TALONARIO = @codTalonario AND C.NRO_COMPROBANTE = @nroComprobante";

        using var connection = _connectionFactory.Create();
        return await connection.QueryFirstOrDefaultAsync<AfipComprobante>(sql, new { codTalonario, nroComprobante });
    }

    public async Task<IEnumerable<AfipAlicuotas>> GetAlicuotasComprobanteAsync(int codTalonario, string nroComprobante)
    {
        const string sql = @"
            SELECT 
                A.ID_IVA,
                SUM(C.IMPORTE) + MIN(CO.AJUSTE_IVA) AS IMPORTE,
                MIN(CO.NETO - CO.DESCUENTO_NETO + CO.RECARGO_NETO + CO.AJUSTE_GRAVADO) AS BASEIMP
            FROM DET_COMPROBANTES_ALICUOTAS C
            INNER JOIN ALICUOTAS A ON A.COD_ALICUOTA = C.COD_ALICUOTA
            INNER JOIN COMPROBANTES CO ON CO.COD_TALONARIO = C.COD_TALONARIO AND CO.NRO_COMPROBANTE = C.NRO_COMPROBANTE
            WHERE C.COD_TALONARIO = @codTalonario AND C.NRO_COMPROBANTE = @nroComprobante
            AND A.ID_IVA IS NOT NULL
            GROUP BY C.COD_TALONARIO, C.NRO_COMPROBANTE, A.ID_IVA, C.PORCENTAJE";

        using var connection = _connectionFactory.Create();
        return await connection.QueryAsync<AfipAlicuotas>(sql, new { codTalonario, nroComprobante });
    }

    public async Task ActualizarRespuestaCAEAsync(int codTalonario, string nroComprobante, DateTime afipFechaProceso, string afipResultado, string afipCae, DateTime? afipVencimientoCae, string? afipObservaciones, string? afipJson)
    {
        string sql;
        using var connection = _connectionFactory.Create();

        if (afipResultado == "A" && afipCae != null && afipVencimientoCae != null)
        {
            sql = @"UPDATE COMPROBANTES C SET
                    C.AFIP_FECHA_PROCESO = @afipFechaProceso,
                    C.AFIP_RESULTADO = @afipResultado,
                    C.AFIP_CAE = @afipCae,
                    C.AFIP_VENCIMIENTO_CAE = @afipVencimientoCae
                    WHERE C.COD_TALONARIO = @codTalonario 
                    AND C.NRO_COMPROBANTE = @nroComprobante";

            await connection.ExecuteAsync(sql, new
            {
                afipFechaProceso,
                afipResultado,
                afipCae,
                afipVencimientoCae,
                codTalonario,
                nroComprobante
            });
        }
        else
        {
            sql = @"UPDATE COMPROBANTES C SET
                    C.AFIP_FECHA_PROCESO = @afipFechaProceso,
                    C.AFIP_RESULTADO = @afipResultado
                    WHERE C.COD_TALONARIO = @codTalonario 
                    AND C.NRO_COMPROBANTE = @nroComprobante";

            await connection.ExecuteAsync(sql, new
            {
                afipFechaProceso,
                afipResultado,
                codTalonario,
                nroComprobante
            });
        }

        if (afipObservaciones != null)
        {
            sql = @"UPDATE COMPROBANTES C SET C.AFIP_OBSERVACIONES = @afipObservaciones
                    WHERE C.COD_TALONARIO = @codTalonario 
                    AND C.NRO_COMPROBANTE = @nroComprobante";

            await connection.ExecuteAsync(sql, new { afipObservaciones, codTalonario, nroComprobante });
        }

        if (afipJson != null)
        {
            sql = @"UPDATE COMPROBANTES C SET C.AFIP_JSON = @afipJson
                    WHERE C.COD_TALONARIO = @codTalonario 
                    AND C.NRO_COMPROBANTE = @nroComprobante";

            await connection.ExecuteAsync(sql, new { afipJson, codTalonario, nroComprobante });
        }
    }

    public async Task<AfipTiposComprobantes?> GetAfipTipoComprobanteAsync(int codTalonario, int codTipoComprobante)
    {
        const string sql = @"
            SELECT A.ID, T.PUNTO_VENTA, T.LETRA
            FROM AFIP_TIPOS_COMPROBANTES A
            INNER JOIN TALONARIOS_TIPOS_COMPROBANTES TTC ON TTC.COD_TIPO_COMPROBANTE = A.COD_TIPO_COMPROBANTE
            INNER JOIN TALONARIOS T ON T.COD_TALONARIO = TTC.COD_TALONARIO AND T.LETRA = A.LETRA_TALONARIO
            WHERE A.COD_TIPO_COMPROBANTE = @codTipoComprobante
            AND T.COD_TALONARIO = @codTalonario";

        using var connection = _connectionFactory.Create();
        return await connection.QueryFirstOrDefaultAsync<AfipTiposComprobantes>(sql, new { codTalonario, codTipoComprobante });
    }

    public async Task ActualizarUltimoComprobanteEmitidoAsync(AfipUltimoComprobanteEmitido ultimoComprobante)
    {
        const string sql = @"
            UPDATE OR INSERT INTO AFIP_ULTIMO_COMPROBANTE_EMITIDO (COD_TALONARIO, NRO_COMPROBANTE, FECHA_CONSULTA)
            VALUES (@CodTalonario, @NroComprobante, CURRENT_TIMESTAMP)
            MATCHING (COD_TALONARIO)";

        using var connection = _connectionFactory.Create();
        await connection.ExecuteAsync(sql, new { ultimoComprobante.CodTalonario, ultimoComprobante.NroComprobante });
    }

    public async Task ActualizarErrorCAEAsync(int codTalonario, string nroComprobante, DateTime afipFechaProceso, string afipResultado, string msgError, string? afipJson)
    {
        using var connection = _connectionFactory.Create();

        string sql = @"UPDATE COMPROBANTES C SET
                    C.AFIP_FECHA_PROCESO = @afipFechaProceso,
                    C.AFIP_RESULTADO = @afipResultado,
                    C.AFIP_OBSERVACIONES = @msgError
                    WHERE C.COD_TALONARIO = @codTalonario 
                    AND C.NRO_COMPROBANTE = @nroComprobante";

        await connection.ExecuteAsync(sql, new
        {
            afipFechaProceso,
            afipResultado,
            msgError,
            codTalonario,
            nroComprobante
        });

        if (afipJson != null)
        {
            sql = @"UPDATE COMPROBANTES C SET 
                    C.AFIP_JSON = @afipJson
                    WHERE C.COD_TALONARIO = @codTalonario 
                    AND C.NRO_COMPROBANTE = @nroComprobante";
            await connection.ExecuteAsync(sql, new { afipJson, codTalonario, nroComprobante });
        }
    }

    public async Task InsertarErrorAsync(AfipErrores error)
    {
        const string sql = @"
            INSERT INTO AFIP_ERRORES (FECHA, COD_ERROR, MSG_ERROR)
            VALUES (CURRENT_TIMESTAMP, @CodError, @MsgError)";
        using var connection = _connectionFactory.Create();
        await connection.ExecuteAsync(sql, new { error.CodError, error.MsgError });
    }

    #endregion
}
