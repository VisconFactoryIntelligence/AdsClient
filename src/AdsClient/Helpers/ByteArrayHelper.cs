using System;
using System.Linq;
using System.Text;
using Viscon.Communication.Ads.Common;

namespace Viscon.Communication.Ads.Helpers
{
    public class ByteArrayHelper
    {
        /// <summary>
        /// Convert byte array to value defined by TypeOfValue
        /// </summary>
        /// <param name="value">The byte array that needs conversion</param>
        /// <param name="TypeOfValue">The type of the result</param>
        /// <returns></returns>
        public static object ByteArrayToTypeValue(byte[] value, Type typeOfValue)
        {
            //if (Type.Equals(TypeOfValue, null)) return null;
            //if (Type.Equals(TypeOfValue, typeof(byte[]))) return value;
            //var method = typeof(GenericHelper).GetMethod("GetResultFromBytes");
            //var generic = method.MakeGenericMethod(TypeOfValue);
            //return generic.Invoke(null, new object[] { value });
            return GenericHelper.GetResultFromBytes(typeOfValue, value, 0);
        }

        public static string ByteArrayToString(byte[] value)
        {
            if (value == null)
                return "";
            else
                return string.Concat(value.Select(b => b <= 0x7f ? (char)b : '?').TakeWhile(b => b > 0)) ?? "";
        }

        public static string ByteArrayToTestString(byte[] value)
        {
            StringBuilder sb = new StringBuilder();
            foreach (byte val in value)
            {
                if (sb.Length > 0) sb.Append(',');
                sb.Append(val.ToString());
            }
            return sb.ToString();
        }

        public static DateTime ByteArrayToDateTime(byte[] value)
        {
            var seconds = BitConverter.ToUInt32(value, 0);
            var val = new Date(seconds);
            return val.ToDateTime(null);
        }
    }
}
