using System;

namespace Ads.Client
{
    [System.AttributeUsage(System.AttributeTargets.Class)]
    public class AdsSerializableAttribute : Attribute
    {
        public AdsSerializableAttribute()
        {

        }
    }
}
