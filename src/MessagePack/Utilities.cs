using System.Buffers;
using MessagePack.Formatters;
using Microsoft;

namespace MessagePack
{
    /// <summary>
    /// Internal utilities and extension methods for various external types.
    /// </summary>
    internal static class Utilities
    {
        /// <summary>
        /// Writes a sequence to the specified writer.
        /// </summary>
        /// <param name="source">The sequence to be copied.</param>
        /// <param name="writer">The writer to copy to.</param>
        internal static void CopyTo(this ReadOnlySequence<byte> source, ref BufferWriter writer)
        {
            foreach (var segment in source)
            {
                writer.Write(segment.Span);
            }
        }
    }
}
