using System;
using System.Buffers;
using System.Collections.Generic;
using System.Text;

namespace MessagePack
{
    /// <summary>
    /// An <see cref="IBufferWriter{T}"/> that reserves some fixed size for a header.
    /// </summary>
    /// <typeparam name="T">The type of element written by this writer.</typeparam>
    /// <remarks>
    /// This type is used for inserting the length of list in the header when the length is not known beforehand.
    /// It is optimized to minimize or avoid copying.
    /// </remarks>
    internal class PrefixingBufferWriter<T> : IBufferWriter<T>
    {
        /// <summary>
        /// The underlying buffer writer.
        /// </summary>
        private readonly IBufferWriter<T> innerWriter;
        private readonly int expectedPrefixSize;
        private readonly int payloadSizeHint;
        private Memory<T> prefixMemory;
        private Memory<T> realMemory;
        private int advanced;

        /// <summary>
        /// Initializes a new instance of the <see cref="PrefixingBufferWriter{T}"/> class.
        /// </summary>
        /// <param name="innerWriter">The underlying writer that should ultimately receive the prefix and payload.</param>
        /// <param name="prefixSize">The length of the header to reserve space for. Must be a positive number.</param>
        /// <param name="payloadSizeHint">A hint at the expected max size of the payload. The real size may be more or less than this, but additional copying is avoided if it does not exceed this amount. If 0, a reasonable guess is made.</param>
        internal PrefixingBufferWriter(IBufferWriter<T> innerWriter, int prefixSize, int payloadSizeHint)
        {
            if (prefixSize <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(prefixSize));
            }

            this.innerWriter = innerWriter ?? throw new ArgumentNullException(nameof(innerWriter));
            this.expectedPrefixSize = prefixSize;
            this.payloadSizeHint = payloadSizeHint;
        }

        public void Advance(int count)
        {
            this.advanced += count;
        }

        public Memory<T> GetMemory(int sizeHint = 0)
        {
            this.EnsureInitialized(sizeHint);
            return this.realMemory.Slice(this.advanced);
        }

        public Span<T> GetSpan(int sizeHint = 0)
        {
            this.EnsureInitialized(sizeHint);
            return this.realMemory.Span.Slice(this.advanced);
        }

        public void Complete(ReadOnlySpan<T> prefix)
        {
            if (prefix.Length != this.expectedPrefixSize)
            {
                throw new ArgumentOutOfRangeException(nameof(prefix), "Prefix was not expected length.");
            }

            if (this.prefixMemory.Length == 0)
            {
                // No payload was actually written, and we never requested memory, so just write it out.
                this.innerWriter.Write(prefix);
            }
            else
            {
                // Payload has been written, so write in the prefix then commit the payload.
                prefix.CopyTo(this.prefixMemory.Span);
                this.innerWriter.Advance(this.advanced);
            }
        }

        private void EnsureInitialized(int sizeHint)
        {
            if (this.prefixMemory.Length == 0)
            {
                int sizeToRequest = this.expectedPrefixSize + Math.Max(sizeHint, this.payloadSizeHint);
                var memory = this.innerWriter.GetMemory(sizeToRequest);
                this.prefixMemory = memory.Slice(0, this.expectedPrefixSize);
                this.realMemory = memory.Slice(this.expectedPrefixSize);
            }
        }
    }
}
