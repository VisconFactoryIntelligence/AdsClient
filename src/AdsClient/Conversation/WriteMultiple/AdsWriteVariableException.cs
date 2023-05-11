using Ads.Client.Common;
using Ads.Client.Variables;

namespace Ads.Client.Conversation.WriteMultiple
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
