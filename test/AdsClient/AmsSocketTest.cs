using System;
using System.Threading;

namespace Ads.Client.Test
{
	class AmsSocketTest : AmsSocketBase
    {

		public byte[] SendMessage { get; set; }
		public byte[] ReceiveMessage { get; set; }
        public Action<byte[], SynchronizationContext> callback;

		public AmsSocketTest() : base("")
		{
			this.Async = new AmsSocketAsyncTest(this);
            this.Connected = false;
		}

		#region implemented abstract members of AmsSocketBase

		public override void CreateSocket ()
		{

		}

        public override void ListenForHeader(byte[] amsheader, Action<byte[], SynchronizationContext> lambda)
		{
			this.callback = lambda;
		}

		protected override void CloseConnection ()
		{

		}

		public bool Connected {get;set;}
		public override bool IsConnected { get {return Connected; }  }

		#endregion


		/*

        public bool Connected { get; set; }
        public bool? ConnectedAsync { get; set; }
        public IPEndPoint LocalEndPoint { get; set; }
        public byte[] SendMessage { get; set; }
        public byte[] ReceiveMessage { get; set; }
        public bool Verbose { get; set; }
        public event AmsSocketResponseDelegate OnReadCallBack;

        public void ConnectAndListen()
        {
            ConnectedAsync = false;
        }

        public void Send(byte[] message)
        {
            Trace.WriteLine("message: " + ByteArrayHelper.ByteArrayToTestString(message));
            Trace.WriteLine("sendmsg: " + ByteArrayHelper.ByteArrayToTestString(SendMessage));
            Assert.IsTrue(message.SequenceEqual(SendMessage));
            int length = ReceiveMessage.Length-6;
            byte[] response = new byte[length];
            Array.Copy(ReceiveMessage, 6, response, 0, length);
            var callbackArgs = new AmsSocketResponseArgs()
            {
                Response = response
            };
            OnReadCallBack(this, callbackArgs);
        }

        public async Task ConnectAndListenAsync()
        {
            ConnectedAsync = true;
        }

        public async Task<bool> SendAsync(byte[] message)
        {
            Send(message);
            return await Task.FromResult(true);
        }


        public string IpTarget
        {
            get { throw new NotImplementedException(); }
        }

        public int PortTarget
        {
            get { throw new NotImplementedException(); }
        }

        public int Subscribers
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
        */
    }
}
