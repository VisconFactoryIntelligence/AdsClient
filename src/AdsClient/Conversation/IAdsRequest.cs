using System;
using Viscon.Communication.Ads.Common;

namespace Viscon.Communication.Ads.Conversation;

public interface IAdsRequest
{
    AdsCommandEnum Command { get; }
    int GetRequestLength();
    int BuildRequest(Span<byte> span);
}