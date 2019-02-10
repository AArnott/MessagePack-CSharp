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
        /// Writes a <see cref="MessagePackCode.Nil"/> value.
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
        /// Write the length of the next array to be written in the most compact form of
        /// <see cref="MessagePackCode.MinFixArray"/>,
        /// <see cref="MessagePackCode.Array16"/>, or
        /// <see cref="MessagePackCode.Array32"/>
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
        /// Writes a 16-bit integer using a built-in 1-byte code when within specific MessagePack-supported ranges,
        /// or the most compact of
        /// <see cref="MessagePackCode.UInt8"/>,
        /// <see cref="MessagePackCode.UInt16"/>,
        /// <see cref="MessagePackCode.Int8"/>, or
        /// <see cref="MessagePackCode.Int16"/>
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
        /// Writes a boolean value using either <see cref="MessagePackCode.True"/> or <see cref="MessagePackCode.False"/>.
        /// </summary>
        /// <param name="value">The value.</param>
        public void WriteBoolean(bool value)
        {
            var span = writer.GetSpan(1);
            span[0] = value ? MessagePackCode.True : MessagePackCode.False;
            writer.Advance(1);
        }

        /// <summary>
        /// Writes an 8-bit value using a 1-byte code when possible, otherwise as <see cref="MessagePackCode.UInt8"/>.
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

        /// <summary>
        /// Writes an 8-bit value using a 1-byte code when possible, otherwise as <see cref="MessagePackCode.Int8"/>.
        /// </summary>
        /// <param name="value">The value.</param>
        public void WriteSByte(sbyte value)
        {
            if (value < MessagePackRange.MinFixNegativeInt)
            {
                var span = writer.GetSpan(2);
                span[0] = MessagePackCode.Int8;
                span[1] = unchecked((byte)(value));
                writer.Advance(2);
            }
            else
            {
                var span = writer.GetSpan(1);
                span[0] = unchecked((byte)value);
                writer.Advance(1);
            }
        }

        /// <summary>
        /// Writes a <see cref="MessagePackCode.Float32"/> value.
        /// </summary>
        /// <param name="value">The value.</param>
        public void WriteSingle(float value)
        {
            var span = writer.GetSpan(5);

            span[0] = MessagePackCode.Float32;

            var num = new Float32Bits(value);
            if (BitConverter.IsLittleEndian)
            {
                span[1] = num.Byte3;
                span[2] = num.Byte2;
                span[3] = num.Byte1;
                span[4] = num.Byte0;
            }
            else
            {
                span[1] = num.Byte0;
                span[2] = num.Byte1;
                span[3] = num.Byte2;
                span[4] = num.Byte3;
            }

            writer.Advance(5);
        }

        /// <summary>
        /// Writes a <see cref="MessagePackCode.Float64"/> value.
        /// </summary>
        /// <param name="value">The value.</param>
        public void WriteDouble(double value)
        {
            var span = writer.GetSpan(9);

            span[0] = MessagePackCode.Float64;

            var num = new Float64Bits(value);
            if (BitConverter.IsLittleEndian)
            {
                span[1] = num.Byte7;
                span[2] = num.Byte6;
                span[3] = num.Byte5;
                span[4] = num.Byte4;
                span[5] = num.Byte3;
                span[6] = num.Byte2;
                span[7] = num.Byte1;
                span[8] = num.Byte0;
            }
            else
            {
                span[1] = num.Byte0;
                span[2] = num.Byte1;
                span[3] = num.Byte2;
                span[4] = num.Byte3;
                span[5] = num.Byte4;
                span[6] = num.Byte5;
                span[7] = num.Byte6;
                span[8] = num.Byte7;
            }

            writer.Advance(9);
        }

        /// <summary>
        /// Writes a span of bytes, prefixed with a length encoded as the smallest fitting from:
        /// <see cref="MessagePackCode.Bin8"/>,
        /// <see cref="MessagePackCode.Bin16"/>, or
        /// <see cref="MessagePackCode.Bin32"/>,
        /// </summary>
        /// <param name="src">The span of bytes to write.</param>
        public void WriteBytes(ReadOnlySpan<byte> src)
        {
            if (src.Length <= byte.MaxValue)
            {
                var size = src.Length + 2;
                var span = writer.GetSpan(size);

                span[0] = MessagePackCode.Bin8;
                span[1] = (byte)src.Length;

                src.CopyTo(span.Slice(2));
                writer.Advance(size);
            }
            else if (src.Length <= UInt16.MaxValue)
            {
                var size = src.Length + 3;
                var span = writer.GetSpan(size);

                unchecked
                {
                    span[0] = MessagePackCode.Bin16;
                    span[1] = (byte)(src.Length >> 8);
                    span[2] = (byte)(src.Length);
                }

                src.CopyTo(span.Slice(3));
                writer.Advance(size);
            }
            else
            {
                var size = src.Length + 5;
                var span = writer.GetSpan(size);

                unchecked
                {
                    span[0] = MessagePackCode.Bin32;
                    span[1] = (byte)(src.Length >> 24);
                    span[2] = (byte)(src.Length >> 16);
                    span[3] = (byte)(src.Length >> 8);
                    span[4] = (byte)(src.Length);
                }

                src.CopyTo(span.Slice(5));
                writer.Advance(size);
            }
        }
    }
}
