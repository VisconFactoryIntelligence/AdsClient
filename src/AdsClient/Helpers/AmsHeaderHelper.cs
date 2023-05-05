using System;

namespace Ads.Client.Helpers
{
    internal class AmsHeaderHelper
    {
        public const int AmsTcpHeaderSize = 6;
        public const int AmsHeaderSize = 32;
        public const int AmsDataLengthOffset = 20;

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
