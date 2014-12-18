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
