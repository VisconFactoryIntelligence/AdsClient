using System;

namespace Ads.Client.Common
{
    public class Date
    {
        private uint intValue;
        private DateTime dateValue;

        public Date(uint value)
        {
            this.intValue = value;
            this.dateValue = new DateTime(1970, 1, 1).AddSeconds(intValue);
        }

        public Date(DateTime value)
        {
            this.dateValue = value;
            this.intValue = (uint)((value - new DateTime(1970, 1, 1)).TotalSeconds);
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
            return dateValue;
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
            return Convert.ToString(dateValue, provider);
        }

        public override string ToString()
        {
            return Convert.ToString(dateValue);
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
