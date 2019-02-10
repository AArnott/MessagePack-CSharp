using MessagePack.Formatters;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.Text;

namespace MessagePack
{
    /// <summary>
    /// A primitive types writer for the MessagePack format.
    /// </summary>
    /// <typeparam name="T">The type of buffer writer in use. Use of a concrete type avoids cost of interface dispatch.</typeparam>
    public ref struct MessagePackWriter<T> where T : IBufferWriter<byte>
    {
        /// <summary>
        /// The writer to use.
        /// </summary>
        private BufferWriter writer;

        /// <summary>
        /// Initializes a new instance of the <see cref="MessagePackWriter{T}"/> struct.
        /// </summary>
        /// <param name="writer">The writer to use.</param>
        public MessagePackWriter(IBufferWriter<byte> writer)
        {
            this.writer = new BufferWriter(writer);
        }
        
        /// <summary>
        /// Writes a nil value.
        /// </summary>
        public void WriteNil()
        {
            var span = writer.GetSpan(1);
            span[0] = MessagePackCode.Nil;
            writer.Advance(1);
        }

        /// <summary>
        /// Copies bytes directly into the message pack writer.
        /// </summary>
        /// <param name="rawMessagePackBlock">The span of bytes to copy from.</param>
        public void WriteRaw(ReadOnlySpan<byte> rawMessagePackBlock) => writer.Write(rawMessagePackBlock);

        /// <summary>
        /// Write the length of the next array to be written.
        /// </summary>
        public void WriteArrayHeader(uint count)
        {
            if (count <= MessagePackRange.MaxFixArrayCount)
            {
                var span = writer.GetSpan(1);
                span[0] = (byte)(MessagePackCode.MinFixArray | count);
                writer.Advance(1);
            }
            else if (count <= ushort.MaxValue)
            {
                var span = writer.GetSpan(3);
                unchecked
                {
                    span[0] = MessagePackCode.Array16;
                    span[1] = (byte)(count >> 8);
                    span[2] = (byte)(count);
                }
                writer.Advance(3);
            }
            else
            {
                var span = writer.GetSpan(5);
                unchecked
                {
                    span[0] = MessagePackCode.Array32;
                    span[1] = (byte)(count >> 24);
                    span[2] = (byte)(count >> 16);
                    span[3] = (byte)(count >> 8);
                    span[4] = (byte)(count);
                }
                writer.Advance(5);
            }
        }

        /// <summary>
        /// Writes a 16-bit integer.
        /// </summary>
        /// <param name="value">The value to write.</param>
        public void WriteInt16(short value)
        {
            if (value >= 0)
            {
                // positive int(use uint)
                if (value <= MessagePackRange.MaxFixPositiveInt)
                {
                    var span = writer.GetSpan(1);
                    span[0] = unchecked((byte)value);
                    writer.Advance(1);
                }
                else if (value <= byte.MaxValue)
                {
                    var span = writer.GetSpan(2);
                    span[0] = MessagePackCode.UInt8;
                    span[1] = unchecked((byte)value);
                    writer.Advance(2);
                }
                else
                {
                    var span = writer.GetSpan(3);
                    span[0] = MessagePackCode.UInt16;
                    span[1] = unchecked((byte)(value >> 8));
                    span[2] = unchecked((byte)value);
                    writer.Advance(3);
                }
            }
            else
            {
                // negative int(use int)
                if (MessagePackRange.MinFixNegativeInt <= value)
                {
                    var span = writer.GetSpan(1);
                    span[0] = unchecked((byte)value);
                    writer.Advance(1);
                }
                else if (sbyte.MinValue <= value)
                {
                    var span = writer.GetSpan(2);
                    span[0] = MessagePackCode.Int8;
                    span[1] = unchecked((byte)value);
                    writer.Advance(2);
                }
                else
                {
                    var span = writer.GetSpan(3);
                    span[0] = MessagePackCode.Int16;
                    span[1] = unchecked((byte)(value >> 8));
                    span[2] = unchecked((byte)value);
                    writer.Advance(3);
                }
            }
        }

        /// <summary>
        /// Writes a boolean value.
        /// </summary>
        /// <param name="value">The value.</param>
        public void WriteBoolean(bool value)
        {
            var span = writer.GetSpan(1);
            span[0] = value ? MessagePackCode.True : MessagePackCode.False;
            writer.Advance(1);
        }

        /// <summary>
        /// Writes an 8-bit value.
        /// </summary>
        /// <param name="value">The value.</param>
        public void WriteByte(byte value)
        {
            if (value <= MessagePackCode.MaxFixInt)
            {
                var span = writer.GetSpan(1);
                span[0] = value;
                writer.Advance(1);
            }
            else
            {
                var span = writer.GetSpan(2);
                span[0] = MessagePackCode.UInt8;
                span[1] = value;
                writer.Advance(2);
            }
        }
    }
}
