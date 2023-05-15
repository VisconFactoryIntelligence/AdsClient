using System;

namespace Viscon.Communication.Ads.Variables;

public record MemoryVariableData(IVariableAddress Address, ReadOnlyMemory<byte> Data) : IVariableData
{
    ReadOnlySpan<byte> IVariableData.Data => Data.Span;
}