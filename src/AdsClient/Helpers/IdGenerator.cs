using System.Runtime.CompilerServices;
using System.Threading;

namespace Ads.Client.Helpers
{
    internal class IdGenerator
    {
        private uint id;

        public uint Next()
        {
            return (uint)Interlocked.Increment(ref Unsafe.As<uint, int>(ref id));
        }
    }
}
