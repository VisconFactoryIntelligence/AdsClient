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
using System.Net;
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

        private List<uint> activeSymhandles = new List<uint>();

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
            AdsWriteReadCommand adsCommand = new AdsWriteReadCommand(0x0000F003, 0x00000000, varName.ToAdsBytes(), 4);
            var result = await adsCommand.RunAsync(this.ams);
            var handle = BitConverter.ToUInt32(result.Data, 0);
            activeSymhandles.Add(handle);
            return handle;
        }

        public async Task<uint> GetSymhandlByNameAsync(IAdsSymhandle symhandle)
        {
            //var symhandle = new AdsSymhandle();
            symhandle.Symhandle = await GetSymhandleByNameAsync(symhandle.VarName);
            symhandle.ConnectionName = Name;
            return symhandle.Symhandle;
        }

        /// <summary>
        /// Release symhandle
        /// </summary>
        /// <param name="symhandle">The handle returned by GetSymhandleByName</param>
        public Task ReleaseSymhandleAsync(uint symhandle)
        {
            activeSymhandles.Remove(symhandle);
            AdsWriteCommand adsCommand = new AdsWriteCommand(0x0000F006, 0x00000000, BitConverter.GetBytes(symhandle));
            return adsCommand.RunAsync(this.ams);
        }

        public Task ReleaseSymhandleAsync(IAdsSymhandle adsSymhandle)
        {
            return ReleaseSymhandleAsync(adsSymhandle.Symhandle);
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

        public Task<byte[]> ReadBytesAsync(IAdsSymhandle adsSymhandle)
        {
            return ReadBytesAsync(adsSymhandle.Symhandle, adsSymhandle.ByteLength);
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
        /// Read the value from the handle returned by GetSymhandleByNameAsync
        /// </summary>
        /// <typeparam name="T">A type like byte, ushort, uint depending on the length of the twincat variable</typeparam>
        /// <param name="varHandle">The handle returned by GetSymhandleByNameAsync</param>
        /// <returns>The value of the twincat variable</returns>
        public async Task<T> ReadAsync<T>(uint varHandle) 
        {
            byte[] value = await ReadBytesAsync(varHandle, GenericHelper.GetByteLengthFromType<T>(DefaultStringLength));
            return GenericHelper.GetResultFromBytes<T>(value, DefaultStringLength);
        }

        public Task<T> ReadAsync<T>(IAdsSymhandle adsSymhandle) 
        {
            return ReadAsync<T>(adsSymhandle.Symhandle);
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

        public Task<uint> AddNotificationAsync(IAdsSymhandle adsSymhandle, AdsTransmissionMode transmissionMode, uint cycleTime, object userData)
        {
            return AddNotificationAsync(adsSymhandle.Symhandle, adsSymhandle.ByteLength, transmissionMode, cycleTime, userData);
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

        public Task<uint> AddNotificationAsync(IAdsSymhandle adsSymhandle, AdsTransmissionMode transmissionMode, uint cycleTime, object userData, Type typeOfValue)
        {
            return AddNotificationAsync(adsSymhandle.Symhandle, adsSymhandle.ByteLength, transmissionMode, cycleTime, userData, typeOfValue);
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

        public Task<uint> AddNotificationAsync<T>(IAdsSymhandle adsSymhandle, AdsTransmissionMode transmissionMode, uint cycleTime, object userData)
        {
            return AddNotificationAsync<T>(adsSymhandle.Symhandle, transmissionMode, cycleTime, userData);
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

        public Task WriteBytesAsync(IAdsSymhandle adsSymhandle, IEnumerable<byte> varValue)
        {
            return WriteBytesAsync(adsSymhandle.Symhandle, varValue);
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

        public Task WriteAsync<T>(IAdsSymhandle adsSymhandle, T varValue) 
        {
            return WriteAsync<T>(adsSymhandle.Symhandle, varValue);
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
            while (activeSymhandles.Count > 0)
            {
                await ReleaseSymhandleAsync(activeSymhandles[0]);
            }
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
            AdsWriteReadCommand adsCommand = new AdsWriteReadCommand(0x0000F003, 0x00000000, varName.ToAdsBytes(), 4);
            var result = adsCommand.Run(this.ams);
            var handle = BitConverter.ToUInt32(result.Data, 0);
            activeSymhandles.Add(handle);
            return handle;
        }

        /// <summary>
        /// Get a handle object from a variable name
        /// </summary>
        /// <param name="varName">A twincat variable like ".XXX"</param>
        /// <returns>An AdsSymhandle object</returns>
        public uint GetSymhandleByName(IAdsSymhandle symHandle)
        {
            symHandle.Symhandle = GetSymhandleByName(symHandle.VarName);
            symHandle.ConnectionName = Name;
            return symHandle.Symhandle;
        }

        /// <summary>
        /// Release symhandle
        /// </summary>
        /// <param name="symhandle">The handle returned by GetSymhandleByName</param>
        public void ReleaseSymhandle(uint symhandle)
        {
            activeSymhandles.Remove(symhandle);
            AdsWriteCommand adsCommand = new AdsWriteCommand(0x0000F006, 0x00000000, BitConverter.GetBytes(symhandle));
            var result = adsCommand.Run(this.ams);
        }

        public void ReleaseSymhandle(IAdsSymhandle adsSymhandle)
        {
            ReleaseSymhandle(adsSymhandle.Symhandle);
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

        public byte[] ReadBytes(IAdsSymhandle adsSymhandle)
        {
            return ReadBytes(adsSymhandle.Symhandle, adsSymhandle.ByteLength);
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
        /// Read the value from the handle returned by GetSymhandleByName
        /// </summary>
        /// <typeparam name="T">A type like byte, ushort, uint depending on the length of the twincat variable</typeparam>
        /// <param name="varHandle">The handle returned by GetSymhandleByName</param>
        /// <returns>The value of the twincat variable</returns>
        public T Read<T>(uint varHandle) 
        {
            byte[] value = ReadBytes(varHandle, GenericHelper.GetByteLengthFromType<T>(DefaultStringLength));
            return GenericHelper.GetResultFromBytes<T>(value, DefaultStringLength);
        }

        public T Read<T>(IAdsSymhandle adsSymhandle) 
        {
            return Read<T>(adsSymhandle.Symhandle);
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

        public uint AddNotification(IAdsSymhandle adsSymhandle, AdsTransmissionMode transmissionMode, uint cycleTime, object userData)
        {
            return AddNotification(adsSymhandle.Symhandle, adsSymhandle.ByteLength, transmissionMode, cycleTime, userData);
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

        public uint AddNotification(IAdsSymhandle adsSymhandle, AdsTransmissionMode transmissionMode, uint cycleTime, object userData, Type TypeOfValue)
        {
            return AddNotification(adsSymhandle.Symhandle, adsSymhandle.ByteLength, transmissionMode, cycleTime, userData, TypeOfValue);
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

        public uint AddNotification<T>(IAdsSymhandle adsSymhandle, AdsTransmissionMode transmissionMode, uint cycleTime, object userData) 
        {
            return AddNotification<T>(adsSymhandle.Symhandle, transmissionMode, cycleTime, userData);
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

        public void WriteBytes(IAdsSymhandle adsSymhandle, IEnumerable<byte> varValue)
        {
            WriteBytes(adsSymhandle.Symhandle, varValue);
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

        public void Write<T>(IAdsSymhandle adsSymhandle, T varValue)
        {
            Write<T>(adsSymhandle, varValue);
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
            while (activeSymhandles.Count > 0)
            {
                ReleaseSymhandle(activeSymhandles[0]);
            }
        }

        #endif
        #endregion

    }
}
