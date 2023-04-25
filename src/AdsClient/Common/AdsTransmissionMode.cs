using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ads.Client.Common
{
    public enum AdsTransmissionMode
    {
        Cyclic   = 3,    //The AdsSyncNotification-Event is fired cyclically
        OnChange = 4     //The AdsSyncNotification-Event is fired when the data changes
    }
}
