using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Viscon.Communication.Ads.Internal;

/// <summary>
/// Serializer to write little endian values to the output stream.
/// </summary>
/// <remarks>
/// Heavily inspired by the TypeExtensions from RabbitMQ.Client
/// (https://github.com/rabbitmq/rabbitmq-dotnet-client).
/// </remarks>
internal static class TypeExtensions
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ref byte GetStart(this Span<byte> span)
    {
        return ref MemoryMarshal.GetReference(span);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ref byte GetOffset(this Span<byte> span, int offset)
    {
        return ref span.GetStart().GetOffset(offset);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ref byte GetOffset(this ref byte source, int offset)
    {
        // The cast to uint is in order to avoid a sign-extending move
        // in the machine code.
        return ref Unsafe.Add(ref source, (uint)offset);
    }
}