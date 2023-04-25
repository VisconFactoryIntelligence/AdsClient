using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ads.Client.Helpers;

namespace Ads.Client.Common
{
    public delegate void AdsNotificationDelegate(object sender, AdsNotificationArgs e);

    public class AdsNotification
    {
        public AdsNotification()
        {
        }

        public uint NotificationHandle { get; set; }
        public byte[] ByteValue { get; set; }
        public Type TypeOfValue { get; set; }
        public object UserData { get; set; }

        public object Value { get { return ByteArrayHelper.ByteArrayToTypeValue(ByteValue, TypeOfValue); } }


        public override string ToString()
        {
            return String.Format("NotificationHandle: {0} Value: {1}", NotificationHandle, BitConverter.ToString(ByteValue));
        }

        //Move this to a helper class?
        internal static List<AdsNotification> GetNotifications(byte[] adsresponseInclAmsHeader)
        {
            var notifications = new List<AdsNotification>();

            int pos = 32;
            uint stamps = BitConverter.ToUInt32(adsresponseInclAmsHeader, pos + 4);
            pos += 8;

            for (int i = 0; i < stamps; i++)
            {
                uint samples = BitConverter.ToUInt32(adsresponseInclAmsHeader, pos + 8);
                pos += 12;

                for (int j = 0; j < samples; j++)
                {
                    var notification = new AdsNotification();
                    notification.NotificationHandle = BitConverter.ToUInt32(adsresponseInclAmsHeader, pos);
                    uint length = BitConverter.ToUInt32(adsresponseInclAmsHeader, pos + 4);
                    pos += 8;
                    notification.ByteValue = new byte[length];
                    Array.Copy(adsresponseInclAmsHeader, pos, notification.ByteValue, 0, (int)length);
                    notifications.Add(notification);
                    pos += (int)length;
                }
            }

            return notifications;
        }


    }
}
