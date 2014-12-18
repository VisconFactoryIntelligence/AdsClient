using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Ads.Client.Winsock
{
    public class AmsSocketSync : AmsSocketBaseSync
    {
        protected new AmsSocket amsSocket;
        public AmsSocketSync(AmsSocket amsSocket) : base(amsSocket)
        {
            this.amsSocket = amsSocket;
        }

        protected override void Connect()
        {
            amsSocket.Socket.Bind(amsSocket.LocalEndPoint);
            amsSocket.Socket.Connect(amsSocket.IpTarget, amsSocket.PortTarget);
        }

        public override void Send(byte[] message)
        {
            amsSocket.Socket.Send(message);
        }

        public override void Receive(byte[] message)
        {
            try
            {
                if (message.Length <= maxPacketSize)
                {
                    amsSocket.Socket.Receive(message);
                }
                else
                {
                    byte[] msg = new byte[maxPacketSize];
                    int totalread = 0;

                    while (totalread < message.Length)
                    {
                        int read = amsSocket.Socket.Receive(msg);
                        Array.Copy(msg, 0, message, totalread, read);
                        totalread += read;
                    }
                }
            }
            catch (Exception ex)
            {
                if (!Object.ReferenceEquals(ex.GetType(), typeof(ObjectDisposedException))) throw;
            }
        }

    }
}
