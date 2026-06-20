using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Xml.Serialization;

namespace Afip.Services.Wsfev1;

[ServiceContract(Namespace = "http://ar.gov.afip.dif.FEV1/")]
public interface ServiceSoap
{
    [OperationContract(Action = "http://ar.gov.afip.dif.FEV1/FECAESolicitar", ReplyAction = "*")]
    [XmlSerializerFormat(Style = OperationFormatStyle.Document, Use = OperationFormatUse.Literal)]
    [return: MessageParameter(Name = "FECAESolicitarResult")]
    FECAEResponse FECAESolicitar([MessageParameter(Name = "Auth")] FEAuthRequest Auth, [MessageParameter(Name = "FeCAEReq")] FECAERequest Request);

    [OperationContract(Action = "http://ar.gov.afip.dif.FEV1/FECompConsultar", ReplyAction = "*")]
    [XmlSerializerFormat(Style = OperationFormatStyle.Document, Use = OperationFormatUse.Literal)]
    [return: MessageParameter(Name = "FECompConsultarResult")]
    FECompConsultaResponse FECompConsultar([MessageParameter(Name = "Auth")] FEAuthRequest Auth, [MessageParameter(Name = "FeCompConsReq")] FECompConsultaReq Request);

    [OperationContract(Action = "http://ar.gov.afip.dif.FEV1/FECompUltimoAutorizado", ReplyAction = "*")]
    [XmlSerializerFormat(Style = OperationFormatStyle.Document, Use = OperationFormatUse.Literal)]
    [return: MessageParameter(Name = "FECompUltimoAutorizadoResult")]
    FERecuperaLastCbteResponse FECompUltimoAutorizado(FEAuthRequest Auth, int PtoVta, int CbteTipo);

    [OperationContract(Action = "http://ar.gov.afip.dif.FEV1/FEDummy", ReplyAction = "*")]
    [XmlSerializerFormat(Style = OperationFormatStyle.Document, Use = OperationFormatUse.Literal)]
    [return: MessageParameter(Name = "FEDummyResult")]
    DummyResponse FEDummy();

    [OperationContract(Action = "http://ar.gov.afip.dif.FEV1/FEParamGetTiposDoc", ReplyAction = "*")]
    [XmlSerializerFormat(Style = OperationFormatStyle.Document, Use = OperationFormatUse.Literal)]
    DocTipoResponse FEParamGetTiposDoc(FEAuthRequest Auth);

    [OperationContract(Action = "http://ar.gov.afip.dif.FEV1/FEParamGetTiposCbte", ReplyAction = "*")]
    [XmlSerializerFormat(Style = OperationFormatStyle.Document, Use = OperationFormatUse.Literal)]
    CbteTipoResponse FEParamGetTiposCbte(FEAuthRequest Auth);

    [OperationContract(Action = "http://ar.gov.afip.dif.FEV1/FEParamGetTiposTributos", ReplyAction = "*")]
    [XmlSerializerFormat(Style = OperationFormatStyle.Document, Use = OperationFormatUse.Literal)]
    FETributoResponse FEParamGetTiposTributos(FEAuthRequest Auth);

    [OperationContract(Action = "http://ar.gov.afip.dif.FEV1/FEParamGetTiposIva", ReplyAction = "*")]
    [XmlSerializerFormat(Style = OperationFormatStyle.Document, Use = OperationFormatUse.Literal)]
    IvaTipoResponse FEParamGetTiposIva(FEAuthRequest Auth);

    [OperationContract(Action = "http://ar.gov.afip.dif.FEV1/FEParamGetPtosVenta", ReplyAction = "*")]
    [XmlSerializerFormat(Style = OperationFormatStyle.Document, Use = OperationFormatUse.Literal)]
    FEPtoVentaResponse FEParamGetPtosVenta(FEAuthRequest Auth);

    [OperationContract(Action = "http://ar.gov.afip.dif.FEV1/FEParamGetCondicionIvaReceptor", ReplyAction = "*")]
    [XmlSerializerFormat(Style = OperationFormatStyle.Document, Use = OperationFormatUse.Literal)]
    CondicionIvaReceptorResponse FEParamGetCondicionIvaReceptor(FEAuthRequest Auth, string Cuit);
}

public class FEAuthRequest
{
    public string Token { get; set; } = string.Empty;
    public string Sign { get; set; } = string.Empty;
    public long Cuit { get; set; }
}

public class FECAERequest
{
    public FECAECabRequest FeCabReq { get; set; } = new FECAECabRequest();
    public FECAEDetRequest[] FeDetReq { get; set; } = Array.Empty<FECAEDetRequest>();
}

public class FECAECabRequest
{
    public int CantReg { get; set; }
    public int CbteTipo { get; set; }
    public int PtoVta { get; set; }
}

public class FECAEDetRequest
{
    public int Concepto { get; set; }
    public int DocTipo { get; set; }
    public long DocNro { get; set; }
    public int? CondicionIVAReceptorId { get; set; }
    public long CbteDesde { get; set; }
    public long CbteHasta { get; set; }
    public string CbteFch { get; set; } = string.Empty;
    public double ImpTotal { get; set; }
    public double ImpNeto { get; set; }
    public double ImpTotConc { get; set; }
    public double ImpIVA { get; set; }
    public string? FchServDesde { get; set; }
    public string? FchServHasta { get; set; }
    public string? FchVtoPago { get; set; }
    public string MonId { get; set; } = "PES";
    public double MonCotiz { get; set; } = 1;
    public string CanMisMonExt { get; set; } = "N";
    public AlicIva[]? Iva { get; set; }
}

