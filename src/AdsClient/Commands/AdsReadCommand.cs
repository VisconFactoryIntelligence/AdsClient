using System;
using System.Collections.Generic;
using System.Linq;
using Viscon.Communication.Ads.CommandResponse;
using Viscon.Communication.Ads.Common;

namespace Viscon.Communication.Ads.Commands
{
    internal class AdsReadCommand : AdsCommand<AdsReadCommandResponse>
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
    }
}
