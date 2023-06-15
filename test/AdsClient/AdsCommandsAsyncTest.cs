using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FakeItEasy;
using Shouldly;
using Viscon.Communication.Ads.Common;
using Viscon.Communication.Ads.Helpers;
using Xunit;

namespace Viscon.Communication.Ads.Test
{
    public class AdsCommandsAsyncTest : IDisposable
    {
        private readonly IAmsSocket amsSocket = A.Fake<IAmsSocket>();
        private readonly AdsClient client;
        private IIncomingMessageHandler messageHandler;
        private bool connected;

        public AdsCommandsAsyncTest()
        {
            A.CallTo(() => amsSocket.ConnectAsync(A<IIncomingMessageHandler>.Ignored, A<CancellationToken>.Ignored))
                .ReturnsLazily(call =>
                {
                    messageHandler = call.GetArgument<IIncomingMessageHandler>(0);
                    connected = true;
                    return Task.CompletedTask;
                });
            A.CallTo(() => amsSocket.Connected).ReturnsLazily(() => connected);

            client = new AdsClient(amsNetIdSource: "10.0.0.120.1.1", amsSocket: amsSocket,
                amsNetIdTarget: "5.1.204.123.1.1");

            if (!client.Ams.ConnectAsync().IsCompletedSuccessfully)
            {
                throw new Exception("Connect call should have completed synchronously.");
            }
        }

        public void Dispose()
        {
            client.Dispose();
        }

        [Fact]
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
            task.IsCompleted.ShouldBeTrue("Call should have completed synchronously.");
            AdsDeviceInfo deviceInfo = task.Result;
            deviceInfo.ToString().ShouldBe("Version: 2.11.1514 Devicename: TCatPlcCtrl");
        }

        [Fact]
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
            task.IsCompleted.ShouldBeTrue("Call should have completed synchronously.");
            var state = task.Result;
            state.ToString().ShouldBe("Ads state: 5 (Run) Device state: 0");
        }

        [Fact]
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
            task.IsCompleted.ShouldBeTrue("Call should have completed synchronously.");
            var handle = task.Result;
            handle.ShouldBe(2751464077);
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

            A.CallTo(() => amsSocket.SendAsync(A<byte[]>.That.Matches(buffer => isMatch(buffer))))
                .ReturnsLazily(call =>
                {
                    // The receive header is consumed by the AmsSocket, so strip it from the data passed to Ams
                    var res = rx[AmsHeaderHelper.AmsTcpHeaderSize..];

                    // Copy the invocation ID from request to response
                    var buffer = call.GetArgument<byte[]>(0);
                    buffer.AsSpan().Slice(invocationIdOffset, sizeof(uint))
                        .CopyTo(res.AsSpan()[(invocationIdOffset - AmsHeaderHelper.AmsTcpHeaderSize)..]);

                    messageHandler.HandleMessage(res);

                    return Task.CompletedTask;
                });
        }
    }
}
