// ReSharper disable once CheckNamespace
namespace System.Text;

#if !NETSTANDARD2_1_OR_GREATER

internal static class EncodingExtensions
{
    public static int GetBytes(this Encoding encoding, ReadOnlySpan<char> chars, Span<byte> buffer)
    {
        var bufferArr = new byte[buffer.Length];

        var len = encoding.GetBytes(chars.ToArray(), 0, chars.Length, bufferArr, 0);

        bufferArr.AsSpan().CopyTo(buffer);

        return len;
    }

    public static string GetString(this Encoding encoding, ReadOnlySpan<byte> buffer)
    {
        return encoding.GetString(buffer.ToArray());
    }
}

#endif
