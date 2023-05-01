using System;
using System.Collections.Generic;
using System.Linq;
using Ads.Client.Commands;

namespace Ads.Client.Helpers
{
    public static class AmsMessageBuilder
    {
        public static byte[] BuildAmsMessage(Ams ams, AdsCommand adsCommand, uint invokeId)
        {

            IEnumerable<byte> data = adsCommand.GetBytes();
            IEnumerable<byte> message = ams.AmsNetIdTarget.Bytes; //AmsNetId Target
            message = message.Concat(
                BitConverter.GetBytes(adsCommand.AmsPortTargetOverride ?? ams.AmsPortTarget)); //AMSPort Target
            message = message.Concat(ams.AmsNetIdSource.Bytes); //AmsNetId Source
            message = message.Concat(BitConverter.GetBytes(ams.AmsPortSource)); //AMSPort Source
            message = message.Concat(BitConverter.GetBytes(adsCommand.CommandId)); //Command Id
            message = message.Concat(BitConverter.GetBytes((ushort)0x0004)); //State Flags
            message = message.Concat(BitConverter.GetBytes((uint)data.Count())); //Length
            message = message.Concat(BitConverter.GetBytes((uint)0)); //Error Code
            message = message.Concat(BitConverter.GetBytes(invokeId)); //Invoke Id
            message = message.Concat(data); //Data

            //2 bytes reserved 0 + 4 bytes for length + the rest
            message = BitConverter.GetBytes((ushort)0).Concat(BitConverter.GetBytes((uint)message.Count()))
                .Concat(message);

            return message.ToArray<byte>();
        }
    }
}
