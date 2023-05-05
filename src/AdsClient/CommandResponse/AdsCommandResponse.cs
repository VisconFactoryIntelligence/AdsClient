using System;

namespace Ads.Client.CommandResponse
{
    public class AdsCommandResponse
    {
        public AdsCommandResponse()
        {
        }

        internal byte[] AdsResponse { get; set; }

        internal uint CommandInvokeId { get; set; }

        internal void SetResponse(byte[] adsresponseInclAmsHeader)
        {
            //32 amsheader + data
            int datalength = BitConverter.ToInt32(adsresponseInclAmsHeader, 20);

            this.AdsResponse = new byte[datalength];
            Array.Copy(adsresponseInclAmsHeader, 32, this.AdsResponse, 0, datalength);

            errorCode = GetErrorCode();

            AdsResponseIsChanged();
        }

        protected virtual void AdsResponseIsChanged()
        {
        }

        public void ProcessResponse() => AdsResponseIsChanged();

        private uint errorCode;
        public uint AdsErrorCode
        {
            get { return errorCode; }
        }

        internal uint AmsErrorCode
        {
            get { return errorCode; }
            set { errorCode = value; }
        }

        protected virtual uint GetErrorCode()
        {
            return BitConverter.ToUInt32(AdsResponse, 0);
        }
    }
}
