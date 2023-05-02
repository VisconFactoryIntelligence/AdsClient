namespace Ads.Client.Conversation;

public record AdsUploadInfoDto(uint SymbolCount, uint SymbolLength, uint DataTypeCount, uint DataTypeLength,
    uint ExtraCount, uint ExtraLength);