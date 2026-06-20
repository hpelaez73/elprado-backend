using Afip.Data.Interfaces;
using Afip.Data.Models;
using Afip.Services.Logger;
using Afip.Services.Wsfev1;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.ServiceModel;

namespace Afip.Services;

public class Wsfe : IDisposable
{
    private const int cTipoDniOtros = 99;
    private const int cTipoDniCUIT = 80;
    private const string cMoneda = "PES";

    private readonly IConfiguration _configuration;
    private readonly IAfipRepository _repo;
    private FEAuthRequest _feAuthRequest = new();
    private ServiceSoapClient? _ws;
    private bool _disposed;
    private bool _modoPrueba;

    public Wsfe(IConfiguration configuration, IAfipRepository afipRepository)
    {
        _configuration = configuration;
        _repo = afipRepository;
        _modoPrueba = false;
        InicializarClienteSoap(_modoPrueba ? Constantes.URL_WSFEV1_WSDL_PRUEBA : Constantes.URL_WSFEV1_WSDL_FINAL);
    }

    private void InicializarClienteSoap(string url)
    {
        var binding = Utils.CrearBinding();
        var endpoint = new EndpointAddress(url);
        _ws = new ServiceSoapClient(binding, endpoint);
        _ws.Endpoint.EndpointBehaviors.Add(new SoapLoggerBehavior());
    }

    public void ActivarModoPrueba()
    {
        _modoPrueba = true;
        InicializarClienteSoap(Constantes.URL_WSFEV1_WSDL_PRUEBA);
    }

    public ApiResponse ConsultarEstadoServicio()
    {
        if (_ws == null)
        {
            Console.WriteLine("Error: El cliente SOAP no está inicializado");
            return new ApiResponse 
            {
                Success = false, 
                Message = "Cliente SOAP no inicializado" 
            };
        }

        DummyResponse dummy = _ws.FEDummy();
        Console.WriteLine("AppServer: " + dummy.AppServer);
        Console.WriteLine("AuthServer: " + dummy.AuthServer);
        Console.WriteLine("DbServer: " + dummy.DbServer);

        if (dummy.AppServer == "OK" && dummy.AuthServer == "OK" && dummy.DbServer == "OK")
        {
            return new ApiResponse
            {
                Success = true, 
                Message = "Servicio OK" 
            };
        }
        else
        {
            return new ApiResponse 
            {  
                Success = false, 
                Message = "Error en el servicio" 
            };
        }
    }

    public async Task<ApiResponse> ConsultarUltimoComprobanteAsync(int codTipoComprobante, int codTalonario)
    {
        await ConfigurarFEAuthRequestAsync();

        if (_ws == null)
        {
            Console.WriteLine("Error: El cliente SOAP no está inicializado");
            await GenerarErrorAsync(0, "Cliente SOAP no inicializado");
            return new ApiResponse
            {
                Success = false,
                Message = "Cliente SOAP no inicializado"
            };
        }

        var afipTipoComprobante = await _repo.GetAfipTipoComprobanteAsync(codTalonario, codTipoComprobante);

        if (afipTipoComprobante == null)
        {
            await GenerarErrorAsync(0, "No se encontró el tipo de comprobante para el talonario y tipo de comprobante especificados");
            return new ApiResponse
            {
                Success = false,
                Message = "No se encontró el tipo de comprobante para el talonario y tipo de comprobante especificados"
            };
        }

        FERecuperaLastCbteResponse response = _ws.FECompUltimoAutorizado(_feAuthRequest, afipTipoComprobante.PuntoVenta, afipTipoComprobante.Id);
        bool res = await ProcesarErroresAsync(response.Errors);

        if (res)
        {
            AfipUltimoComprobanteEmitido ultimoComprobante = new()
            {
                CodTalonario = codTalonario,
                NroComprobante = afipTipoComprobante.Letra + "-" + response.PtoVta.ToString().PadLeft(4, '0') + "-" + response.CbteNro.ToString().PadLeft(8, '0'),
            };

            await _repo.ActualizarUltimoComprobanteEmitidoAsync(ultimoComprobante);
            return new ApiResponse
            {
                Success = true,
                Message = "Último comprobante actualizado correctamente",
                CodTalonario = codTalonario,
                NroComprobante = ultimoComprobante.NroComprobante
            };
        }
        else
        {
            return new ApiResponse
            {
                Success = false,
                Message = "Error al actualizar el último comprobante"
            };
        }
        
    }

