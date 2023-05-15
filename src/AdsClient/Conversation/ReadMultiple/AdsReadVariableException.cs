using Viscon.Communication.Ads.Common;
using Viscon.Communication.Ads.Variables;

namespace Viscon.Communication.Ads.Conversation.ReadMultiple;

public class AdsReadVariableException : AdsException
{
    public IVariableAddressAndSize Variable { get; }

    public AdsReadVariableException(IVariableAddressAndSize variable, uint errorCode) : base(errorCode)
    {
        Variable = variable;
    }
}