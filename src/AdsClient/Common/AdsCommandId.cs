using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ads.Client.Common
{
    public static class AdsCommandId 
    {
        public const ushort ReadDeviceInfo             = 1;
        public const ushort Read                       = 2;
        public const ushort Write                      = 3;
        public const ushort ReadState                  = 4;
        public const ushort WriteControl               = 5;
        public const ushort AddDeviceNotification      = 6;
        public const ushort DeleteDeviceNotification   = 7;
        public const ushort DeviceNotification         = 8;
        public const ushort ReadWrite                  = 9;
    }
}
