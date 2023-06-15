using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Viscon.Communication.Ads.Commands;
using Viscon.Communication.Ads.Common;
using Viscon.Communication.Ads.Helpers;

namespace Viscon.Communication.Ads.Special
{
    /// <summary>
    /// Special functions. (functionality not documented by Beckhoff)
    /// </summary>
    public class AdsSpecial
    {
        private Ams ams;

        internal AdsSpecial(Ams ams)
        {
            this.ams = ams;
        }

        /// <summary>
        /// Get an xml description of the plc
        /// You can use XDocument.Parse(xml).ToString() to make the xml more readable
        /// </summary>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        /// <returns></returns>
        public async Task<string> GetTargetDescAsync(CancellationToken cancellationToken = default)
        {
            AdsReadCommand adsCommand = new AdsReadCommand(0x000002bc, 0x00000001, 4) { AmsPortTargetOverride = 10000 };
            var result = await adsCommand.RunAsync(ams, cancellationToken).ConfigureAwait(false);
            uint length = BitConverter.ToUInt32(result.Data, 0);
            adsCommand = new AdsReadCommand(0x000002bc, 0x00000001, length) { AmsPortTargetOverride = 10000 };
            result = await adsCommand.RunAsync(ams, cancellationToken).ConfigureAwait(false);
            string xml = ByteArrayHelper.ByteArrayToString(result.Data);
            return xml;
        }

        /// <summary>
        /// Get the current routes
        /// </summary>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        /// <returns></returns>
        public async Task<IList<string>> GetCurrentRoutesAsync(CancellationToken cancellationToken = default)
        {
            bool ok = true;
            uint index = 0;
            var routes = new List<string>();

            while (ok)
            {
                try
                {
                    AdsReadCommand adsCommand =
                        new AdsReadCommand(0x00000323, index++, 0x0800) { AmsPortTargetOverride = 10000 };
                    var result = await adsCommand.RunAsync(ams, cancellationToken).ConfigureAwait(false);
                    int length = result.Data.Length - 44;
                    byte[] routeBytes = new byte[length];
                    Array.Copy(result.Data, 44, routeBytes, 0, length);
                    string routeString = ByteArrayHelper.ByteArrayToString(routeBytes);
                    int stringLlength = routeString.Length + 1;
                    Array.Copy(routeBytes, stringLlength, routeBytes, 0, length - stringLlength);
                    routeString += " " + ByteArrayHelper.ByteArrayToString(routeBytes);
                    routes.Add(routeString);
                }
                catch (AdsException ex)
                {
                    if ((uint) ex.ErrorCode == 1814) ok = false;
                    else throw;
                }
            }
            return routes;
        }
    }
}
