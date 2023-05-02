using System.Runtime.CompilerServices;
using Ads.Client.Common;

namespace Ads.Client.Internal;

internal static class EnumExtensions
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static uint ToUInt32(this AdsReservedIndexGroup indexGroup) =>
        Unsafe.As<AdsReservedIndexGroup, uint>(ref indexGroup);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static AdsDataType ToDataType(this uint value) => Unsafe.As<uint, AdsDataType>(ref value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static AdsDataTypeFlags ToDataTypeFlags(this uint value) => Unsafe.As<uint, AdsDataTypeFlags>(ref value);
}