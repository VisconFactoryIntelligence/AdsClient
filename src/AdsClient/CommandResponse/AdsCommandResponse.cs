/*  Copyright (c) 2011 Inando
 
    Permission is hereby granted, free of charge, to any person obtaining a 
    copy of this software and associated documentation files (the "Software"), 
    to deal in the Software without restriction, including without limitation 
    the rights to use, copy, modify, merge, publish, distribute, sublicense, 
    and/or sell copies of the Software, and to permit persons to whom the 
    Software is furnished to do so, subject to the following conditions:

    The above copyright notice and this permission notice shall be included 
    in all copies or substantial portions of the Software.

    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, 
    EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF 
    MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. 
    IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, 
    DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, 
    TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE 
    OR THE USE OR OTHER DEALINGS IN THE SOFTWARE. 
 */
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
