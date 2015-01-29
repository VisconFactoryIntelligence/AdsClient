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

using Ads.Client.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ads.Client.Helpers
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
