﻿using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace Ads.Client
{
    public sealed class AmsSocketAsync : IAmsSocketAsync
    {
        protected new AmsSocket amsSocket;
        public AmsSocketAsync(AmsSocket amsSocket)
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

        private Task ConnectAsync()
        {
            var tcs = new TaskCompletionSource<bool>(amsSocket.Socket);
            amsSocket.Socket.Bind(amsSocket.LocalEndPoint);

            var args = new SocketAsyncEventArgs();
            args.RemoteEndPoint = new DnsEndPoint(amsSocket.IpTarget, amsSocket.PortTarget);
            args.Completed += (_, e) => CompleteSocketCall(tcs, e);

            if (!amsSocket.Socket.ConnectAsync(args))
            {
                CompleteSocketCall(tcs, args);
            }

            return tcs.Task;
        }

        public Task SendAsync(byte[] message)
        {
            var tcs = new TaskCompletionSource<bool>(amsSocket.Socket);
            var args = new SocketAsyncEventArgs();
            args.Completed += (_, e) => CompleteSocketCall(tcs, e);

            args.SetBuffer(message, 0, message.Length);

            if (!amsSocket.Socket.SendAsync(args))
            {
                CompleteSocketCall(tcs, args);
            }

            return tcs.Task;
        }

        public  Task ReceiveAsync(byte[] message)
        {
            var tcs = new TaskCompletionSource<bool>(amsSocket.Socket);
            SocketAsyncEventArgs args = new SocketAsyncEventArgs();

            void Complete(SocketAsyncEventArgs e)
            {
                try
                {
                    // Watch out for chunked data transfer!
                    if (e.Count != e.BytesTransferred)
                    {
                        // We set the same buffer, but with an offset and new byte count. The already received data stays untouched.
                        e.SetBuffer(e.Buffer, e.Offset + e.BytesTransferred, e.Count - e.BytesTransferred);
                        StartReceiveAsync(e, Complete);
                    }
                    else
                    {
                        CompleteSocketCall(tcs, e);
                    }
                }
                catch (Exception ex)
                {
                    tcs.TrySetException(ex);
                }
            }

            args.Completed += (_, e) => Complete(e);
            args.SetBuffer(message, 0, message.Length);
            StartReceiveAsync(args, Complete);
            return tcs.Task;
        }

        private void StartReceiveAsync(SocketAsyncEventArgs args, Action<SocketAsyncEventArgs> onComplete)
        {
            try
            {
                if (!amsSocket.Socket.ReceiveAsync(args))
                {
                    onComplete(args);
                }
            }
            catch (Exception ex)
            {
                if (!Object.ReferenceEquals(ex.GetType(), typeof(ObjectDisposedException))) throw ex;
            }
        }

        private static void CompleteSocketCall(TaskCompletionSource<bool> taskCompletionSource, SocketAsyncEventArgs args)
        {
            try
            {
                if (args.SocketError == SocketError.Success)
                {
                    taskCompletionSource.TrySetResult(true);
                }
                else
                {
                    taskCompletionSource.SetException(new SocketException((int)args.SocketError));
                }
            }
            finally
            {
                args.Dispose();
            }
        }
    }
}
