using System;
using System.Collections.Generic;
using System.Linq;
using Ads.Client.CommandResponse;
using Ads.Client.Common;

namespace Ads.Client.Commands
{
    public class AdsDeleteDeviceNotificationCommand : AdsCommand<AdsDeleteDeviceNotificationCommandResponse>
    {
        public AdsDeleteDeviceNotificationCommand(uint notificationHandle)
            : base(AdsCommandId.DeleteDeviceNotification)
        {

            this.notificationHandle = notificationHandle;
        }

        private uint notificationHandle;

        internal override IEnumerable<byte> GetBytes()
        {
            IEnumerable<byte> data = BitConverter.GetBytes(notificationHandle);
            return data;
        }

        protected override void RunAfter(Ams ams)
        {
            var notification = ams.NotificationRequests.FirstOrDefault(n => n.NotificationHandle == notificationHandle);
            if (notification != null)
                ams.NotificationRequests.Remove(notification);
        }
    }
}
