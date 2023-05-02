using System;
using Ads.Client.Common;

namespace Ads.Client.Conversation;

public interface IAdsRequest
{
    AdsCommandEnum Command { get; }
    int GetRequestLength();
    int BuildRequest(Span<byte> span);
}