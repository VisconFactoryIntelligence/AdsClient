using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Ads.Client.Commands;
using Ads.Client.Common;
using Ads.Client.Conversation;
using Ads.Client.Helpers;
using Ads.Client.Special;

namespace Ads.Client
{
    public class AdsClient : IDisposable
    {

        /// <summary>
        /// AdsClient Constructor
        /// This can be used if you wan't to use your own IAmsSocket implementation.
        /// </summary>
        /// <param name="amsSocket">Your own IAmsSocket implementation</param>
        /// <param name="amsNetIdSource">
        /// The AmsNetId of this device. You can choose something in the form of x.x.x.x.x.x
        /// You have to define this ID in combination with the IP as a route on the target Ads device
        /// </param>
        /// <param name="amsNetIdTarget">The AmsNetId of the target Ads device</param>
        /// <param name="amsPortTarget">Ams port. Default 801</param>
        public AdsClient(string amsNetIdSource, IAmsSocket amsSocket, string amsNetIdTarget, ushort amsPortTarget = 801)
        {
            ams = new Ams(amsSocket);
            ams.AmsNetIdSource = new AmsNetId(amsNetIdSource);
            ams.AmsNetIdTarget = new AmsNetId(amsNetIdTarget);
            ams.AmsPortTarget = amsPortTarget;
        }

        /// <summary>
        /// AdsClient Constructor
        /// </summary>
        /// <param name="ipTarget">IP of the target Ads device</param>
        /// <param name="amsNetIdSource">
        /// The AmsNetId of this device. You can choose something in the form of x.x.x.x.x.x
        /// You have to define this ID in combination with the IP as a route on the target Ads device
        /// </param>
        /// <param name="amsNetIdTarget">The AmsNetId of the target Ads device</param>
        /// <param name="amsPortTarget">Ams port. Default 801</param>
        public AdsClient(string amsNetIdSource, string ipTarget, string amsNetIdTarget, ushort amsPortTarget = 801) :
            this(amsNetIdSource, new AmsSocket(ipTarget), amsNetIdTarget, amsPortTarget)
        {
        }

        /// <summary>
        /// AdsClient Constructor
        /// </summary>
        /// <param name="settings">The connection settings</param>
        public AdsClient(IAdsConnectionSettings settings)
        {
			ams = new Ams(settings.AmsSocket);
            ams.AmsNetIdSource = new AmsNetId(settings.AmsNetIdSource);
            ams.AmsNetIdTarget = new AmsNetId(settings.AmsNetIdTarget);
            ams.AmsPortTarget = settings.AmsPortTarget;
            this.Name = settings.Name;
        }

        private Ams ams;
        public Ams Ams { get { return ams; } }

        /// <summary>
        /// An internal list of handles and its associated lock object.
        /// </summary>
        private Dictionary<string, uint> activeSymhandles = new Dictionary<string, uint>();
        private object activeSymhandlesLock = new object();

        /// <summary>
        /// Clears the dictionary of handles.
        /// </summary>
        public void ClearSymhandleDictionary()
        {
            lock (activeSymhandlesLock)
                activeSymhandles.Clear();
        }

        /// <summary>
        /// Special functions. (functionality not documented by Beckhoff)
        /// </summary>
        private AdsSpecial special;
        public AdsSpecial Special
        {
            get
            {
                if (special == null)
                {
                    special = new AdsSpecial(ams);
                }
                return special;
            }
        }

        /// <summary>
        /// When using the generic string method, this is the default string length
        /// </summary>
        private uint defaultStringLenght = 81;
        public uint DefaultStringLength
        {
            get { return defaultStringLenght; }
            set { defaultStringLenght = value; }
        }

        public string Name { get; set; }

        /// <summary>
        /// This event is called when a subscribed notification is raised
        /// </summary>
        public event AdsNotificationDelegate OnNotification
        {
            add { ams.OnNotification += value; }
            remove { ams.OnNotification -= value; }
        }

