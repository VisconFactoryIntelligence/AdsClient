using System.Collections.Generic;
using System.Threading;
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

        public Task<AdsReadStateCommandResponse> RunAsync(Ams ams, CancellationToken cancellationToken)
        {
            return RunAsync<AdsReadStateCommandResponse>(ams, cancellationToken);
        }
    }
}
