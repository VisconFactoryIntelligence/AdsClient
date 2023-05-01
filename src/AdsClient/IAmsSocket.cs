using System.Threading.Tasks;

namespace Ads.Client
{
    public interface IAmsSocket
    {
        bool Connected { get;}

        void Close();

        Task ConnectAsync(IIncomingMessageHandler messageHandler);

        Task SendAsync(byte[] message);
    }
}
