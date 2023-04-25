using System.Net;
using System.Threading.Tasks;
using Ads.Client.Common;
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