        protected virtual void Dispose(bool managed)
        {
            if (ams != null) ams.Dispose();
        }

        public void Dispose()
        {
            Dispose(true);
        }

        #region Async Methods

        /// <summary>
        /// Get a handle from a variable name
        /// </summary>
        /// <param name="varName">A twincat variable like ".XXX"</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        /// <returns>The handle</returns>
        public async Task<uint> GetSymhandleByNameAsync(string varName, CancellationToken cancellationToken = default)
        {
            // Check, if the handle is already present.
            lock (activeSymhandlesLock)
            {
                if (activeSymhandles.ContainsKey(varName))
                    return activeSymhandles[varName];
            }

            // It was not retrieved before - get it from the control.
            var adsCommand = new AdsWriteReadCommand(0x0000F003, 0x00000000, varName.ToAdsBytes(), 4);
            var result = await adsCommand.RunAsync(this.ams, cancellationToken);
            if (result == null || result.Data == null || result.Data.Length < 4)
                return 0;

            var handle = BitConverter.ToUInt32(result.Data, 0);

            // Now, try to add it.
            lock (activeSymhandlesLock)
            {
                if (!activeSymhandles.ContainsKey(varName))
                    activeSymhandles.Add(varName, handle);

                return handle;
            }
        }

        /// <summary>
        /// Release symhandle
        /// </summary>
        /// <param name="symhandle">The handle returned by GetSymhandleByName</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        /// <returns>An awaitable task.</returns>
        public Task ReleaseSymhandleAsync(uint symhandle, CancellationToken cancellationToken = default)
        {
            // Perform a reverse-lookup at the dictionary.
            lock (activeSymhandlesLock)
            {
                var key = "";

                foreach (var kvp in activeSymhandles)
                {
                    if (kvp.Value != symhandle)
                        continue;
                    key = kvp.Key;
                    break;
                }

                activeSymhandles.Remove(key);
            }

            return ReleaseSymhandleAsyncInternal(symhandle, cancellationToken);
        }

        private Task ReleaseSymhandleAsyncInternal(uint symhandle, CancellationToken cancellationToken)
        {
            // Run the release command.
            var adsCommand = new AdsWriteCommand(0x0000F006, 0x00000000, BitConverter.GetBytes(symhandle));
            return adsCommand.RunAsync(this.ams, cancellationToken);
        }

        /// <summary>
        /// Read the value from the handle returned by GetSymhandleByNameAsync
        /// </summary>
        /// <param name="varHandle">The handle returned by GetSymhandleByNameAsync</param>
        /// <param name="readLength"></param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        /// <returns>A byte[] with the value of the twincat variable</returns>
        public async Task<byte[]> ReadBytesAsync(uint varHandle, uint readLength, CancellationToken cancellationToken = default)
        {
            AdsReadCommand adsCommand = new AdsReadCommand(0x0000F005, varHandle, readLength);
            var result = await adsCommand.RunAsync(this.ams, cancellationToken);
            return result.Data;
        }

        public async Task<byte[]> ReadBytesI_Async(uint offset, uint readLength, CancellationToken cancellationToken = default)
        {
            AdsReadCommand adsCommand = new AdsReadCommand(0x0000F020, offset, readLength);
            var result = await adsCommand.RunAsync(this.ams, cancellationToken);
            return result.Data;
        }

        public async Task<byte[]> ReadBytesQ_Async(uint offset, uint readLength, CancellationToken cancellationToken = default)
        {
            AdsReadCommand adsCommand = new AdsReadCommand(0x0000F030, offset, readLength);
            var result = await adsCommand.RunAsync(this.ams, cancellationToken);
            return result.Data;
        }

        public async Task<AdsReadMultipleCommand.ReadMultiResult[]> ReadMultipleAsync(AdsSymbol[] symbols, CancellationToken cancellationToken = default)
        {
            return await new AdsReadMultipleCommand(symbols).RunAsync(ams, cancellationToken);
        }

