using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
