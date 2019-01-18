using System.Buffers;
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
        /// <typeparam name="T">The type of element in the sequence to be copied.</typeparam>
        /// <param name="source">The sequence to be copied.</param>
        /// <param name="writer">The writer to copy to.</param>
        internal static void CopyTo<T>(this ReadOnlySequence<T> source, IBufferWriter<T> writer)
        {
            Requires.NotNull(writer, nameof(writer));

            foreach (var segment in source)
            {
                writer.Write(segment.Span);
            }
        }
    }
}
