namespace Ads.Client.Variables;

public interface IVariableAddressAndSize : IVariableAddress
{
    uint Size { get; }
}