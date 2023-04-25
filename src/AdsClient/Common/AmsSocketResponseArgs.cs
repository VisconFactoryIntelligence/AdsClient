using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Ads.Client.Common
{
    public class AmsSocketResponseArgs : EventArgs
    {
        public Exception Error { get; set; }
        public byte[] Response { get; set; }
        public SynchronizationContext Context { get; set; }
    }
}
