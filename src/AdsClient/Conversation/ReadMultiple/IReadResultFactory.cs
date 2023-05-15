using System;
using Ads.Client.Variables;

namespace Ads.Client.Conversation.ReadMultiple;

public interface IReadResultFactory<out TResult>
{
    TResult CreateResult(IVariableAddressAndSize[] variables, ReadOnlySpan<byte> span);
}