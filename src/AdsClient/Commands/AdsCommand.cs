using System;
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

        #if !NO_ASYNC
        protected async Task<T> RunAsync<T>(Ams ams) where T : AdsCommandResponse
        {
            RunBefore(ams);
            var result = await ams.RunCommandAsync<T>(this);
            if (result.AdsErrorCode > 0)
                throw new AdsException(result.AdsErrorCode);
            RunAfter(ams);
            return result;
        }
        #endif


        #if !SILVERLIGHT
        protected T Run<T>(Ams ams) where T : AdsCommandResponse
        {
            RunBefore(ams);
            var result = ams.RunCommand<T>(this);
            if (result.AdsErrorCode > 0)
                throw new AdsException(result.AdsErrorCode);
            RunAfter(ams);
            return result;
        }
        #endif

        internal abstract IEnumerable<byte> GetBytes();
    }
}
