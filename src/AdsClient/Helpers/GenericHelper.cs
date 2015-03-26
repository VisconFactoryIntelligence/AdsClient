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
using Ads.Client.Common;

namespace Ads.Client.Helpers
{
    /// <summary>
    /// An enumeration of types known to ADS.
    /// </summary>
    internal enum AdsTypeEnum { Unknown, Bool, Byte, Char, Int16, Int32, Int64, UInt16, UInt32, UInt64, Single, Double, String, DateTime, Date, Time };

    /// <summary>
    /// A helper class for various conversions.
    /// </summary>
    internal static class GenericHelper
    {
        const string TypeNotImplementedError = "Type {0} must be implemented or has the AdsSerializable attribute!";

        /// <summary>
        /// Get length in bytes from a valuetype or AdsSerializable 
        /// </summary>
        /// <param name="defaultStringLength">The default string length.</param>
        /// <param name="arrayLength">The array length.</param>
        /// <returns>The length.</returns>
        public static uint GetByteLengthFromType<T>(uint defaultStringLength, uint arrayLength = 1) 
        {
            return GetByteLengthFromType(typeof(T), defaultStringLength, arrayLength);
        }

        /// <summary>
        /// Get length in bytes from a valuetype or AdsSerializable 
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="defaultStringLength">The default string length.</param>
        /// <param name="arrayLength">The array length.</param>
        /// <returns>The length</returns>
        public static uint GetByteLengthFromType(Type type, uint defaultStringLength, uint arrayLength = 1)
        {
            uint factor = 1;

            if (type.IsArray)
            {
                factor = arrayLength;
                type = type.GetElementType();
            }

            var adsType = GetEnumFromType(type);

            if (adsType != AdsTypeEnum.Unknown)
            {
                var length = GetByteLengthFromConvertible(adsType, defaultStringLength);
                if (length == 0)
                    throw new AdsException(String.Format("Function GetByteLengthFromType doesn't support this type ({0}) yet!", type.FullName));
                return length * factor;
            }
            else
            {
                if (type.IsAdsSerializable())
                {
                    uint length = 0;
                    var attributes = GetAdsAttributes(type, defaultStringLength);
                    foreach (var a in attributes)
                    {
                        length += a.ByteSize;
                    }
                    return length * factor;
                }
                else
                    throw new AdsException(String.Format(TypeNotImplementedError, type.FullName));
            }
        }

        /// <summary>
        ///     Convert byte array to valuetype or AdsSerializable
        /// </summary>
        /// <param name="type"></param>
        /// <param name="value"></param>
        /// <param name="defaultStringLength"></param>
        /// <returns></returns>
        public static object GetResultFromBytes(Type type, byte[] value, uint defaultStringLength, uint arrayLength = 1)
        {
            if (value == null)
                throw new ArgumentNullException("value", "GetResultFromBytes");

            if (type.IsArray)
            {
                var elementType = type.GetElementType();
                var elementSize = (int)(value.Length / arrayLength);
                var array = Array.CreateInstance(elementType, new int[] { (int)arrayLength });

                for (var i = 0; i < arrayLength; i++)
                {
                    var valarray = new byte[elementSize];
                    Array.Copy(value, i * elementSize, valarray, 0, elementSize);
                    var val = GetResultFromBytesInternal(elementType, valarray, defaultStringLength);
                    array.SetValue(val, new int[] { i });
                }

                return array;
            }
            else
                return GetResultFromBytesInternal(type, value, defaultStringLength);
        }

        private static object GetResultFromBytesInternal(Type type, byte[] value, uint defaultStringLength)
        {
            var adsType = GetEnumFromType(type);
            if (adsType != AdsTypeEnum.Unknown)
            {
                return ConvertBytesToType(adsType, value);
            }
            else
            {
                if (type.IsAdsSerializable())
                {
                    var adsObj = Activator.CreateInstance(type);
                    var attributes = GetAdsAttributes(type, defaultStringLength);

                    uint pos = 0;

                    foreach (var attr in attributes)
                    {
                        try
                        {
                            byte[] valarray = new byte[attr.ByteSize];
                            Array.Copy(value, (int)pos, valarray, 0, (int)attr.ByteSize);
                            var proptype = attr.Member.GetMemberType();
                            adsType = GetEnumFromType(proptype);
                            object val = ConvertBytesToType(adsType, valarray);
                            attr.Member.SetValue(adsObj, val);
                            pos += attr.ByteSize;
                        }
                        catch
                        { break; }
                    }

                    return adsObj;
                }
                else
                    throw new AdsException(String.Format(TypeNotImplementedError, type.FullName));
            }
        }


