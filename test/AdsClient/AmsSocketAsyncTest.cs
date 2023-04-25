using System;
using NUnit.Framework;
using System.Threading.Tasks;
using System.Linq;

namespace Ads.Client.Test
{
	public class AmsSocketAsyncTest : AmsSocketBaseAsync
	{
		#region implemented abstract members of AmsSocketBaseAsync

		protected override System.Threading.Tasks.Task ConnectAsync ()
		{
			var amsSocket = (AmsSocketTest)this.amsSocket;
			amsSocket.ConnectedAsync = true;
			amsSocket.Connected = true;
			return Task.FromResult(true);
		}

		public override System.Threading.Tasks.Task SendAsync (byte[] message)
		{
			var amsSocket = (AmsSocketTest)this.amsSocket;
			Assert.That(message.SequenceEqual(amsSocket.SendMessage));
			amsSocket.callback(amsSocket.ReceiveMessage, null);
			return Task.FromResult(true);
		}

		public override System.Threading.Tasks.Task ReceiveAsync (byte[] message)
		{
			var amsSocket = (AmsSocketTest)this.amsSocket;
			int length = amsSocket.ReceiveMessage.Length-6;
			Array.Copy(amsSocket.ReceiveMessage, 6, message, 0, length);
			return Task.FromResult(true);
		}

		#endregion

		public AmsSocketAsyncTest(AmsSocketBase amsSocket):base(amsSocket)
		{
		}
	}
}

