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
using Ads.Client.CommandResponse;
using Ads.Client.Common;

namespace Ads.Client.Commands
{
    public class AdsAddDeviceNotificationCommand : AdsCommand
    {
        public AdsAddDeviceNotificationCommand(uint indexGroup, uint indexOffset, uint readLength, AdsTransmissionMode transmissionMode)
            : base(AdsCommandId.AddDeviceNotification)
        {
            this.readLength = readLength;
            this.indexGroup = indexGroup;
            this.indexOffset = indexOffset;
            this.transmissionMode = transmissionMode;
            this.notification = new AdsNotification();
       
        }

        private AdsNotification notification;
        public AdsNotification Notification
        {
            get { return notification; }
        }

        public object UserData
        {
            get { return notification.UserData; }
            set { notification.UserData = value; }
        }

        public Type TypeOfValue
        {
            get { return notification.TypeOfValue; }
            set { notification.TypeOfValue = value; }
        }

        private AdsTransmissionMode transmissionMode;
        private uint readLength;
        private uint indexOffset;
        private uint indexGroup;

        public uint MaxDelay { get; set; }

        private uint cycleTime;
        public uint CycleTime
        {
            get { return cycleTime/10000; }
            set { cycleTime = value*10000; }
        }

        internal override IEnumerable<byte> GetBytes()
        {
            IEnumerable<byte> data = BitConverter.GetBytes(indexGroup);
            data = data.Concat(BitConverter.GetBytes(indexOffset));
            data = data.Concat(BitConverter.GetBytes(readLength));
            data = data.Concat(BitConverter.GetBytes((uint)transmissionMode));
            data = data.Concat(BitConverter.GetBytes(MaxDelay));
            data = data.Concat(BitConverter.GetBytes(cycleTime));
            data = data.Concat(BitConverter.GetBytes((UInt64)0));
            data = data.Concat(BitConverter.GetBytes((UInt64)0));
            return data;
        }

        protected override void RunBefore(Ams ams)
        {
            ams.NotificationRequests.Add(Notification);
        }

        #if !NO_ASYNC
        public Task<AdsAddDeviceNotificationCommandResponse> RunAsync(Ams ams)
        {
            return RunAsync<AdsAddDeviceNotificationCommandResponse>(ams);
        }
        #endif

        #if !SILVERLIGHT
        public AdsAddDeviceNotificationCommandResponse Run(Ams ams)
        {
            return Run<AdsAddDeviceNotificationCommandResponse>(ams);
        }
        #endif

    }
}
