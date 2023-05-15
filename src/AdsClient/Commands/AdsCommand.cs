using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Viscon.Communication.Ads.CommandResponse;
using Viscon.Communication.Ads.Common;
using Viscon.Communication.Ads.Conversation;

namespace Viscon.Communication.Ads.Commands
{
    public abstract class AdsCommand<TResponse> where TResponse : AdsCommandResponse, new()
    {
        public AdsCommand(ushort commandId)
        {
            this.commandId = commandId;
        }


        private ushort commandId;
        public ushort CommandId
        {
            get { return commandId; }
        }

        public ushort? AmsPortTargetOverride { get; set; }

        protected virtual void RunBefore(Ams ams)
        {
        }

        protected virtual void RunAfter(Ams ams)
        {
        }

        public async Task<TResponse> RunAsync(Ams ams, CancellationToken cancellationToken)
        {
            RunBefore(ams);
            var result = await ams.PerformRequestAsync(new AdsCommandConversation(this),
                AmsPortTargetOverride ?? ams.AmsPortTarget, cancellationToken).ConfigureAwait(false);
            RunAfter(ams);

            return result;
        }

        internal abstract IEnumerable<byte> GetBytes();

        private readonly struct AdsCommandRequest : IAdsRequest
        {
            private readonly AdsCommand<TResponse> command;
            private readonly Lazy<IEnumerable<byte>> data;

            public AdsCommandRequest(AdsCommand<TResponse> command)
            {
                this.command = command;
                data = new Lazy<IEnumerable<byte>>(command.GetBytes);
            }


            public AdsCommandEnum Command => (AdsCommandEnum)command.CommandId;

            public int GetRequestLength()
            {
                return data.Value.Count();
            }

            public int BuildRequest(Span<byte> span)
            {
                var i = 0;
                foreach (var b in data.Value)
                {
                    span[i] = b;
                    i++;
                }

                return i;
            }
        }

        private class AdsCommandConversation : IAdsConversation<AdsCommandRequest, TResponse>
        {
            public AdsCommand<TResponse> Command { get; }

            public AdsCommandConversation(AdsCommand<TResponse> command)
            {
                Command = command;
            }

            public AdsCommandRequest BuildRequest()
            {
                return new AdsCommandRequest(Command);
            }

            public TResponse ParseResponse(ReadOnlySpan<byte> buffer)
            {
                var response = new TResponse();

                // AdsCommandResponse expects [uint error, ...data]
                // Error is already checked by PerformRequestAsync, leaving 0 here.
                var target = new byte[4 + buffer.Length];
                var targetSpan = target.AsSpan();
                buffer.CopyTo(targetSpan.Slice(4));

                response.AdsResponse = target;
                response.ProcessResponse();

                return response;
            }
        }
    }
}
