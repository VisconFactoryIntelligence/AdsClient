using Ads.Client.Variables;

namespace Ads.Client.Conversation.ReadMultiple;

public record ArrayMultiReadResult(IVariableData[] Results) : IMultiReadResult;