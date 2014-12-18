using System;

namespace Ads.Client.Winsock
{
	public class AdsClient : Ads.Client.AdsClient
	{
		/// <summary>
		/// AdsClient Constructor
		/// </summary>
		/// <param name="ipTarget">IP of the target Ads device</param>
		/// <param name="amsNetIdSource">
		/// The AmsNetId of this device. You can choose something in the form of x.x.x.x.x.x 
		/// You have to define this ID in combination with the IP as a route on the target Ads device
		/// </param>
		/// <param name="amsNetIdTarget">The AmsNetId of the target Ads device</param>
		/// <param name="amsPortTarget">Ams port. Default 801</param>
		public AdsClient(string amsNetIdSource, string ipTarget, string amsNetIdTarget, ushort amsPortTarget = 801) 
			: base(amsNetIdSource,
				new AmsSocket(ipTarget),
				amsNetIdTarget,
				amsPortTarget)
        {
		
        }
	}
}

