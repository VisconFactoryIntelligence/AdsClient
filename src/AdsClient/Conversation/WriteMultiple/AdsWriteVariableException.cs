using Viscon.Communication.Ads.Common;
using Viscon.Communication.Ads.Variables;

namespace Viscon.Communication.Ads.Conversation.WriteMultiple
{
    public class AdsWriteVariableException : AdsException
    {
        public IVariableData Variable { get; }

        public AdsWriteVariableException(IVariableData variable, uint errorCode) : base(errorCode)
        {
            Variable = variable;
        }
    }
}
