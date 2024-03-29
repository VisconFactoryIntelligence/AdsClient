﻿using System;

namespace Viscon.Communication.Ads.CommandResponse
{
    public class AdsReadCommandResponse : AdsCommandResponse
    {
        private byte[] data;
        public byte[] Data
        {
            get { return data; }
        }


        protected override void AdsResponseIsChanged()
        {
            uint dataLength = BitConverter.ToUInt32(this.AdsResponse, 4);
            data = new byte[dataLength];
            Array.Copy(AdsResponse, 8, data, 0, (int)dataLength);
        }
    }
}
