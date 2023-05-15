using System;
using System.Collections.Generic;
using System.Linq;
using Viscon.Communication.Ads.CommandResponse;
using Viscon.Communication.Ads.Common;

namespace Viscon.Communication.Ads.Commands
{
    public class AdsWriteReadCommand : AdsCommand<AdsWriteReadCommandResponse>
    {
        private uint readLength;
        private byte[] value;

        public AdsWriteReadCommand(uint indexGroup, uint indexOffset, byte[] value, uint readLength)
            : base(AdsCommandId.ReadWrite)
        {
            this.readLength = readLength;
            this.value = value;
            this.indexGroup = indexGroup;
            this.indexOffset = indexOffset;
        }

        private uint indexOffset;
        private uint indexGroup;

        internal override IEnumerable<byte> GetBytes()
        {
            IEnumerable<byte> data = BitConverter.GetBytes(indexGroup);
            data = data.Concat(BitConverter.GetBytes(indexOffset));
            data = data.Concat(BitConverter.GetBytes(readLength));
            data = data.Concat(BitConverter.GetBytes((uint)value.Length));
            data = data.Concat(value);
            return data;
        }
    }
}
