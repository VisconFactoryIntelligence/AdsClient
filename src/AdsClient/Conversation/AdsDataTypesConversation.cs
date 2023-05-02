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
        var results = new List<AdsDataTypeDto>();
        while (buffer.Length > 0)
        {
            var len = LittleEndianDeserializer.ReadInt32(buffer);
            results.Add(AdsDataTypeParser.ParseDataType(buffer.Slice(4, len)));

            buffer = buffer.Slice(4 + len);
        }

        return results;
    }
}