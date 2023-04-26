using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Ads.Client.Commands;
using Ads.Client.Common;
using Ads.Client.CommandResponse;
using Ads.Client.Helpers;
using System.Threading;

namespace Ads.Client
{

    public class Ams : IDisposable
    {

//        internal Ams(string ipTarget, int ipPortTarget = 48898)
//        {
//            this.IpTarget = ipTarget;
//            this.NotificationRequests = new List<AdsNotification>();
//            this.amsSocket = AmsSocketHelper.GetOrCreateAmsSocket(ipTarget, ipPortTarget);
//            this.amsSocket.OnReadCallBack += new AmsSocketResponseDelegate(ReadCallback);
//        }

        internal Ams(IAmsSocket amsSocket)
        {
			this.NotificationRequests = new List<AdsNotification>();
            this.amsSocket = amsSocket;
            this.amsSocket.OnReadCallBack += new AmsSocketResponseDelegate(ReadCallback);
        }

        /// <summary>
        /// Ams source port. Default of 32905
        /// </summary>
        private ushort amsPortSource = 32905;
        public ushort AmsPortSource
        {
            get { return amsPortSource; }
            set { amsPortSource = value; }
        }

        /// <summary>
        /// The localendpoint for the socket connection
        /// </summary>
        //public IPEndPoint LocalEndPoint
        //{
        //    get { return amsSocket.LocalEndPoint; }
        //    set { amsSocket.LocalEndPoint = value; }
        //}

        /// <summary>
        /// Specify a timeout time for the non async functions.
        /// The default is 5000 ms.
        /// -1 means wait forever
        /// </summary>
        private int commandTimeOut = 5000;
        public int CommandTimeOut
        {
            get { return commandTimeOut; }
            set { commandTimeOut = value; }
        }

        /// <summary>
        /// Run the notifications in the main thread.
        /// This only works when there is a SynchronizationContext available
        /// </summary>
        public bool RunNotificationsOnMainThread { get; set; }

		internal string IpTarget { get {return amsSocket.IpTarget; } }
        internal ushort AmsPortTarget { get; set; }
        internal AmsNetId AmsNetIdTarget { get; set; }
        internal AmsNetId AmsNetIdSource { get; set; }
        internal List<AdsNotification> NotificationRequests;

        private IAmsSocket amsSocket;
        public IAmsSocket AmsSocket { get { return amsSocket; } private set { amsSocket = value; } }
        private uint invokeId = 0;

        private object pendingResultsLock = new object();
        private List<AdsCommandResponse> pendingResults = new List<AdsCommandResponse>();

        private byte[] GetAmsMessage(AdsCommand adsCommand)
        {
            IEnumerable<byte> data = adsCommand.GetBytes();
            IEnumerable<byte> message = AmsNetIdTarget.Bytes;                       //AmsNetId Target
            message = message.Concat(BitConverter.GetBytes(AmsPortTarget));         //AMSPort Target
            message = message.Concat(AmsNetIdSource.Bytes);                         //AmsNetId Source
            message = message.Concat(BitConverter.GetBytes(AmsPortSource));         //AMSPort Source
            message = message.Concat(BitConverter.GetBytes(adsCommand.CommandId));  //Command Id
            message = message.Concat(BitConverter.GetBytes((ushort)0x0004));        //State Flags
            message = message.Concat(BitConverter.GetBytes((uint)data.Count()));    //Length
            message = message.Concat(BitConverter.GetBytes((uint)0));               //Error Code
            message = message.Concat(BitConverter.GetBytes(invokeId));              //Invoke Id
            message = message.Concat(data);                                         //Data

            //2 bytes reserved 0 + 4 bytes for length + the rest
            message = BitConverter.GetBytes((ushort)0).Concat(BitConverter.GetBytes((uint)message.Count())).Concat(message);

            return message.ToArray<byte>();
        }

        internal bool Connected
        {
            get { return (amsSocket.IsConnected); }
        }

