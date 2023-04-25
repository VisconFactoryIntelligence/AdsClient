using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Ads.Client.CommandResponse;
using Ads.Client.Common;

namespace Ads.Client.Commands
{
    internal class AdsReadCommand : AdsCommand
    {
        public AdsReadCommand(uint indexGroup, uint indexOffset, uint readLength)
            : base(AdsCommandId.Read)
        {
            this.readLength = readLength;
            this.indexGroup = indexGroup;
            this.indexOffset = indexOffset;
        }

        private uint readLength;
        private uint indexOffset;
        private uint indexGroup;

        internal override IEnumerable<byte> GetBytes()
        {
            IEnumerable<byte> data = BitConverter.GetBytes(indexGroup);
            data = data.Concat(BitConverter.GetBytes(indexOffset));
            data = data.Concat(BitConverter.GetBytes(readLength));

            return data;
        }

#if !NO_ASYNC
        public Task<AdsReadCommandResponse> RunAsync(Ams ams)
        {
            return RunAsync<AdsReadCommandResponse>(ams);
        }
#endif

#if !SILVERLIGHT
        public AdsReadCommandResponse Run(Ams ams)
        {
            return Run<AdsReadCommandResponse>(ams);
        }
#endif
    }
}
