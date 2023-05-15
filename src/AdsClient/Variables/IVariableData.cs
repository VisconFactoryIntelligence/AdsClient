using System;

namespace Ads.Client.Variables;

public interface IVariableData
{
    IVariableAddress Address { get; }
    ReadOnlySpan<byte> Data { get; }
}