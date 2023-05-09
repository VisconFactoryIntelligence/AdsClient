using System;
using System.Linq;
using Ads.Client.Common;
using Ads.Client.Internal;

namespace Ads.Client.Conversation.CreateVariableHandles
{
    internal class AdsCreateVariableHandlesConversation : IAdsConversation<AdsCreateVariableHandlesRequest, uint[]>
    {
        public string[] VariableNames { get; }

        public AdsCreateVariableHandlesConversation(string[] variableNames)
        {
            VariableNames = variableNames;
        }

        public AdsCreateVariableHandlesRequest BuildRequest()
        {
            return new AdsCreateVariableHandlesRequest(VariableNames);
        }

        public uint[] ParseResponse(ReadOnlySpan<byte> buffer)
        {
            var offset = WireFormatting.ReadInt32(buffer, out var dataLength);
            if (dataLength != buffer.Length - 4)
            {
                throw new Exception(
                    $"Received {buffer.Length} bytes of data while length indicates length should be {dataLength} bytes.");
            }

            foreach (var _ in VariableNames)
            {
                offset += WireFormatting.ReadUInt32(buffer.Slice(offset), out var errorCode);
                if (errorCode != 0) throw new AdsException(errorCode);

                offset += WireFormatting.ReadUInt32(buffer.Slice(offset), out var length);
                if (length != sizeof(uint))
                    throw new AdsException(
                        $"Received handle response of length {length}, expected length {sizeof(uint)}.");
            }

            var handles = new uint[VariableNames.Length];
            for (var i = 0; i < VariableNames.Length; i++)
            {
                offset += WireFormatting.ReadUInt32(buffer.Slice(offset), out handles[i]);
            }

            return handles;
        }
    }

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
                    AdsReservedIndexGroup.SymbolHandleByName);
                offset += WireFormatting.WriteUInt32(ref span.GetOffset(offset), (uint) 0);
                // Size of the requested data, which is sizeof(Symbol.IndexOffset)
                offset += WireFormatting.WriteUInt32(ref span.GetOffset(offset), (uint) sizeof(uint));
                offset += WireFormatting.WriteUInt32(ref span.GetOffset(offset), (uint)variableName.Length + 1);
            }

            foreach (var variableName in VariableNames)
            {
                offset += WireFormatting.WriteString(span.Slice(offset), variableName);
            }

            return offset;
        }
    }

}
