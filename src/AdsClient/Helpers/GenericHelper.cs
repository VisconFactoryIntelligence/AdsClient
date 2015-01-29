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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ads.Client.Common;
using System.Reflection;

namespace Ads.Client.Helpers
{

    internal enum AdsTypeEnum { Unknown, Bool, Byte, Char, Int16, Int32, Int64, UInt16, UInt32, UInt64, Single, Double, String, DateTime, Date, Time };

    internal static class GenericHelper
    {
        /// <summary>
        /// Get length in bytes from a valuetype or AdsSerializable 
        /// </summary>
        /// <typeparam name="T">ValueType or AdsSerializable</typeparam>
        /// <returns></returns>
        public static uint GetByteLengthFromType<T>(uint defaultStringLength) 
        {
            var adsType =  GetEnumFromType(typeof(T));
            if (adsType != AdsTypeEnum.Unknown)
            {
                var length = GetByteLengthFromType(adsType, defaultStringLength);
                if (length == 0) throw new AdsException(String.Format("Function GetByteLengthFromType doesn't support this type ({0}) yet!", typeof(T).FullName));
                return length;
            }
            else
            {
                if (AdsAttribute.IsAdsSerializable<T>())
                {
                    uint length = 0;
                    List<AdsAttribute> attributes = GetAdsAttributes(typeof(T), defaultStringLength);
                    foreach (var a in attributes)
                    {
                        length += a.ByteSize;
                    }
                    return length;
                }
                else throw new AdsException(String.Format(TypeNotImplementedError, typeof(T).FullName));
            }
        }

        /// <summary>
        ///     Convert byte array to valuetype or AdsSerializable
        /// </summary>
        /// <param name="type"></param>
        /// <param name="value"></param>
        /// <param name="defaultStringLength"></param>
        /// <returns></returns>
        public static object GetResultFromBytes(Type type, byte[] value, uint defaultStringLength)
        {
            if (value == null)
                throw new ArgumentNullException("value", "GetResultFromBytes");

            var adsType = GetEnumFromType(type);
            if (adsType != AdsTypeEnum.Unknown)
            {
                object v = ConvertBytesToType(adsType, value);
                if (v == null)
                    throw new AdsException("Function GetResultFromBytes doesn't support this type yet!");
                return v;
            }
            else
            {
                if (AdsAttribute.IsAdsSerializable(type))
                {
                    var adsObj = Activator.CreateInstance(type);
                    List<AdsAttribute> attributes = GetAdsAttributes(type, defaultStringLength);

                    uint pos = 0;
                    foreach (var attr in attributes)
                    {
                        byte[] valarray = new byte[attr.ByteSize];
                        Array.Copy(value, (int)pos, valarray, 0, (int)attr.ByteSize);
						var proptype = attr.GetPropery().FieldType;
                        adsType = GetEnumFromType(proptype);
                        object val = ConvertBytesToType(adsType, valarray);
                        attr.GetPropery().SetValue(adsObj, val);
                        pos += attr.ByteSize;
                    }

                    return adsObj;
                }
                else throw new AdsException(String.Format(TypeNotImplementedError, type.FullName));
            }
        }


        /// <summary>
        /// Convert byte array to generic valuetype or AdsSerializable
        /// </summary>
        /// <typeparam name="T">ValueType or AdsSerializable</typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public static T GetResultFromBytes<T>(byte[] value, uint defaultStringLength) 
        {
            var o = GetResultFromBytes(typeof(T), value, defaultStringLength);
            return (T)Convert.ChangeType(o, typeof(T));
        }

