using System.Net.Sockets;

namespace Viscon.Communication.Ads.Internal;

/// <summary>
/// Socket extensions to send and receive using <see cref="SocketAwaitable" />.
/// </summary>
/// <remarks>
/// Based on https://devblogs.microsoft.com/pfxteam/awaiting-socket-operations/.
/// </remarks>
internal static class SocketExtensions
{
    public static SocketAwaitable ReceiveAwaitable(this Socket socket, SocketAwaitable awaitable)
    {
        awaitable.Reset();
        if (!socket.ReceiveAsync(awaitable))
            awaitable.WasCompleted = true;
        return awaitable;
    }

    public static SocketAwaitable SendAwaitable(this Socket socket, SocketAwaitable awaitable)
    {
        awaitable.Reset();
        if (!socket.SendAsync(awaitable))
            awaitable.WasCompleted = true;
        return awaitable;
    }
}