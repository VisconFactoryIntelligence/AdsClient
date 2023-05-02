using System;
using Ads.Client.Common;
using Ads.Client.Internal;

namespace Ads.Client.Conversation;

internal class AdsUploadInfoConversation : IAdsConversation<AdsReadRequest, AdsUploadInfoDto>
{
    public AdsReadRequest BuildRequest() => new(AdsReservedIndexGroup.SymbolUploadInfo2.ToUInt32(), 0, 24);

    public AdsUploadInfoDto ParseResponse(ReadOnlySpan<byte> buffer)
    {
        return new AdsUploadInfoDto(
            LittleEndianDeserializer.ReadUInt32(buffer),
            LittleEndianDeserializer.ReadUInt32(buffer.Slice(4)),
            LittleEndianDeserializer.ReadUInt32(buffer.Slice(8)),
            LittleEndianDeserializer.ReadUInt32(buffer.Slice(12)),
            LittleEndianDeserializer.ReadUInt32(buffer.Slice(16)),
            LittleEndianDeserializer.ReadUInt32(buffer.Slice(20)));
    }
}