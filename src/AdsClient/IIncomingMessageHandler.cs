using System;

namespace Ads.Client;

public interface IIncomingMessageHandler
{
    void HandleException(Exception exception);
    void HandleMessage(byte[] message);
}