using System;
using Ads.Client.Common;
using Ads.Client.Internal;

namespace Ads.Client.Conversation.ReadSymbols;

internal class AdsSymbolParser
{
    public static AdsSymbol ParseSymbol(ReadOnlySpan<byte> buffer)
    {
        var offset = WireFormatting.ReadUInt32(buffer, out var indexGroup);
        offset += WireFormatting.ReadUInt32(buffer.Slice(offset), out var indexOffset);
        offset += WireFormatting.ReadUInt32(buffer.Slice(offset), out var size);
        offset += WireFormatting.ReadUInt32(buffer.Slice(offset), out var adsDataType);
        offset += WireFormatting.ReadUInt16(buffer.Slice(offset), out var adsSymbolFlags);
        offset += WireFormatting.ReadUInt16(buffer.Slice(offset), out var arrayDimension);
        offset += WireFormatting.ReadUInt16(buffer.Slice(offset), out var nameLength);
        offset += WireFormatting.ReadUInt16(buffer.Slice(offset), out var typeLength);
        offset += WireFormatting.ReadUInt16(buffer.Slice(offset), out var commentLength);
        offset += WireFormatting.ReadString(buffer.Slice(offset), nameLength, out var name);
        offset += WireFormatting.ReadString(buffer.Slice(offset), typeLength, out var type);
        offset += WireFormatting.ReadString(buffer.Slice(offset), commentLength, out var comment);

        return new AdsSymbol(indexGroup, indexOffset, size, name, type, comment);
    }
}