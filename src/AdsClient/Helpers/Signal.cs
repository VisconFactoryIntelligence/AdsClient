using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace Ads.Client.Helpers
{
    [DebuggerNonUserCode]
    [DebuggerDisplay(nameof(NeedToWait) + ": {" + nameof(NeedToWait) + ",nq}")]
    internal class Signal : IDisposable
    {
        public Signal()
        {
            if (!channel.Writer.TryWrite(0))
            {
                throw new Exception("Failed to initialize the send signal.");
            }
        }

        private readonly Channel<int> channel = Channel.CreateBounded<int>(1);

        public void Dispose() => channel.Writer.Complete();

        public ValueTask<int> WaitAsync(CancellationToken cancellationToken) =>
            channel.Reader.ReadAsync(cancellationToken);

        public bool TryRelease() => channel.Writer.TryWrite(0);

        private bool NeedToWait => channel.Reader.Count == 0;
    }
}
