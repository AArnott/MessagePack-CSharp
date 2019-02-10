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
        /// Read a map header from
        /// <see cref="MessagePackCode.Map16"/>,
        /// <see cref="MessagePackCode.Map32"/>, or
        /// some built-in code between <see cref="MessagePackCode.MinFixMap"/> and <see cref="MessagePackCode.MaxFixMap"/>.
        /// </summary>
        public uint ReadMapHeader()
        {
            ThrowInsufficientBufferUnless(this.reader.TryRead(out byte code));

            switch(code)
            {
                case MessagePackCode.Map16:
                    ThrowInsufficientBufferUnless(this.reader.TryReadBigEndian(out short shortValue));
                    return (ushort)shortValue;
                case MessagePackCode.Map32:
                    ThrowInsufficientBufferUnless(this.reader.TryReadBigEndian(out int intValue));
                    return (uint)intValue;
                default:
                    if (code >= MessagePackCode.MinFixMap && code <= MessagePackCode.MaxFixMap)
                    {
                        return (uint)(code & 0xF);
                    }

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
        /// Reads a <see cref="ushort"/> from any of:
        /// <see cref="MessagePackCode.UInt8"/>,
        /// <see cref="MessagePackCode.UInt16"/>,
        /// or anything between <see cref="MessagePackCode.MinFixInt"/> and <see cref="MessagePackCode.MaxFixInt"/>.
        /// </summary>
        /// <returns>A 16-bit integer.</returns>
        public ushort ReadUInt16()
        {
            ThrowInsufficientBufferUnless(this.reader.TryRead(out byte code));

            switch (code)
            {
                case MessagePackCode.UInt8:
                    ThrowInsufficientBufferUnless(this.reader.TryRead(out byte byteValue));
                    return byteValue;
                case MessagePackCode.UInt16:
                    ThrowInsufficientBufferUnless(this.reader.TryReadBigEndian(out short shortValue));
                    return (ushort)shortValue;
                default:
                    if (code >= MessagePackCode.MinFixInt && code <= MessagePackCode.MaxFixInt)
                    {
                        return code;
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
                    ThrowInsufficientBufferUnless(this.reader.TryRead(out byte byteValue));
                    return byteValue;
                case MessagePackCode.Int8:
                    ThrowInsufficientBufferUnless(this.reader.TryRead(out byteValue));
                    return (sbyte)byteValue;
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
        /// Reads a <see cref="uint"/> from any of:
        /// <see cref="MessagePackCode.UInt8"/>,
        /// <see cref="MessagePackCode.UInt16"/>,
        /// <see cref="MessagePackCode.UInt32"/>,
        /// or anything between <see cref="MessagePackCode.MinFixInt"/> and <see cref="MessagePackCode.MaxFixInt"/>.
        /// </summary>
        /// <returns>A 32-bit integer.</returns>
        public uint ReadUInt32()
        {
            ThrowInsufficientBufferUnless(this.reader.TryRead(out byte code));

            switch (code)
            {
                case MessagePackCode.UInt8:
                    ThrowInsufficientBufferUnless(this.reader.TryRead(out byte byteValue));
                    return byteValue;
                case MessagePackCode.UInt16:
                    ThrowInsufficientBufferUnless(this.reader.TryReadBigEndian(out short shortValue));
                    return (ushort)shortValue;
                case MessagePackCode.UInt32:
                    ThrowInsufficientBufferUnless(this.reader.TryReadBigEndian(out int intValue));
                    return (uint)intValue;
                default:
                    if (code >= MessagePackCode.MinFixInt && code <= MessagePackCode.MaxFixInt)
                    {
                        return code;
                    }

                    throw ThrowInvalidCode(code);
            }
        }

        /// <summary>
        /// Reads a <see cref="int"/> from any of:
        /// <see cref="MessagePackCode.UInt8"/>,
        /// <see cref="MessagePackCode.Int8"/>,
        /// <see cref="MessagePackCode.UInt16"/>,
        /// <see cref="MessagePackCode.Int16"/>,
        /// <see cref="MessagePackCode.UInt32"/>,
        /// <see cref="MessagePackCode.Int32"/>,
        /// or anything between <see cref="MessagePackCode.MinNegativeFixInt"/> and <see cref="MessagePackCode.MaxNegativeFixInt"/>,
        /// or anything between <see cref="MessagePackCode.MinFixInt"/> and <see cref="MessagePackCode.MaxFixInt"/>.
        /// </summary>
        /// <returns>A 32-bit integer.</returns>
        public int ReadInt32()
        {
            ThrowInsufficientBufferUnless(this.reader.TryRead(out byte code));

            switch (code)
            {
                case MessagePackCode.UInt8:
                    ThrowInsufficientBufferUnless(this.reader.TryRead(out byte byteValue));
                    return byteValue;
                case MessagePackCode.Int8:
                    ThrowInsufficientBufferUnless(this.reader.TryRead(out byteValue));
                    return (sbyte)byteValue;
                case MessagePackCode.UInt16:
                    ThrowInsufficientBufferUnless(this.reader.TryReadBigEndian(out short shortValue));
                    return (ushort)shortValue;
                case MessagePackCode.Int16:
                    ThrowInsufficientBufferUnless(this.reader.TryReadBigEndian(out shortValue));
                    return shortValue;
                case MessagePackCode.UInt32:
                case MessagePackCode.Int32:
                    ThrowInsufficientBufferUnless(this.reader.TryReadBigEndian(out int intValue));
                    return intValue;
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
        /// Reads a <see cref="ulong"/> from any of:
        /// <see cref="MessagePackCode.UInt8"/>,
        /// <see cref="MessagePackCode.UInt16"/>,
        /// <see cref="MessagePackCode.UInt32"/>,
        /// <see cref="MessagePackCode.UInt64"/>,
        /// or anything between <see cref="MessagePackCode.MinFixInt"/> and <see cref="MessagePackCode.MaxFixInt"/>.
        /// </summary>
        /// <returns>A 64-bit integer.</returns>
        public ulong ReadUInt64()
        {
            ThrowInsufficientBufferUnless(this.reader.TryRead(out byte code));

            switch (code)
            {
                case MessagePackCode.UInt8:
                    ThrowInsufficientBufferUnless(this.reader.TryRead(out byte byteValue));
                    return byteValue;
                case MessagePackCode.UInt16:
                    ThrowInsufficientBufferUnless(this.reader.TryReadBigEndian(out short shortValue));
                    return (ushort)shortValue;
                case MessagePackCode.UInt32:
                    ThrowInsufficientBufferUnless(this.reader.TryReadBigEndian(out int intValue));
                    return (uint)intValue;
                case MessagePackCode.UInt64:
                    ThrowInsufficientBufferUnless(this.reader.TryReadBigEndian(out long longValue));
                    return (ulong)longValue;
                default:
                    if (code >= MessagePackCode.MinFixInt && code <= MessagePackCode.MaxFixInt)
                    {
                        return code;
                    }

                    throw ThrowInvalidCode(code);
            }
        }

        /// <summary>
        /// Reads a <see cref="long"/> from any of:
        /// <see cref="MessagePackCode.UInt8"/>,
        /// <see cref="MessagePackCode.Int8"/>,
        /// <see cref="MessagePackCode.UInt16"/>,
        /// <see cref="MessagePackCode.Int16"/>,
        /// <see cref="MessagePackCode.UInt32"/>,
        /// <see cref="MessagePackCode.Int32"/>,
        /// <see cref="MessagePackCode.UInt64"/>,
        /// <see cref="MessagePackCode.Int64"/>,
        /// or anything between <see cref="MessagePackCode.MinNegativeFixInt"/> and <see cref="MessagePackCode.MaxNegativeFixInt"/>,
        /// or anything between <see cref="MessagePackCode.MinFixInt"/> and <see cref="MessagePackCode.MaxFixInt"/>.
        /// </summary>
        /// <returns>A 64-bit integer.</returns>
        public long ReadInt64()
        {
            ThrowInsufficientBufferUnless(this.reader.TryRead(out byte code));

            switch (code)
            {
                case MessagePackCode.UInt8:
                    ThrowInsufficientBufferUnless(this.reader.TryRead(out byte byteValue));
                    return byteValue;
                case MessagePackCode.Int8:
                    ThrowInsufficientBufferUnless(this.reader.TryRead(out byteValue));
                    return (sbyte)byteValue;
                case MessagePackCode.UInt16:
                    ThrowInsufficientBufferUnless(this.reader.TryReadBigEndian(out short shortValue));
                    return (ushort)shortValue;
                case MessagePackCode.Int16:
                    ThrowInsufficientBufferUnless(this.reader.TryReadBigEndian(out shortValue));
                    return shortValue;
                case MessagePackCode.UInt32:
                    ThrowInsufficientBufferUnless(this.reader.TryReadBigEndian(out int intValue));
                    return (uint)intValue;
                case MessagePackCode.Int32:
                    ThrowInsufficientBufferUnless(this.reader.TryReadBigEndian(out intValue));
                    return intValue;
                case MessagePackCode.UInt64:
                case MessagePackCode.Int64:
                    ThrowInsufficientBufferUnless(this.reader.TryReadBigEndian(out long longValue));
                    return longValue;
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
        /// Reads a <see cref="char"/> from any of:
        /// <see cref="MessagePackCode.UInt8"/>,
        /// <see cref="MessagePackCode.UInt16"/>,
        /// or anything between <see cref="MessagePackCode.MinFixInt"/> and <see cref="MessagePackCode.MaxFixInt"/>.
        /// </summary>
        /// <returns>A character.</returns>
        public char ReadChar() => (char)this.ReadUInt16();

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
                    this.reader.Advance(4);
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
        /// Reads an <see cref="double"/> value from any value encoded with:
        /// <see cref="MessagePackCode.Float64"/>,
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
        public unsafe double ReadDouble()
        {
            ThrowInsufficientBufferUnless(this.reader.TryRead(out byte code));

            switch (code)
            {
                case MessagePackCode.Float64:
                    byte* pScratch8 = stackalloc byte[8];
                    Span<byte> scratch8 = new Span<byte>(pScratch8, 8);
                    ThrowInsufficientBufferUnless(this.reader.TryCopyTo(scratch8));
                    this.reader.Advance(scratch8.Length);
                    var doubleValue = new Float64Bits(scratch8);
                    return doubleValue.Value;
                case MessagePackCode.Float32:
                    byte* pScratch4 = stackalloc byte[4];
                    Span<byte> scratch4 = new Span<byte>(pScratch4, 4);
                    ThrowInsufficientBufferUnless(this.reader.TryCopyTo(scratch4));
                    this.reader.Advance(scratch4.Length);
                    var floatValue = new Float32Bits(scratch4);
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
        /// Reads a <see cref="DateTime"/> from a value encoded with 
        /// <see cref="MessagePackCode.FixExt4"/>,
        /// <see cref="MessagePackCode.FixExt8"/>, or
        /// <see cref="MessagePackCode.Ext8"/>.
        /// Expects extension type code <see cref="ReservedMessagePackExtensionTypeCode.DateTime"/>.
        /// </summary>
        /// <returns>The value.</returns>
        public DateTime ReadDateTime()
        {
            var header = ReadExtensionFormatHeader();
            if (header.TypeCode != ReservedMessagePackExtensionTypeCode.DateTime)
            {
                throw new InvalidOperationException(string.Format("Extension TypeCode is invalid. typeCode: {0}", header.TypeCode));
            }

            switch (header.Length)
            {
                case 4:
                    ThrowInsufficientBufferUnless(this.reader.TryReadBigEndian(out int intValue));
                    return DateTimeConstants.UnixEpoch.AddSeconds((uint)intValue);
                case 8:
                    ThrowInsufficientBufferUnless(this.reader.TryReadBigEndian(out long longValue));
                    var nanoseconds = (long)(longValue >> 34);
                    var seconds = longValue & 0x00000003ffffffffL;
                    return DateTimeConstants.UnixEpoch.AddSeconds(seconds).AddTicks(nanoseconds / DateTimeConstants.NanosecondsPerTick);
                default:
                    throw new InvalidOperationException($"Length of extension was {header.Length}. Either 4 or 8 were expected.");
            }
        }

        /// <summary>
        /// Reads a span of bytes, whose length is determined by a header of one of these types:
        /// <see cref="MessagePackCode.Bin8"/>,
        /// <see cref="MessagePackCode.Bin16"/>,
        /// <see cref="MessagePackCode.Bin32"/>.
        /// </summary>
        /// <returns>
        /// A sequence of bytes.
        /// The data is a slice from the original sequence passed to this reader's constructor.
        /// </returns>
        public ReadOnlySequence<byte> ReadBytes()
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
            var result = this.reader.Sequence.Slice(this.reader.Position, length);
            this.reader.Advance(length);
            return result;
        }

        /// <summary>
        /// Reads a string of bytes, whose length is determined by a header of one of these types:
        /// <see cref="MessagePackCode.Str8"/>,
        /// <see cref="MessagePackCode.Str16"/>,
        /// <see cref="MessagePackCode.Str32"/>,
        /// or a code between <see cref="MessagePackCode.MinFixStr"/> and <see cref="MessagePackCode.MaxFixStr"/>.
        /// </summary>
        /// <returns>
        /// The sequence of bytes. 
        /// The data is a slice from the original sequence passed to this reader's constructor.
        /// </returns>
        public ReadOnlySequence<byte> ReadStringSegment()
        {
            int length = GetStringLengthInBytes();
            ThrowInsufficientBufferUnless(this.reader.Remaining >= length);
            var result = this.reader.Sequence.Slice(this.reader.Position, length);
            this.reader.Advance(length);
            return result;
        }

        /// <summary>
        /// Reads a string, whose length is determined by a header of one of these types:
        /// <see cref="MessagePackCode.Str8"/>,
        /// <see cref="MessagePackCode.Str16"/>,
        /// <see cref="MessagePackCode.Str32"/>,
        /// or a code between <see cref="MessagePackCode.MinFixStr"/> and <see cref="MessagePackCode.MaxFixStr"/>.
        /// </summary>
        /// <returns>A string.</returns>
        public string ReadString()
        {
            int byteLength = GetStringLengthInBytes();

            ThrowInsufficientBufferUnless(this.reader.Remaining >= byteLength);
            if (this.reader.UnreadSpan.Length >= byteLength)
            {
                // Fast path: all bytes to decode appear in the same span.
                string value = StringEncoding.UTF8.GetString(this.reader.UnreadSpan.Slice(0, byteLength));
                this.reader.Advance(byteLength);
                return value;
            }
            else
            {
                // Slow path
#if NETSTANDARD1_6
                // We need to concatenate all the bytes together into one array.
                byte[] byteArray = ArrayPool<byte>.Shared.Rent(byteLength);
                ThrowInsufficientBufferUnless(this.reader.TryCopyTo(byteArray));
                this.reader.Advance(byteArray.Length);
                string value = StringEncoding.UTF8.GetString(byteArray);
                ArrayPool<byte>.Shared.Return(byteArray);
                return value;
#else
                // We need to decode bytes incrementally across multiple spans.
                int maxCharLength = StringEncoding.UTF8.GetMaxCharCount(byteLength);
                char[] charArray = ArrayPool<char>.Shared.Rent(maxCharLength);
                var decoder = StringEncoding.UTF8.GetDecoder();

                int remainingByteLength = byteLength;
                int initializedChars = 0;
                while (remainingByteLength > 0)
                {
                    int bytesRead = Math.Min(remainingByteLength, this.reader.UnreadSpan.Length);
                    remainingByteLength -= bytesRead;
                    bool flush = remainingByteLength == 0;
#if NETCOREAPP2_1
                    initializedChars += decoder.GetChars(this.reader.UnreadSpan.Slice(0, bytesRead), charArray.AsSpan(initializedChars), flush);
#else
                    unsafe
                    {
                        fixed (byte* pUnreadSpan = this.reader.UnreadSpan)
                        fixed (char* pCharArray = &charArray[initializedChars])
                        {
                            initializedChars += decoder.GetChars(pUnreadSpan, bytesRead, pCharArray, charArray.Length - initializedChars, flush);
                        }
                    }
#endif
                }

                string value = new string(charArray, 0, initializedChars);
                ArrayPool<char>.Shared.Return(charArray);
                return value;
#endif
            }
        }

        /// <summary>
        /// Reads an extension format header, based on one of these codes:
        /// <see cref="MessagePackCode.FixExt1"/>,
        /// <see cref="MessagePackCode.FixExt2"/>,
        /// <see cref="MessagePackCode.FixExt4"/>,
        /// <see cref="MessagePackCode.FixExt8"/>,
        /// <see cref="MessagePackCode.FixExt16"/>,
        /// <see cref="MessagePackCode.Ext8"/>,
        /// <see cref="MessagePackCode.Ext16"/>, or
        /// <see cref="MessagePackCode.Ext32"/>.
        /// </summary>
        /// <returns>The extension header.</returns>
        public ExtensionHeader ReadExtensionFormatHeader()
        {
            ThrowInsufficientBufferUnless(this.reader.TryRead(out byte code));

            uint length;
            switch (code)
            {
                case MessagePackCode.FixExt1:
                    length = 1;
                    break;
                case MessagePackCode.FixExt2:
                    length = 2;
                    break;
                case MessagePackCode.FixExt4:
                    length = 4;
                    break;
                case MessagePackCode.FixExt8:
                    length = 8;
                    break;
                case MessagePackCode.FixExt16:
                    length = 16;
                    break;
                case MessagePackCode.Ext8:
                    ThrowInsufficientBufferUnless(this.reader.TryRead(out byte byteLength));
                    length = byteLength;
                    break;
                case MessagePackCode.Ext16:
                    ThrowInsufficientBufferUnless(this.reader.TryReadBigEndian(out short shortLength));
                    length = (ushort)shortLength;
                    break;
                case MessagePackCode.Ext32:
                    ThrowInsufficientBufferUnless(this.reader.TryReadBigEndian(out int intLength));
                    length = (uint)intLength;
                    break;
                default:
                    throw ThrowInvalidCode(code);
            }

            ThrowInsufficientBufferUnless(this.reader.TryRead(out byte typeCode));
            return new ExtensionHeader((sbyte)typeCode, length);
        }

        /// <summary>
        /// Reads an extension format header and data, based on one of these codes:
        /// <see cref="MessagePackCode.FixExt1"/>,
        /// <see cref="MessagePackCode.FixExt2"/>,
        /// <see cref="MessagePackCode.FixExt4"/>,
        /// <see cref="MessagePackCode.FixExt8"/>,
        /// <see cref="MessagePackCode.FixExt16"/>,
        /// <see cref="MessagePackCode.Ext8"/>,
        /// <see cref="MessagePackCode.Ext16"/>, or
        /// <see cref="MessagePackCode.Ext32"/>.
        /// </summary>
        /// <returns>
        /// The extension format. 
        /// The data is a slice from the original sequence passed to this reader's constructor.
        /// </returns>
        public ExtensionResult ReadExtensionFormat()
        {
            var header = ReadExtensionFormatHeader();
            var data = this.reader.Sequence.Slice(this.reader.Position, header.Length);
            this.reader.Advance(header.Length);
            return new ExtensionResult(header.TypeCode, data);
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

        private int GetStringLengthInBytes()
        {
            ThrowInsufficientBufferUnless(this.reader.TryRead(out byte code));

            int byteLength;
            switch (code)
            {
                case MessagePackCode.Str8:
                    ThrowInsufficientBufferUnless(this.reader.TryRead(out byte byteValue));
                    byteLength = byteValue;
                    break;
                case MessagePackCode.Str16:
                    ThrowInsufficientBufferUnless(this.reader.TryReadBigEndian(out short shortValue));
                    byteLength = (ushort)shortValue;
                    break;
                case MessagePackCode.Str32:
                    ThrowInsufficientBufferUnless(this.reader.TryReadBigEndian(out int intValue));
                    byteLength = intValue;
                    break;
                default:
                    if (code >= MessagePackCode.MinFixStr && code <= MessagePackCode.MaxFixStr)
                    {
                        byteLength = code & 0x1F;
                    }

                    throw ThrowInvalidCode(code);
            }

            return byteLength;
        }
    }
}
