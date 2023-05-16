[![.NET](https://github.com/VisconFactoryIntelligence/AdsClient/actions/workflows/dotnet.yml/badge.svg)](https://github.com/VisconFactoryIntelligence/AdsClient/actions/workflows/dotnet.yml)

This is the client implementation of the [Twincat](http://www.beckhoff.com/english.asp?twincat/default.htm) Ads protocol from [Beckhoff](http://http://www.beckhoff.com/).   

The implementation is in C# and targets .NET Framework 4.6.2, .NET Standard 2.0 and .NET Standard 2.1.

All communication methods are async.

Contributors
============
- [Inando](https://github.com/inando)
- [MrCircuit](https://github.com/MrCircuit)
- [mycroes](https://github.com/mycroes)
- [Viscon Factory Intelligence](https://github.com/VisconFactoryIntelligence)

Getting started
===============

Ads Route
---------

First you have to give your device/machine the permission to communicate with the Twincat Ads server by adding a route.

There are different ways of doing this depending on the device.
You can use the Twincat Remote Manager for example.
On a CX9001 device you can connect with cerhost.exe and add a route with 
\Hard Disk\System\TcAmsRemoteMgr.exe
(You may not to reboot after this!)

*If the library is not working, an incorrect/missing route may be the problem!.*

Installation
------------
You only need this library.
Twincat is _not_ needed. 
It will not work if you have programs like system manager or PLC control running.

The package is available from [NuGet](https://www.nuget.org/packages/Viscon.Communication.Ads).

Examples
========

## Connect to the PLC

<!-- snippet: Connect -->
<a id='snippet-connect'></a>
```cs
using var client = new AdsClient(amsNetIdSource: "10.0.0.120.1.1", ipTarget: "10.0.0.2",
    amsNetIdTarget: "10.0.0.2.1.1");

await client.Ams.ConnectAsync();
```
<sup><a href='/samples/Samples/Samples.cs#L13-L18' title='Snippet source file'>snippet source</a> | <a href='#snippet-connect' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->

## Read device info

<!-- snippet: ReadDeviceInfoAsync -->
<a id='snippet-readdeviceinfoasync'></a>
```cs
AdsDeviceInfo deviceInfo = await client.ReadDeviceInfoAsync();
Console.WriteLine(deviceInfo.ToString());
```
<sup><a href='/samples/Samples/Samples.cs#L20-L23' title='Snippet source file'>snippet source</a> | <a href='#snippet-readdeviceinfoasync' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->

## Read/Write a variable by name

<!-- snippet: ReadWriteVariableByName -->
<a id='snippet-readwritevariablebyname'></a>
```cs
var varHandle = await client.GetSymhandleByNameAsync(".TestVar");
await client.WriteAsync<byte>(varHandle, 0);
var value = await client.ReadAsync<byte>(varHandle);
await client.ReleaseSymhandleAsync(varHandle);
```
<sup><a href='/samples/Samples/Samples.cs#L25-L30' title='Snippet source file'>snippet source</a> | <a href='#snippet-readwritevariablebyname' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->

You can also use the AdsCommands directly if you need to write directly with IndexGroup/IndexOffset

## Working with notifications

<!-- snippet: WorkingWithNotifications -->
<a id='snippet-workingwithnotifications'></a>
```cs
client.OnNotification += (sender, e) => { Console.WriteLine(e.Notification.ToString()); };
var varHandle1 = await client.GetSymhandleByNameAsync(".VarTest1");
var varHandle2 = await client.GetSymhandleByNameAsync(".VarTest2");
var notificationHandle1 = await client.AddNotificationAsync<byte>(varHandle1, AdsTransmissionMode.Cyclic, 2000, null);
var notificationHandle2 = await client.AddNotificationAsync<byte>(varHandle2, AdsTransmissionMode.OnChange, 10, null);
```
<sup><a href='/samples/Samples/Samples.cs#L32-L38' title='Snippet source file'>snippet source</a> | <a href='#snippet-workingwithnotifications' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->

## Simple example with most basic functions

Here is a sample which shows usage of most basic functions.

<!-- snippet: Program -->
<a id='snippet-program'></a>
```cs
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
```
<sup><a href='/samples/Samples/Program.cs#L1-L69' title='Snippet source file'>snippet source</a> | <a href='#snippet-program' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->

## Using commands directly

<!-- snippet: UsingCommands -->
<a id='snippet-usingcommands'></a>
```cs
var stateCmd = new AdsReadStateCommand();
var state = (await stateCmd.RunAsync(client.Ams, CancellationToken.None)).AdsState.ToString();
Console.WriteLine($"State: {state}");
```
<sup><a href='/samples/Samples/Samples.cs#L40-L44' title='Snippet source file'>snippet source</a> | <a href='#snippet-usingcommands' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->

## Serialize to class

It's possible to read directly to a class or write from a class.  
You need to set the AdsSerializable attribute on the class and the Ads attribute on the fields/properties you need.  
The fields without the Ads attribute are ignored. 

<!-- snippet: TestClass -->
<a id='snippet-testclass'></a>
```cs
[AdsSerializable]
public class TestClass
{
    [Ads]
    public ushort Var1 { get; set; }

    [Ads]
    public byte Var2 { get; set; }
}
```
<sup><a href='/samples/Samples/Samples.cs#L58-L68' title='Snippet source file'>snippet source</a> | <a href='#snippet-testclass' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->

<!-- snippet: ReadTestClass -->
<a id='snippet-readtestclass'></a>
```cs
var handle = await client.GetSymhandleByNameAsync(".Test");
var testInstance = await client.ReadAsync<TestClass>(handle);
await client.WriteAsync(handle, testInstance);
```
<sup><a href='/samples/Samples/Samples.cs#L46-L50' title='Snippet source file'>snippet source</a> | <a href='#snippet-readtestclass' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->

This is an example struct in Twincat:
```
TYPE TestStruct :
STRUCT
    Var1 : INT;
    Var2 : BYTE;
END_STRUCT
END_TYPE
```

## Special functions

These functions aren't documented by Beckhoff:

### Get target description

<!-- snippet: GetTargetDesc -->
<a id='snippet-gettargetdesc'></a>
```cs
var xml = await client.Special.GetTargetDescAsync();
xml = XDocument.Parse(xml).ToString();
```
<sup><a href='/samples/Samples/Samples.cs#L52-L55' title='Snippet source file'>snippet source</a> | <a href='#snippet-gettargetdesc' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->

Credits, sources and inspiration
================================
* [ads-client NodeJS library by Jussi Isotalo](https://github.com/jisotalo/ads-client)
* [RabbitMQ .NET Client](https://github.com/rabbitmq/rabbitmq-dotnet-client)
* [Sally7](https://github.com/mycroes/Sally7)
