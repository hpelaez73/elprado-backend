using Afip.Data.Interfaces;
using Afip.Data.Models;
using Afip.Services.Logger;
using Microsoft.Extensions.Configuration;
using System.Security.Cryptography.Pkcs;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel;
using System.Text;
using System.Xml;

namespace Afip.Services;

public class LoginTicket
{
    public uint UniqueId { get; set; }
    public DateTime GenerationTime { get; set; }
    public DateTime ExpirationTime { get; set; }
    public string Sign { get; set; } = string.Empty;
    public string Token { get; set; } = string.Empty;

    private XmlDocument XmlLoginTicketRequest = null!;
    private XmlDocument XmlLoginTicketResponse = null!;
    private const string XmlStrLoginTicketRequestTemplate = "<loginTicketRequest><header><uniqueId></uniqueId><generationTime></generationTime><expirationTime></expirationTime></header><service></service></loginTicketRequest>";
    private static uint _globalUniqueID = 0;
    private bool _modoPrueba;
    private readonly IConfiguration _configuration;
    private readonly IAfipRepository _repo;

    public LoginTicket(IConfiguration configuration, IAfipRepository afipRepository)
    {
        _configuration = configuration;
        _repo = afipRepository;
    }

    private void GenerarLoginTicketResponse()
    {
        string cmsFirmadoBase64;
        string loginTicketResponse;

        XmlNode xmlNodoUniqueId;
        XmlNode xmlNodoGenerationTime;
        XmlNode xmlNodoExpirationTime;
        XmlNode xmlNodoService;

        try
        {
            XmlLoginTicketRequest = new XmlDocument();
            XmlLoginTicketRequest.LoadXml(XmlStrLoginTicketRequestTemplate);

            xmlNodoUniqueId = XmlLoginTicketRequest.SelectSingleNode("//uniqueId")!;
            xmlNodoGenerationTime = XmlLoginTicketRequest.SelectSingleNode("//generationTime")!;
            xmlNodoExpirationTime = XmlLoginTicketRequest.SelectSingleNode("//expirationTime")!;
            xmlNodoService = XmlLoginTicketRequest.SelectSingleNode("//service")!;

            xmlNodoGenerationTime.InnerText = DateTime.Now.AddMinutes(-10).ToString("s");
            xmlNodoExpirationTime.InnerText = DateTime.Now.AddMinutes(+10).ToString("s");
            xmlNodoUniqueId.InnerText = Convert.ToString(_globalUniqueID);
            xmlNodoService.InnerText = Constantes.DEFAULT_SERVICIO;

            _globalUniqueID += 1;
        }
        catch (Exception excepcionAlGenerarLoginTicketRequest)
        {
            throw new Exception("***Error GENERANDO el LoginTicketRequest : " + excepcionAlGenerarLoginTicketRequest.Message);
        }

        try
        {
            string rutaCertificado = _configuration["rutaCertificado"] ?? string.Empty;
            string nombreCertificado = (_modoPrueba) ? Constantes.CERTIFICADO_PRUEBA : Constantes.CERTIFICADO_FINAL;
            X509Certificate2 certFirmante = CertificadosX509Lib.ObtieneCertificadoDesdeArchivo(Path.Combine(rutaCertificado, nombreCertificado));

            Encoding EncodedMsg = Encoding.UTF8;
            byte[] msgBytes = EncodedMsg.GetBytes(XmlLoginTicketRequest.OuterXml);

            byte[] encodedSignedCms = CertificadosX509Lib.FirmaBytesMensaje(msgBytes, certFirmante);
            cmsFirmadoBase64 = Convert.ToBase64String(encodedSignedCms);
        }
        catch (Exception excepcionAlFirmar)
        {
            throw new Exception("***Error FIRMANDO el LoginTicketRequest : " + excepcionAlFirmar.Message);
        }

        try
        {
            string url = (_modoPrueba) ? Constantes.URL_WSAA_WSDL_PRUEBA : Constantes.URL_WSAA_WSDL_FINAL;

            var binding = Utils.CrearBinding();
            var endpoint = new EndpointAddress(url);
            var servicioWsaa = new Wsaa.LoginCMSServiceClient(binding, endpoint);

            servicioWsaa.Endpoint.EndpointBehaviors.Add(new SoapLoggerBehavior());

            loginTicketResponse = servicioWsaa.loginCms(cmsFirmadoBase64);
            servicioWsaa.Close();
        }
        catch (Exception excepcionAlInvocarWsaa)
        {
            throw new Exception("***Error INVOCANDO al servicio WSAA : " + excepcionAlInvocarWsaa.Message, excepcionAlInvocarWsaa.InnerException);
        }

        try
        {
            XmlLoginTicketResponse = new XmlDocument();
            XmlLoginTicketResponse.LoadXml(loginTicketResponse);

            UniqueId = uint.Parse(XmlLoginTicketResponse.SelectSingleNode("//uniqueId")!.InnerText);
            GenerationTime = DateTime.Parse(XmlLoginTicketResponse.SelectSingleNode("//generationTime")!.InnerText);
            ExpirationTime = DateTime.Parse(XmlLoginTicketResponse.SelectSingleNode("//expirationTime")!.InnerText);
            Sign = XmlLoginTicketResponse.SelectSingleNode("//sign")!.InnerText;
            Token = XmlLoginTicketResponse.SelectSingleNode("//token")!.InnerText;
            XmlLoginTicketResponse.Save(Constantes.TICKET_ACCESO);
        }
        catch (Exception excepcionAlAnalizarLoginTicketResponse)
        {
            throw new Exception("***Error ANALIZANDO el LoginTicketResponse : " + excepcionAlAnalizarLoginTicketResponse.Message);
        }
    }

