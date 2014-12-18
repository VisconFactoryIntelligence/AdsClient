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
using Ads.Client.Helpers;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Ads.Client.Common;
using System.Diagnostics;

namespace Ads.Client
{
    public abstract class AmsSocketBaseSync : IAmsSocketSync
    {
		protected AmsSocketBase amsSocket;
        public AmsSocketBaseSync(AmsSocketBase amsSocket)
        {
			this.amsSocket = amsSocket;
        }

        protected int maxPacketSize = 1514;

        public void ConnectAndListen()
        {
            if (!amsSocket.IsConnected)
            {
                amsSocket.ConnectedAsync = false;
                Connect();
                amsSocket.Listen();
            }
        }

        protected abstract void Connect();
        //Debug.WriteLine("Sending bytes: " + ByteArrayHelper.ByteArrayToTestString(message));
        public abstract void Send(byte[] message);
        public abstract void Receive(byte[] message);
    }
}