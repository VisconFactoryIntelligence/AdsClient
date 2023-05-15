using System;

namespace Viscon.Communication.Ads.Common
{
    public class AmsSocketResponseArgs : EventArgs
    {
        public Exception Error { get; set; }
        public byte[] Response { get; set; }
    }
}