    public async Task<bool> BuscarLoginTicketResponseAsync(bool ModoPrueba)
    {
        _modoPrueba = ModoPrueba;

        AfipConfiguraciones? config = await _repo.GetAfipConfigurationAsync();

        if (config == null)
        {
            GenerarLoginTicketResponse();
            await GuardarNuevoTicketAsync();
        }
        else
        {
            uint idUnico = config.AfipIdUnico;
            TimeSpan limite = config.AfipExpiracion - DateTime.Now;

            if (limite.TotalHours < 0.02)
            {
                GenerarLoginTicketResponse();
                if (idUnico == UniqueId)
                {
                    Console.WriteLine("No se pudo generar el ticket");
                    return false;
                }
                else
                {
                    await GuardarNuevoTicketAsync();
                }
            }
            else
            {
                Console.WriteLine("El ticket no esta vencido");
                Token = config.AfipToken;
                Sign = config.AfipSign;
                GenerationTime = config.AfipGeneracion;
                ExpirationTime = config.AfipExpiracion;
                UniqueId = config.AfipIdUnico;
            }
        }

        return true;
    }

    private async Task GuardarNuevoTicketAsync()
    {
        var config = new AfipConfiguraciones
        {
            AfipToken = Token,
            AfipSign = Sign,
            AfipGeneracion = GenerationTime,
            AfipExpiracion = ExpirationTime,
            AfipIdUnico = UniqueId
        };
        await _repo.UpdateAfipConfigurationAsync(config);
    }
}

public static class CertificadosX509Lib
{
    public static byte[] FirmaBytesMensaje(byte[] argBytesMsg, X509Certificate2 argCertFirmante)
    {
        try
        {
            ContentInfo infoContenido = new(argBytesMsg);
            SignedCms cmsFirmado = new(infoContenido);

            CmsSigner cmsFirmante = new(argCertFirmante)
            {
                IncludeOption = X509IncludeOption.EndCertOnly
            };

            cmsFirmado.ComputeSignature(cmsFirmante);

            return cmsFirmado.Encode();
        }
        catch (Exception excepcionAlFirmar)
        {
            throw new Exception("***Error al firmar: " + excepcionAlFirmar.Message);
        }
    }

    public static X509Certificate2 ObtieneCertificadoDesdeArchivo(string argArchivo)
    {
        try
        {
            byte[] certBytes = File.ReadAllBytes(argArchivo);
            return new X509Certificate2(certBytes, "origen", X509KeyStorageFlags.MachineKeySet | X509KeyStorageFlags.Exportable);
        }
        catch (Exception excepcionAlImportarCertificado)
        {
            throw new Exception("argArchivo=" + argArchivo + " excepcion=" + excepcionAlImportarCertificado.Message + " " + excepcionAlImportarCertificado.StackTrace);
        }
    }
}
