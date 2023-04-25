using System;
using System.Collections.Generic;
using System.Linq;

namespace Ads.Client.Helpers
{
    internal class AmsSocketHelper
    {
        static AmsSocketHelper()
        {
            SocketList = new List<IAmsSocket>();
        }

        private static List<IAmsSocket> SocketList;

		public static IAmsSocket GetOrCreateAmsSocket<T>(string ipTarget, int portTarget) where T: IAmsSocket, new()
        {
            IAmsSocket amsSocket = SocketList.FirstOrDefault(s => (s.IpTarget == ipTarget) && (s.PortTarget == portTarget));
            if (amsSocket == null)
            {
				amsSocket = (T)Activator.CreateInstance (typeof(T), ipTarget, portTarget);
                SocketList.Add(amsSocket);
            }
            amsSocket.Subscribers++;
            return amsSocket;
        }

        public static void UnsibscribeAmsSocket(string ipTarget)
        {
            IAmsSocket amsSocket = SocketList.FirstOrDefault(s => s.IpTarget == ipTarget);
            if (amsSocket != null)
            {
                amsSocket.Subscribers--;
                if (amsSocket.Subscribers <= 0)
                {
                    SocketList.Remove(amsSocket);
                    amsSocket.Dispose();
                }
            }
        }
    }
}
