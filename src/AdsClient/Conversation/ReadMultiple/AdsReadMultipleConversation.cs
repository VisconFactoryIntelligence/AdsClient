using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Ads.Client.Internal;
using Ads.Client.Variables;

namespace Ads.Client.Conversation.ReadMultiple
{
    internal class AdsReadMultipleConversation<TResult> : IAdsConversation<AdsReadMultipleRequest, TResult>
    {
        private readonly IVariableAddressAndSize[] variables;
        private readonly IReadResultFactory<TResult> resultFactory;

        public AdsReadMultipleConversation(IVariableAddressAndSize[] variables, IReadResultFactory<TResult> resultFactory)
        {
            this.variables = variables;
            this.resultFactory = resultFactory;
        }

        public AdsReadMultipleRequest BuildRequest()
        {
            return new AdsReadMultipleRequest(variables);
        }

        public TResult ParseResponse(ReadOnlySpan<byte> buffer)
        {
            var offset = WireFormatting.ReadInt32(buffer, out var length);
            Assertions.AssertDataLength(buffer, length, offset);

            var resultsLength = variables.Length * sizeof(uint);
            var results = MemoryMarshal.Cast<byte, uint>(buffer.Slice(offset, resultsLength));
            List<Exception> exceptions = null;
            for (var i = 0; i < variables.Length; i++)
            {
                if (results[i] == 0) continue;

                exceptions ??= new List<Exception>();
                exceptions.Add(new AdsReadVariableException(variables[i], results[i]));
            }

            if (exceptions is not null) ReadMultipleException.Throw(exceptions);

            offset += resultsLength;

            return resultFactory.CreateResult(variables, buffer.Slice(offset));
        }
    }
}
