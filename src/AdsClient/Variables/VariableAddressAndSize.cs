namespace Ads.Client.Variables;

public record VariableAddressAndSize(uint IndexGroup, uint IndexOffset, uint Size) : VariableAddress(IndexGroup, IndexOffset), IVariableAddressAndSize;