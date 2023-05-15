using Ads.Client.Variables;

namespace Ads.Client.Conversation.ReadMultiple;

public interface IMultiReadResult
{
    IVariableData[] Results { get; }
}