    public async Task<ApiResponse> ActualizarCAEComprobanteEmitidoAsync(int codTalonario, string nroComprobante)
    {
        await ConfigurarFEAuthRequestAsync();

        if (_ws == null)
        {
            Console.WriteLine("Error: El cliente SOAP no está inicializado");
            await GenerarErrorAsync(0, "Cliente SOAP no inicializado");
            return new ApiResponse
            {
                Success = false,
                Message = "Cliente SOAP no inicializado"
            };
        }

        var comprobante = await _repo.GetComprobanteParaCAEAsync(codTalonario, nroComprobante);
        if (comprobante == null)
        {
            await GenerarErrorAsync(0, "El comprobante no está generado");
            return new ApiResponse
            {
                Success = false,
                Message = "El comprobante no está generado"
            };
        }

        FECompConsultaReq request = new()
        {
            CbteNro = Convert.ToInt64(nroComprobante.Substring(7, 8)),
            CbteTipo = comprobante.AfipTipoComprobante,
            PtoVta = comprobante.PuntoVenta
        };

        FECompConsultaResponse response = _ws.FECompConsultar(_feAuthRequest, request);
        bool res = await ProcesarErroresAsync(response.Errors);

        if (res && response.ResultGet != null)
        {
            Console.WriteLine("CAE: " + response.ResultGet.CodAutorizacion);

            DateTime afipFechaProceso = DateTime.ParseExact(response.ResultGet.FchProceso, "yyyyMMddHHmmss", null);
            DateTime? afipVencimientoCae = null;
            string? afipObservaciones = null;

            if (response.ResultGet.Observaciones != null)
            {
                afipObservaciones = string.Join(", ", response.ResultGet.Observaciones.Select(obs => obs.Msg));
            }

            if (response.ResultGet.FchVto != null)
            {
                afipVencimientoCae = DateTime.ParseExact(response.ResultGet.FchVto, "yyyyMMdd", null);
            }

            await _repo.ActualizarRespuestaCAEAsync(
                codTalonario,
                nroComprobante,
                afipFechaProceso,
                response.ResultGet.Resultado,
                response.ResultGet.CodAutorizacion,
                afipVencimientoCae,
                afipObservaciones,
                null
            );

            return new ApiResponse
            {
                Success = true,
                Message = "CAE actualizado correctamente",
                CodTalonario = codTalonario,
                NroComprobante = nroComprobante,
                NroCAE = response.ResultGet.CodAutorizacion
            };
        }
        else
        {
            await _repo.ActualizarErrorCAEAsync(
                codTalonario,
                nroComprobante,
                DateTime.Now,
                "R",
                (response.Errors != null && response.Errors.Length > 0) ? response.Errors[0].Msg : "Error desconocido",
                null
            );

            return new ApiResponse
            {
                Success = false,
                Message = (response.Errors != null && response.Errors.Length > 0) ? response.Errors[0].Msg : "Error desconocido",
                CodTalonario = codTalonario,
                NroComprobante = nroComprobante
            };
        }
    }

