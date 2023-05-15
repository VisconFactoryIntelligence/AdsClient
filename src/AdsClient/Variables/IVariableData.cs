using System;

namespace Viscon.Communication.Ads.Variables;

public interface IVariableData
{
    IVariableAddress Address { get; }
    ReadOnlySpan<byte> Data { get; }
}