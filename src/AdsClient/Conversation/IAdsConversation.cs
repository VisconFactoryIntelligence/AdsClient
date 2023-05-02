using System;

namespace Ads.Client.Conversation;

public interface IAdsConversation<out TRequest, out TResponse> where TRequest : struct, IAdsRequest
{
    TRequest BuildRequest();
    TResponse ParseResponse(ReadOnlySpan<byte> buffer);
}