        /// <summary>
        /// Read the value from the handle returned by GetSymhandleByNameAsync.
        /// </summary>
        /// <typeparam name="T">A type like byte, ushort, uint depending on the length of the twincat variable</typeparam>
        /// <param name="varHandle">The handle returned by GetSymhandleByNameAsync</param>
        /// <param name="arrayLength">An optional array length.</param>
        /// <param name="adsObj">An optional existing object.</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        /// <returns>The value of the twincat variable</returns>
        public async Task<T> ReadAsync<T>(uint varHandle, uint arrayLength = 1,
            object adsObj = null, CancellationToken cancellationToken = default)
        {
            var length = GenericHelper.GetByteLengthFromType<T>(DefaultStringLength, arrayLength);
            var value = await ReadBytesAsync(varHandle, length, cancellationToken);

            if (value != null)
                return GenericHelper.GetResultFromBytes<T>(value, DefaultStringLength, arrayLength, adsObj);
            else
                return default(T);
        }

        /// <summary>
        /// Add a noticiation when a variable changes or cyclic after a defined time in ms
        /// </summary>
        /// <param name="varHandle">The handle returned by GetSymhandleByNameAsync</param>
        /// <param name="length">The length of the data that must be send by the notification</param>
        /// <param name="transmissionMode">On change or cyclic</param>
        /// <param name="cycleTime">The cyclic time in ms. If used with OnChange, then the value is send once after this time in ms</param>
        /// <param name="userData">A custom object that can be used in the callback</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        /// <returns>The notification handle</returns>
        public Task<uint> AddNotificationAsync(uint varHandle, uint length, AdsTransmissionMode transmissionMode,
            uint cycleTime, object userData, CancellationToken cancellationToken = default)
        {
            return AddNotificationAsync(varHandle, length, transmissionMode, cycleTime, userData, typeof(byte[]), cancellationToken);
        }

        /// <summary>
        /// Add a noticiation when a variable changes or cyclic after a defined time in ms
        /// </summary>
        /// <param name="varHandle">The handle returned by GetSymhandleByNameAsync</param>
        /// <param name="length">The length of the data that must be send by the notification</param>
        /// <param name="transmissionMode">On change or cyclic</param>
        /// <param name="cycleTime">The cyclic time in ms. If used with OnChange, then the value is send once after this time in ms</param>
        /// <param name="userData">A custom object that can be used in the callback</param>
        /// <param name="typeOfValue">The type of the returned notification value</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        /// <returns>The notification handle</returns>
        public async Task<uint> AddNotificationAsync(uint varHandle, uint length, AdsTransmissionMode transmissionMode,
            uint cycleTime, object userData, Type typeOfValue, CancellationToken cancellationToken = default)
        {
            var adsCommand = new AdsAddDeviceNotificationCommand(0x0000F005, varHandle, length, transmissionMode);
            adsCommand.CycleTime = cycleTime;
            adsCommand.UserData = userData;
            adsCommand.TypeOfValue = typeOfValue;
            var result = await adsCommand.RunAsync(this.ams, cancellationToken);
            adsCommand.Notification.NotificationHandle = result.NotificationHandle;
            return result.NotificationHandle; ;
        }

        /// <summary>
        /// Add a noticiation when a variable changes or cyclic after a defined time in ms
        /// </summary>
        /// <typeparam name="T">Type for defining the length of the data that must be send by the notification</typeparam>
        /// <param name="varHandle">The handle returned by GetSymhandleByNameAsync</param>
        /// <param name="transmissionMode">On change or cyclic</param>
        /// <param name="cycleTime">The cyclic time in ms. If used with OnChange, then the value is send once after this time in ms</param>
        /// <param name="userData">A custom object that can be used in the callback</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        /// <returns></returns>
        public Task<uint> AddNotificationAsync<T>(uint varHandle, AdsTransmissionMode transmissionMode, uint cycleTime,
            object userData, CancellationToken cancellationToken = default)
        {
            uint length = GenericHelper.GetByteLengthFromType<T>(DefaultStringLength);
            return AddNotificationAsync(varHandle, length, transmissionMode, cycleTime, userData, typeof(T), cancellationToken);
        }

