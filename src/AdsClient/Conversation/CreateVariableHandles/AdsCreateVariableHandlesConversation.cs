using System;
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
}
