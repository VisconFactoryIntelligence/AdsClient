using System;
using System.Threading;
using System.Threading.Tasks;

namespace Viscon.Communication.Ads
{
    public interface IAmsSocket
    {
        bool Connected { get;}

        void Close();

        Task ConnectAsync(IIncomingMessageHandler messageHandler, CancellationToken cancellationToken = default);

        Task SendAsync(byte[] message);

        Task SendAsync(ArraySegment<byte> buffer);
    }
}
