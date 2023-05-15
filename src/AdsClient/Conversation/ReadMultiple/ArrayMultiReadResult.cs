using Viscon.Communication.Ads.Variables;

namespace Viscon.Communication.Ads.Conversation.ReadMultiple;

public record ArrayMultiReadResult(IVariableData[] Results) : IMultiReadResult;