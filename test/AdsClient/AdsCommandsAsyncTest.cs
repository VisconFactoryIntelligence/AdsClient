﻿using NUnit.Framework;
using Ads.Client.Common;

namespace Ads.Client.Test
{
	[TestFixture()]
    public class AdsCommandsAsyncTest
    {
		[Test()]
        public void ReadDeviceInfoAsync()
        {
            var amsTestSocket = new AmsSocketTest();
			amsTestSocket.SendMessage = new byte[] {
                    0,0,32,0,0,0,5,1,204,123,1,1,33,3,10,0,0,
                    120,1,1,137,128,1,0,4,0,0,0,0,0,0,0,0,0,1,0,0,0};

            amsTestSocket.ReceiveMessage = new byte[] { 
                    0,0,56,0,0,0,10,0,0,120,1,1,137,128,5,1,204,123,
                    1,1,33,3,1,0,5,0,24,0,0,0,0,0,0,0,1,0,0,0,0,0,0,
                    0,2,11,234,5,84,67,97,116,80,108,99,67,116,114,108,0,0,0,0,0};

            using (AdsClient client = new AdsClient(
                    amsNetIdSource: "10.0.0.120.1.1",
                    amsSocket: amsTestSocket,
                    amsNetIdTarget: "5.1.204.123.1.1"))
            {
                var task = client.ReadDeviceInfoAsync();
                task.Wait(5000);
                AdsDeviceInfo deviceInfo = task.Result;
                Assert.AreEqual(deviceInfo.ToString(), "Version: 2.11.1514 Devicename: TCatPlcCtrl");
            }
        }

		[Test()]
        public void ReadStateAsync()
        {
            var amsTestSocket = new AmsSocketTest();
            amsTestSocket.SendMessage = new byte[] {
                    0,0,32,0,0,0,5,1,204,123,1,1,33,3,10,0,0,120,
                    1,1,137,128,4,0,4,0,0,0,0,0,0,0,0,0,1,0,0,0};

            amsTestSocket.ReceiveMessage = new byte[] { 
                    0,0,40,0,0,0,10,0,0,120,1,1,137,128,5,1,204,123,1,1,
                    33,3,4,0,5,0,8,0,0,0,0,0,0,0,1,0,0,0,0,0,0,0,5,0,0,0};

            using (AdsClient client = new AdsClient(
                    amsNetIdSource: "10.0.0.120.1.1",
                    amsSocket: amsTestSocket,
                    amsNetIdTarget: "5.1.204.123.1.1"))
            {
                var task = client.ReadStateAsync();
                task.Wait(5000);
                var state = task.Result;
                Assert.AreEqual(state.ToString(), "Ads state: 5 (Run) Device state: 0");
            }
        }

		[Test()]
        public void GetSymhandleByNameAsync()
        {
            var amsTestSocket = new AmsSocketTest();
            amsTestSocket.SendMessage = new byte[] {
                    0,0,58,0,0,0,5,1,204,123,1,1,33,3,10,0,0,120,
                    1,1,137,128,9,0,4,0,26,0,0,0,0,0,0,0,1,0,0,0,
                    3,240,0,0,0,0,0,0,4,0,0,0,10,0,0,0,46,84,69,83,
                    84,84,73,77,69,0};

            amsTestSocket.ReceiveMessage = new byte[] { 
                    0,0,44,0,0,0,10,0,0,120,1,1,137,128,5,1,204,123,
                    1,1,33,3,9,0,5,0,12,0,0,0,0,0,0,0,1,0,0,0,0,0,0,
                    0,4,0,0,0,141,2,0,164};

            using (AdsClient client = new AdsClient(
                    amsNetIdSource: "10.0.0.120.1.1",
                    amsSocket: amsTestSocket,
                    amsNetIdTarget: "5.1.204.123.1.1"))
            {
                var task = client.GetSymhandleByNameAsync(".TESTTIME");
                task.Wait(5000);
                var handle = task.Result;
                Assert.AreEqual(handle, 2751464077);
            }
        }
    }
}
