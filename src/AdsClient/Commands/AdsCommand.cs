using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Ads.Client.CommandResponse;
using Ads.Client.Common;
using Ads.Client.Conversation;

namespace Ads.Client.Commands
{
    public abstract class AdsCommand
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

        protected async Task<T> RunAsync<T>(Ams ams, CancellationToken cancellationToken) where T : AdsCommandResponse, new()
        {
            RunBefore(ams);
            var result = await ams.PerformRequestAsync(new AdsCommandConversation<T>(this),
                AmsPortTargetOverride ?? ams.AmsPortTarget, cancellationToken).ConfigureAwait(false);
            RunAfter(ams);

            return result;
        }

        internal abstract IEnumerable<byte> GetBytes();

        private readonly struct AdsCommandRequest : IAdsRequest
        {
            private readonly AdsCommand command;
            private readonly Lazy<IEnumerable<byte>> data;

            public AdsCommandRequest(AdsCommand command)
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

        private class AdsCommandConversation<TResponse> : IAdsConversation<AdsCommandRequest, TResponse>
            where TResponse : AdsCommandResponse, new()
        {
            public AdsCommand Command { get; }

            public AdsCommandConversation(AdsCommand command)
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
