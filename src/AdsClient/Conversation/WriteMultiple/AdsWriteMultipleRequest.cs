using System;
using System.Linq;
using Ads.Client.Common;
using Ads.Client.Internal;
using Ads.Client.Variables;

namespace Ads.Client.Conversation.WriteMultiple;

public readonly struct AdsWriteMultipleRequest : IAdsRequest
{
    private readonly IVariableData[] variables;

    public AdsWriteMultipleRequest(IVariableData[] variables)
    {
        this.variables = variables;
    }

    public AdsCommandEnum Command => AdsCommandEnum.ReadWrite;

    public int GetRequestLength()
    {
        return sizeof(uint) * 4 + GetDataLength();
    }

    public int BuildRequest(Span<byte> span)
    {
        var offset = WireFormatting.WriteUInt32(ref span.GetStart(), AdsReservedIndexGroup.SumCommandWrite);
        // Number of variables
        offset += WireFormatting.WriteUInt32(ref span.GetOffset(offset), (uint) variables.Length);
        // Expected response size (error code per item)
        offset += WireFormatting.WriteUInt32(ref span.GetOffset(offset), (uint) (sizeof(uint) * variables.Length));
        // Data length (address + length of value + value per item)
        offset += WireFormatting.WriteUInt32(ref span.GetOffset(offset), (uint) GetDataLength());

        foreach (var variable in variables)
        {
            offset += WireFormatting.WriteUInt32(ref span.GetOffset(offset), variable.Address.IndexGroup);
            offset += WireFormatting.WriteUInt32(ref span.GetOffset(offset), variable.Address.IndexOffset);
            offset += WireFormatting.WriteUInt32(ref span.GetOffset(offset), variable.Data.Length);
        }

        foreach (var variable in variables)
        {
            var variableData = variable.Data;
            variableData.CopyTo(span.Slice(offset));
            offset += variableData.Length;
        }

        return offset;
    }

    private int GetDataLength() => variables.Sum(v => v.Data.Length + sizeof(uint) * 3);
}