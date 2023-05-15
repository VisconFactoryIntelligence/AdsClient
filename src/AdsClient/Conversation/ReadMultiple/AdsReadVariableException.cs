using Ads.Client.Common;
using Ads.Client.Variables;

namespace Ads.Client.Conversation.ReadMultiple;

public class AdsReadVariableException : AdsException
{
    public IVariableAddressAndSize Variable { get; }

    public AdsReadVariableException(IVariableAddressAndSize variable, uint errorCode) : base(errorCode)
    {
        Variable = variable;
    }
}