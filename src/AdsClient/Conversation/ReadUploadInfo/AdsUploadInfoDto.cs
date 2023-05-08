namespace Ads.Client.Conversation.ReadUploadInfo;

public record AdsUploadInfoDto(uint SymbolCount, uint SymbolLength, uint DataTypeCount, uint DataTypeLength,
    uint ExtraCount, uint ExtraLength);