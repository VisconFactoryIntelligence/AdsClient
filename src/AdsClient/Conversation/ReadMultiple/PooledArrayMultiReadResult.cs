using System;
using System.Buffers;
using Viscon.Communication.Ads.Variables;

namespace Viscon.Communication.Ads.Conversation.ReadMultiple;

public class PooledArrayMultiReadResult : IMultiReadResult, IDisposable
{
    private readonly byte[] data;

    public PooledArrayMultiReadResult(IVariableData[] results, byte[] data)
    {
        Results = results;

        this.data = data;
    }

    public IVariableData[] Results { get; }

    public void Dispose()
    {
        ArrayPool<byte>.Shared.Return(data);
    }
}