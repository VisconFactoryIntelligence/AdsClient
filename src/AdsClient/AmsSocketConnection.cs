using Ads.Client.Helpers;
using Ads.Client.Internal;
using System;
using System.Diagnostics;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace Ads.Client;

internal class AmsSocketConnection
{
    private const int ReceiveTaskTimeout = 3000;

    private readonly Socket socket;
    private readonly IIncomingMessageHandler messageHandler;
    private readonly Task receiveTask;

    public AmsSocketConnection(Socket socket, IIncomingMessageHandler messageHandler)
    {
        this.socket = socket;
        this.messageHandler = messageHandler;
        receiveTask = Task.Run(ReceiveLoop);
    }

    private bool closed;

    public void Close()
    {
        closed = true;
        socket.Shutdown(SocketShutdown.Both);
        socket.Close();

        receiveTask.Wait(ReceiveTaskTimeout);
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
        using var args = new SocketAsyncEventArgs();
        args.SetBuffer(buffer, 0, buffer.Length);
        var awaitable = new SocketAwaitable(args);

        do
        {
            args.SetBuffer(args.Offset + args.BytesTransferred, args.Count - args.BytesTransferred);
            await socket.ReceiveAsync(awaitable);

            if (args.BytesTransferred == 0)
            {
                messageHandler.HandleException(new Exception("Remote host closed the connection."));
                Close();
            }
        } while (args.Count != args.BytesTransferred);
    }

    private async Task ReceiveLoop()
    {
        while (!closed)
        {
            await Listen();
        }
    }
}