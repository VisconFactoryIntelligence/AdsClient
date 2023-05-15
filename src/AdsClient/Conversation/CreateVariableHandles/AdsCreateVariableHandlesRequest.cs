using System;
using System.Linq;
using Viscon.Communication.Ads.Common;
using Viscon.Communication.Ads.Internal;

namespace Viscon.Communication.Ads.Conversation.CreateVariableHandles;

internal readonly struct AdsCreateVariableHandlesRequest : IAdsRequest
{
    public readonly string[] VariableNames;

    public AdsCreateVariableHandlesRequest(string[] variableNames)
    {
        VariableNames = variableNames;
    }

    public AdsCommandEnum Command => AdsCommandEnum.ReadWrite;

    private int GetRequestDataLength()
    {
        // 16 bytes per sub-request + encoded name lengths
        return VariableNames.Length * 16 + VariableNames.Sum(x => x.Length + 1);
    }

    public int GetRequestLength()
    {
        // 16 bytes command + request data length
        return 16 + GetRequestDataLength();
    }

    public int BuildRequest(Span<byte> span)
    {
        var offset = WireFormatting.WriteUInt32(ref span.GetStart(), AdsReservedIndexGroup.SumCommandReadWrite.ToUInt32());
        offset += WireFormatting.WriteUInt32(ref span.GetOffset(offset), (uint) VariableNames.Length);
        // Response data length
        offset += WireFormatting.WriteUInt32(ref span.GetOffset(offset), (uint)(12 * VariableNames.Length));
        // Request data length
        offset += WireFormatting.WriteUInt32(ref span.GetOffset(offset), GetRequestDataLength());

        foreach (var variableName in VariableNames)
        {
            offset += WireFormatting.WriteUInt32(ref span.GetOffset(offset),
                AdsReservedIndexGroup.SymbolHandleByName.ToUInt32());
            offset += WireFormatting.WriteUInt32(ref span.GetOffset(offset), (uint) 0);
            // Size of the requested data, which is sizeof(Symbol.IndexOffset)
            offset += WireFormatting.WriteUInt32(ref span.GetOffset(offset), (uint) sizeof(uint));
            offset += WireFormatting.WriteUInt32(ref span.GetOffset(offset), (uint) variableName.Length + 1);
        }

        foreach (var variableName in VariableNames)
        {
            offset += WireFormatting.WriteString(span.Slice(offset), variableName);
        }

        return offset;
    }
}