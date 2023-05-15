namespace Viscon.Communication.Ads.Variables;

public record VariableAddressAndSize(uint IndexGroup, uint IndexOffset, uint Size) : VariableAddress(IndexGroup, IndexOffset), IVariableAddressAndSize;