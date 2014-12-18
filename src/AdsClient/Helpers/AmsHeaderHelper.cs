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

namespace Ads.Client.Helpers
{
    internal class AmsHeaderHelper
    {
        public const int AmsTcpHeaderSize = 6;

        public static uint GetResponseLength(byte[] tcpHeader)
        {
            return BitConverter.ToUInt32(tcpHeader, 2);
        }

        public static byte[] GetAmsNetIdTarget(byte[] amsHeader)
        {
            byte[] id = new byte[6];
            Array.Copy(amsHeader, 0, id, 0, 6);
            return id;
        }

        public static ushort GetAmsPortTarget(byte[] amsHeader)
        {
            return BitConverter.ToUInt16(amsHeader, 14);
        }

        public static byte[] GetAmsNetIdSource(byte[] amsHeader)
        {
            byte[] id = new byte[6];
            Array.Copy(amsHeader, 8, id, 0, 6);
            return id;
        }

        public static ushort GetAmsPortSource(byte[] amsHeader)
        {
            return BitConverter.ToUInt16(amsHeader, 14);
        }

        public static ushort GetCommandId(byte[] amsHeader)
        {
            return BitConverter.ToUInt16(amsHeader, 16);
        }

        public static uint GetErrorCode(byte[] amsHeader)
        {
            return BitConverter.ToUInt32(amsHeader, 24);
        }

        public static uint GetInvokeId(byte[] amsHeader)
        {
            return BitConverter.ToUInt32(amsHeader, 28);
        }

    }
}
