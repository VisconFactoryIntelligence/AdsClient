using System;
using System.Linq;
using Viscon.Communication.Ads.Common;
using Viscon.Communication.Ads.Internal;
using Viscon.Communication.Ads.Variables;

namespace Viscon.Communication.Ads.Conversation.ReadMultiple;

public readonly struct AdsReadMultipleRequest : IAdsRequest
{
    private readonly IVariableAddressAndSize[] variables;

    public AdsReadMultipleRequest(IVariableAddressAndSize[] variables)
    {
        this.variables = variables;
    }

    public AdsCommandEnum Command => AdsCommandEnum.ReadWrite;

    public int GetRequestLength()
    {
        return sizeof(uint) * 4 + variables.Length * sizeof(uint) * 3;
    }

    public int BuildRequest(Span<byte> span)
    {
        var offset =
            WireFormatting.WriteUInt32(ref span.GetStart(), AdsReservedIndexGroup.SumCommandRead.ToUInt32());
        offset += WireFormatting.WriteUInt32(ref span.GetOffset(offset), variables.Length);
        offset += WireFormatting.WriteUInt32(ref span.GetOffset(offset), (uint) variables.Sum(v => v.Size + sizeof(uint)));
        offset += WireFormatting.WriteUInt32(ref span.GetOffset(offset), (uint) GetDataLength());

        foreach (var variable in variables)
        {
            offset += WireFormatting.WriteUInt32(ref span.GetOffset(offset), variable.IndexGroup);
            offset += WireFormatting.WriteUInt32(ref span.GetOffset(offset), variable.IndexOffset);
            offset += WireFormatting.WriteUInt32(ref span.GetOffset(offset), variable.Size);
        }

        return offset;
    }

    private int GetDataLength()
    {
        return variables.Length * sizeof(uint) * 3;
    }
}