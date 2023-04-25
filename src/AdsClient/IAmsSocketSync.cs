namespace Ads.Client
{
    public interface IAmsSocketSync
    {
        void ConnectAndListen();
        void Send(byte[] message);
        void Receive(byte[] message);
    }
}
