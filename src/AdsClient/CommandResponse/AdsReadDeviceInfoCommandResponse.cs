using System;
using Viscon.Communication.Ads.Common;
using Viscon.Communication.Ads.Helpers;

namespace Viscon.Communication.Ads.CommandResponse
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
