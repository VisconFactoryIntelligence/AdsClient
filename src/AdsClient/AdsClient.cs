using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Ads.Client.Commands;
using Ads.Client.Common;
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
            #if !SILVERLIGHT
            if (ams.ConnectedAsync == false)
            {
                DeleteActiveNotifications();
                ReleaseActiveSymhandles();
            }
            #endif

            if (ams != null) ams.Dispose();
        }

        public void Dispose()
        {
            Dispose(true);
        }

        #region Async Methods
#if !NO_ASYNC

        /// <summary>
        /// Get a handle from a variable name
        /// </summary>
        /// <param name="varName">A twincat variable like ".XXX"</param>
        /// <returns>The handle</returns>
        public async Task<uint> GetSymhandleByNameAsync(string varName)
        {
            // Check, if the handle is already present.
            lock (activeSymhandlesLock)
            {
                if (activeSymhandles.ContainsKey(varName))
                    return activeSymhandles[varName];
            }

            // It was not retrieved before - get it from the control.
            var adsCommand = new AdsWriteReadCommand(0x0000F003, 0x00000000, varName.ToAdsBytes(), 4);
            var result = await adsCommand.RunAsync(this.ams);
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
        /// <returns>An awaitable task.</returns>
        public Task ReleaseSymhandleAsync(uint symhandle)
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

            return ReleaseSymhandleAsyncInternal(symhandle);
        }

        private Task ReleaseSymhandleAsyncInternal(uint symhandle)
        {
            // Run the release command.
            var adsCommand = new AdsWriteCommand(0x0000F006, 0x00000000, BitConverter.GetBytes(symhandle));
            return adsCommand.RunAsync(this.ams);
        }

        /// <summary>
        /// Read the value from the handle returned by GetSymhandleByNameAsync
        /// </summary>
        /// <param name="varHandle">The handle returned by GetSymhandleByNameAsync</param>
        /// <returns>A byte[] with the value of the twincat variable</returns>
        public async Task<byte[]> ReadBytesAsync(uint varHandle, uint readLength)
        {
            AdsReadCommand adsCommand = new AdsReadCommand(0x0000F005, varHandle, readLength);
            var result = await adsCommand.RunAsync(this.ams);
            return result.Data;
        }

        public async Task<byte[]> ReadBytesI_Async(uint offset, uint readLength)
        {
            AdsReadCommand adsCommand = new AdsReadCommand(0x0000F020, offset, readLength);
            var result = await adsCommand.RunAsync(this.ams);
            return result.Data;
        }

        public async Task<byte[]> ReadBytesQ_Async(uint offset, uint readLength)
        {
            AdsReadCommand adsCommand = new AdsReadCommand(0x0000F030, offset, readLength);
            var result = await adsCommand.RunAsync(this.ams);
            return result.Data;
        }

        /// <summary>
        /// Read the value from the handle returned by GetSymhandleByNameAsync.
        /// </summary>
        /// <typeparam name="T">A type like byte, ushort, uint depending on the length of the twincat variable</typeparam>
        /// <param name="varHandle">The handle returned by GetSymhandleByNameAsync</param>
        /// <param name="arrayLength">An optional array length.</param>
        /// <param name="adsObj">An optional existing object.</param>
        /// <returns>The value of the twincat variable</returns>
        public async Task<T> ReadAsync<T>(uint varHandle, uint arrayLength = 1, object adsObj = null)
        {
            var length = GenericHelper.GetByteLengthFromType<T>(DefaultStringLength, arrayLength);
            var value = await ReadBytesAsync(varHandle, length);

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
        /// <returns>The notification handle</returns>
        public Task<uint> AddNotificationAsync(uint varHandle, uint length, AdsTransmissionMode transmissionMode, uint cycleTime, object userData)
        {
            return AddNotificationAsync(varHandle, length, transmissionMode, cycleTime, userData, typeof(byte[]));
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
        /// <returns>The notification handle</returns>
        public async Task<uint> AddNotificationAsync(uint varHandle, uint length, AdsTransmissionMode transmissionMode, uint cycleTime, object userData, Type typeOfValue)
        {
            var adsCommand = new AdsAddDeviceNotificationCommand(0x0000F005, varHandle, length, transmissionMode);
            adsCommand.CycleTime = cycleTime;
            adsCommand.UserData = userData;
            adsCommand.TypeOfValue = typeOfValue;
            var result = await adsCommand.RunAsync(this.ams);
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
        /// <returns></returns>
        public Task<uint> AddNotificationAsync<T>(uint varHandle, AdsTransmissionMode transmissionMode, uint cycleTime, object userData)
        {
            uint length = GenericHelper.GetByteLengthFromType<T>(DefaultStringLength);
            return AddNotificationAsync(varHandle, length, transmissionMode, cycleTime, userData, typeof(T));
        }

        /// <summary>
        /// Delete a previously registerd notification
        /// </summary>
        /// <param name="notificationHandle">The handle returned by AddNotification(Async)</param>
        /// <returns></returns>
        public Task DeleteNotificationAsync(uint notificationHandle)
        {
            var adsCommand = new AdsDeleteDeviceNotificationCommand(notificationHandle);
            return adsCommand.RunAsync(this.ams);
        }

        /// <summary>
        /// Write the value to the handle returned by GetSymhandleByNameAsync
        /// </summary>
        /// <param name="varHandle">The handle returned by GetSymhandleByNameAsync</param>
        /// <param name="varValue">The byte[] value to be sent</param>
        public Task WriteBytesAsync(uint varHandle, IEnumerable<byte> varValue)
        {
            AdsWriteCommand adsCommand = new AdsWriteCommand(0x0000F005, varHandle, varValue);
            return adsCommand.RunAsync(this.ams);
        }

        /// <summary>
        /// Write the value to the handle returned by GetSymhandleByNameAsync
        /// </summary>
        /// <typeparam name="T">A type like byte, ushort, uint depending on the length of the twincat variable</typeparam>
        /// <param name="varHandle">The handle returned by GetSymhandleByNameAsync</param>
        /// <param name="varValue">The value to be sent</param>
        public Task WriteAsync<T>(uint varHandle, T varValue)
        {
            IEnumerable<byte> varValueBytes = GenericHelper.GetBytesFromType<T>(varValue, defaultStringLenght);
            return this.WriteBytesAsync(varHandle, varValueBytes);
        }

        /// <summary>
        /// Get some information of the ADS device (version, name)
        /// </summary>
        /// <returns></returns>
        public async Task<AdsDeviceInfo> ReadDeviceInfoAsync()
        {
            AdsReadDeviceInfoCommand adsCommand = new AdsReadDeviceInfoCommand();
            var result = await adsCommand.RunAsync(this.ams);
            return result.AdsDeviceInfo;
        }

        /// <summary>
        /// Read the ads state
        /// </summary>
        /// <returns></returns>
        public async Task<AdsState> ReadStateAsync()
        {
            var adsCommand = new AdsReadStateCommand();
            var result = await adsCommand.RunAsync(this.ams);
            return result.AdsState;
        }

        public async Task DeleteActiveNotificationsAsync()
        {

            while (ams.NotificationRequests.Count > 0)
            {
                await DeleteNotificationAsync(ams.NotificationRequests[0].NotificationHandle);
            }
        }

        public async Task ReleaseActiveSymhandlesAsync()
        {
            var handles = new List<uint>();

            lock (activeSymhandlesLock)
            {
                handles = activeSymhandles.Values.ToList();
                activeSymhandles.Clear();
            }

            foreach (var handle in handles)
                await ReleaseSymhandleAsyncInternal(handle);
        }
#endif
        #endregion

        #region Blocking Methods
        #if !SILVERLIGHT

        /// <summary>
        /// Get a handle from a variable name
        /// </summary>
        /// <param name="varName">A twincat variable like ".XXX"</param>
        /// <returns>The handle</returns>
        public uint GetSymhandleByName(string varName)
        {
            // Check, if the handle is already present.
            lock (activeSymhandlesLock)
            {
                if (activeSymhandles.ContainsKey(varName))
                    return activeSymhandles[varName];
            }

            // It was not retrieved before - get it from the control.
            var adsCommand = new AdsWriteReadCommand(0x0000F003, 0x00000000, varName.ToAdsBytes(), 4);
            var result = adsCommand.Run(this.ams);
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

        private void ReleaseSymhandleInternal(uint symhandle)
        {
            // Run the release command.
            var adsCommand = new AdsWriteCommand(0x0000F006, 0x00000000, BitConverter.GetBytes(symhandle));
            adsCommand.Run(this.ams);
        }

        /// <summary>
        /// Release symhandle
        /// </summary>
        /// <param name="symhandle">The handle returned by GetSymhandleByName</param>
        public void ReleaseSymhandle(uint symhandle)
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

            ReleaseSymhandleInternal(symhandle);
        }

        /// <summary>
        /// Read the value from the handle returned by GetSymhandleByName
        /// </summary>
        /// <param name="varHandle">The handle returned by GetSymhandleByName</param>
        /// <returns>A byte[] with the value of the twincat variable</returns>
        public byte[] ReadBytes(uint varHandle, uint readLength)
        {
            AdsReadCommand adsCommand = new AdsReadCommand(0x0000F005, varHandle, readLength);
            var result = adsCommand.Run(this.ams);
            return result.Data;
        }

        public byte[] ReadBytesI(uint offset, uint readLength)
        {
            AdsReadCommand adsCommand = new AdsReadCommand(0x0000F020, offset, readLength);
            var result = adsCommand.Run(this.ams);
            return result.Data;
        }

        public byte[] ReadBytesQ(uint offset, uint readLength)
        {
            AdsReadCommand adsCommand = new AdsReadCommand(0x0000F030, offset, readLength);
            var result = adsCommand.Run(this.ams);
            return result.Data;
        }

        /// <summary>
        /// Read the value from the handle returned by GetSymhandleByName.
        /// </summary>
        /// <typeparam name="T">A type like byte, ushort, uint depending on the length of the twincat variable</typeparam>
        /// <param name="varHandle">The handle returned by GetSymhandleByName</param>
        /// <param name="arrayLength">An optional array length.</param>
        /// <param name="adsObj">An optional existing object.</param>
        /// <returns>The value of the twincat variable</returns>
        public T Read<T>(uint varHandle, uint arrayLength = 1, object adsObj = null)
        {
            var length = GenericHelper.GetByteLengthFromType<T>(DefaultStringLength, arrayLength);
            var value = ReadBytes(varHandle, length);

            if (value != null)
                return GenericHelper.GetResultFromBytes<T>(value, DefaultStringLength, arrayLength, adsObj);
            else
                return default(T);
        }

        /// <summary>
        /// Add a noticiation when a variable changes or cyclic after a defined time in ms
        /// </summary>
        /// <param name="varHandle">The handle returned by GetSymhandleByName</param>
        /// <param name="length">The length of the data that must be send by the notification</param>
        /// <param name="transmissionMode">On change or cyclic</param>
        /// <param name="cycleTime">The cyclic time in ms. If used with OnChange, then the value is send once after this time in ms</param>
        /// <param name="userData">A custom object that can be used in the callback</param>
        /// <returns>The notification handle</returns>
        public uint AddNotification(uint varHandle, uint length, AdsTransmissionMode transmissionMode, uint cycleTime, object userData)
        {
            return AddNotification(varHandle, length, transmissionMode, cycleTime, userData, typeof(byte[]));
        }

        /// <summary>
        /// Add a noticiation when a variable changes or cyclic after a defined time in ms
        /// </summary>
        /// <param name="varHandle">The handle returned by GetSymhandleByName</param>
        /// <param name="length">The length of the data that must be send by the notification</param>
        /// <param name="transmissionMode">On change or cyclic</param>
        /// <param name="cycleTime">The cyclic time in ms. If used with OnChange, then the value is send once after this time in ms</param>
        /// <param name="userData">A custom object that can be used in the callback</param>
        /// <param name="TypeOfValue">The type of the returned notification value</param>
        /// <returns>The notification handle</returns>
        public uint AddNotification(uint varHandle, uint length, AdsTransmissionMode transmissionMode, uint cycleTime, object userData, Type TypeOfValue)
        {
            var adsCommand = new AdsAddDeviceNotificationCommand(0x0000F005, varHandle, length, transmissionMode);
            adsCommand.CycleTime = cycleTime;
            adsCommand.UserData = userData;
            adsCommand.TypeOfValue = TypeOfValue;
            var result = adsCommand.Run(this.ams);
            adsCommand.Notification.NotificationHandle = result.NotificationHandle;
            return result.NotificationHandle;
        }

        /// <summary>
        /// Add a noticiation when a variable changes or cyclic after a defined time in ms
        /// </summary>
        /// <typeparam name="T">Type for defining the length of the data that must be send by the notification</typeparam>
        /// <param name="varHandle">The handle returned by GetSymhandleByNameAsync</param>
        /// <param name="transmissionMode">On change or cyclic</param>
        /// <param name="cycleTime">The cyclic time in ms. If used with OnChange, then the value is send once after this time in ms</param>
        /// <param name="userData">A custom object that can be used in the callback</param>
        /// <returns></returns>
        public uint AddNotification<T>(uint varHandle, AdsTransmissionMode transmissionMode, uint cycleTime, object userData)
        {
            uint length = GenericHelper.GetByteLengthFromType<T>(DefaultStringLength);
            return AddNotification(varHandle, length, transmissionMode, cycleTime, userData, typeof(T));
        }

        /// <summary>
        /// Delete a previously registerd notification
        /// </summary>
        /// <param name="notificationHandle">The handle returned by AddNotification</param>
        /// <returns></returns>
        public void DeleteNotification(uint notificationHandle)
        {
            var adsCommand = new AdsDeleteDeviceNotificationCommand(notificationHandle);
            var result = adsCommand.Run(this.ams);
        }

        /// <summary>
        /// Write the value to the handle returned by GetSymhandleByName
        /// </summary>
        /// <param name="varHandle">The handle returned by GetSymhandleByName</param>
        /// <param name="varValue">The byte[] value to be sent</param>
        public void WriteBytes(uint varHandle, IEnumerable<byte> varValue)
        {
            AdsWriteCommand adsCommand = new AdsWriteCommand(0x0000F005, varHandle, varValue);
            var result = adsCommand.Run(this.ams);
        }

        /// <summary>
        /// Write the value to the handle returned by GetSymhandleByName
        /// </summary>
        /// <typeparam name="T">A type like byte, ushort, uint depending on the length of the twincat variable</typeparam>
        /// <param name="varHandle">The handle returned by GetSymhandleByName</param>
        /// <param name="varValue">The value to be sent</param>
        public void Write<T>(uint varHandle, T varValue)
        {
            IEnumerable<byte> varValueBytes = GenericHelper.GetBytesFromType<T>(varValue, defaultStringLenght);
            this.WriteBytes(varHandle, varValueBytes);
        }

        /// <summary>
        /// Get some information of the ADS device (version, name)
        /// </summary>
        /// <returns></returns>
        public AdsDeviceInfo ReadDeviceInfo()
        {
            AdsReadDeviceInfoCommand adsCommand = new AdsReadDeviceInfoCommand();
            var result = adsCommand.Run(this.ams);
            return result.AdsDeviceInfo;
        }

        /// <summary>
        /// Read the ads state
        /// </summary>
        /// <returns></returns>
        public AdsState ReadState()
        {
            var adsCommand = new AdsReadStateCommand();
            var result = adsCommand.Run(this.ams);
            return result.AdsState;
        }

        public void DeleteActiveNotifications()
        {
            if (ams.NotificationRequests != null)
            {
                while (ams.NotificationRequests.Count > 0)
                {
                    DeleteNotification(ams.NotificationRequests[0].NotificationHandle);
                }
            }
        }

        public void ReleaseActiveSymhandles()
        {
            var handles = new List<uint>();

            lock (activeSymhandlesLock)
            {
                handles = activeSymhandles.Values.ToList();
                activeSymhandles.Clear();
            }

            foreach (var handle in handles)
                ReleaseSymhandleInternal(handle);
        }

        #endif
        #endregion

    }
}
