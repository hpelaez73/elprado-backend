using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Dispatcher;
using System.Text;
using System.Xml;

namespace Afip.Services.Logger
{
    public class SoapLogger : IClientMessageInspector
    {
        public object BeforeSendRequest(
            ref Message request,
            IClientChannel channel)
        {
            var buffer = request.CreateBufferedCopy(int.MaxValue);

            var copy = buffer.CreateMessage();
            request = buffer.CreateMessage();

            string xml = MessageToString(copy);

            Console.WriteLine("========== SOAP REQUEST ==========");
            Console.WriteLine(xml);

            return null;
        }

        public void AfterReceiveReply(
            ref Message reply,
            object correlationState)
        {
            var buffer = reply.CreateBufferedCopy(int.MaxValue);

            var copy = buffer.CreateMessage();
            reply = buffer.CreateMessage();

            string xml = MessageToString(copy);

            Console.WriteLine("========== SOAP RESPONSE ==========");
            Console.WriteLine(xml);
        }

        private string MessageToString(Message message)
        {
            using var ms = new MemoryStream();
            using var writer = XmlWriter.Create(ms);

            message.WriteMessage(writer);

            writer.Flush();

            return Encoding.UTF8.GetString(ms.ToArray());
        }
    }
}
