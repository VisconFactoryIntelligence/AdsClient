// ReSharper disable InconsistentNaming
// ReSharper disable IdentifierTypo
// ReSharper disable UnusedMember.Global
// ReSharper disable CommentTypo
namespace Ads.Client.Common;

/// <summary>
/// Specifies the ADS commands.
/// </summary>
/// <remarks>
/// This list is imported from ads-client (https://github.com/jisotalo/ads-client), which sourced
/// most values from the TwinCAT.Ads.dll by Beckhoff.
/// </remarks>
// Temporary name, until AdsCommand from Commands namespace is gone.
public enum AdsCommandEnum : ushort
{
    None = 0,
    ReadDeviceInfo = 1,
    Read = 2,
    Write = 3,
    ReadState = 4,
    WriteControl = 5,
    AddNotification = 6,
    DeleteNotification = 7,
    Notification = 8,
    ReadWrite = 9,
}