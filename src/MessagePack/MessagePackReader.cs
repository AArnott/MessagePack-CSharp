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

                    throw this.ThrowInvalidCode(code);
            }
        }

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

        private Exception ThrowInvalidCode(byte code)
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
