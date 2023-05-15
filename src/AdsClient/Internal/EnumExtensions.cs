using System.Runtime.CompilerServices;
using Viscon.Communication.Ads.Common;

namespace Viscon.Communication.Ads.Internal;

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