public class AlicIva
{
    public short Id { get; set; }
    public double Importe { get; set; }
    public double BaseImp { get; set; }
}

public class FECAEResponse
{
    public FECabResponse? FeCabResp { get; set; }

    [XmlArrayItem("FECAEDetResponse")]
    public FEDetResponse[]? FeDetResp { get; set; }

    public Err[]? Errors { get; set; }
}

public class FECabResponse
{
    public string FchProceso { get; set; } = string.Empty;
}

public class FEDetResponse
{
    public string Resultado { get; set; } = string.Empty;
    public string CAE { get; set; } = string.Empty;
    public string CAEFchVto { get; set; } = string.Empty;
    public Obs[]? Observaciones { get; set; }
}

public class Obs
{
    public int Code { get; set; }
    public string Msg { get; set; } = string.Empty;
}

public class FECompConsultaReq
{
    public long CbteNro { get; set; }
    public int CbteTipo { get; set; }
    public int PtoVta { get; set; }
}

public class FECompConsultaResponse
{
    public FECompConsultaResultGet? ResultGet { get; set; }
    public Err[]? Errors { get; set; }
}

public class FECompConsultaResultGet
{
    public string FchProceso { get; set; } = string.Empty;
    public string Resultado { get; set; } = string.Empty;
    public string CodAutorizacion { get; set; } = string.Empty;
    public string FchVto { get; set; } = string.Empty;
    public Obs[]? Observaciones { get; set; }
}

public class FERecuperaLastCbteResponse
{
    public int PtoVta { get; set; }
    public long CbteNro { get; set; }
    public Err[]? Errors { get; set; }
}

public class DummyResponse
{
    public string AppServer { get; set; } = string.Empty;
    public string AuthServer { get; set; } = string.Empty;
    public string DbServer { get; set; } = string.Empty;
}

public class DocTipoResponse
{
    public DocTipoResultGet[]? ResultGet { get; set; }
    public Err[]? Errors { get; set; }
}

public class DocTipoResultGet
{
    public int Id { get; set; }
    public string Desc { get; set; } = string.Empty;
}

public class CbteTipoResponse
{
    public CbteTipoResultGet[]? ResultGet { get; set; }
    public Err[]? Errors { get; set; }
}

public class CbteTipoResultGet
{
    public int Id { get; set; }
    public string Desc { get; set; } = string.Empty;
}

public class FETributoResponse
{
    public FETributoResultGet[]? ResultGet { get; set; }
    public Err[]? Errors { get; set; }
}

public class FETributoResultGet
{
    public int Id { get; set; }
    public string Desc { get; set; } = string.Empty;
}

public class IvaTipoResponse
{
    public IvaTipoResultGet[]? ResultGet { get; set; }
    public Err[]? Errors { get; set; }
}

public class IvaTipoResultGet
{
    public int Id { get; set; }
    public string Desc { get; set; } = string.Empty;
}

public class FEPtoVentaResponse
{
    public FEPtoVentaResultGet[]? ResultGet { get; set; }
    public Err[]? Errors { get; set; }
}

public class FEPtoVentaResultGet
{
    public int Nro { get; set; }
    public string EmisionTipo { get; set; } = string.Empty;
    public string Bloqueado { get; set; } = string.Empty;
}

public class CondicionIvaReceptorResponse
{
    public int[]? ResultGet { get; set; }
    public Err[]? Errors { get; set; }
}

public class Err
{
    public int Code { get; set; }
    public string Msg { get; set; } = string.Empty;
}

public class ServiceSoapClient : ClientBase<ServiceSoap>, ServiceSoap
{
    public ServiceSoapClient(Binding binding, EndpointAddress remoteAddress)
        : base(binding, remoteAddress)
    {
    }

    public FECAEResponse FECAESolicitar(FEAuthRequest Auth, FECAERequest Request)
    {
        return Channel.FECAESolicitar(Auth, Request);
    }

    public FECompConsultaResponse FECompConsultar(FEAuthRequest Auth, FECompConsultaReq Request)
    {
        return Channel.FECompConsultar(Auth, Request);
    }

    public FERecuperaLastCbteResponse FECompUltimoAutorizado(FEAuthRequest Auth, int PtoVta, int CbteTipo)
    {
        return Channel.FECompUltimoAutorizado(Auth, PtoVta, CbteTipo);
    }

    public DummyResponse FEDummy()
    {
        return Channel.FEDummy();
    }

    public DocTipoResponse FEParamGetTiposDoc(FEAuthRequest Auth)
    {
        return Channel.FEParamGetTiposDoc(Auth);
    }

    public CbteTipoResponse FEParamGetTiposCbte(FEAuthRequest Auth)
    {
        return Channel.FEParamGetTiposCbte(Auth);
    }

    public FETributoResponse FEParamGetTiposTributos(FEAuthRequest Auth)
    {
        return Channel.FEParamGetTiposTributos(Auth);
    }

    public IvaTipoResponse FEParamGetTiposIva(FEAuthRequest Auth)
    {
        return Channel.FEParamGetTiposIva(Auth);
    }

    public FEPtoVentaResponse FEParamGetPtosVenta(FEAuthRequest Auth)
    {
        return Channel.FEParamGetPtosVenta(Auth);
    }

    public CondicionIvaReceptorResponse FEParamGetCondicionIvaReceptor(FEAuthRequest Auth, string Cuit)
    {
        return Channel.FEParamGetCondicionIvaReceptor(Auth, Cuit);
    }
}
