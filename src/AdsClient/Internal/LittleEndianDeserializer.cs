using System;
using System.Buffers;
using System.Buffers.Binary;
using System.Runtime.CompilerServices;

namespace Ads.Client.Internal;

/// <summary>
/// Serializer to read little endian values from the input stream.
/// </summary>
/// <remarks>
/// Heavily inspired by the NetworkOrderDeserializer from RabbitMQ.Client
/// (https://github.com/rabbitmq/rabbitmq-dotnet-client).
/// </remarks>
internal static class LittleEndianDeserializer
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ushort ReadUInt16(ReadOnlySpan<byte> span)
    {
        return BinaryPrimitives.ReadUInt16LittleEndian(span);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ushort ReadUInt16(ReadOnlySequence<byte> buffer)
    {
        if (!BinaryPrimitives.TryReadUInt16LittleEndian(buffer.First.Span, out var value))
        {
            Span<byte> bytes = stackalloc byte[4];
            buffer.Slice(0, 4).CopyTo(bytes);
            value = BinaryPrimitives.ReadUInt16LittleEndian(bytes);
        }

        return value;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int ReadInt32(ReadOnlySpan<byte> span)
    {
        return BinaryPrimitives.ReadInt32LittleEndian(span);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static uint ReadUInt32(ReadOnlySpan<byte> span)
    {
        return BinaryPrimitives.ReadUInt32LittleEndian(span);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static uint ReadUInt32(ReadOnlySequence<byte> buffer)
    {
        if (!BinaryPrimitives.TryReadUInt32LittleEndian(buffer.First.Span, out var value))
        {
            Span<byte> bytes = stackalloc byte[4];
            buffer.Slice(0, 4).CopyTo(bytes);
            value = BinaryPrimitives.ReadUInt32LittleEndian(bytes);
        }

        return value;
    }
}