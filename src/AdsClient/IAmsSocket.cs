using Ads.Client.Common;
using System;

namespace Ads.Client
{
    public interface IAmsSocket : IDisposable
    {
        bool IsConnected { get;}
		string IpTarget { get; set; }
		int PortTarget { get; set; }
        int Subscribers { get; set; }

        event EventHandler<AmsSocketResponseArgs> OnReadCallBack;

        IAmsSocketAsync Async { get; set; }
    }
}
