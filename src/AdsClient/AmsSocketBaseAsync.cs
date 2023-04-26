using System;
using System.Threading.Tasks;

namespace Ads.Client
{
    public abstract class AmsSocketBaseAsync : IAmsSocketAsync
    {
		protected AmsSocketBase amsSocket;
        public AmsSocketBaseAsync(AmsSocketBase amsSocket)
        {
			this.amsSocket = amsSocket;
        }

        public async Task ConnectAndListenAsync()
        {
            if (!amsSocket.IsConnected)
            {
                await ConnectAsync();
                amsSocket.Listen();
            }
        }

        protected abstract Task ConnectAsync();
        public abstract Task SendAsync(byte[] message);
        public abstract Task ReceiveAsync(byte[] message);

        public void Dispose()
        {
            throw new NotImplementedException();
        }

    }
}