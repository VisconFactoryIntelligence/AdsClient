// ReSharper disable InconsistentNaming
// ReSharper disable IdentifierTypo
// ReSharper disable UnusedMember.Global
// ReSharper disable CommentTypo
namespace Viscon.Communication.Ads.Common;

/// <summary>
/// Specifies the ADS data types.
/// </summary>
/// <remarks>
/// This list is imported from ads-client (https://github.com/jisotalo/ads-client), which sourced
/// most values from the TwinCAT.Ads.dll by Beckhoff.
/// </remarks>
public enum AdsDataType : uint
{
    /// <summary>Empty Type</summary>
    ADST_VOID = 0,
    /// <summary>Integer 16 Bit</summary>
    ADST_INT16 = 2,
    /// <summary>Integer 32 Bit</summary>
    ADST_INT32 = 3,
    /// <summary>Real (32 Bit)</summary>
    ADST_REAL32 = 4,
    /// <summary>Real 64 Bit</summary>
    ADST_REAL64 = 5,
    /// <summary>Integer 8 Bit</summary>
    ADST_INT8 = 16,
    /// <summary>Unsigned integer 8 Bit</summary>
    ADST_UINT8 = 17,
    /// <summary>Unsigned integer 16 Bit</summary>
    ADST_UINT16 = 18,
    /// <summary>Unsigned Integer 32 Bit</summary>
    ADST_UINT32 = 19,
    /// <summary>LONG Integer 64 Bit</summary>
    ADST_INT64 = 20,
    /// <summary>Unsigned Long integer 64 Bit</summary>
    ADST_UINT64 = 21,
    /// <summary>STRING</summary>
    ADST_STRING = 30,
    /// <summary>WSTRING</summary>
    ADST_WSTRING = 31,
    /// <summary>ADS REAL80</summary>
    ADST_REAL80 = 32,
    /// <summary>ADS BIT</summary>
    ADST_BIT = 33,
    /// <summary>Internal Only</summary>
    ADST_MAXTYPES = 34,
    /// <summary>Blob</summary>
    ADST_BIGTYPE = 65,
}
