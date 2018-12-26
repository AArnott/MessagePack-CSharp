using System;
using System.Text;

namespace MessagePack
{
    internal static class StringEncoding
    {
        internal static readonly Encoding UTF8 = new UTF8Encoding(false);

        internal static unsafe int GetBytes(this Encoding encoding, ReadOnlySpan<char> chars, Span<byte> bytes)
        {
            fixed (char* pChars = &chars[0])
            fixed (byte* pBytes = &bytes[0])
            {
                return encoding.GetBytes(pChars, chars.Length, pBytes, bytes.Length);
            }
        }

        internal static unsafe int GetBytes(this Encoding encoding, string chars, Span<byte> bytes) => GetBytes(encoding, chars.AsSpan(), bytes);
    }
}