    public async Task<ApiResponse> SolicitarCAEAsync(int codTalonario, string nroComprobante)
    {
        await ConfigurarFEAuthRequestAsync();

        if (_ws == null)
        {
            Console.WriteLine("Error: El cliente SOAP no está inicializado");
            await GenerarErrorAsync(0, "Cliente SOAP no inicializado");
            return new ApiResponse
            {
                Success = false,
                Message = "Cliente SOAP no inicializado"
            };
        }

        AfipComprobante? comprobante = await _repo.GetComprobanteParaCAEAsync(codTalonario, nroComprobante);
        if (comprobante == null)
        {
            await GenerarErrorAsync(0, "El comprobante no está generado");
            return new ApiResponse
            {
                Success = false,
                Message = "El comprobante no está generado"
            };
        }

        FECAERequest fecaeRequest = new();
        FECAECabRequest fecaeCabRequest = new();
        FECAEDetRequest fecaeDetRequest = new();

        fecaeCabRequest.CantReg = 1;
        fecaeCabRequest.CbteTipo = comprobante.AfipTipoComprobante;
        fecaeCabRequest.PtoVta = comprobante.PuntoVenta;

        fecaeDetRequest.Concepto = comprobante.AfipConcepto;
        if (comprobante.Cuit.HasValue && comprobante.Cuit != 0)
        {
            fecaeDetRequest.DocTipo = cTipoDniCUIT;
            fecaeDetRequest.DocNro = comprobante.Cuit.Value;
        }
        else if (!comprobante.AfipTipoDocumento.HasValue || string.IsNullOrEmpty(comprobante.NroDocumento))
        {
            fecaeDetRequest.DocTipo = cTipoDniOtros;
            fecaeDetRequest.DocNro = 0;
        }
        else
        {
            fecaeDetRequest.DocTipo = comprobante.AfipTipoDocumento.Value;
            fecaeDetRequest.DocNro = Convert.ToInt64(comprobante.NroDocumento);
        }

        fecaeDetRequest.CondicionIVAReceptorId = comprobante.AfipCondicion ?? 1;
        fecaeDetRequest.CbteDesde = Convert.ToInt64(nroComprobante.Substring(7, 8));
        fecaeDetRequest.CbteHasta = Convert.ToInt64(nroComprobante.Substring(7, 8));
        fecaeDetRequest.CbteFch = comprobante.Fecha.ToString("yyyyMMdd");
        fecaeDetRequest.ImpTotal = Convert.ToDouble(comprobante.Total);
        fecaeDetRequest.ImpNeto = Convert.ToDouble(comprobante.Neto);
        fecaeDetRequest.ImpTotConc = Convert.ToDouble(comprobante.NoGravado);
        fecaeDetRequest.ImpIVA = Convert.ToDouble(comprobante.Iva);

        if (fecaeDetRequest.Concepto > 1)
        {
            fecaeDetRequest.FchServDesde = comprobante.AfipServicioDesde.HasValue ? comprobante.AfipServicioDesde.Value.ToString("yyyyMMdd") : "";
            fecaeDetRequest.FchServHasta = comprobante.AfipServicioHasta.HasValue ? comprobante.AfipServicioHasta.Value.ToString("yyyyMMdd") : "";
            fecaeDetRequest.FchVtoPago = comprobante.Fecha.ToString("yyyyMMdd");
        }

        fecaeDetRequest.MonId = cMoneda;
        fecaeDetRequest.MonCotiz = 1;
        fecaeDetRequest.CanMisMonExt = "N";

        if (fecaeDetRequest.ImpIVA > 0)
        {
            var listAlicuotas = await _repo.GetAlicuotasComprobanteAsync(codTalonario, nroComprobante);

            if (listAlicuotas.Any())
            {
                fecaeDetRequest.Iva = new AlicIva[listAlicuotas.Count()];
                int idx = 0;
                foreach (var alicuota in listAlicuotas)
                {
                    AlicIva alicIva = new()
                    {
                        Id = alicuota.IdIva,
                        Importe = Convert.ToDouble(alicuota.Importe),
                        BaseImp = Convert.ToDouble(alicuota.BaseImp)
                    };
                    fecaeDetRequest.Iva[idx] = alicIva;
                    idx++;
                }
            }
        }

        fecaeRequest.FeCabReq = fecaeCabRequest;
        fecaeRequest.FeDetReq = new FECAEDetRequest[1];
        fecaeRequest.FeDetReq[0] = fecaeDetRequest;

        FECAEResponse fecaeResponse = _ws.FECAESolicitar(_feAuthRequest, fecaeRequest);

        string jsonRequest = ConvertirFECAEResponseAJson(fecaeRequest);
        bool res = await ProcesarErroresAsync(fecaeResponse.Errors);
        if (res && fecaeResponse.FeDetResp != null && fecaeResponse.FeDetResp.Length > 0)
        {
            Console.WriteLine("CAE: " + fecaeResponse.FeDetResp[0].CAE);

            DateTime afipFechaProceso = (fecaeResponse.FeCabResp != null) ? DateTime.ParseExact(fecaeResponse.FeCabResp.FchProceso, "yyyyMMddHHmmss", null) : DateTime.Now;
            DateTime? afipVencimientoCae = null;
            string? afipObservaciones = null;

            if (fecaeResponse.FeDetResp[0].Observaciones != null)
            {
                afipObservaciones = string.Join(", ", fecaeResponse.FeDetResp[0].Observaciones.Select(obs => obs.Msg));
            }

            if (fecaeResponse.FeDetResp[0].CAEFchVto != null)
            {
                afipVencimientoCae = DateTime.ParseExact(fecaeResponse.FeDetResp[0].CAEFchVto, "yyyyMMdd", null);
            }

            await _repo.ActualizarRespuestaCAEAsync(
                codTalonario,
                nroComprobante,
                afipFechaProceso,
                fecaeResponse.FeDetResp[0].Resultado,
                fecaeResponse.FeDetResp[0].CAE,
                afipVencimientoCae,
                afipObservaciones,
                jsonRequest
            );
            return new ApiResponse
            {
                Success = true,
                Message = "CAE solicitado correctamente",
                CodTalonario = codTalonario,
                NroComprobante = nroComprobante,
                NroCAE = fecaeResponse.FeDetResp[0].CAE
            };
        }
        else
        {
            await _repo.ActualizarErrorCAEAsync(
                codTalonario,
                nroComprobante,
                DateTime.Now,
                "R",
                fecaeResponse.Errors?[0].Msg ?? "Error desconocido",
                jsonRequest
            );
            return new ApiResponse 
            {
                Success = false,
                Message = fecaeResponse.Errors?[0].Msg ?? "Error desconocido",
                CodTalonario = codTalonario,
                NroComprobante = nroComprobante
            };
        }
    }

