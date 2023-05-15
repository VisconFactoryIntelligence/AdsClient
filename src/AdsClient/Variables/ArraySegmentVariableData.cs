using System;

namespace Viscon.Communication.Ads.Variables;

public record ArraySegmentVariableData(IVariableAddress Address, ArraySegment<byte> Data) : IVariableData
{
    ReadOnlySpan<byte> IVariableData.Data => Data;
}