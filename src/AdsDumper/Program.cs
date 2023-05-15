using Viscon.Communication.Ads;

using var client = new AdsClient("172.31.20.201.1.1", "172.31.20.20", "172.31.20.20.1.1");
try
{
    await client.Ams.ConnectAsync();
    Console.WriteLine("Connected");

    Console.WriteLine();
    Console.WriteLine("Reading device info...");
    var deviceInfo = await client.ReadDeviceInfoAsync();
    Console.WriteLine(deviceInfo);

    Console.WriteLine();
    Console.WriteLine("Reading device state...");
    var deviceState = await client.ReadStateAsync();
    Console.WriteLine(deviceState);

    Console.WriteLine();
    Console.WriteLine("Reading symbols...");
    var symbols = await client.GetSymbolsAsync();

    Console.WriteLine($"Name\tTypeName (size)\tComment\tIndexGroup\tIndexOffset");
    foreach (var symbol in symbols)
    {
        Console.WriteLine(
            $"{symbol.Name}\t{symbol.TypeName} ({symbol.Size})\t{symbol.Comment}\t{symbol.IndexGroup}\t{symbol.IndexOffset}");
    }

    Console.WriteLine();
    Console.WriteLine("Reading types...");
    var types = await client.GetDataTypesAsync();

    Console.WriteLine("Name\tType (size)\tComment\tDataType\tVersion");
    foreach (var type in types)
    {
        Console.WriteLine($"{type.Name}\t{type.Type} ({type.Size})\t{type.Comment}\t{type.DataType}\t{type.Version}");
        foreach (var subItem in type.SubItems)
        {
            Console.WriteLine(
                $"- {subItem.Name}\t{subItem.Type} ({subItem.Size})\t{subItem.Comment}\t{subItem.DataType}\t{subItem.Version}");
        }
    }
}
catch (Exception e)
{
    Console.WriteLine($"Unhandled exception occurred: {e}");
    Environment.Exit(1);
}
