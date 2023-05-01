using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Ads.Client.CommandResponse;
using Ads.Client.Common;

namespace Ads.Client.Commands
{
    public class AdsReadDeviceInfoCommand : AdsCommand
    {

        public AdsReadDeviceInfoCommand()
            : base(AdsCommandId.ReadDeviceInfo)
        {

        }

        internal override IEnumerable<byte> GetBytes()
        {
            return new List<byte>();
        }

        public Task<AdsReadDeviceInfoCommandResponse> RunAsync(Ams ams, CancellationToken cancellationToken)
        {
            return RunAsync<AdsReadDeviceInfoCommandResponse>(ams, cancellationToken);
        }
    }
}
