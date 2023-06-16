using System;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Threading;

namespace Viscon.Communication.Ads.Internal;

/// <summary>
/// Reusable awaitable for socket operations.
/// </summary>
/// <remarks>
/// Based on https://devblogs.microsoft.com/pfxteam/awaiting-socket-operations/.
/// </remarks>
internal sealed class SocketAwaitable : SocketAsyncEventArgs, INotifyCompletion
{
    private static readonly Action Sentinel = () => { };

    public volatile bool WasCompleted;
    private Action? continuation;

    protected override void OnCompleted(SocketAsyncEventArgs _)
    {
        var prev = continuation ?? Interlocked.CompareExchange(ref continuation, Sentinel, null);
        prev?.Invoke();
    }

    internal void Reset()
    {
        WasCompleted = false;
        continuation = null;
    }

    public SocketAwaitable GetAwaiter()
    {
        return this;
    }

    public bool IsCompleted => WasCompleted;

    public void OnCompleted(Action continuation)
    {
        if (this.continuation == Sentinel ||
            Interlocked.CompareExchange(ref this.continuation, continuation, null) == Sentinel)
        {
            continuation.Invoke();
        }
    }

    public void GetResult()
    {
        if (SocketError != SocketError.Success)
        {
            throw new SocketException((int)SocketError);
        }
    }
}