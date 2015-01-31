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
                int totalread = 0;

                while (totalread < message.Length)
                {
                    var read = amsSocket.Socket.Receive(message, totalread, message.Length - totalread, SocketFlags.None);
                    totalread += read;
                }
            }
            catch (Exception ex)
            {
                if (!Object.ReferenceEquals(ex.GetType(), typeof(ObjectDisposedException))) throw;
            }
        }

    }
}
