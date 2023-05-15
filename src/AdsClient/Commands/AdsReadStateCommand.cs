using System.Collections.Generic;
using Viscon.Communication.Ads.CommandResponse;
using Viscon.Communication.Ads.Common;

namespace Viscon.Communication.Ads.Commands
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
