using System.Xml.Linq;
using Viscon.Communication.Ads;
using Viscon.Communication.Ads.Commands;
using Viscon.Communication.Ads.Common;
using Viscon.Communication.Ads.Helpers;

namespace Samples;

public class Samples
{
    public async Task Sample()
    {
        // begin-snippet: Connect
        using var client = new AdsClient(amsNetIdSource: "10.0.0.120.1.1", ipTarget: "10.0.0.2",
            amsNetIdTarget: "10.0.0.2.1.1");

        await client.Ams.ConnectAsync();
        // end-snippet

        // begin-snippet: ReadDeviceInfoAsync
        AdsDeviceInfo deviceInfo = await client.ReadDeviceInfoAsync();
        Console.WriteLine(deviceInfo.ToString());
        // end-snippet

        // begin-snippet: ReadWriteVariableByName
        var varHandle = await client.GetSymhandleByNameAsync(".TestVar");
        await client.WriteAsync<byte>(varHandle, 0);
        var value = await client.ReadAsync<byte>(varHandle);
        await client.ReleaseSymhandleAsync(varHandle);
        // end-snippet

        // begin-snippet: WorkingWithNotifications
        client.OnNotification += (sender, e) => { Console.WriteLine(e.Notification.ToString()); };
        var varHandle1 = await client.GetSymhandleByNameAsync(".VarTest1");
        var varHandle2 = await client.GetSymhandleByNameAsync(".VarTest2");
        var notificationHandle1 = await client.AddNotificationAsync<byte>(varHandle1, AdsTransmissionMode.Cyclic, 2000, null);
        var notificationHandle2 = await client.AddNotificationAsync<byte>(varHandle2, AdsTransmissionMode.OnChange, 10, null);
        // end-snippet

        // begin-snippet: UsingCommands
        var stateCmd = new AdsReadStateCommand();
        var state = (await stateCmd.RunAsync(client.Ams, CancellationToken.None)).AdsState.ToString();
        Console.WriteLine($"State: {state}");
        // end-snippet

        // begin-snippet: ReadTestClass
        var handle = await client.GetSymhandleByNameAsync(".Test");
        var testInstance = await client.ReadAsync<TestClass>(handle);
        await client.WriteAsync(handle, testInstance);
        // end-snippet

        // begin-snippet: GetTargetDesc
        var xml = await client.Special.GetTargetDescAsync();
        xml = XDocument.Parse(xml).ToString();
        // end-snippet
    }

    // begin-snippet: TestClass
    [AdsSerializable]
    public class TestClass
    {
        [Ads]
        public ushort Var1 { get; set; }

        [Ads]
        public byte Var2 { get; set; }
    }
    // end-snippet
}