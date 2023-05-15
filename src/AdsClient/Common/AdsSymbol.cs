using Viscon.Communication.Ads.Variables;

namespace Viscon.Communication.Ads.Common;

public record AdsSymbol(uint IndexGroup, uint IndexOffset, uint Size, string Name, string TypeName, string Comment) : IVariableAddressAndSize;