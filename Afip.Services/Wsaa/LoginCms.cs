using System.ServiceModel;
using System.ServiceModel.Channels;

namespace Afip.Services.Wsaa;

[ServiceContract(
    Namespace = "http://wsaa.view.sua.dvadac.desein.afip.gov"
)]
public interface LoginCMSService
{
    [OperationContract(Action = "", ReplyAction = "*")]
    [XmlSerializerFormat(Style = OperationFormatStyle.Document, Use = OperationFormatUse.Literal)]

    [return: MessageParameter(Name = "loginCmsReturn")]
    string loginCms([MessageParameter(Name = "in0")] string in0);
}

public class LoginCMSServiceClient : ClientBase<LoginCMSService>, LoginCMSService
{
    public LoginCMSServiceClient(Binding binding, EndpointAddress remoteAddress)
        : base(binding, remoteAddress)
    {
    }

    public string loginCms(string in0)
    {
        return Channel.loginCms(in0);
    }
}
