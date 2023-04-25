using System;
using System.Collections.Generic;

namespace Ads.Client.Common
{
    public class AmsNetId
    {
        public IList<byte> Bytes { get; set; }

        internal AmsNetId(string amsNetId)
        {
            ParseString(amsNetId);
        }

        private void ParseString(string amsNetId)
        {
            Bytes = new List<byte>();

            string[] byteStrings = amsNetId.Split('.');
            foreach (string byteString in byteStrings)
            {
                byte b = Convert.ToByte(byteString);
                Bytes.Add(b);
            }
        }
    }
}
