using System;

namespace Ads.Client.Common
{
    public class AdsSymbol
    {
        public string VarName { get; set; }
        public UInt32 IndexGroup { get; set; }
        public UInt32 IndexOffset { get; set; }
        public string TypeName { get; set; }
        public string Comment { get; set; }
        public uint Size { get; set; }

        public override string ToString()
        {
            return $"{VarName} {TypeName} {Comment}";
        }
    }
}
