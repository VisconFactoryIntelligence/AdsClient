using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Ads.Client.CommandResponse;
using Ads.Client.Common;
using Ads.Client.Internal;

namespace Ads.Client.Commands;

public class AdsReadMultipleCommand : AdsCommand
{
    private const uint SumCommandRead = 0xF080;

    private readonly AdsSymbol[] symbols;

    public AdsReadMultipleCommand(AdsSymbol[] symbols) : base(AdsCommandId.ReadWrite)
    {
        this.symbols = symbols;
    }

    internal override IEnumerable<byte> GetBytes()
    {
        return BitConverter.GetBytes(SumCommandRead)
            .Concat(BitConverter.GetBytes(symbols.Length))
            .Concat(BitConverter.GetBytes((int)(4 * symbols.Length + symbols.Sum(s => s.Size))))
            .Concat(BitConverter.GetBytes(12 * symbols.Length))
            .Concat(FormatSymbolAddresses());
    }

    private byte[] FormatSymbolAddresses()
    {
        var res = new byte[symbols.Length * 12];
        var span = res.AsSpan();
        for (var i = 0; i < symbols.Length; i++)
        {
            var slice = span.Slice(i * 12);
            var symbol = symbols[i];
            BinaryPrimitives.WriteUInt32LittleEndian(slice, symbol.IndexGroup);
            BinaryPrimitives.WriteUInt32LittleEndian(slice.Slice(4), symbol.IndexOffset);
            BinaryPrimitives.WriteUInt32LittleEndian(slice.Slice(8), symbol.Size);
        }

        return res;
    }

    public async Task<ReadMultiResult[]> RunAsync(Ams ams, CancellationToken cancellationToken)
    {
        var res = await RunAsync<AdsCommandResponse>(ams, cancellationToken).ConfigureAwait(false);

        return ProcessAdsResponse(res.AdsResponse);
    }

    private ReadMultiResult[] ProcessAdsResponse(Span<byte> data)
    {
        var offset = sizeof(uint); // ErrorCode is first, but has already been validated.
        offset += WireFormatting.ReadUInt32(data.Slice(offset), out var length);

        if (length + offset != data.Length)
        {
            throw new Exception(
                $"Length field indicates response should have {length} bytes of data, but actual length is {data.Length - offset}");
        }

        data = data.Slice(offset);

        var items = new ReadMultiResult[symbols.Length];
        var returnCodeSpan = data.Slice(0, symbols.Length * 4);
        var dataSpan = data.Slice(returnCodeSpan.Length);

        for (var i = 0; i < symbols.Length; i++)
        {
            var symbol = symbols[i];
            var symbolSize = (int)symbol.Size;

            items[i] = new ReadMultiResult(symbol, dataSpan.Slice(0, symbolSize).ToArray(),
                BinaryPrimitives.ReadUInt32LittleEndian(returnCodeSpan));

            dataSpan = dataSpan.Slice(symbolSize);
            returnCodeSpan = returnCodeSpan.Slice(4);
        }

        return items;
    }

    public class ReadMultiResult
    {
        public ReadMultiResult(AdsSymbol symbol, byte[] data, uint errorCode)
        {
            Symbol = symbol;
            Data = data;
            ErrorCode = errorCode;
        }

        public AdsSymbol Symbol { get; }
        public byte[] Data { get; }
        public uint ErrorCode { get; }
    }
}