    private static string ConvertirFECAEResponseAJson(FECAERequest request)
    {
        try
        {
            string json = JsonConvert.SerializeObject(request, Formatting.Indented);
            return json;
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error al convertir FECAERequest a JSON: " + ex.Message);
            return null!;
        }
    }

    private async Task ConfigurarFEAuthRequestAsync()
    {
        LoginTicket lt = new(_configuration, _repo);
        await lt.BuscarLoginTicketResponseAsync(_modoPrueba);
        _feAuthRequest.Token = lt.Token;
        _feAuthRequest.Sign = lt.Sign;
        _feAuthRequest.Cuit = _modoPrueba ? Constantes.CUIT_PRUEBA : Constantes.CUIT_FINAL;
    }

    private async Task<bool> ProcesarErroresAsync(Err[]? errores)
    {
        if (errores != null && errores.Length > 0)
        {
            await GenerarErrorAsync(errores[0].Code, errores[0].Msg);
            return false;
        }
        else
        {
            return true;
        }
    }

    private async Task GenerarErrorAsync(int codError, string msgError)
    {
        Console.WriteLine("Error: " + msgError);

        AfipErrores error = new()
        {
            CodError = codError,
            MsgError = msgError
        };

        await _repo.InsertarErrorAsync(error);
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                if (_ws != null)
                {
                    if (_ws.State == CommunicationState.Opened)
                    {
                        _ws.Close();
                    }
                    else
                    {
                        _ws.Abort();
                    }
                }
            }
            _disposed = true;
        }
    }
}
