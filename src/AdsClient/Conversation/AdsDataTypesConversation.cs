using System;
using System.Collections.Generic;
using Ads.Client.Common;
using Ads.Client.Internal;

namespace Ads.Client.Conversation;

internal class AdsDataTypesConversation : IAdsConversation<AdsReadRequest, List<AdsDataTypeDto>>
{
    public uint DataTypeLength { get; }

    public AdsDataTypesConversation(uint dataTypeLength) => DataTypeLength = dataTypeLength;

    public AdsDataTypesConversation(AdsUploadInfoDto uploadInfo) : this(uploadInfo.DataTypeLength) { }

    public AdsReadRequest BuildRequest() => new(AdsReservedIndexGroup.SymbolDataTypeUpload.ToUInt32(), 0, DataTypeLength);

    public List<AdsDataTypeDto> ParseResponse(ReadOnlySpan<byte> buffer)
    {
        var length = LittleEndianDeserializer.ReadInt32(buffer);
        if (length != buffer.Length - 4)
        {
            throw new Exception(
                $"Received {buffer.Length} bytes of data while length indicates length should be {length} bytes.");
        }

        buffer = buffer.Slice(sizeof(uint));

        var results = new List<AdsDataTypeDto>();
        while (buffer.Length > 0)
        {
            var len = LittleEndianDeserializer.ReadInt32(buffer);
            results.Add(AdsDataTypeParser.ParseDataType(buffer.Slice(4, len - 4)));

            buffer = buffer.Slice(len);
        }

        return results;
    }
}