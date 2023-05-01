using System;
using Ads.Client.Helpers;
using Ads.Client.Common;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;

namespace Ads.Client
{
    public sealed class AmsSocket : IAmsSocket
    {
		public AmsSocket(string ipTarget, int portTarget = 48898)
        {
            Subscribers = 0;
            CreateSocket();
			this.IpTarget = ipTarget;
			this.PortTarget = portTarget;

            this.Async = new AmsSocketAsync(this);
        }

        public Socket Socket { get; set; }
        public IPEndPoint LocalEndPoint { get; set; }

        public void CreateSocket()
        {
            LocalEndPoint = new IPEndPoint(IPAddress.Any, 0);
            this.Socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        }

		public string IpTarget { get; set;}
		public int PortTarget { get; set;}

        public int Subscribers { get; set; }

        public event EventHandler<AmsSocketResponseArgs> OnReadCallBack;

        /// <inheritdoc cref="System.Net.Sockets.Socket.Connected"/>
        public bool IsConnected
        {
            get { return ((Socket != null) && (Socket.Connected)); }
        }

        public void ListenForHeader(byte[] amsheader, Action<byte[]> lambda)
        {
            using (SocketAsyncEventArgs args = new SocketAsyncEventArgs())
            {
                args.SetBuffer(amsheader, 0, amsheader.Length);
                args.Completed += (sender, e) =>
                {
                    if (args.BytesTransferred == 0)
                    {
                        throw new Exception($"Remote host closed the connection.");
                    }

                    lambda(e.Buffer);
                };
                bool receivedAsync = Socket.ReceiveAsync(args);
                if (!receivedAsync)
                {
                    if (args.BytesTransferred == 0)
                    {
                        throw new Exception($"Remote host closed the connection.");
                    }

                    lambda(amsheader);
                }
            }
        }

        public void Listen()
        {
            if (IsConnected)
            {
                try
                {
                    //First wait for the Ams header (starts new thread)
                    byte[] amsheader = new byte[AmsHeaderHelper.AmsTcpHeaderSize];

                    ListenForHeader(amsheader, buffer =>
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

                            var callbackArgs = new AmsSocketResponseArgs { Response = response };
                            OnReadCallBack(this, callbackArgs);
                            Listen();
                        }
                        catch (Exception ex)
                        {
                            var callbackArgs = new AmsSocketResponseArgs { Error = ex };
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
            // Todo: properly handle async receiving
            Async.ReceiveAsync(response).Wait();
        }

        private void CloseConnection()
        {
            if (Socket.Connected)
            {
                Socket.Shutdown(SocketShutdown.Both);
                Socket.Close();
            }

            if (Socket != null) Socket.Dispose();
        }

        private void Dispose(bool managed)
        {
            CloseConnection();
        }

        public void Dispose()
        {
            Dispose(true);
        }

        public IAmsSocketAsync Async { get; }
    }
}