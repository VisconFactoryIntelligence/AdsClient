using System;

namespace Ads.Client.Variables;

public record ArraySegmentVariableData(IVariableAddress Address, ArraySegment<byte> Data) : IVariableData
{
    ReadOnlySpan<byte> IVariableData.Data => Data;
}