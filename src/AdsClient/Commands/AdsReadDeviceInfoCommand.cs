using System.Collections.Generic;
using Ads.Client.CommandResponse;
using Ads.Client.Common;

namespace Ads.Client.Commands
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
