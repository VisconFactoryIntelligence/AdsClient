namespace Ads.Client.Common;

public record AdsDataTypeDto(uint Version, uint HashValue, uint BaseTypeHashValue, uint Size, uint Offset,
    AdsDataType DataType, AdsDataTypeFlags Flags, string Name, string Type, string Comment, AdsArrayRange[] ArrayRanges,
    AdsDataTypeDto[] SubItems);