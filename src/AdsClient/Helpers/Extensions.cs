using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ads.Client.Common;

namespace Ads.Client
{
    public static class Extensions
    {
        public static byte[] ToAdsBytes(this String value)
        {
            byte[] result = new byte[value.Length + 1];

            for (int index = 0; index < value.Length; ++index)
            {
                char ch = value[index];
                if (ch <= 0x7f) 
                    result[index] = (byte)ch;
                else 
                    result[index] = (byte)'?';
            }
            result[result.Length-1] = 0;

            return result;
        }

        public static Time ToTime(this TimeSpan value)
        {
            return new Time(value);
        }

        public static Date ToDate(this DateTime value)
        {
            return new Date(value);
        }
    }
}
