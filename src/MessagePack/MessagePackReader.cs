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
        private SequenceReader<byte> sequenceReader;

        /// <summary>
        /// Initializes a new instance of the <see cref="MessagePackReader"/> struct.
        /// </summary>
        /// <param name="readOnlySequence">The sequence to read from.</param>
        public MessagePackReader(ReadOnlySequence<byte> readOnlySequence)
        {
            this.sequenceReader = new SequenceReader<byte>(readOnlySequence);
        }

        /// <summary>
        /// Checks whether the reader position is pointing at a nil value.
        /// </summary>
        public bool IsNil => this.sequenceReader.TryPeek(out byte code) && code == MessagePackCode.Nil;

        /// <summary>
        /// Reads a nil value.
        /// </summary>
        /// <returns>A nil value.</returns>
        public Nil ReadNil()
        {
            ThrowInsufficientBufferUnless(this.sequenceReader.TryRead(out byte code));

            return code == MessagePackCode.Nil
                ? Nil.Default
                : throw ThrowInvalidCode(code);
        }

        /// <summary>
        /// Read the length of the next array.
        /// </summary>
        public uint ReadArrayHeader()
        {
            ThrowInsufficientBufferUnless(this.sequenceReader.TryRead(out byte code));

            switch (code)
            {
                case MessagePackCode.Array16:
                    ThrowInsufficientBufferUnless(this.sequenceReader.TryReadBigEndian(out short shortValue));
                    return (uint)shortValue;
                case MessagePackCode.Array32:
                    ThrowInsufficientBufferUnless(this.sequenceReader.TryReadBigEndian(out int intValue));
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
        /// Reads a 16-bit integer.
        /// </summary>
        /// <returns>A 16-bit integer.</returns>
        public short ReadInt16()
        {
            ThrowInsufficientBufferUnless(this.sequenceReader.TryRead(out byte code));

            switch (code)
            {
                case MessagePackCode.UInt8:
                case MessagePackCode.Int8:
                    ThrowInsufficientBufferUnless(this.sequenceReader.TryRead(out byte byteValue));
                    return byteValue;
                case MessagePackCode.UInt16:
                case MessagePackCode.Int16:
                    ThrowInsufficientBufferUnless(this.sequenceReader.TryReadBigEndian(out short shortValue));
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
        /// Reads a boolean value.
        /// </summary>
        /// <returns>The value.</returns>
        public bool ReadBoolean()
        {
            ThrowInsufficientBufferUnless(this.sequenceReader.TryRead(out byte code));
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
        /// Reads a <see cref="byte"/> value.
        /// </summary>
        /// <returns>The value.</returns>
        public byte ReadByte()
        {
            ThrowInsufficientBufferUnless(this.sequenceReader.TryRead(out byte code));
            if (code >= MessagePackCode.MinFixInt && code <= MessagePackCode.MaxFixInt)
            {
                return code;
            }

            ThrowInsufficientBufferUnless(this.sequenceReader.TryRead(out byte result));
            return result;
        }

        /// <summary>
        /// Reads an <see cref="sbyte"/> value.
        /// </summary>
        /// <returns>The value.</returns>
        public sbyte ReadSByte()
        {
            ThrowInsufficientBufferUnless(this.sequenceReader.TryRead(out byte code));
            switch (code)
            {
                case MessagePackCode.Int8:
                    ThrowInsufficientBufferUnless(this.sequenceReader.TryRead(out byte result));
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
        /// Reads a span of bytes, whose length is determined by a header.
        /// </summary>
        /// <returns>A span of bytes.</returns>
        public ReadOnlySpan<byte> ReadBytes()
        {
            ThrowInsufficientBufferUnless(this.sequenceReader.TryRead(out byte code));

            int length;
            switch (code)
            {
                case MessagePackCode.Bin8:
                    ThrowInsufficientBufferUnless(this.sequenceReader.TryRead(out byte byteLength));
                    length = byteLength;
                    break;
                case MessagePackCode.Bin16:
                    ThrowInsufficientBufferUnless(this.sequenceReader.TryReadBigEndian(out short shortLength));
                    length = (ushort)shortLength;
                    break;
                case MessagePackCode.Bin32:
                    ThrowInsufficientBufferUnless(this.sequenceReader.TryReadBigEndian(out length));
                    break;
                default:
                    throw ThrowInvalidCode(code);
            }

            // Check that we have enough bytes before allocating memory to copy it in.
            ThrowInsufficientBufferUnless(this.sequenceReader.Remaining >= length);
            var result = new byte[length];
            ThrowInsufficientBufferUnless(this.sequenceReader.TryCopyTo(result));
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