        /// <summary>
        /// Convert byte array to generic valuetype or AdsSerializable
        /// </summary>
        /// <typeparam name="T">ValueType or AdsSerializable</typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public static T GetResultFromBytes<T>(byte[] value, uint defaultStringLength, uint arrayLength = 1) 
        {
            Type type = typeof(T);
            var o = GetResultFromBytes(type, value, defaultStringLength, arrayLength);
            if (o == null)
                return default(T);
            else
                return (T)Convert.ChangeType(o, type);
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
            var type = typeof(T);

            var adsType = GetEnumFromType(type);
            if (adsType != AdsTypeEnum.Unknown)
            {
                varValueBytes = GetBytesFromConvertible(adsType, varValue, defaultStringLength).ToList();
            }
            else
            {
                if (type.IsAdsSerializable())
                {
                    var totallength = GetByteLengthFromType(type, defaultStringLength);
                    varValueBytes = new List<byte>((int)totallength);
                    var attributes = GetAdsAttributes(type, defaultStringLength);
                    
                    foreach (var attr in attributes)
                    {
						var memberType = attr.Member.GetMemberType();
                        adsType = GetEnumFromType(memberType);
                        var val = attr.Member.GetValue(varValue);
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

            if (varValueBytes == null)
                throw new AdsException("Function GetBytesFromType doesn't support this type yet!");

            return varValueBytes;
        }

        /// <summary>
        /// Gets all members marked with an <see cref="AdsAttribute"/>.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="defaultStringLength">The default string length.</param>
        /// <returns>An enumeration of attributes.</returns>
        public static IEnumerable<AdsAttribute> GetAdsAttributes(Type type, uint defaultStringLength)
        {
            var attributes = new List<AdsAttribute>();
            var members = type.GetPublicFieldsAndProperties();

            foreach (var member in members)
            {
                var attr = member.GetAdsAttribute();

                if (attr != null)
                {
                    attr.Member = member;
                    if (attr.ByteSize == 0)
                    {
                        var memberType = member.GetMemberType();
                        attr.ByteSize = GetByteLengthFromType(memberType, defaultStringLength, attr.ArraySize);
                    }
                    attributes.Add(attr);
                }
            }

            return attributes.OrderBy(a => a.Order);
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

        private static uint GetByteLengthFromConvertible(AdsTypeEnum adsType, uint defaultStringLength)
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
            try
            {
                switch (adsType)
                {
                    case AdsTypeEnum.Bool: return BitConverter.ToBoolean(value, 0);
                    case AdsTypeEnum.Byte: return value[0];
                    case AdsTypeEnum.Int16: return BitConverter.ToInt16(value, 0);
                    case AdsTypeEnum.Int32: return BitConverter.ToInt32(value, 0);
                    case AdsTypeEnum.Int64: return BitConverter.ToInt64(value, 0);
                    case AdsTypeEnum.UInt16: return BitConverter.ToUInt16(value, 0);
                    case AdsTypeEnum.UInt32: return BitConverter.ToUInt32(value, 0);
                    case AdsTypeEnum.UInt64: return BitConverter.ToUInt64(value, 0);
                    case AdsTypeEnum.Single: return BitConverter.ToSingle(value, 0);
                    case AdsTypeEnum.Double: return BitConverter.ToDouble(value, 0);
                    case AdsTypeEnum.String: return ByteArrayHelper.ByteArrayToString(value);
                    case AdsTypeEnum.DateTime: return ByteArrayHelper.ByteArrayToDateTime(value);
                    case AdsTypeEnum.Date: return new Date(BitConverter.ToUInt32(value, 0));
                    case AdsTypeEnum.Time: return new Time(BitConverter.ToUInt32(value, 0));
                    default:
                        return null;
                }
            }
            catch
            {
                return null;
            }
        }

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
