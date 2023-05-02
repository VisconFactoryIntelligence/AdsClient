using System;
using Ads.Client.Internal;

namespace Ads.Client.Conversation;

internal static class AdsDataTypeParser
{
    public static AdsDataTypeDto ParseDataType(ReadOnlySpan<byte> buffer)
    {
        var offset = WireFormatting.ReadUInt32(buffer, out var version);
        offset += WireFormatting.ReadUInt32(buffer.Slice(offset), out var hashValue);
        offset += WireFormatting.ReadUInt32(buffer.Slice(offset), out var baseTypeHashValue);
        offset += WireFormatting.ReadUInt32(buffer.Slice(offset), out var size);
        offset += WireFormatting.ReadUInt32(buffer.Slice(offset), out var typeOffset);
        offset += WireFormatting.ReadUInt32(buffer.Slice(offset), out var adsDataType);
        offset += WireFormatting.ReadUInt32(buffer.Slice(offset), out var flags);
        offset += WireFormatting.ReadUInt16(buffer.Slice(offset), out var nameLength);
        offset += WireFormatting.ReadUInt16(buffer.Slice(offset), out var typeLength);
        offset += WireFormatting.ReadUInt16(buffer.Slice(offset), out var commentLength);
        offset += WireFormatting.ReadUInt16(buffer.Slice(offset), out var arrayDimension);
        offset += WireFormatting.ReadUInt16(buffer.Slice(offset), out var subItemCount);
        offset += WireFormatting.ReadString(buffer.Slice(offset), nameLength, out var name);
        offset += WireFormatting.ReadString(buffer.Slice(offset), typeLength, out var type);
        offset += WireFormatting.ReadString(buffer.Slice(offset), commentLength, out var comment);

        // End here for now, there's more data available.

        return new AdsDataTypeDto(version, hashValue, baseTypeHashValue, size, typeOffset, adsDataType.ToDataType(),
            flags.ToDataTypeFlags(), name, type, comment, arrayDimension, subItemCount);
    }
}