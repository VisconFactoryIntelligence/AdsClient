using System;

namespace Ads.Client.Variables;

public record ArrayVariableData(IVariableAddress Address, byte[] Data) : IVariableData
{
    ReadOnlySpan<byte> IVariableData.Data => Data;
}