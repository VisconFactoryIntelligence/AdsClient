namespace Viscon.Communication.Ads.Common
{
    public class AdsConnectionSettings : IAdsConnectionSettings
    {
        public AdsConnectionSettings ()
        {
            AmsPortTarget = 801;
        }

        public string Name { get; set; }
        public string AmsNetIdSource { get; set; }
		//public string IpTarget { get; set; }
		public IAmsSocket AmsSocket {get;set;}
        public string AmsNetIdTarget { get; set; }
        public  ushort AmsPortTarget { get; set; }

    }
}