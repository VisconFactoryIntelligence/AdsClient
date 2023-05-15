namespace Viscon.Communication.Ads.Variables;

public interface IVariableAddressAndSize : IVariableAddress
{
    uint Size { get; }
}