using System;
using Ads.Client.Helpers;
using System.Threading;
using Ads.Client.Common;
using System.Diagnostics;

namespace Ads.Client
{
    public abstract class AmsSocketBase : IAmsSocket
    {
		public AmsSocketBase(string ipTarget, int portTarget = 48898)
        {
            Subscribers = 0;
            CreateSocket();
			this.IpTarget = ipTarget;
			this.PortTarget = portTarget;
            this.synchronizationContext = SynchronizationContext.Current;
        }

        public abstract void CreateSocket();

        protected SynchronizationContext synchronizationContext;

		public string IpTarget { get; set;}
		public int PortTarget { get; set;}

        public int Subscribers { get; set; }

        public event AmsSocketResponseDelegate OnReadCallBack;

        public bool ConnectedAsync { get; set; }

        public abstract bool IsConnected { get; }

        public abstract void ListenForHeader(byte[] amsheader, Action<byte[], SynchronizationContext> lambda);

        public void Listen()
        {
            if (IsConnected)
            {
                try
                {
                    //First wait for the Ams header (starts new thread)
                    byte[] amsheader = new byte[AmsHeaderHelper.AmsTcpHeaderSize];

                    ListenForHeader(amsheader, (buffer, usertoken) =>
                    {
                        //If a ams header is received, then read the rest (this is the new thread)
                        try
                        {
                            byte[] response = GetAmsMessage(buffer);

#if DEBUG_AMS
                            Debug.WriteLine("Received bytes: " +
                                    ByteArrayHelper.ByteArrayToTestString(buffer) + ',' +
                                    ByteArrayHelper.ByteArrayToTestString(response));
#endif

                            var syncContext = synchronizationContext;
                            if (usertoken != null) syncContext = usertoken as SynchronizationContext;

                            var callbackArgs = new AmsSocketResponseArgs() {
                                Response = response,
                                Context = syncContext };
                            OnReadCallBack(this, callbackArgs);
                            Listen();
                        }
                        catch (Exception ex)
                        {
                            var callbackArgs = new AmsSocketResponseArgs() { Error = ex };
                            OnReadCallBack(this, callbackArgs);
                        }
                    });
                }
                catch (Exception ex)
                {
                    if (!Object.ReferenceEquals(ex.GetType(), typeof(ObjectDisposedException))) throw;
                }
            }
        }

        private byte[] GetAmsMessage(byte[] tcpHeader)
        {
            uint responseLength = AmsHeaderHelper.GetResponseLength(tcpHeader);
            byte[] response = new byte[responseLength];
            GetMessage(response);
            return response;
        }

        private void GetMessage(byte[] response)
        {
            if (ConnectedAsync)
                Async.ReceiveAsync(response).Wait();
            else
                Sync.Receive(response);

        }

        protected abstract void CloseConnection();

        protected virtual void Dispose(bool managed)
        {
            CloseConnection();
        }

        public void Dispose()
        {
            Dispose(true);
        }


        public virtual IAmsSocketSync Sync { get; set; }
        public virtual IAmsSocketAsync Async { get; set; }
    }
}