using System;
using System.Collections.Generic;
using System.Linq;
using Ads.Client.CommandResponse;
using Ads.Client.Common;

namespace Ads.Client.Commands
{
    public class AdsWriteCommand : AdsCommand<AdsWriteCommandResponse>
    {
        private IEnumerable<byte> varValue;

        public AdsWriteCommand(uint indexGroup, uint indexOffset, IEnumerable<byte> varValue)
            : base(AdsCommandId.Write)
        {
            this.varValue = varValue;
            this.indexGroup = indexGroup;
            this.indexOffset = indexOffset;
        }

        private uint indexOffset;
        private uint indexGroup;

        internal override IEnumerable<byte> GetBytes()
        {
            IEnumerable<byte> data = BitConverter.GetBytes(indexGroup);
            data = data.Concat(BitConverter.GetBytes(indexOffset));
            data = data.Concat(BitConverter.GetBytes((uint)varValue.Count()));
            data = data.Concat(varValue);

            return data;
        }
    }
}
