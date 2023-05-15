namespace Viscon.Communication.Ads.Variables;

public record VariableAddress(uint IndexGroup, uint IndexOffset) : IVariableAddress;