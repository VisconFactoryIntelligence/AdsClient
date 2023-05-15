using System.Collections.Generic;
using Ads.Client.CommandResponse;
using Ads.Client.Common;

namespace Ads.Client.Commands
{
    public class AdsReadStateCommand : AdsCommand<AdsReadStateCommandResponse>
    {
        public AdsReadStateCommand()
            : base(AdsCommandId.ReadState)
        {

        }

        internal override IEnumerable<byte> GetBytes()
        {
            return new List<byte>();
        }
    }
}
