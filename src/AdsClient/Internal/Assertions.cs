using System;

namespace Viscon.Communication.Ads.Internal;

internal class Assertions
{
    public static void AssertDataLength(ReadOnlySpan<byte> buffer, int length, int offset)
    {
        if (length != buffer.Length - offset)
        {
            throw new Exception(
                $"Received {buffer.Length} bytes of data, but length indicates {length} bytes remaining at offset {offset}, resulting in a expected total of {length + offset} bytes.");
        }
    }

    public static void AssertTimeoutIsValid(TimeSpan value)
    {
        var totalMilliseconds = (long)value.TotalMilliseconds;
        if (totalMilliseconds is < -1 or > int.MaxValue)
        {
            ThrowTimeoutIsInvalid(value);
        }
    }

    private static void ThrowTimeoutIsInvalid(TimeSpan value) =>
        throw new ArgumentOutOfRangeException(nameof(value),
            $"The timeout {value.TotalMilliseconds}ms is less than -1 or greater than Int32.MaxValue.");
}