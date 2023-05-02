using Ads.Client.Common;

namespace Ads.Client.Conversation;

public record AdsDataTypeDto(uint Version, uint HashValue, uint BaseTypeHashValue, uint Size, uint Offset,
    AdsDataType DataType, AdsDataTypeFlags Flags, string Name, string Type, string Comment, ushort ArrayDimension,
    ushort SubItemCount);