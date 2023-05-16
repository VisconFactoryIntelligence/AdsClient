// begin-snippet: Program
using Viscon.Communication.Ads;
using Viscon.Communication.Ads.Common;

namespace Samples;

public static class Program
{
    static async Task Main()
    {
        var timeout = Task.Delay(10000);
        var task = await Task.WhenAny(RunTestAsync(), timeout);
        if (task == timeout)
        {
            Console.Error.WriteLine("Operation timed out!");
        }
        else
        {
            Console.WriteLine("Done!");
        }
    }

    private static async Task RunTestAsync()
    {
        using var client = new AdsClient(
            amsNetIdSource:"192.168.5.6.1.1",
            ipTarget:"192.168.3.4",
            amsNetIdTarget:"192.168.3.4.1.1");

        var deviceInfo = await client.ReadDeviceInfoAsync();
        Console.WriteLine("Device info: " + deviceInfo.ToString());

        var state = await client.ReadStateAsync();
        Console.WriteLine("State: " + state.ToString());

        client.OnNotification += (sender,e) => {
            Console.WriteLine(e.Notification.ToString());
        };

        var varHandle1 = await client.GetSymhandleByNameAsync(".VariableName1");
        Console.WriteLine("Variable1 handle: " + varHandle1.ToString());

        var varHandle2 = await client.GetSymhandleByNameAsync(".VariableName2");
        Console.WriteLine("Variable2 handle: " + varHandle2.ToString());

        var notification1Handle = await client.AddNotificationAsync<byte>(
            varHandle1, AdsTransmissionMode.Cyclic, 5000, null);
        var notification2Handle = await client.AddNotificationAsync<byte>(
            varHandle2, AdsTransmissionMode.OnChange, 10, null);

        var value = await client.ReadAsync<byte>(varHandle1);
        Console.WriteLine("Value before write: " + value.ToString());

        await client.WriteAsync<byte>(varHandle1, 1);
        Console.WriteLine("I turned something on");

        value = await client.ReadAsync<byte>(varHandle1);
        Console.WriteLine("Value after write: " + value.ToString());

        await Task.Delay(5000);

        await client.WriteAsync<byte>(varHandle1, 0);
        Console.WriteLine("I turned something off");

        Console.WriteLine("Deleting active notifications...");
        await client.DeleteActiveNotificationsAsync();
    }
}
// end-snippet