        //This is different thread!
        private void ReadCallback(object sender, AmsSocketResponseArgs args)
        {
            if (args.Error != null)
            {
                // Lock the list.
                lock (pendingResultsLock)
                {
                    if (pendingResults.Count > 0)
                    {
                        foreach (var adsCommandResult in pendingResults.ToList())
                        {
                            adsCommandResult.UnknownException = args.Error;
                            adsCommandResult.Callback.Invoke(adsCommandResult);
                        }
                    }
                    else throw args.Error;
                }
            }

            if ((args.Response != null) && (args.Response.Length > 0) && (args.Error == null))
            {
                uint amsErrorCode = AmsHeaderHelper.GetErrorCode(args.Response);
                uint invokeId = AmsHeaderHelper.GetInvokeId(args.Response);
                bool isNotification = (AmsHeaderHelper.GetCommandId(args.Response) == AdsCommandId.DeviceNotification);

                if (AmsPortTarget != AmsHeaderHelper.GetAmsPortSource(args.Response)) return;
                if (!AmsNetIdTarget.Bytes.SequenceEqual(AmsHeaderHelper.GetAmsNetIdSource(args.Response))) return;

                //If notification then just start the callback
                if (isNotification && (OnNotification != null))
                {
                    var notifications = AdsNotification.GetNotifications(args.Response);
                    foreach (var notification in notifications)
                    {
                        var notificationRequest = NotificationRequests.FirstOrDefault(n => n.NotificationHandle == notification.NotificationHandle);
                        if (notificationRequest != null)
                        {
                            notificationRequest.ByteValue = notification.ByteValue;

                            if ((args.Context != null) && (RunNotificationsOnMainThread))
                                args.Context.Post(
                                    new SendOrPostCallback(delegate
                                    {
                                        OnNotification(null, new AdsNotificationArgs(notificationRequest));
                                    }), null);
                            else
                                OnNotification(null, new AdsNotificationArgs(notificationRequest));
                        }
                    }
                }

                //If not a notification then find the original command and call async callback
                if (!isNotification)
                {
                    AdsCommandResponse adsCommandResult = null;

                    // Do some locking before fiddling with the list.
                    lock (pendingResultsLock)
                    {
                        adsCommandResult = pendingResults.FirstOrDefault(r => r.CommandInvokeId == invokeId);
                        if (adsCommandResult == null)
                            return;
                            //throw new AdsException("I received a response from a request I didn't send?");

                        pendingResults.Remove(adsCommandResult);
                    }

                    if (amsErrorCode > 0)
                        adsCommandResult.AmsErrorCode = amsErrorCode;
                    else
                        adsCommandResult.SetResponse(args.Response);
                    adsCommandResult.Callback.Invoke(adsCommandResult);
                }
            }
        }

        private T EndGetResponse<T>(IAsyncResult ar) where T : AdsCommandResponse
        {
            return (T)ar;
        }

        private IAsyncResult BeginGetResponse<T>(AsyncCallback callback, object state) where T : AdsCommandResponse
        {
            uint invokeId = (state as uint?).Value;
            var result = Activator.CreateInstance<T>();
            result.CommandInvokeId = invokeId;
            result.Callback = callback;
            // Do some locking before fiddling with the list.
            lock (pendingResultsLock)
            {
                pendingResults.Add(result);
            }
            return result;
        }

        internal event AdsNotificationDelegate OnNotification;

        protected virtual void Dispose(bool managed)
        {
            AmsSocketHelper.UnsibscribeAmsSocket(IpTarget);
        }

        public void Dispose()
        {
            Dispose(true);
        }

        internal async Task<T> RunCommandAsync<T>(AdsCommand adsCommand) where T : AdsCommandResponse
        {
            await this.amsSocket.Async.ConnectAndListenAsync();
            invokeId++;
            byte[] message = GetAmsMessage(adsCommand);
            var responseTask = Task.Factory.FromAsync<T>(BeginGetResponse<T>, EndGetResponse<T>, invokeId);
            await amsSocket.Async.SendAsync(message);
            return await responseTask;
        }
    }
}
