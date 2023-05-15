using System;

namespace Viscon.Communication.Ads.Conversation;

public interface IAdsConversation<out TRequest, out TResponse> where TRequest : struct, IAdsRequest
{
    TRequest BuildRequest();
    TResponse ParseResponse(ReadOnlySpan<byte> buffer);
}