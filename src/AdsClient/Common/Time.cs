using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ads.Client.Common
{
    public class Time
    {
        private uint intValue;
        private TimeSpan timeSpanValue;

        public Time(uint value)
        {
            this.intValue = value;
            this.timeSpanValue = TimeSpan.FromMilliseconds(intValue);
        }

        public Time(TimeSpan value)
        {
            this.timeSpanValue = value;
            this.intValue = (uint)value.TotalMilliseconds;
        }

        public TimeSpan ToTimeSpan()
        {
            return this.timeSpanValue;
        }

        public bool ToBoolean(IFormatProvider provider)
        {
            return Convert.ToBoolean(intValue);
        }

        public byte ToByte(IFormatProvider provider)
        {
            return Convert.ToByte(intValue);
        }

        public char ToChar(IFormatProvider provider)
        {
            return Convert.ToChar(intValue);
        }

        public DateTime ToDateTime(IFormatProvider provider)
        {
            return new DateTime(1900, 1, 1).AddMilliseconds(intValue);
        }

        public decimal ToDecimal(IFormatProvider provider)
        {
            return Convert.ToDecimal(intValue); ;
        }

        public double ToDouble(IFormatProvider provider)
        {
            return Convert.ToDouble(intValue);
        }

        public short ToInt16(IFormatProvider provider)
        {
            return Convert.ToInt16(intValue);
        }

        public int ToInt32(IFormatProvider provider)
        {
            return Convert.ToInt32(intValue);
        }

        public long ToInt64(IFormatProvider provider)
        {
            return Convert.ToInt64(intValue);
        }

        public sbyte ToSByte(IFormatProvider provider)
        {
            return Convert.ToSByte(intValue);
        }

        public float ToSingle(IFormatProvider provider)
        {
            return Convert.ToSingle(intValue);
        }

        public string ToString(IFormatProvider provider)
        {
            return Convert.ToString(timeSpanValue, provider);
        }

        public override string ToString()
        {
            return Convert.ToString(timeSpanValue);
        }

        public object ToType(Type conversionType, IFormatProvider provider)
        {
            return Convert.ChangeType(intValue, conversionType, provider);
        }

        public ushort ToUInt16(IFormatProvider provider)
        {
            return Convert.ToUInt16(intValue);
        }

        public uint ToUInt32(IFormatProvider provider)
        {
            return Convert.ToUInt32(intValue);
        }

        public ulong ToUInt64(IFormatProvider provider)
        {
            return Convert.ToUInt64(intValue);
        }
    }
}
