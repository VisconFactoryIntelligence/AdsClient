using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
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

        public Task<AdsAddDeviceNotificationCommandResponse> RunAsync(Ams ams, CancellationToken cancellationToken)
        {
            return RunAsync<AdsAddDeviceNotificationCommandResponse>(ams, cancellationToken);
        }
    }
}