        /// <summary>
        /// Convert ValueType or AdsSerializable to byte array
        /// </summary>
        /// <typeparam name="T">ValueType or AdsSerializable</typeparam>
        /// <param name="varValue2">The value that needs conversion</param>
        /// <returns></returns>
        public static IEnumerable<byte> GetBytesFromType<T>(T varValue, uint defaultStringLength) 
        {
            List<byte> varValueBytes = null;

            var adsType = GetEnumFromType(typeof(T));
            if (adsType != AdsTypeEnum.Unknown)
            {
                varValueBytes = GetBytesFromConvertible(adsType, varValue, defaultStringLength).ToList();
            }
            else
            {
                if (AdsAttribute.IsAdsSerializable<T>())
                {
                    var totallength = GetByteLengthFromType<T>(defaultStringLength);
                    varValueBytes = new List<byte>((int)totallength);
                    List<AdsAttribute> attributes = GetAdsAttributes(typeof(T), defaultStringLength);
                    foreach (var attr in attributes)
                    {
						var type = attr.GetPropery().FieldType;
                        adsType = GetEnumFromType(type);
                        var val = attr.GetPropery().GetValue(varValue);
                        var bytes = GetBytesFromConvertible(adsType, val, defaultStringLength);
                        if (bytes.Count() != attr.ByteSize)
                        {
                            if (bytes.Count() > attr.ByteSize)
                                bytes = bytes.Take((int)attr.ByteSize).ToList();
                        }
                        varValueBytes.AddRange(bytes);
                    }
                }
            }

            if (varValueBytes == null) throw new AdsException("Function GetBytesFromType doesn't support this type yet!");

            return varValueBytes;
        }

        private static IEnumerable<byte> GetBytesFromConvertible(AdsTypeEnum adsType, object value, uint defaultStringLength)
        {
            IEnumerable<byte> varValueBytes = null;

            if (value == null) return null;

            switch (adsType)
            {  
                case AdsTypeEnum.Bool: varValueBytes = BitConverter.GetBytes((bool)value); break;
                case AdsTypeEnum.Byte: varValueBytes = new byte[] { (byte)value }; break;
                case AdsTypeEnum.Char: varValueBytes = BitConverter.GetBytes((char)value); break;
                case AdsTypeEnum.Int16: varValueBytes = BitConverter.GetBytes((Int16)value); break;
                case AdsTypeEnum.Int32: varValueBytes = BitConverter.GetBytes((Int32)value); break;
                case AdsTypeEnum.Int64: varValueBytes = BitConverter.GetBytes((Int64)value); break;
                case AdsTypeEnum.UInt16: varValueBytes = BitConverter.GetBytes((UInt16)value); break;
                case AdsTypeEnum.UInt32: varValueBytes = BitConverter.GetBytes((UInt32)value); break;
                case AdsTypeEnum.UInt64: varValueBytes = BitConverter.GetBytes((UInt64)value); break;
                case AdsTypeEnum.Single: varValueBytes = BitConverter.GetBytes((Single)value); break;
                case AdsTypeEnum.Double: varValueBytes = BitConverter.GetBytes((Double)value); break;
                case AdsTypeEnum.DateTime: varValueBytes = BitConverter.GetBytes((Int32)value); break;
                case AdsTypeEnum.String: varValueBytes = value.ToString().ToAdsBytes(); break;
                case AdsTypeEnum.Date: varValueBytes = BitConverter.GetBytes((Int32)value); break;
                case AdsTypeEnum.Time: varValueBytes = BitConverter.GetBytes((Int32)value); break;
            }

            return varValueBytes;
        }

        private static uint GetByteLengthFromType(AdsTypeEnum adsType, uint defaultStringLength)
        {
            uint length = 0;

            switch (adsType)
            {
                case AdsTypeEnum.Bool: length = 1; break;
                case AdsTypeEnum.Byte: length = 1; break;
                case AdsTypeEnum.Int16: length = 2; break;
                case AdsTypeEnum.Int32: length = 4; break;
                case AdsTypeEnum.Int64: length = 8; break;
                case AdsTypeEnum.UInt16: length = 2; break;
                case AdsTypeEnum.UInt32: length = 4; break;
                case AdsTypeEnum.UInt64: length = 8; break;
                case AdsTypeEnum.Single: length = 4; break;
                case AdsTypeEnum.Double: length = 8; break;
                case AdsTypeEnum.String: length = defaultStringLength; break;
                case AdsTypeEnum.DateTime: length = 4; break;
                case AdsTypeEnum.Date: length = 4; break;
                case AdsTypeEnum.Time: length = 4; break;
            }

            return length;
        }

