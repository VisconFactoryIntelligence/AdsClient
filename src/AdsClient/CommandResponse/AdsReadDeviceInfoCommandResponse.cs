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
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ads.Client.Common;
using Ads.Client.Helpers;

namespace Ads.Client.CommandResponse
{
    public class AdsReadDeviceInfoCommandResponse : AdsCommandResponse
    {
        private AdsDeviceInfo adsDeviceInfo;
        public AdsDeviceInfo AdsDeviceInfo
        {
            get { return adsDeviceInfo; }
        }
        
        protected override void AdsResponseIsChanged()
        {
            adsDeviceInfo = new AdsDeviceInfo();
            adsDeviceInfo.MajorVersion = this.AdsResponse[4];
            adsDeviceInfo.MinorVersion = this.AdsResponse[5];
            adsDeviceInfo.VersionBuild = BitConverter.ToUInt16(this.AdsResponse, 6);
            var deviceNameArray = new byte[16];
            Array.Copy(this.AdsResponse, 8, deviceNameArray, 0, 16);
            adsDeviceInfo.DeviceName = ByteArrayHelper.ByteArrayToString(deviceNameArray);
        }
    }
}
