using System;

namespace Ads.Client.CommandResponse
{
    public class AdsWriteReadCommandResponse : AdsCommandResponse
    {
        private byte[] data;
        public byte[] Data
        {
            get { return data; }
        }


        protected override void AdsResponseIsChanged()
        {
            uint dataLength = BitConverter.ToUInt16(this.AdsResponse, 4);
            data = new byte[dataLength];
            Array.Copy(AdsResponse, 8, data, 0, (int)dataLength);
        }
    }
}
