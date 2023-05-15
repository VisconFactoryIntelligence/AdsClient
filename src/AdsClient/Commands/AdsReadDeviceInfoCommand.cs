using System.Collections.Generic;
using Viscon.Communication.Ads.CommandResponse;
using Viscon.Communication.Ads.Common;

namespace Viscon.Communication.Ads.Commands
{
    public class AdsReadDeviceInfoCommand : AdsCommand<AdsReadDeviceInfoCommandResponse>
    {

        public AdsReadDeviceInfoCommand()
            : base(AdsCommandId.ReadDeviceInfo)
        {

        }

        internal override IEnumerable<byte> GetBytes()
        {
            return new List<byte>();
        }
    }
}
