namespace Ads.Client.Variables;

public record VariableAddress(uint IndexGroup, uint IndexOffset) : IVariableAddress;