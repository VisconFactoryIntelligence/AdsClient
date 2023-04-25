using System;
using System.Threading;

namespace Ads.Client.Common
{
    public class AmsSocketResponseArgs : EventArgs
    {
        public Exception Error { get; set; }
        public byte[] Response { get; set; }
        public SynchronizationContext Context { get; set; }
    }
}
