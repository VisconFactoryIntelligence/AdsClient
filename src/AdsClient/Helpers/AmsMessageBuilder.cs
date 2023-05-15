using System;
using System.Collections.Generic;
using System.Linq;
using Viscon.Communication.Ads.CommandResponse;
using Viscon.Communication.Ads.Commands;
using Viscon.Communication.Ads.Common;
using Viscon.Communication.Ads.Internal;

namespace Viscon.Communication.Ads.Helpers
{
    internal static class AmsMessageBuilder
    {
        public static byte[] BuildAmsMessage<TResponse>(Ams ams, AdsCommand<TResponse> adsCommand, uint invokeId) where TResponse : AdsCommandResponse, new()
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

        public static void WriteHeader(Span<byte> buffer, Ams ams, AdsCommandEnum command, ushort amsPortTarget, uint invokeId, int messageLength)
        {
            var offset = WireFormatting.WriteUInt16(ref buffer.GetStart(), default);
            offset += WireFormatting.WriteUInt32(ref buffer.GetOffset(offset),
                AmsHeaderHelper.AmsHeaderSize + messageLength);

            ams.AmsNetIdTarget.Bytes.ToArray().CopyTo(buffer.Slice(offset));
            offset += 6;

            offset += WireFormatting.WriteUInt16(ref buffer.GetOffset(offset), amsPortTarget);

            ams.AmsNetIdSource.Bytes.ToArray().CopyTo(buffer.Slice(offset));
            offset += 6;

            offset += WireFormatting.WriteUInt16(ref buffer.GetOffset(offset), ams.AmsPortSource);
            offset += WireFormatting.WriteUInt16(ref buffer.GetOffset(offset), command);
            offset += WireFormatting.WriteUInt16(ref buffer.GetOffset(offset), (ushort) 0x0004);
            offset += WireFormatting.WriteUInt32(ref buffer.GetOffset(offset), (uint) messageLength);
            offset += WireFormatting.WriteUInt32(ref buffer.GetOffset(offset), (uint) 0);
            offset += WireFormatting.WriteUInt32(ref buffer.GetOffset(offset), invokeId);
        }
    }
}
