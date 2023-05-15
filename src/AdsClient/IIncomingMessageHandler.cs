using System;

namespace Viscon.Communication.Ads;

public interface IIncomingMessageHandler
{
    void HandleException(Exception exception);
    void HandleMessage(byte[] message);
}