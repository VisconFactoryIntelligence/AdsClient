namespace Ads.Client
{
    public abstract class AmsSocketBaseSync : IAmsSocketSync
    {
		protected AmsSocketBase amsSocket;
        public AmsSocketBaseSync(AmsSocketBase amsSocket)
        {
			this.amsSocket = amsSocket;
        }

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