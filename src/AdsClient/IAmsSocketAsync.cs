using System.Threading.Tasks;

namespace Ads.Client
{
    public interface IAmsSocketAsync
    {
        Task ConnectAndListenAsync();
        Task SendAsync(byte[] message);
        Task ReceiveAsync(byte[] message);
    }
}
