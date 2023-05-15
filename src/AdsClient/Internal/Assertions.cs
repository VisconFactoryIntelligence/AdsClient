using System;
using Ads.Client.Common;

namespace Ads.Client.Internal;

internal class Assertions
{
    public static void AssertDataLength(ReadOnlySpan<byte> buffer, int length, int offset)
    {
        if (length + offset != buffer.Length)
        {
            throw new Exception(
                $"Received {buffer.Length} bytes of data, but length indicates {length} bytes remaining at offset {offset}, resulting in a expected total of {length + offset} bytes.");
        }
    }
}