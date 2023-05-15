namespace Viscon.Communication.Ads.Common
{
    public interface IAdsConnectionSettings
    {
        string Name { get; set; }
        string AmsNetIdSource { get; set; }
		//string IpTarget { get; set; }
		IAmsSocket AmsSocket { get; set; }
        string AmsNetIdTarget { get; set; }
        ushort AmsPortTarget { get; set; }
    }
}
