using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Ads.Client.Winsock
{
    public class AmsSocketAsync : AmsSocketBaseAsync
    {
        protected new AmsSocket amsSocket;
        public AmsSocketAsync(AmsSocket amsSocket) : base(amsSocket)
        {
            this.amsSocket = amsSocket;
        }


        protected override Task ConnectAsync()
        {
            var tcs = new TaskCompletionSource<bool>(amsSocket.Socket);
            #if !SILVERLIGHT
            amsSocket.Socket.Bind(amsSocket.LocalEndPoint);
            #endif
            using (SocketAsyncEventArgs args = new SocketAsyncEventArgs())
            {
                args.RemoteEndPoint = new DnsEndPoint(amsSocket.IpTarget, amsSocket.PortTarget);
                args.Completed += (sender, e) => { tcs.TrySetResult(e.SocketError == SocketError.Success); e.Dispose(); };
                amsSocket.Socket.ConnectAsync(args);
            }
            return tcs.Task;
        }

        public override Task SendAsync(byte[] message)
        {
            var tcs = new TaskCompletionSource<bool>(amsSocket.Socket);
            SocketAsyncEventArgs args = new SocketAsyncEventArgs();
            args.Completed += (sender, e) => { tcs.TrySetResult(e.SocketError == SocketError.Success); e.Dispose(); };
            args.SetBuffer(message, 0, message.Length);
            amsSocket.Socket.SendAsync(args);
            return tcs.Task;
        }

        public override Task ReceiveAsync(byte[] message)
        {
            var tcs = new TaskCompletionSource<bool>(amsSocket.Socket);
            SocketAsyncEventArgs args = new SocketAsyncEventArgs();
            args.Completed += (sender, e) =>
            {
                try
                {
                    // Watch out for chunked data transfer!
                    if (e.Count != e.BytesTransferred)
                    {
                        // We set the same buffer, but with an offset and new byte count. The already received data stays untouched.
                        e.SetBuffer(e.Buffer, e.BytesTransferred, e.Count - e.BytesTransferred);
                        StartReceiveAsync(e);
                    }
                    else
                    {
                        tcs.TrySetResult(e.SocketError == SocketError.Success);
                        e.Dispose();
                    }
                }
                catch (Exception ex) { tcs.TrySetException(ex); }
            };
            args.SetBuffer(message, 0, message.Length);
            StartReceiveAsync(args);
            return tcs.Task;
        }

        private void StartReceiveAsync(SocketAsyncEventArgs args)
        {
            try
            {
                amsSocket.Socket.ReceiveAsync(args);
            }
            catch (Exception ex)
            {
                if (!Object.ReferenceEquals(ex.GetType(), typeof(ObjectDisposedException))) throw ex;
            }
        }
    }
}
