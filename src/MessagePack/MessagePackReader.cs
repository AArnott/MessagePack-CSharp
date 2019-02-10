using System;
using System.Buffers;
using System.IO;
using System.Runtime.InteropServices;
using MessagePack.Decoders;
using MessagePack.Formatters;
using MessagePack.Internal;

namespace MessagePack
{
    public ref struct MessagePackReader
    {
        /// <summary>
        /// The reader over the sequence.
        /// </summary>
        private SequenceReader<byte> reader;

        /// <summary>
        /// Initializes a new instance of the <see cref="MessagePackReader"/> struct.
        /// </summary>
        /// <param name="readOnlySequence">The sequence to read from.</param>
        public MessagePackReader(ReadOnlySequence<byte> readOnlySequence)
        {
            this.reader = new SequenceReader<byte>(readOnlySequence);
        }

        /// <summary>
        /// Checks whether the reader position is pointing at a nil value.
        /// </summary>
        public bool IsNil => this.reader.TryPeek(out byte code) && code == MessagePackCode.Nil;

        /// <summary>
        /// Reads a <see cref="MessagePackCode.Nil"/> value.
        /// </summary>
        /// <returns>A nil value.</returns>
        public Nil ReadNil()
        {
            ThrowInsufficientBufferUnless(this.reader.TryRead(out byte code));

            return code == MessagePackCode.Nil
                ? Nil.Default
                : throw ThrowInvalidCode(code);
        }

        /// <summary>
        /// Read an array header from
        /// <see cref="MessagePackCode.Array16"/>,
        /// <see cref="MessagePackCode.Array32"/>, or
        /// some built-in code between <see cref="MessagePackCode.MinFixArray"/> and <see cref="MessagePackCode.MaxFixArray"/>.
        /// </summary>
        public uint ReadArrayHeader()
        {
            ThrowInsufficientBufferUnless(this.reader.TryRead(out byte code));

            switch (code)
            {
                case MessagePackCode.Array16:
                    ThrowInsufficientBufferUnless(this.reader.TryReadBigEndian(out short shortValue));
                    return (uint)shortValue;
                case MessagePackCode.Array32:
                    ThrowInsufficientBufferUnless(this.reader.TryReadBigEndian(out int intValue));
                    return (uint)intValue;
                default:
                    if (code >= MessagePackCode.MinFixArray && code <= MessagePackCode.MaxFixArray)
                    {
                        return (uint)code & 0xF;
                    }

                    throw ThrowInvalidCode(code);
            }
        }

        /// <summary>
        /// Reads a <see cref="short"/> from any of:
        /// <see cref="MessagePackCode.UInt8"/>,
        /// <see cref="MessagePackCode.Int8"/>,
        /// <see cref="MessagePackCode.UInt16"/>,
        /// <see cref="MessagePackCode.Int16"/>,
        /// or anything between <see cref="MessagePackCode.MinNegativeFixInt"/> and <see cref="MessagePackCode.MaxNegativeFixInt"/>,
        /// or anything between <see cref="MessagePackCode.MinFixInt"/> and <see cref="MessagePackCode.MaxFixInt"/>.
        /// </summary>
        /// <returns>A 16-bit integer.</returns>
        public short ReadInt16()
        {
            ThrowInsufficientBufferUnless(this.reader.TryRead(out byte code));

            switch (code)
            {
                case MessagePackCode.UInt8:
                case MessagePackCode.Int8:
                    ThrowInsufficientBufferUnless(this.reader.TryRead(out byte byteValue));
                    return byteValue;
                case MessagePackCode.UInt16:
                case MessagePackCode.Int16:
                    ThrowInsufficientBufferUnless(this.reader.TryReadBigEndian(out short shortValue));
                    return shortValue;
                default:
                    if (code >= MessagePackCode.MinNegativeFixInt && code <= MessagePackCode.MaxNegativeFixInt)
                    {
                        return (sbyte)code;
                    }
                    else if (code >= MessagePackCode.MinFixInt && code <= MessagePackCode.MaxFixInt)
                    {
                        return code;
                    }

                    throw ThrowInvalidCode(code);
            }
        }

        /// <summary>
        /// Reads a boolean value from either a <see cref="MessagePackCode.False"/> or <see cref="MessagePackCode.True"/>.
        /// </summary>
        /// <returns>The value.</returns>
        public bool ReadBoolean()
        {
            ThrowInsufficientBufferUnless(this.reader.TryRead(out byte code));
            switch (code)
            {
                case MessagePackCode.True:
                    return true;
                case MessagePackCode.False:
                    return false;
                default:
                    throw ThrowInvalidCode(code);
            }
        }

        /// <summary>
        /// Reads a <see cref="byte"/> value either from
        /// a built-in code between <see cref="MessagePackCode.MinFixInt"/> and <see cref="MessagePackCode.MaxFixInt"/>,
        /// or a <see cref="MessagePackCode.UInt8"/>.
        /// </summary>
        /// <returns>The value.</returns>
        public byte ReadByte()
        {
            ThrowInsufficientBufferUnless(this.reader.TryRead(out byte code));
            switch (code)
            {
                case MessagePackCode.UInt8:
                    ThrowInsufficientBufferUnless(this.reader.TryRead(out byte result));
                    return result;
                default:
                    if (code >= MessagePackCode.MinFixInt && code <= MessagePackCode.MaxFixInt)
                    {
                        return code;
                    }

                    throw ThrowInvalidCode(code);
            }
        }

        /// <summary>
        /// Reads an <see cref="sbyte"/> value from:
        /// <see cref="MessagePackCode.Int8"/>,
        /// or some value between <see cref="MessagePackCode.MinNegativeFixInt"/> and <see cref="MessagePackCode.MaxNegativeFixInt"/>,
        /// or some value between <see cref="MessagePackCode.MinFixInt"/> and <see cref="MessagePackCode.MaxFixInt"/>.
        /// </summary>
        /// <returns>The value.</returns>
        public sbyte ReadSByte()
        {
            ThrowInsufficientBufferUnless(this.reader.TryRead(out byte code));
            switch (code)
            {
                case MessagePackCode.Int8:
                    ThrowInsufficientBufferUnless(this.reader.TryRead(out byte result));
                    return unchecked((sbyte)result);
                default:
                    if (code >= MessagePackCode.MinNegativeFixInt && code <= MessagePackCode.MaxNegativeFixInt)
                    {
                        return (sbyte)code;
                    }
                    else if (code >= MessagePackCode.MinFixInt && code <= MessagePackCode.MaxFixInt)
                    {
                        return (sbyte)code;
                    }

                    throw ThrowInvalidCode(code);
            }
        }

        /// <summary>
        /// Reads an <see cref="float"/> value from any value encoded with:
        /// <see cref="MessagePackCode.Float32"/>,
        /// <see cref="MessagePackCode.Int8"/>,
        /// <see cref="MessagePackCode.Int16"/>,
        /// <see cref="MessagePackCode.Int32"/>,
        /// <see cref="MessagePackCode.Int64"/>,
        /// <see cref="MessagePackCode.UInt8"/>,
        /// <see cref="MessagePackCode.UInt16"/>,
        /// <see cref="MessagePackCode.UInt32"/>,
        /// <see cref="MessagePackCode.UInt64"/>,
        /// or some value between <see cref="MessagePackCode.MinNegativeFixInt"/> and <see cref="MessagePackCode.MaxNegativeFixInt"/>,
        /// or some value between <see cref="MessagePackCode.MinFixInt"/> and <see cref="MessagePackCode.MaxFixInt"/>.
        /// </summary>
        /// <returns>The value.</returns>
        public unsafe float ReadSingle()
        {
            ThrowInsufficientBufferUnless(this.reader.TryRead(out byte code));

            switch (code)
            {
                case MessagePackCode.Float32:
                    byte* pScratch = stackalloc byte[4];
                    Span<byte> scratch = new Span<byte>(pScratch, 4);
                    ThrowInsufficientBufferUnless(this.reader.TryCopyTo(scratch));
                    var floatValue = new Float32Bits(scratch);
                    return floatValue.Value;
                case MessagePackCode.Int8:
                    ThrowInsufficientBufferUnless(this.reader.TryRead(out byte byteValue));
                    return unchecked((sbyte)byteValue);
                case MessagePackCode.Int16:
                    ThrowInsufficientBufferUnless(this.reader.TryReadBigEndian(out short shortValue));
                    return shortValue;
                case MessagePackCode.Int32:
                    ThrowInsufficientBufferUnless(this.reader.TryReadBigEndian(out int intValue));
                    return intValue;
                case MessagePackCode.Int64:
                    ThrowInsufficientBufferUnless(this.reader.TryReadBigEndian(out long longValue));
                    return longValue;
                case MessagePackCode.UInt8:
                    ThrowInsufficientBufferUnless(this.reader.TryRead(out byteValue));
                    return byteValue;
                case MessagePackCode.UInt16:
                    ThrowInsufficientBufferUnless(this.reader.TryReadBigEndian(out shortValue));
                    return (ushort)shortValue;
                case MessagePackCode.UInt32:
                    ThrowInsufficientBufferUnless(this.reader.TryReadBigEndian(out intValue));
                    return (uint)intValue;
                case MessagePackCode.UInt64:
                    ThrowInsufficientBufferUnless(this.reader.TryReadBigEndian(out longValue));
                    return (ulong)longValue;
                default:
                    if (code >= MessagePackCode.MinNegativeFixInt && code <= MessagePackCode.MaxNegativeFixInt)
                    {
                        return code;
                    }
                    else if (code >= MessagePackCode.MinFixInt && code <= MessagePackCode.MaxFixInt)
                    {
                        return code;
                    }

                    throw ThrowInvalidCode(code);
            }
        }

        /// <summary>
        /// Reads a span of bytes, whose length is determined by a header of one of these types:
        /// <see cref="MessagePackCode.Bin8"/>,
        /// <see cref="MessagePackCode.Bin16"/>,
        /// <see cref="MessagePackCode.Bin32"/>.
        /// </summary>
        /// <returns>A span of bytes.</returns>
        public ReadOnlySpan<byte> ReadBytes()
        {
            ThrowInsufficientBufferUnless(this.reader.TryRead(out byte code));

            int length;
            switch (code)
            {
                case MessagePackCode.Bin8:
                    ThrowInsufficientBufferUnless(this.reader.TryRead(out byte byteLength));
                    length = byteLength;
                    break;
                case MessagePackCode.Bin16:
                    ThrowInsufficientBufferUnless(this.reader.TryReadBigEndian(out short shortLength));
                    length = (ushort)shortLength;
                    break;
                case MessagePackCode.Bin32:
                    ThrowInsufficientBufferUnless(this.reader.TryReadBigEndian(out length));
                    break;
                default:
                    throw ThrowInvalidCode(code);
            }

            // Check that we have enough bytes before allocating memory to copy it in.
            ThrowInsufficientBufferUnless(this.reader.Remaining >= length);
            var result = new byte[length];
            ThrowInsufficientBufferUnless(this.reader.TryCopyTo(result));
            return result;
        }

        private static Exception ThrowInvalidCode(byte code)
        {
            throw new InvalidOperationException(string.Format("code is invalid. code: {0} format: {1}", code, MessagePackCode.ToFormatName(code)));
        }

        private static void ThrowInsufficientBufferUnless(bool condition)
        {
            if (!condition)
            {
                throw new EndOfStreamException();
            }
        }
    }
}
