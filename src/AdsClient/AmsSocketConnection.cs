using System;
using System.Net.Sockets;
using System.Threading.Tasks;
using Viscon.Communication.Ads.Helpers;
using Viscon.Communication.Ads.Internal;

namespace Viscon.Communication.Ads;

internal sealed class AmsSocketConnection
{
    private const int ReceiveTaskTimeout = 3000;

    private readonly Socket socket;
    private readonly IIncomingMessageHandler messageHandler;
    private readonly Task receiveTask;
    private readonly SocketAwaitable socketAwaitable = new SocketAwaitable();

    public AmsSocketConnection(Socket socket, IIncomingMessageHandler messageHandler)
    {
        this.socket = socket;
        this.messageHandler = messageHandler;
        receiveTask = ReceiveLoop();
    }

    private volatile bool closed;

    public void Close()
    {
        closed = true;
        socket.Shutdown(SocketShutdown.Both);
        socket.Close();

        receiveTask.Wait(ReceiveTaskTimeout);
        socketAwaitable.Dispose();
    }

    private async Task<byte[]> GetAmsMessage(byte[] tcpHeader)
    {
        uint responseLength = AmsHeaderHelper.GetResponseLength(tcpHeader);
        byte[] response = new byte[responseLength];

        await GetMessage(response);

        return response;
    }

    private Task GetMessage(byte[] response)
    {
        return ReceiveAsync(response);
    }

    private async Task Listen()
    {
        try
        {
            var buffer = await ListenForHeader();
            // If a ams header is received, then read the rest
            try
            {
                byte[] response = await GetAmsMessage(buffer);

#if DEBUG_AMS
                Debug.WriteLine("Received bytes: " + ByteArrayHelper.ByteArrayToTestString(buffer) + ',' +
                    ByteArrayHelper.ByteArrayToTestString(response));
#endif

                messageHandler.HandleMessage(response);
            }
            catch (Exception ex)
            {
                messageHandler.HandleException(ex);
            }
        }
        catch (Exception ex)
        {
            if (!ReferenceEquals(ex.GetType(), typeof(ObjectDisposedException))) throw;
        }
    }

    private async Task<byte[]> ListenForHeader()
    {
        var buffer = new byte[AmsHeaderHelper.AmsTcpHeaderSize];
        await ReceiveAsync(buffer);

        return buffer;
    }

    private async Task ReceiveAsync(byte[] buffer)
    {
        var sa = socketAwaitable;
        sa.SetBuffer(buffer, 0, buffer.Length);

        do
        {
            sa.SetBuffer(sa.Offset + sa.BytesTransferred, sa.Count - sa.BytesTransferred);
            await socket.ReceiveAwaitable(sa);

            if (sa.BytesTransferred == 0)
            {
                messageHandler.HandleException(new Exception("Remote host closed the connection."));
                Close();
            }
        } while (socketAwaitable.BytesTransferred != buffer.Length);
    }

    private async Task ReceiveLoop()
    {
        await Task.Yield();

        while (!closed)
        {
            await Listen();
        }
    }
}