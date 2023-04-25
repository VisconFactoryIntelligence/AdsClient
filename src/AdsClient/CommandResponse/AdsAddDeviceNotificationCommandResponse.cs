using System;

namespace Ads.Client.CommandResponse
{
    public class AdsAddDeviceNotificationCommandResponse : AdsCommandResponse
    {
        private uint notificationHandle;
        public uint NotificationHandle
        {
            get { return notificationHandle; }
        }


        protected override void AdsResponseIsChanged()
        {
            notificationHandle = BitConverter.ToUInt32(this.AdsResponse, 4);
        }
    }
}
