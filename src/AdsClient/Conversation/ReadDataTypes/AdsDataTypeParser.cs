using System;
using Ads.Client.Common;
using Ads.Client.Internal;

namespace Ads.Client.Conversation.ReadDataTypes;

internal static class AdsDataTypeParser
{
    public static AdsDataTypeDto ParseDataType(ReadOnlySpan<byte> buffer)
    {
        var offset = WireFormatting.ReadUInt32(buffer, out var version);
        offset += WireFormatting.ReadUInt32(buffer.Slice(offset), out var hashValue);
        offset += WireFormatting.ReadUInt32(buffer.Slice(offset), out var baseTypeHashValue);
        offset += WireFormatting.ReadUInt32(buffer.Slice(offset), out var size);
        offset += WireFormatting.ReadUInt32(buffer.Slice(offset), out var typeOffset);
        offset += WireFormatting.ReadUInt32(buffer.Slice(offset), out var adsDataType);
        offset += WireFormatting.ReadUInt32(buffer.Slice(offset), out var flags);
        offset += WireFormatting.ReadUInt16(buffer.Slice(offset), out var nameLength);
        offset += WireFormatting.ReadUInt16(buffer.Slice(offset), out var typeLength);
        offset += WireFormatting.ReadUInt16(buffer.Slice(offset), out var commentLength);
        offset += WireFormatting.ReadUInt16(buffer.Slice(offset), out var arrayDimensions);
        offset += WireFormatting.ReadUInt16(buffer.Slice(offset), out var subItemCount);
        offset += WireFormatting.ReadString(buffer.Slice(offset), nameLength, out var name);
        offset += WireFormatting.ReadString(buffer.Slice(offset), typeLength, out var type);
        offset += WireFormatting.ReadString(buffer.Slice(offset), commentLength, out var comment);
        offset += ReadArrayRanges(buffer.Slice(offset), arrayDimensions, out var arrayRanges);
        offset += ReadSubItems(buffer.Slice(offset), subItemCount, out var subItems);

        // End here for now, there's more data available.

        return new AdsDataTypeDto(version, hashValue, baseTypeHashValue, size, typeOffset, adsDataType.ToDataType(),
            flags.ToDataTypeFlags(), name, type, comment, arrayRanges, subItems);
    }

    private static int ReadArrayRanges(ReadOnlySpan<byte> buffer, ushort dimensions, out AdsArrayRange[] ranges)
    {
        ranges = new AdsArrayRange[dimensions];
        var offset = 0;
        for (var i = 0; i < dimensions; i++)
        {
            offset += WireFormatting.ReadUInt32(buffer.Slice(offset), out var start);
            offset += WireFormatting.ReadUInt32(buffer.Slice(offset), out var length);

            ranges[i] = new AdsArrayRange(start, length);
        }

        return offset;
    }

    private static int ReadSubItems(ReadOnlySpan<byte> buffer, ushort count, out AdsDataTypeDto[] subItems)
    {
        subItems = new AdsDataTypeDto[count];
        var offset = 0;
        for (var i = 0; i < count; i++)
        {
            WireFormatting.ReadInt32(buffer, out var len);
            subItems[i] = ParseDataType(buffer.Slice(4, len - 4));

            offset += len;
            buffer = buffer.Slice(len);
        }

        return offset;
    }
}

