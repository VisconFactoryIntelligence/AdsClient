namespace Ads.Client.Common
{
    public enum AdsTransmissionMode
    {
        Cyclic   = 3,    //The AdsSyncNotification-Event is fired cyclically
        OnChange = 4     //The AdsSyncNotification-Event is fired when the data changes
    }
}
