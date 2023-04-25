using System;

namespace Ads.Client.Common
{
    public class AdsNotificationArgs : EventArgs
    {
        public AdsNotificationArgs(AdsNotification notification)
        {
            this.Notification = notification;
        }

        public AdsNotification Notification { get; set; }
    }
}
