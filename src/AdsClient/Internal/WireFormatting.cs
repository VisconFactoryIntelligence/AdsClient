using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Ads.Client.Helpers;

namespace Ads.Client.Internal;

/// <summary>
/// Helpers for reading and writing data in the expect wire format for the ADS protocol.
/// </summary>
public static class WireFormatting
{
    public static int ReadInt32(ReadOnlySpan<byte> span, out int value)
    {
        value = LittleEndianDeserializer.ReadInt32(span);
        return sizeof(int);
    }

    public static int ReadString(ReadOnlySpan<byte> span, ushort length, out string value)
    {
        value = StringHelper.Decode(span.Slice(0, length));
        return length + 1;
    }

    public static int ReadUInt16(ReadOnlySpan<byte> span, out ushort value)
    {
        value = LittleEndianDeserializer.ReadUInt16(span);
        return sizeof(ushort);
    }

    public static int ReadUInt32(ReadOnlySpan<byte> span, out uint value)
    {
        value = LittleEndianDeserializer.ReadUInt32(span);
        return sizeof(uint);
    }

    public static int WriteString(Span<byte> destination, string value)
    {
        return StringHelper.Encode(value.AsSpan(), destination);
    }

    public static int WriteUInt16(ref byte destination, ushort value)
    {
        LittleEndianSerializer.WriteUInt16(ref destination, value);
        return sizeof(ushort);
    }

    public static int WriteUInt16<T>(ref byte destination, T value)
    {
        Debug.Assert(Unsafe.SizeOf<T>() == sizeof(ushort),
            $"Passed type {typeof(T)} of size {Unsafe.SizeOf<T>()} to method {nameof(WriteUInt32)}<T>, expected size of {sizeof(ushort)}");

        return WriteUInt16(ref destination, Unsafe.As<T, ushort>(ref value));
    }

    public static int WriteUInt32(ref byte destination, uint value)
    {
        LittleEndianSerializer.WriteUInt32(ref destination, value);
        return sizeof(uint);
    }

    public static int WriteUInt32<T>(ref byte destination, T value) where T : struct
    {
        Debug.Assert(Unsafe.SizeOf<T>() == sizeof(uint), $"Passed type {typeof(T)} of size {Unsafe.SizeOf<T>()} to method {nameof(WriteUInt32)}<T>, expected size of {sizeof(uint)}");

        return WriteUInt32(ref destination, Unsafe.As<T, uint>(ref value));
    }
}