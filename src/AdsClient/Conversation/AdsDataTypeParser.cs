using System;
using Ads.Client.Common;
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
        offset += WireFormatting.ReadUInt16(buffer.Slice(offset), out var arrayDimensions);
        offset += WireFormatting.ReadUInt16(buffer.Slice(offset), out var subItemCount);
        offset += WireFormatting.ReadString(buffer.Slice(offset), nameLength, out var name);
        offset += WireFormatting.ReadString(buffer.Slice(offset), typeLength, out var type);
        offset += WireFormatting.ReadString(buffer.Slice(offset), commentLength, out var comment);
        offset += ReadArrayRanges(buffer.Slice(offset), arrayDimensions, out var arrayRanges);
        offset += ReadSubItems(buffer.Slice(offset), subItemCount, out var subItems);

        // End here for now, there's more data available.

        return new AdsDataTypeDto(version, hashValue, baseTypeHashValue, size, typeOffset, adsDataType.ToDataType(),
            flags.ToDataTypeFlags(), name, type, comment, arrayRanges, subItems);
    }

    private static int ReadArrayRanges(ReadOnlySpan<byte> buffer, ushort dimensions, out AdsArrayRange[] ranges)
    {
        ranges = new AdsArrayRange[dimensions];
        var offset = 0;
        for (var i = 0; i < dimensions; i++)
        {
            offset += WireFormatting.ReadUInt32(buffer.Slice(offset), out var start);
            offset += WireFormatting.ReadUInt32(buffer.Slice(offset), out var length);

            ranges[i] = new AdsArrayRange(start, length);
        }

        return offset;
    }

    private static int ReadSubItems(ReadOnlySpan<byte> buffer, ushort count, out AdsDataTypeDto[] subItems)
    {
        subItems = new AdsDataTypeDto[count];
        var offset = 0;
        for (var i = 0; i < count; i++)
        {
            WireFormatting.ReadInt32(buffer, out var len);
            subItems[i] = ParseDataType(buffer.Slice(4, len - 4));

            offset += len;
            buffer = buffer.Slice(len);
        }

        return offset;
    }
}
