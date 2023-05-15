// ReSharper disable InconsistentNaming
// ReSharper disable IdentifierTypo
// ReSharper disable UnusedMember.Global
// ReSharper disable CommentTypo
namespace Ads.Client.Common;

/// <summary>
/// Specifies the ADS data type flags.
/// </summary>
/// <remarks>
/// This list is imported from ads-client (https=//github.com/jisotalo/ads-client), which sourced
/// most values from the TwinCAT.Ads.dll by Beckhoff.
/// </remarks>
public enum AdsDataTypeFlags : uint
{
    None = 0,
    DataType = 1 << 0,
    DataItem = 1 << 1,
    ReferenceTo = 1 << 2,
    MethodDeref = 1 << 3,
    Oversample = 1 << 4,
    BitValues = 1 << 5,
    PropItem = 1 << 6,
    TypeGuid = 1 << 7,
    Persistent = 1 << 8,
    CopyMask = 1 << 9,
    TComInterfacePtr = 1 << 10,
    MethodInfos = 1 << 11,
    Attributes = 1 << 12,
    EnumInfos = 1 << 13,

    /// <summary>
    /// Sets whether the datatype is aligned.
    /// </summary>
    Aligned = 1 << 16,

    /// <summary>
    /// Sets whether the data item is static - do not use offs.
    /// </summary>
    Static = 1 << 17,

    // means "ContainSpLevelss" for DATATYPES and "HasSpLevels" for DATAITEMS
    SpLevels = 1 << 18,

    /// <summary>
    /// Sets whether persistent data is not restored.
    /// </summary>
    IgnorePersist = 1 << 19,

    //Any size array (ADSDATATYPEFLAG_ANYSIZEARRAY)
    // <remarks>
    // If the index is exeeded, a value access to this array will return <see cref="F=TwinCAT.Ads.AdsErrorCode.DeviceInvalidArrayIndex" />
    // </remarks>
    AnySizeArray = 1 << 20,

    /// <summary>
    /// Sets whether the data type is used for persistent variables.
    /// </summary>
    PersistantDatatype = 1 << 21,

    /// <summary>
    /// Sets whether persistent data will not be restored after reset (cold).
    /// </summary>
    InitOnResult = 1 << 22,
}
