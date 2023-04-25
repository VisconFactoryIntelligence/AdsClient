using System;
using Ads.Client.Helpers;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Ads.Client.Common;
using System.Diagnostics;

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
                amsSocket.ConnectedAsync = true;
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