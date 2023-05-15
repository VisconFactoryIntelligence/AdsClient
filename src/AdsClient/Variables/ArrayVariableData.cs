using System;

namespace Viscon.Communication.Ads.Variables;

public record ArrayVariableData(IVariableAddress Address, byte[] Data) : IVariableData
{
    ReadOnlySpan<byte> IVariableData.Data => Data;
}