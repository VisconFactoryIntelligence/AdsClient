using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Viscon.Communication.Ads.Internal;
using Viscon.Communication.Ads.Variables;

namespace Viscon.Communication.Ads.Conversation.WriteMultiple
{
    internal class AdsWriteMultipleConversation : IAdsConversation<AdsWriteMultipleRequest, object>
    {
        public IVariableData[] Variables { get; }

        public AdsWriteMultipleConversation(IVariableData[] variables)
        {
            Variables = variables;
        }

        public AdsWriteMultipleRequest BuildRequest()
        {
            return new AdsWriteMultipleRequest(Variables);
        }

        public object ParseResponse(ReadOnlySpan<byte> buffer)
        {
            var offset = WireFormatting.ReadInt32(buffer, out var dataLength);
            Assertions.AssertDataLength(buffer, dataLength, offset);

            var results = MemoryMarshal.Cast<byte, uint>(buffer.Slice(offset));
            List<Exception> exceptions = null;
            for (var i = 0; i < Variables.Length; i++)
            {
                if (results[i] <= 0) continue;

                exceptions ??= new List<Exception>();
                exceptions.Add(new AdsWriteVariableException(Variables[i], results[i]));
            }

            if (exceptions is not null) WriteMultipleException.Throw(exceptions);

            return null;
        }
    }
}
