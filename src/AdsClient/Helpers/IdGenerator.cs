using System.Threading;

namespace Viscon.Communication.Ads.Helpers
{
    internal class IdGenerator
    {
        private int id;

        public uint Next()
        {
            return (uint)Interlocked.Increment(ref id);
        }
    }
}