        /// <summary>
        /// Delete a previously registerd notification
        /// </summary>
        /// <param name="notificationHandle">The handle returned by AddNotification(Async)</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        /// <returns></returns>
        public Task DeleteNotificationAsync(uint notificationHandle, CancellationToken cancellationToken = default)
        {
            var adsCommand = new AdsDeleteDeviceNotificationCommand(notificationHandle);
            return adsCommand.RunAsync(this.ams, cancellationToken);
        }

        /// <summary>
        /// Write the value to the handle returned by GetSymhandleByNameAsync
        /// </summary>
        /// <param name="varHandle">The handle returned by GetSymhandleByNameAsync</param>
        /// <param name="varValue">The byte[] value to be sent</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        public Task WriteBytesAsync(uint varHandle, IEnumerable<byte> varValue, CancellationToken cancellationToken = default)
        {
            AdsWriteCommand adsCommand = new AdsWriteCommand(0x0000F005, varHandle, varValue);
            return adsCommand.RunAsync(this.ams, cancellationToken);
        }

        /// <summary>
        /// Write the value to the handle returned by GetSymhandleByNameAsync
        /// </summary>
        /// <typeparam name="T">A type like byte, ushort, uint depending on the length of the twincat variable</typeparam>
        /// <param name="varHandle">The handle returned by GetSymhandleByNameAsync</param>
        /// <param name="varValue">The value to be sent</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        public Task WriteAsync<T>(uint varHandle, T varValue, CancellationToken cancellationToken = default)
        {
            IEnumerable<byte> varValueBytes = GenericHelper.GetBytesFromType<T>(varValue, defaultStringLenght);
            return this.WriteBytesAsync(varHandle, varValueBytes, cancellationToken);
        }

        /// <summary>
        /// Get some information of the ADS device (version, name)
        /// </summary>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        /// <returns></returns>
        public async Task<AdsDeviceInfo> ReadDeviceInfoAsync(CancellationToken cancellationToken = default)
        {
            AdsReadDeviceInfoCommand adsCommand = new AdsReadDeviceInfoCommand();
            var result = await adsCommand.RunAsync(this.ams, cancellationToken);
            return result.AdsDeviceInfo;
        }

        /// <summary>
        /// Read the ads state
        /// </summary>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        /// <returns></returns>
        public async Task<AdsState> ReadStateAsync(CancellationToken cancellationToken = default)
        {
            var adsCommand = new AdsReadStateCommand();
            var result = await adsCommand.RunAsync(this.ams, cancellationToken);
            return result.AdsState;
        }

        public async Task DeleteActiveNotificationsAsync(CancellationToken cancellationToken = default)
        {

            while (ams.NotificationRequests.Count > 0)
            {
                await DeleteNotificationAsync(ams.NotificationRequests[0].NotificationHandle, cancellationToken);
            }
        }

        public async Task ReleaseActiveSymhandlesAsync(CancellationToken cancellationToken = default)
        {
            var handles = new List<uint>();

            lock (activeSymhandlesLock)
            {
                handles = activeSymhandles.Values.ToList();
                activeSymhandles.Clear();
            }

            foreach (var handle in handles)
                await ReleaseSymhandleAsyncInternal(handle, cancellationToken);
        }

        public async Task<List<AdsDataTypeDto>> GetDataTypesAsync(CancellationToken cancellationToken = default)
        {
            var uploadInfo = await ams.PerformRequestAsync(new AdsUploadInfoConversation(), cancellationToken)
                .ConfigureAwait(false);

            var types = await ams.PerformRequestAsync(new AdsDataTypesConversation(uploadInfo), cancellationToken)
                .ConfigureAwait(false);

            return types;
        }
        #endregion
    }
}
