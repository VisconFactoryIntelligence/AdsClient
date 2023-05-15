using Viscon.Communication.Ads.Variables;

namespace Viscon.Communication.Ads.Conversation.ReadMultiple;

public interface IMultiReadResult
{
    IVariableData[] Results { get; }
}