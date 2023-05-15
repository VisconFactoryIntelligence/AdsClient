using System;
using Viscon.Communication.Ads.Common;
using Viscon.Communication.Ads.Internal;

namespace Viscon.Communication.Ads.Conversation;

public readonly struct AdsReadRequest : IAdsRequest
{
    public AdsReadRequest(uint indexGroup, uint indexOffset, uint size)
    {
        IndexGroup = indexGroup;
        IndexOffset = indexOffset;
        Size = size;
    }

    public readonly uint IndexGroup;
    public readonly uint IndexOffset;
    public readonly uint Size;

    public AdsCommandEnum Command => AdsCommandEnum.Read;

    public int GetRequestLength()
    {
        return 12;
    }

    public int BuildRequest(Span<byte> span)
    {
        var offset = WireFormatting.WriteUInt32(ref span.GetStart(), IndexGroup);
        offset += WireFormatting.WriteUInt32(ref span.GetOffset(offset), IndexOffset);
        offset += WireFormatting.WriteUInt32(ref span.GetOffset(offset), Size);

        return offset;
    }
}