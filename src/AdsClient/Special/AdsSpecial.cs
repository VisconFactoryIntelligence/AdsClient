/*  Copyright (c) 2011 Inando
 
    Permission is hereby granted, free of charge, to any person obtaining a 
    copy of this software and associated documentation files (the "Software"), 
    to deal in the Software without restriction, including without limitation 
    the rights to use, copy, modify, merge, publish, distribute, sublicense, 
    and/or sell copies of the Software, and to permit persons to whom the 
    Software is furnished to do so, subject to the following conditions:

    The above copyright notice and this permission notice shall be included 
    in all copies or substantial portions of the Software.

    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, 
    EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF 
    MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. 
    IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, 
    DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, 
    TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE 
    OR THE USE OR OTHER DEALINGS IN THE SOFTWARE. 
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ads.Client.Commands;
using Ads.Client.Common;
using Ads.Client.Helpers;

namespace Ads.Client.Special
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

        #if !SILVERLIGHT

        /// <summary>
        /// Get an xml description of the plc
        /// You can use XDocument.Parse(xml).ToString() to make the xml more readable
        /// </summary>
        /// <returns></returns>
		public string GetTargetDesc()
        {
			var amsSpecial = new Ams(ams.AmsSocket);
            amsSpecial.AmsNetIdSource = ams.AmsNetIdSource;
            amsSpecial.AmsNetIdTarget = ams.AmsNetIdTarget;
            amsSpecial.AmsPortTarget = 10000;
            AdsReadCommand adsCommand = new AdsReadCommand(0x000002bc, 0x00000001, 4);
            var result = adsCommand.Run(amsSpecial);
            uint length = BitConverter.ToUInt32(result.Data, 0);
            adsCommand = new AdsReadCommand(0x000002bc, 0x00000001, length);
            result = adsCommand.Run(amsSpecial);
            string xml = ByteArrayHelper.ByteArrayToString(result.Data);
            return xml;
        }

        /// <summary>
        /// Get the current routes
        /// </summary>
        /// <returns></returns>
        public IList<string> GetCurrentRoutes()
        {
			var amsSpecial = new Ams(ams.AmsSocket);
            amsSpecial.AmsNetIdSource = ams.AmsNetIdSource;
            amsSpecial.AmsNetIdTarget = ams.AmsNetIdTarget;
            amsSpecial.AmsPortTarget = 10000;
            bool ok = true;
            uint index = 0;
            var routes = new List<string>();

            while (ok)
            {
                try
                {
                    AdsReadCommand adsCommand = new AdsReadCommand(0x00000323, index++, 0x0800);
                    var result = adsCommand.Run(amsSpecial);
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
                    if (ex.ErrorCode == 1814) ok = false;
                    else throw;
                }
            }
            return routes;
        }

        public IList<AdsSymhandle> GetSymbols()
        {
            AdsReadCommand adsCommand = new AdsReadCommand(0x0000f00f, 0x000000, 0x30);
            var result = adsCommand.Run(this.ams);

            uint readLength = (uint)BitConverter.ToInt32(result.Data, 4);
            adsCommand = new AdsReadCommand(0x0000f00b, 0x000000, readLength);
            result = adsCommand.Run(this.ams);

            return GetSymbolsFromBytes(result.Data);
        }

        #endif

        #if !NO_ASYNC

        public async Task<IList<AdsSymhandle>> GetSymbolsAsync()
        {
            AdsReadCommand adsCommand = new AdsReadCommand(0x0000f00f, 0x000000, 0x30);
            var result = await adsCommand.RunAsync(ams);

            uint readLength = (uint)BitConverter.ToInt32(result.Data, 4);
            adsCommand = new AdsReadCommand(0x0000f00b, 0x000000, readLength);
            result = await adsCommand.RunAsync(ams);

            var symbols = GetSymbolsFromBytes(result.Data);

            return symbols;
        }

        #endif

        private IList<AdsSymhandle> GetSymbolsFromBytes(byte[] data)
        {
            int symbolStartPos = 0;
            var symbols = new List<AdsSymhandle>();

            while (symbolStartPos < data.Length)
            {
                int pos = symbolStartPos;
                AdsSymhandle symbol = new AdsSymhandle();

                int readLength = (int)BitConverter.ToUInt32(data, pos);
                symbolStartPos += readLength;

                symbol.IndexGroup = BitConverter.ToUInt32(data, pos + 4);
                symbol.IndexOffset = BitConverter.ToUInt32(data, pos + 8);
                //symbol.Size = BitConverter.ToUInt32(result.Data, pos + 12);
                //symbol.Type = BitConverter.ToUInt32(result.Data, pos + 16);  ADST_... type constants

                //BitConverter.ToUInt32(result.Data, pos + 20); ???

                UInt16 nameLength = BitConverter.ToUInt16(data, pos + 24);
                nameLength++;
                UInt16 typeLength = BitConverter.ToUInt16(data, pos + 26);
                typeLength++;
                UInt16 commentLength = BitConverter.ToUInt16(data, pos + 28);
                commentLength++;

                byte[] nameBytes = new byte[nameLength];
                Array.Copy(data, pos + 30, nameBytes, 0, nameLength);
                pos += 30 + nameLength;
                symbol.VarName = ByteArrayHelper.ByteArrayToString(nameBytes);

                byte[] typeBytes = new byte[typeLength];
                Array.Copy(data, pos, typeBytes, 0, typeLength);
                pos += typeLength;
                symbol.TypeName = ByteArrayHelper.ByteArrayToString(typeBytes);

                byte[] commentBytes = new byte[commentLength];
                Array.Copy(data, pos, commentBytes, 0, commentLength);
                pos += commentLength;
                symbol.Comment = ByteArrayHelper.ByteArrayToString(commentBytes);

                symbols.Add(symbol);
            }

            return symbols;

        }


    }
}
