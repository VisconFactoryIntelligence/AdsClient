using System;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using Ads.Client.Common;
using Ads.Client.Helpers;
using FakeItEasy;

namespace Ads.Client.Test
{
    [TestFixture()]
    public class AdsCommandsAsyncTest : IDisposable
    {
        private readonly IAmsSocketAsync amsSocketAsync = A.Fake<IAmsSocketAsync>();
        private readonly IAmsSocket amsSocket = A.Fake<IAmsSocket>();
        private readonly AdsClient client;

        public AdsCommandsAsyncTest()
        {
            A.CallTo(() => amsSocket.Async).Returns(amsSocketAsync);
            A.CallTo(() => amsSocketAsync.ConnectAndListenAsync()).Returns(Task.CompletedTask);

            client = new AdsClient(amsNetIdSource: "10.0.0.120.1.1", amsSocket: amsSocket,
                amsNetIdTarget: "5.1.204.123.1.1");
        }

        public void Dispose()
        {
            client.Dispose();
        }

        [Test()]
        public void ReadDeviceInfoAsync()
        {
            // Arrange
            var msgSend = new byte[]
            {
                0, 0, 32, 0, 0, 0, 5, 1, 204, 123, 1, 1, 33, 3, 10, 0, 0, 120, 1, 1, 137, 128, 1, 0, 4, 0, 0, 0, 0,
                0, 0, 0, 0, 0, 1, 0, 0, 0
            };

            var msgReceive = new byte[]
            {
                0, 0, 56, 0, 0, 0, 10, 0, 0, 120, 1, 1, 137, 128, 5, 1, 204, 123, 1, 1, 33, 3, 1, 0, 5, 0, 24, 0, 0,
                0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 2, 11, 234, 5, 84, 67, 97, 116, 80, 108, 99, 67, 116, 114,
                108, 0, 0, 0, 0, 0
            };

            SetupRequestResponse(msgSend, msgReceive);

            // Act
            var task = client.ReadDeviceInfoAsync();

            // Assert
            Assert.IsTrue(task.IsCompleted, "Call should have completed synchronously.");
            AdsDeviceInfo deviceInfo = task.Result;
            Assert.AreEqual(deviceInfo.ToString(), "Version: 2.11.1514 Devicename: TCatPlcCtrl");
        }

        [Test()]
        public void ReadStateAsync()
        {
            // Arrange
            var msgSend = new byte[]
            {
                0, 0, 32, 0, 0, 0, 5, 1, 204, 123, 1, 1, 33, 3, 10, 0, 0, 120, 1, 1, 137, 128, 4, 0, 4, 0, 0, 0, 0,
                0, 0, 0, 0, 0, 1, 0, 0, 0
            };

            var msgReceive = new byte[]
            {
                0, 0, 40, 0, 0, 0, 10, 0, 0, 120, 1, 1, 137, 128, 5, 1, 204, 123, 1, 1, 33, 3, 4, 0, 5, 0, 8, 0, 0,
                0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 5, 0, 0, 0
            };

            SetupRequestResponse(msgSend, msgReceive);

            // Act
            var task = client.ReadStateAsync();

            // Assert
            Assert.IsTrue(task.IsCompleted, "Call should have completed synchronously.");
            var state = task.Result;
            Assert.AreEqual(state.ToString(), "Ads state: 5 (Run) Device state: 0");
        }

        [Test()]
        public void GetSymhandleByNameAsync()
        {
            // Arrange
            var msgSend = new byte[]
            {
                0, 0, 58, 0, 0, 0, 5, 1, 204, 123, 1, 1, 33, 3, 10, 0, 0, 120, 1, 1, 137, 128, 9, 0, 4, 0, 26, 0, 0,
                0, 0, 0, 0, 0, 1, 0, 0, 0, 3, 240, 0, 0, 0, 0, 0, 0, 4, 0, 0, 0, 10, 0, 0, 0, 46, 84, 69, 83, 84,
                84, 73, 77, 69, 0
            };

            var msgReceive = new byte[]
            {
                0, 0, 44, 0, 0, 0, 10, 0, 0, 120, 1, 1, 137, 128, 5, 1, 204, 123, 1, 1, 33, 3, 9, 0, 5, 0, 12, 0, 0,
                0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 4, 0, 0, 0, 141, 2, 0, 164
            };

            SetupRequestResponse(msgSend, msgReceive);

            // Act
            var task = client.GetSymhandleByNameAsync(".TESTTIME");

            // Assert
            Assert.IsTrue(task.IsCompleted, "Call should have completed synchronously.");
            var handle = task.Result;
            Assert.AreEqual(handle, 2751464077);
        }

        private void SetupRequestResponse(byte[] tx, byte[] rx)
        {
            const int invocationIdOffset = 34;

            var isMatch = (byte[] sendData) =>
            {
                var beforeInvocationId = new Range(Index.Start, invocationIdOffset);
                var afterInvocationId = new Range(invocationIdOffset + sizeof(uint), Index.End);

                return tx[beforeInvocationId].SequenceEqual(sendData[beforeInvocationId]) &&
                    tx[afterInvocationId].SequenceEqual(sendData[afterInvocationId]);
            };

            A.CallTo(() => amsSocketAsync.SendAsync(A<byte[]>.That.Matches(buffer => isMatch(buffer))))
                .ReturnsLazily(call =>
                {
                    // The receive header is consumed by the AmsSocket, so strip it from the data passed to Ams
                    var res = rx[AmsHeaderHelper.AmsTcpHeaderSize..];

                    // Copy the invocation ID from request to response
                    var buffer = call.GetArgument<byte[]>(0);
                    buffer.AsSpan().Slice(invocationIdOffset, sizeof(uint))
                        .CopyTo(res.AsSpan()[(invocationIdOffset - AmsHeaderHelper.AmsTcpHeaderSize)..]);

                    amsSocket.OnReadCallBack += Raise.With(new AmsSocketResponseArgs { Response = res });
                    return Task.CompletedTask;
                });
        }
    }
}