/*

//Subitems data
dataType.subItems = []
for (let i = 0; i < dataType.subItemCount; i++) {
//Get subitem length
let len = data.readUInt32LE(pos)
pos += 4

//Parse the subitem recursively
dataType.subItems.push(await _parseDataType.call(this, data.slice(pos, pos + len)))

pos += (len - 4)
}

//If flags contain TypeGuid
if (dataType.flagsStr.includes('TypeGuid')) {
dataType.typeGuid = data.slice(pos, pos + 16).toString('hex')
pos += 16
}

//If flags contain CopyMask
if (dataType.flagsStr.includes('CopyMask')) {
//Let's skip this for now
pos += dataType.size
}

dataType.rpcMethods = []

//If flags contain MethodInfos (TwinCAT.Ads.dll: AdsMethodEntry)
if (dataType.flagsStr.includes('MethodInfos')) {
dataType.methodCount = data.readUInt16LE(pos)
pos += 2

//RPC methods
for (let i = 0; i < dataType.methodCount; i++) {
const method = {}

//Get method length
let len = data.readUInt32LE(pos)
pos += 4

//4..7 Version
method.version = data.readUInt32LE(pos)
pos += 4

//8..11 Virtual table index
method.vTableIndex = data.readUInt32LE(pos)
pos += 4

//12..15 Return size
method.returnSize = data.readUInt32LE(pos)
pos += 4

//16..19 Return align size
method.returnAlignSize = data.readUInt32LE(pos)
pos += 4

//20..23 Reserved
method.reserved = data.readUInt32LE(pos)
pos += 4

//24..27 Return type GUID
method.returnTypeGuid = data.slice(pos, pos + 16).toString('hex')
pos += 16

//28..31 Return data type
method.retunAdsDataType = data.readUInt32LE(pos)
method.retunAdsDataTypeStr = ADS.ADS_DATA_TYPES.toString(method.retunAdsDataType)
pos += 4

//27..30 Flags (AdsDataTypeFlags)
method.flags = data.readUInt16LE(pos)
method.flagsStr = ADS.ADS_DATA_TYPE_FLAGS.toStringArray(method.flags)
pos += 4

//31..32 Name length
method.nameLength = data.readUInt16LE(pos)
pos += 2

//33..34 Return type length
method.returnTypeLength = data.readUInt16LE(pos)
pos += 2

//35..36 Comment length
method.commentLength = data.readUInt16LE(pos)
pos += 2

//37..38 Parameter count
method.parameterCount = data.readUInt16LE(pos)
pos += 2

//39.... Name
method.name = _trimPlcString(iconv.decode(data.slice(pos, pos + method.nameLength + 1), 'cp1252'))
pos += method.nameLength + 1

//...... Return type
method.returnType = _trimPlcString(iconv.decode(data.slice(pos, pos + method.returnTypeLength + 1), 'cp1252'))
pos += method.returnTypeLength + 1

//...... Comment
method.comment = _trimPlcString(iconv.decode(data.slice(pos, pos + method.commentLength + 1), 'cp1252'))
pos += method.commentLength + 1

method.parameters = []

for (let p = 0; p < method.parameterCount; p++) {
const param = {}

let paramStartPos = pos

//Get parameter length
let paramLen = data.readUInt32LE(pos)
pos += 4

//4..7 Size
param.size = data.readUInt32LE(pos)
pos += 4

//8..11 Align size
param.alignSize = data.readUInt32LE(pos)
pos += 4

//12..15 Data type
param.adsDataType = data.readUInt32LE(pos)
param.adsDataTypeStr = ADS.ADS_DATA_TYPES.toString(param.adsDataType)
pos += 4

//16..19 Flags (RCP_METHOD_PARAM_FLAGS)
param.flags = data.readUInt16LE(pos)
param.flagsStr = ADS.RCP_METHOD_PARAM_FLAGS.toStringArray(param.flags)
pos += 4

//20..23 Reserved
param.reserved = data.readUInt32LE(pos)
pos += 4

//24..27 Type GUID
param.typeGuid = data.slice(pos, pos + 16).toString('hex')
pos += 16

//28..31 LengthIsPara
param.lengthIsPara = data.readUInt16LE(pos)
pos += 2

//32..33 Name length
param.nameLength = data.readUInt16LE(pos)
pos += 2

//34..35 Type length
param.typeLength = data.readUInt16LE(pos)
pos += 2

//36..37 Comment length
param.commentLength = data.readUInt16LE(pos)
pos += 2

//38.... Name
param.name = _trimPlcString(iconv.decode(data.slice(pos, pos + param.nameLength + 1), 'cp1252'))
pos += param.nameLength + 1

//...... Type
param.type = _trimPlcString(iconv.decode(data.slice(pos, pos + param.typeLength + 1), 'cp1252'))
pos += param.typeLength + 1

//...... Comment
param.comment = _trimPlcString(iconv.decode(data.slice(pos, pos + param.commentLength + 1), 'cp1252'))
pos += param.commentLength + 1

if (pos - paramStartPos > paramLen) {
  //There is some additional data
  param.reserved2 = data.slice(pos)
}
method.parameters.push(param)
}

dataType.rpcMethods.push(method)
}
}

//If flags contain Attributes (TwinCAT.Ads.dll: AdsAttributeEntry)
//Attribute is for example, a pack-mode attribute above struct
dataType.attributes = []
if (dataType.flagsStr.includes('Attributes')) {
dataType.attributeCount = data.readUInt16LE(pos)
pos += 2

//Attributes
for (let i = 0; i < dataType.attributeCount; i++) {
let attr = {}

//Name length
let nameLen = data.readUInt8(pos)
pos += 1

//Value length
let valueLen = data.readUInt8(pos)
pos += 1

//Name
attr.name = _trimPlcString(iconv.decode(data.slice(pos, pos + nameLen + 1), 'cp1252'))
pos += (nameLen + 1)

//Value
attr.value = _trimPlcString(iconv.decode(data.slice(pos, pos + valueLen + 1), 'cp1252'))
pos += (valueLen + 1)

dataType.attributes.push(attr)
}
}


//If flags contain EnumInfos (TwinCAT.Ads.dll: AdsEnumInfoEntry)
//EnumInfo contains the enumeration values as string
if (dataType.flagsStr.includes('EnumInfos')) {
dataType.enumInfoCount = data.readUInt16LE(pos)
pos += 2
//EnumInfos
dataType.enumInfo = []
for (let i = 0; i < dataType.enumInfoCount; i++) {
let enumInfo = {}

//Name length
let nameLen = data.readUInt8(pos)
pos += 1

//Name
enumInfo.name = _trimPlcString(iconv.decode(data.slice(pos, pos + nameLen + 1), 'cp1252'))
pos += (nameLen + 1)

//Value
enumInfo.value = data.slice(pos, pos + dataType.size)
pos += dataType.size

dataType.enumInfo.push(enumInfo)
}
}

//Reserved, if any
dataType.reserved = data.slice(pos)

return dataType
}
 */