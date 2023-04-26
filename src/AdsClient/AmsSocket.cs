using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace Ads.Client
{
    public class AmsSocket : AmsSocketBase
    {
		public AmsSocket (string ipTarget, int portTarget = 48898) : base(ipTarget, portTarget)
	    {
			this.Async = new AmsSocketAsync(this);
        }

        public Socket Socket { get; set; }
		public IPEndPoint LocalEndPoint { get; set; }

        public override void CreateSocket()
        {
			LocalEndPoint = new IPEndPoint(IPAddress.Any, 0);
            this.Socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        }

        public override bool IsConnected
        {
            get { return ((Socket != null) && (Socket.Connected)); }
        }

        public override void ListenForHeader(byte[] amsheader, Action<byte[], SynchronizationContext> lambda)
        {
            using (SocketAsyncEventArgs args = new SocketAsyncEventArgs())
            {
                args.SetBuffer(amsheader, 0, amsheader.Length);
                args.UserToken = synchronizationContext;
                args.Completed += (sender, e) =>
                {
                    if (args.BytesTransferred == 0)
                    {
                        throw new Exception($"Remote host closed the connection.");
                    }

                    lambda(e.Buffer, e.UserToken as SynchronizationContext);
                };
                bool receivedAsync = Socket.ReceiveAsync(args);
                if (!receivedAsync)
                {
                    if (args.BytesTransferred == 0)
                    {
                        throw new Exception($"Remote host closed the connection.");
                    }
                    lambda(amsheader, null);
                }
            }
        }

        protected override void CloseConnection()
        {
            if (Socket.Connected)
            {
                Socket.Shutdown(SocketShutdown.Both);
                Socket.Close();
            }

            if (Socket != null) Socket.Dispose();
        }

    }
}
