using System.Threading.Tasks;
using System;

namespace Ads.Client
{
    public interface IAmsSocketAsync : IDisposable
    {
        Task ConnectAndListenAsync();
        Task SendAsync(byte[] message);
        Task ReceiveAsync(byte[] message);
    }
}
