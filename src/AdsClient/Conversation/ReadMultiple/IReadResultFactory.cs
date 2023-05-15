using System;
using Viscon.Communication.Ads.Variables;

namespace Viscon.Communication.Ads.Conversation.ReadMultiple;

public interface IReadResultFactory<out TResult>
{
    TResult CreateResult(IVariableAddressAndSize[] variables, ReadOnlySpan<byte> span);
}