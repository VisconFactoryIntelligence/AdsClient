// ReSharper disable InconsistentNaming
// ReSharper disable IdentifierTypo
// ReSharper disable UnusedMember.Global
// ReSharper disable CommentTypo
namespace Ads.Client.Common;

/// <summary>
/// Specifies the reserved index groups.
/// </summary>
/// <remarks>
/// This list is imported from ads-client (https://github.com/jisotalo/ads-client), which sourced
/// most values from the TwinCAT.Ads.dll by Beckhoff.
/// </remarks>
public enum AdsReservedIndexGroup : uint
{
    PlcRWIB = 0x4000,
    PlcRWOB = 0x4010,
    PlcRWMB = 0x4020,
    PlcRWRB = 0x4030,
    PlcRWDB = 0x4040,
    SymbolTable = 0xf000,
    SymbolName = 0xf001,
    SymbolValue = 0xf002,
    SymbolHandleByName = 0xf003,
    SymbolValueByName = 0xf004,
    SymbolValueByHandle = 0xf005,
    SymbolReleaseHandle = 0xf006,
    SymbolInfoByName = 0xf007,
    SymbolVersion = 0xf008,
    SymbolInfoByNameEx = 0xf009,
    SymbolDownload = 0xf00a,
    SymbolUpload = 0xf00b,
    SymbolUploadInfo = 0xf00c,
    SymbolDownload2 = 0xf00d,
    SymbolDataTypeUpload = 0xf00e,
    SymbolUploadInfo2 = 0xf00f, // 24 bytes of info, uploadinfo3 would contain 64 bytes
    SymbolNote = 0xf010,
    DataDataTypeInfoByNameEx = 0xf011,
    IOImageRWIB = 0xf020,
    IOImageRWIX = 0xf021,
    IOImageRWOB = 0xf030,
    IOImageRWOX = 0xf031,
    IOImageClearI = 0xf040,
    IOImageClearO = 0xf050,
    //
    //ADS Sum Read Command (ADSIGRP_SUMUP_READ)
    //
    SumCommandRead = 0xf080,
    //
    //ADS Sum Write Command (ADSIGRP_SUMUP_WRITE)
    //
    SumCommandWrite = 0xf081,
    //
    //ADS sum Read/Write command (ADSIGRP_SUMUP_READWRITE)
    //
    SumCommandReadWrite = 0xf082,
    //
    //ADS sum ReadEx command (ADSIGRP_SUMUP_READEX)
    //AdsRW  IOffs list size
    //W= {list of IGrp, IOffs, Length}
    //R= {list of results, Length} followed by {list of data (expepted lengths)}
    //
    SumCommandReadEx = 0xf083,
    //
    //ADS sum ReadEx2 command (ADSIGRP_SUMUP_READEX2)
    //AdsRW  IOffs list size
    //W= {list of IGrp, IOffs, Length}
    //R= {list of results, Length} followed by {list of data (returned lengths)}
    //
    SumCommandReadEx2 = 0xf084,
    //
    //ADS sum AddDevNote command (ADSIGRP_SUMUP_ADDDEVNOTE)
    //AdsRW  IOffs list size
    //W= {list of IGrp, IOffs, Attrib}
    //R= {list of results, handles}
    //
    SumCommandAddDevNote = 0xf085,
    //
    //ADS sum DelDevNot command (ADSIGRP_SUMUP_DELDEVNOTE)
    //AdsRW  IOffs list size
    //W= {list of handles}
    //R= {list of results}
    //
    SumCommandDelDevNote = 0xf086,
    DeviceData = 0xf100,
}