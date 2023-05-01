using Ads.Client.Common;
using System;

namespace Ads.Client
{
    public interface IAmsSocket : IDisposable
    {
        bool IsConnected { get;}
		string IpTarget { get; }
		int PortTarget { get; }
        int Subscribers { get; set; }

        event EventHandler<AmsSocketResponseArgs> OnReadCallBack;

        IAmsSocketAsync Async { get; }
    }
}
