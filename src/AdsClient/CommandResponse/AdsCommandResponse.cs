using System;
using System.Collections.Generic;
using Ads.Client.Common;

namespace Ads.Client.CommandResponse
{
    public class AdsCommandResponse : IAsyncResult
    {
        public AdsCommandResponse()
        {
            isCompleted = false;
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

            isCompleted = true;
        }

        protected virtual void AdsResponseIsChanged()
        {
        }

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

        public Exception UnknownException { get; set; }
        
        protected virtual uint GetErrorCode()
        {
            return BitConverter.ToUInt32(AdsResponse, 0);
        }

        public AsyncCallback Callback { get; set; }

        public object AsyncState
        {
            get { return CommandInvokeId; }
        }

        public System.Threading.WaitHandle AsyncWaitHandle
        {
            get { return null; }
        }

        public bool CompletedSynchronously
        {
            get { return false; }
        }

        private bool isCompleted;
        public bool IsCompleted
        {
            get { return isCompleted; }
        }
    }
}
