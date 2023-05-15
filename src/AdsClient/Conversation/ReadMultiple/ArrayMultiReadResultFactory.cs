using System;
using Ads.Client.Variables;

namespace Ads.Client.Conversation.ReadMultiple;

public class ArrayMultiReadResultFactory : IReadResultFactory<ArrayMultiReadResult>
{
    public ArrayMultiReadResult CreateResult(IVariableAddressAndSize[] variables, ReadOnlySpan<byte> span)
    {
        var results = new IVariableData[variables.Length];

        var offset = 0;
        for (var i = 0; i < variables.Length; i++)
        {
            var variable = variables[i];

            results[i] = new ArrayVariableData(variable, span.Slice(offset, (int)variable.Size).ToArray());

            offset += (int)variable.Size;
        }

        return new ArrayMultiReadResult(results);
    }
}