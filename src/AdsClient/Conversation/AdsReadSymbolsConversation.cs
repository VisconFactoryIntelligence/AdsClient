using System;
using System.Collections.Generic;
using Ads.Client.Common;
using Ads.Client.Internal;

namespace Ads.Client.Conversation
{
    internal class AdsReadSymbolsConversation : IAdsConversation<AdsReadRequest, List<AdsSymbol>>
    {
        public uint SymbolLength { get; }

        public AdsReadSymbolsConversation(uint symbolLength)
        {
            SymbolLength = symbolLength;
        }

        public AdsReadSymbolsConversation(AdsUploadInfoDto uploadInfo) : this(uploadInfo.SymbolLength)
        {
        }

        public AdsReadRequest BuildRequest()
        {
            return new AdsReadRequest(AdsReservedIndexGroup.SymbolUpload.ToUInt32(), 0, SymbolLength);
        }

        public List<AdsSymbol> ParseResponse(ReadOnlySpan<byte> buffer)
        {
            var results = new List<AdsSymbol>();
            while (buffer.Length > 0)
            {
                var offset = WireFormatting.ReadInt32(buffer, out var len);
                results.Add(AdsSymbolParser.ParseSymbol(buffer.Slice(offset, len)));

                buffer = buffer.Slice(offset + len);
            }

            return results;
        }
    }
}
