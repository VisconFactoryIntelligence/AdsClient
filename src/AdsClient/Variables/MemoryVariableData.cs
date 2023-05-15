using System;

namespace Ads.Client.Variables;

public record MemoryVariableData(IVariableAddress Address, ReadOnlyMemory<byte> Data) : IVariableData
{
    ReadOnlySpan<byte> IVariableData.Data => Data.Span;
}