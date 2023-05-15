using System;
using System.Buffers;
using System.Text;

namespace Ads.Client.Helpers
{
    public static class StringHelper
    {
        public static Encoding CP1252 = CodePagesEncodingProvider.Instance.GetEncoding(1252);

        public static int Encode(ReadOnlySpan<char> chars, Span<byte> buffer)
        {
            var len = CP1252.GetBytes(chars, buffer);
            buffer[len] = 0;

            return len + 1;
        }

        public static string Decode(ReadOnlySpan<byte> buffer)
        {
            var end = buffer.IndexOf((byte) 0);
            if (end > -1) buffer = buffer.Slice(0, end);

            return CP1252.GetString(buffer);
        }

        public static string Decode(ReadOnlySequence<byte> buffer)
        {
            return Decode(buffer.First.Span.Length < buffer.Length ? buffer.ToArray() : buffer.First.Span);
        }
    }
}
