using System;
using System.Buffers;

namespace MessagePack
{
    /// <summary>
    /// An <see cref="IBufferWriter{T}"/> implementation that writes to a preallocated area in memory.
    /// </summary>
    /// <typeparam name="T">The type of element to be written.</typeparam>
    internal class MemoryBufferWriter<T> : IBufferWriter<T>
    {
        private Memory<T> remainingMemory;

        /// <summary>
        /// Initializes a new instance of the <see cref="MemoryBufferWriter{T}"/> class.
        /// </summary>
        /// <param name="memory">The memory to write to.</param>
        internal MemoryBufferWriter(Memory<T> memory)
        {
            this.remainingMemory = memory;
        }

        public void Advance(int count)
        {
            this.remainingMemory = this.remainingMemory.Slice(count);
        }

        public Memory<T> GetMemory(int sizeHint = 0)
        {
            CheckAvailableMemory(sizeHint);
            return this.remainingMemory;
        }

        public Span<T> GetSpan(int sizeHint = 0)
        {
            CheckAvailableMemory(sizeHint);
            return this.remainingMemory.Span;
        }

        private void CheckAvailableMemory(int sizeHint)
        {
            if (sizeHint > this.remainingMemory.Length)
            {
                throw new OutOfMemoryException("This writer has a fixed amount of memory available, which is below the requested size.");
            }
        }
    }
}
