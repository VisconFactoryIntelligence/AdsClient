using System.Collections.Generic;
using System.Threading.Tasks;
using Ads.Client.CommandResponse;
using Ads.Client.Common;

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

        protected virtual void RunBefore(Ams ams)
        {
        }

        protected virtual void RunAfter(Ams ams)
        {
        }

        protected async Task<T> RunAsync<T>(Ams ams) where T : AdsCommandResponse, new()
        {
            RunBefore(ams);
            var result = await ams.RunCommandAsync<T>(this);
            if (result.AdsErrorCode > 0)
                throw new AdsException(result.AdsErrorCode);
            RunAfter(ams);
            return result;
        }

        internal abstract IEnumerable<byte> GetBytes();
    }
}