        private static object ConvertBytesToType(AdsTypeEnum adsType, byte[] value)
        {
            object v = null;
            
            switch (adsType)
            {
                case AdsTypeEnum.Bool: v = value[0]; break;
                case AdsTypeEnum.Byte: v = value[0]; break;
                case AdsTypeEnum.Int16: v = BitConverter.ToInt16(value, 0); break;
                case AdsTypeEnum.Int32: v = BitConverter.ToInt32(value, 0); break;
                case AdsTypeEnum.Int64: v = BitConverter.ToInt64(value, 0); break;
                case AdsTypeEnum.UInt16: v = BitConverter.ToUInt16(value, 0); break;
                case AdsTypeEnum.UInt32: v = BitConverter.ToUInt32(value, 0); break;
                case AdsTypeEnum.UInt64: v = BitConverter.ToUInt64(value, 0); break;
                case AdsTypeEnum.Single: v = BitConverter.ToSingle(value, 0); break;
                case AdsTypeEnum.Double: v = BitConverter.ToDouble(value, 0); break;
                case AdsTypeEnum.String: v = ByteArrayHelper.ByteArrayToString(value); break;
                case AdsTypeEnum.DateTime: v = ByteArrayHelper.ByteArrayToDateTime(value); break;
                case AdsTypeEnum.Date: v = new Date(BitConverter.ToUInt32(value, 0)); break;
                case AdsTypeEnum.Time: v = new Time(BitConverter.ToUInt32(value, 0)); break;
            }

            return (v);
        }

        public static List<AdsAttribute> GetAdsAttributes(Type type, uint defaultStringLength)
        {
            List<AdsAttribute> attributes = new List<AdsAttribute>();

			var props = type.GetRuntimeFields().Where(f => f.IsPublic);
			//var props = type.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            foreach (var p in props)
            {
                AdsAttribute attr = AdsAttribute.GetAdsAttribute(p);
                if (attr != null)
                {
                    attr.SetProperty(p);
                    if (attr.ByteSize == 0)
                    {
						var adsType = GetEnumFromType(p.FieldType);
                        attr.ByteSize = GetByteLengthFromType(adsType, defaultStringLength);
                    }
                    attributes.Add(attr);
                }
            }

            attributes = attributes.OrderBy(a => a.Order).ToList();

            return attributes;
        }

        const string TypeNotImplementedError = "Type {0} must be implemented or has the AdsSerializable attribute!";

        public static AdsTypeEnum GetEnumFromType(Type type)
        {
            if (Type.Equals(type, typeof(bool))) return AdsTypeEnum.Bool;
            if (Type.Equals(type, typeof(byte))) return AdsTypeEnum.Byte;
            if (Type.Equals(type, typeof(Int16))) return AdsTypeEnum.Int16;
            if (Type.Equals(type, typeof(Int32))) return AdsTypeEnum.Int32;
            if (Type.Equals(type, typeof(Int64))) return AdsTypeEnum.Int64;
            if (Type.Equals(type, typeof(UInt16))) return AdsTypeEnum.UInt16;
            if (Type.Equals(type, typeof(UInt32))) return AdsTypeEnum.UInt32;
            if (Type.Equals(type, typeof(UInt64))) return AdsTypeEnum.UInt64;
            if (Type.Equals(type, typeof(Single))) return AdsTypeEnum.Single;
            if (Type.Equals(type, typeof(Double))) return AdsTypeEnum.Double;
            if (Type.Equals(type, typeof(String))) return AdsTypeEnum.String;
            if (Type.Equals(type, typeof(DateTime))) return AdsTypeEnum.DateTime;
            if (Type.Equals(type, typeof(Date))) return AdsTypeEnum.Date;
            if (Type.Equals(type, typeof(Time))) return AdsTypeEnum.Time;

            return AdsTypeEnum.Unknown;
        }
    }
}
