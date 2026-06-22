using System.ServiceModel;

namespace Afip.Services
{
    static class Utils
    {
        public static BasicHttpsBinding CrearBinding()
        {
            return new BasicHttpsBinding
            {
                Security =
            {
                Mode = BasicHttpsSecurityMode.Transport
            },

                MaxReceivedMessageSize = 1024 * 1024 * 10,

                ReaderQuotas =
            {
                MaxDepth = 32,
                MaxStringContentLength = 1024 * 1024,
                MaxArrayLength = 1024 * 1024
            },

                OpenTimeout = TimeSpan.FromSeconds(30),
                CloseTimeout = TimeSpan.FromSeconds(30),
                SendTimeout = TimeSpan.FromMinutes(2),
                ReceiveTimeout = TimeSpan.FromMinutes(2)
            };
        }
    }
}
