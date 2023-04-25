using System.Net;
using System.Threading.Tasks;
using Ads.Client.Common;
using System;

namespace Ads.Client
{
    public interface IAmsSocketSync
    {
        void ConnectAndListen();
        void Send(byte[] message);
        void Receive(byte[] message);
    }
}
