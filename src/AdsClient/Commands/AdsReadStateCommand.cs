using System.Collections.Generic;
using System.Threading.Tasks;
using Ads.Client.CommandResponse;
using Ads.Client.Common;

namespace Ads.Client.Commands
{
    public class AdsReadStateCommand : AdsCommand
    {
        public AdsReadStateCommand()
            : base(AdsCommandId.ReadState)
        {

        }

        internal override IEnumerable<byte> GetBytes()
        {
            return new List<byte>();
        }

        #if !NO_ASYNC
        public Task<AdsReadStateCommandResponse> RunAsync(Ams ams)
        {
            return RunAsync<AdsReadStateCommandResponse>(ams);
        }
        #endif
    }
}
