using System;
using Ads.Client.Common;
using Ads.Client.Internal;

namespace Ads.Client.Conversation.ReadUploadInfo;

internal class AdsReadUploadInfoConversation : IAdsConversation<AdsReadRequest, AdsUploadInfoDto>
{
    public AdsReadRequest BuildRequest() => new(AdsReservedIndexGroup.SymbolUploadInfo2.ToUInt32(), 0, 24);

    public AdsUploadInfoDto ParseResponse(ReadOnlySpan<byte> buffer)
    {
        var length = LittleEndianDeserializer.ReadUInt32(buffer);
        if (length != 24)
        {
            throw new Exception(
                $"Received {buffer.Length} bytes of data while length indicates length should be {length} bytes.");
        }

        return new AdsUploadInfoDto(
            LittleEndianDeserializer.ReadUInt32(buffer.Slice(4)),
            LittleEndianDeserializer.ReadUInt32(buffer.Slice(8)),
            LittleEndianDeserializer.ReadUInt32(buffer.Slice(12)),
            LittleEndianDeserializer.ReadUInt32(buffer.Slice(16)),
            LittleEndianDeserializer.ReadUInt32(buffer.Slice(20)),
            LittleEndianDeserializer.ReadUInt32(buffer.Slice(24)));
    }
}