using System;
using Ads.Client;
using NUnit.Framework;
using Ads.Client.Helpers;
using System.Linq;

namespace Ads.Client.Test
{
	public class AmsSocketSyncTest : AmsSocketBaseSync
	{
		#region implemented abstract members of AmsSocketBaseSync

		protected override void Connect ()
		{
			var amsSocket = (AmsSocketTest)this.amsSocket;
			amsSocket.ConnectedAsync = false;
			amsSocket.Connected = true;
		}

		public override void Send (byte[] message)
		{
			var amsSocket = (AmsSocketTest)this.amsSocket;
			Assert.That(message.SequenceEqual(amsSocket.SendMessage));
			amsSocket.callback(amsSocket.ReceiveMessage, null);
		}

		public override void Receive (byte[] message)
		{
			var amsSocket = (AmsSocketTest)this.amsSocket;
			int length = amsSocket.ReceiveMessage.Length-6;
			Array.Copy(amsSocket.ReceiveMessage, 6, message, 0, length);
		}

		#endregion

		public AmsSocketSyncTest(AmsSocketBase amsSocket) : base(amsSocket)
		{
		}
	}
}

