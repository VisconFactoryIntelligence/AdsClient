using System;
using System.Buffers.Binary;
using System.Runtime.CompilerServices;

namespace Ads.Client.Internal;

/// <summary>
/// Serializer to write little endian values to the output stream.
/// </summary>
/// <remarks>
/// Heavily inspired by the NetworkOrderSerializer from RabbitMQ.Client
/// (https://github.com/rabbitmq/rabbitmq-dotnet-client).
/// </remarks>
internal static class LittleEndianSerializer
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void WriteUInt16(ref byte destination, ushort val)
    {
        Unsafe.WriteUnaligned(ref destination,
            BitConverter.IsLittleEndian ? val : BinaryPrimitives.ReverseEndianness(val));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void WriteUInt32(ref byte destination, uint val)
    {
        Unsafe.WriteUnaligned(ref destination,
            BitConverter.IsLittleEndian ? val : BinaryPrimitives.ReverseEndianness(val));
    }
}