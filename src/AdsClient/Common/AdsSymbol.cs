using Ads.Client.Variables;

namespace Ads.Client.Common;

public record AdsSymbol(uint IndexGroup, uint IndexOffset, uint Size, string Name, string TypeName, string Comment) : IVariableAddressAndSize;