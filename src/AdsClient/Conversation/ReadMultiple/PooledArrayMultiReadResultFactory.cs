using System;
using System.Buffers;
using System.Linq;
using Viscon.Communication.Ads.Variables;

namespace Viscon.Communication.Ads.Conversation.ReadMultiple;

public class PooledArrayMultiReadResultFactory : IReadResultFactory<PooledArrayMultiReadResult>
{
    public PooledArrayMultiReadResult CreateResult(IVariableAddressAndSize[] variables, ReadOnlySpan<byte> span)
    {
        var data = ArrayPool<byte>.Shared.Rent((int)variables.Sum(v => v.Size));
        try
        {
            var results = new IVariableData[variables.Length];
            span.CopyTo(data);

            var offset = 0;
            for (var i = 0; i < variables.Length;i++)
            {
                var variable = variables[i];

                results[i] = new ArraySegmentVariableData(variable,
                    new ArraySegment<byte>(data, offset, (int)variable.Size));

                offset += (int) variable.Size;
            }

            return new PooledArrayMultiReadResult(results, data);
        }
        catch
        {
            ArrayPool<byte>.Shared.Return(data);
            throw;
        }
    }
}