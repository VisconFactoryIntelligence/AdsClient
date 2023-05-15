[![.NET](https://github.com/VisconFactoryIntelligence/AdsClient/actions/workflows/dotnet.yml/badge.svg)](https://github.com/VisconFactoryIntelligence/AdsClient/actions/workflows/dotnet.yml)

This is the client implementation of the [Twincat](http://www.beckhoff.com/english.asp?twincat/default.htm) Ads protocol from [Beckhoff](http://http://www.beckhoff.com/).   
(I'm not affiliated with Beckhoff)

The implementation is in C# and can be used in .Net >= 4.5, Mono >= 3.2.8, Windows 8.1 WinRT/Phone.  
Because of mono you can use it on unix/linux systems and even on Android/iPhone if you have xamarin.

This library contains sync and async methods.
You can't combine them.

AdsClient is the Portable Class Library.  
AdsClient.WinSock can be used in normal .Net programs and Mono/Xamarin.  
AdsClient.WinRT can be use in Widows 8.1 WinRT/Phone.  

Contributors
============
[MrCircuit](https://github.com/MrCircuit)
[mycroes](https://github.com/mycroes)

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

Here is the NuGet package: https://nuget.org/packages?q=AdsClient

Mono
----
You need mono >= 3.2.8

Xamarin Android: 
Remember to set internet permissions in the manifest.
You must also configure a route for your android device.

External documentation
----------------------

[Specification for the ADS/AMS protocol](http://infosys.beckhoff.com/english.php?content=../content/1033/TcAdsAmsSpec/HTML/TcAdsAmsSpec_Intro.htm&id=)

[Index-Group/Offset specification](http://infosys.beckhoff.com/content/1033/tcadsdeviceplc/html/tcadsdeviceplc_intro.htm?id=11742)

Examples
========

## Simple hello machine

```C#
using (AdsClient client = new AdsClient(
        amsNetIdSource: "10.0.0.120.1.1",
        ipTarget: "10.0.0.2",
        amsNetIdTarget: "5.1.204.160.1.1"))
{
    AdsDeviceInfo deviceInfo = client.ReadDeviceInfo();
    Console.WriteLine(deviceInfo.ToString());
}
```

Async version:

```C#
using (AdsClient client = new AdsClient(
        amsNetIdSource: "10.0.0.120.1.1",
        ipTarget: "10.0.0.2",
        amsNetIdTarget: "5.1.204.160.1.1"))
{
    AdsDeviceInfo deviceInfo = await client.ReadDeviceInfoAsync();
    Console.WriteLine(deviceInfo.ToString());
}
```

Disposing AdsClient may give you first chance exceptions in the output window.
This happens because I'm closing the socket while it's listening for ads packets.
These exceptions are handled in the library and don't cause any problems.
(If someone knows a better way, please let me know)

## Read/Write a variable by name

```C#
using (AdsClient client = new AdsClient(
        amsNetIdSource: "10.0.0.120.1.1",
        ipTarget: "10.0.0.2",
        amsNetIdTarget: "5.1.204.160.1.1"))
{
    uint varHandle = client.GetSymhandleByName(".TestVar");
    client.Write<byte>(varHandle, 0);
    byte value = client.Read<byte>(varHandle);
    client.ReleaseSymhandle(varHandle);
}
```

Async version:

```C#
using (AdsClient client = new AdsClient(
        amsNetIdSource: "10.0.0.120.1.1",
        ipTarget: "10.0.0.2",
        amsNetIdTarget: "5.1.204.160.1.1"))
{
    uint varHandle = await client.GetSymhandleByNameAsync(".TestVar");
    await client.WriteAsync<byte>(varHandle, 0);
    byte value = await client.ReadAsync<byte>(varHandle);
    await client.ReleaseSymhandleAsync(varHandle);
}
```

You can also use the AdsCommands directly if you need to write directly with IndexGroup/IndexOffset

## Working with notifications

```C#
using (AdsClient client = new AdsClient(
        amsNetIdSource: "10.0.0.120.1.1",
        ipTarget: "10.0.0.2",
        amsNetIdTarget: "5.1.204.160.1.1"))
{
  client.OnNotification += (sender, e) => { Console.WriteLine(e.Notification.ToString()); };
  uint hVar1 = client.GetSymhandleByName(".VarTest1");
  uint hVar2 = client.GetSymhandleByName(".VarTest2");
  uint hNoti1 = client.AddNotification<byte>(hVar1, 
                                    AdsTransmissionMode.Cyclic, 2000, null);
  uint hNoti2 = client.AddNotification<byte>(hVar2, 
                                    AdsTransmissionMode.OnChange, 10, null);
  Thread.Sleep(5000);
}
```

## Simple async example with most basic functions

Here is an async example.
The non async functions work the same. (functions without async at the end) 
Just remove all the await/async words

```C#
using System;
using System.Threading;
using System.Threading.Tasks;
using Ads.Client;
using Ads.Client.Common;

namespace AdsTest
{
  class Program
  {
    static void Main(string[] args)
    {
      if (!RunTestAsync().Wait(10000)) 
        Console.WriteLine("Timeout!");
      else
        Console.WriteLine("done");
      Console.ReadKey();
    }

    private static async Task RunTest()
    {
      using (AdsClient client = new AdsClient(
                        amsNetIdSource:"192.168.5.6.1.1",  
                        ipTarget:"192.168.3.4",       
                        amsNetIdTarget:"192.168.3.4.1.1"))  
      {
          AdsDeviceInfo deviceInfo = await client.ReadDeviceInfoAsync();
          Console.WriteLine("Device info: " + deviceInfo.ToString());

          AdsState state = await client.ReadStateAsync();
          Console.WriteLine("State: " + state.ToString());

          client.OnNotification += (sender,e) => { 
                Console.WriteLine(e.Notification.ToString()); 
          };

          uint varHandle1 = await client.GetSymhandleByNameAsync(".VariableName1");
          Console.WriteLine("Variable1 handle: " + varHandle1.ToString());

          uint varHandle2 = await client.GetSymhandleByNameAsync(".VariableName2");
          Console.WriteLine("Variable2 handle: " + varHandle2.ToString());

          uint notification1Handle = await client.AddNotificationAsync<byte>(
                varHandle1, AdsTransmissionMode.Cyclic, 5000, null);
          uint notification2Handle = await client.AddNotificationAsync<byte>(
                varHandle2, AdsTransmissionMode.OnChange, 10, null);

          byte value = await client.ReadAsync<byte>(varHandle1);
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
  }
}
```

## Using commands directly

```C#
AdsReadStateCommand stateCmd = new AdsReadStateCommand();
string state = stateCmd.Run(client.Ams).AdsState.ToString();
Console.WriteLine("State: " + state);
```

## Serialize to class

It's possible to read directly to a class or write from a class.  
You need to set the AdsSerializable attribute on the class and the Ads attribute on the fields/properties you need.  
The fields without the Ads attribute are ignored. 

```C#
[AdsSerializable]
public class TestClass
{
    [Ads]
    public UInt16 Var1 { get; set; }

    [Ads]
    public byte Var2 { get; set; }
}

var testobject = client.Read<TestClass>(handle);
client.Write<TestClass>(handle, testobject);
```

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

```C#
using (AdsClient client = new AdsClient(
        amsNetIdSource: "10.0.0.120.1.1",
        ipTarget: "10.0.0.2",
        amsNetIdTarget: "5.1.204.130.1.1"))
{
  string xml = adsClient.Special.GetTargetDesc();
  xml = XDocument.Parse(xml).ToString();
}
```

Credits, sources and inspiration
================================
* [ads-client NodeJS library by Jussi Isotalo](https://github.com/jisotalo/ads-client)
* [RabbitMQ .NET Client](https://github.com/rabbitmq/rabbitmq-dotnet-client)
* [Sally7](https://github.com/mycroes/Sally7)
