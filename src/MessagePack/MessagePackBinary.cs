using System;
using System.Buffers;
using System.IO;
using System.Runtime.InteropServices;
using MessagePack.Decoders;
using MessagePack.Internal;

namespace MessagePack
{
    /// <summary>
    /// Encode/Decode Utility of MessagePack Spec.
    /// https://github.com/msgpack/msgpack/blob/master/spec.md
    /// </summary>
    public static partial class MessagePackBinary
    {
        private const int MaxSize = 256; // [0] ~ [255]
        private const int ArrayMaxSize = 0x7FFFFFC7; // https://msdn.microsoft.com/en-us/library/system.array

        private static readonly IMapHeaderDecoder[] mapHeaderDecoders = new IMapHeaderDecoder[MaxSize];
        private static readonly IArrayHeaderDecoder[] arrayHeaderDecoders = new IArrayHeaderDecoder[MaxSize];
        private static readonly IBooleanDecoder[] booleanDecoders = new IBooleanDecoder[MaxSize];
        private static readonly IByteDecoder[] byteDecoders = new IByteDecoder[MaxSize];
        private static readonly IBytesDecoder[] bytesDecoders = new IBytesDecoder[MaxSize];
        private static readonly IBytesSegmentDecoder[] bytesSegmentDecoders = new IBytesSegmentDecoder[MaxSize];
        private static readonly ISByteDecoder[] sbyteDecoders = new ISByteDecoder[MaxSize];
        private static readonly ISingleDecoder[] singleDecoders = new ISingleDecoder[MaxSize];
        private static readonly IDoubleDecoder[] doubleDecoders = new IDoubleDecoder[MaxSize];
        private static readonly IInt16Decoder[] int16Decoders = new IInt16Decoder[MaxSize];
        private static readonly IInt32Decoder[] int32Decoders = new IInt32Decoder[MaxSize];
        private static readonly IInt64Decoder[] int64Decoders = new IInt64Decoder[MaxSize];
        private static readonly IUInt16Decoder[] uint16Decoders = new IUInt16Decoder[MaxSize];
        private static readonly IUInt32Decoder[] uint32Decoders = new IUInt32Decoder[MaxSize];
        private static readonly IUInt64Decoder[] uint64Decoders = new IUInt64Decoder[MaxSize];
        private static readonly IStringDecoder[] stringDecoders = new IStringDecoder[MaxSize];
        private static readonly IStringSegmentDecoder[] stringSegmentDecoders = new IStringSegmentDecoder[MaxSize];
        private static readonly IExtDecoder[] extDecoders = new IExtDecoder[MaxSize];
        private static readonly IExtHeaderDecoder[] extHeaderDecoders = new IExtHeaderDecoder[MaxSize];
        private static readonly IDateTimeDecoder[] dateTimeDecoders = new IDateTimeDecoder[MaxSize];
        private static readonly IReadNextDecoder[] readNextDecoders = new IReadNextDecoder[MaxSize];

        static MessagePackBinary()
        {
            // Init LookupTable.
            for (int i = 0; i < MaxSize; i++)
            {
                mapHeaderDecoders[i] = Decoders.InvalidMapHeader.Instance;
                arrayHeaderDecoders[i] = Decoders.InvalidArrayHeader.Instance;
                booleanDecoders[i] = Decoders.InvalidBoolean.Instance;
                byteDecoders[i] = Decoders.InvalidByte.Instance;
                bytesDecoders[i] = Decoders.InvalidBytes.Instance;
                bytesSegmentDecoders[i] = Decoders.InvalidBytesSegment.Instance;
                sbyteDecoders[i] = Decoders.InvalidSByte.Instance;
                singleDecoders[i] = Decoders.InvalidSingle.Instance;
                doubleDecoders[i] = Decoders.InvalidDouble.Instance;
                int16Decoders[i] = Decoders.InvalidInt16.Instance;
                int32Decoders[i] = Decoders.InvalidInt32.Instance;
                int64Decoders[i] = Decoders.InvalidInt64.Instance;
                uint16Decoders[i] = Decoders.InvalidUInt16.Instance;
                uint32Decoders[i] = Decoders.InvalidUInt32.Instance;
                uint64Decoders[i] = Decoders.InvalidUInt64.Instance;
                stringDecoders[i] = Decoders.InvalidString.Instance;
                stringSegmentDecoders[i] = Decoders.InvalidStringSegment.Instance;
                extDecoders[i] = Decoders.InvalidExt.Instance;
                extHeaderDecoders[i] = Decoders.InvalidExtHeader.Instance;
                dateTimeDecoders[i] = Decoders.InvalidDateTime.Instance;
            }

            // Number
            for (int i = MessagePackCode.MinNegativeFixInt; i <= MessagePackCode.MaxNegativeFixInt; i++)
            {
                sbyteDecoders[i] = Decoders.FixSByte.Instance;
                int16Decoders[i] = Decoders.FixNegativeInt16.Instance;
                int32Decoders[i] = Decoders.FixNegativeInt32.Instance;
                int64Decoders[i] = Decoders.FixNegativeInt64.Instance;
                singleDecoders[i] = Decoders.FixNegativeFloat.Instance;
                doubleDecoders[i] = Decoders.FixNegativeDouble.Instance;
                readNextDecoders[i] = Decoders.ReadNext1.Instance;
            }
            for (int i = MessagePackCode.MinFixInt; i <= MessagePackCode.MaxFixInt; i++)
            {
                byteDecoders[i] = Decoders.FixByte.Instance;
                sbyteDecoders[i] = Decoders.FixSByte.Instance;
                int16Decoders[i] = Decoders.FixInt16.Instance;
                int32Decoders[i] = Decoders.FixInt32.Instance;
                int64Decoders[i] = Decoders.FixInt64.Instance;
                uint16Decoders[i] = Decoders.FixUInt16.Instance;
                uint32Decoders[i] = Decoders.FixUInt32.Instance;
                uint64Decoders[i] = Decoders.FixUInt64.Instance;
                singleDecoders[i] = Decoders.FixFloat.Instance;
                doubleDecoders[i] = Decoders.FixDouble.Instance;
                readNextDecoders[i] = Decoders.ReadNext1.Instance;
            }

            byteDecoders[MessagePackCode.UInt8] = Decoders.UInt8Byte.Instance;
            sbyteDecoders[MessagePackCode.Int8] = Decoders.Int8SByte.Instance;
            int16Decoders[MessagePackCode.UInt8] = Decoders.UInt8Int16.Instance;
            int16Decoders[MessagePackCode.UInt16] = Decoders.UInt16Int16.Instance;
            int16Decoders[MessagePackCode.Int8] = Decoders.Int8Int16.Instance;
            int16Decoders[MessagePackCode.Int16] = Decoders.Int16Int16.Instance;
            int32Decoders[MessagePackCode.UInt8] = Decoders.UInt8Int32.Instance;
            int32Decoders[MessagePackCode.UInt16] = Decoders.UInt16Int32.Instance;
            int32Decoders[MessagePackCode.UInt32] = Decoders.UInt32Int32.Instance;
            int32Decoders[MessagePackCode.Int8] = Decoders.Int8Int32.Instance;
            int32Decoders[MessagePackCode.Int16] = Decoders.Int16Int32.Instance;
            int32Decoders[MessagePackCode.Int32] = Decoders.Int32Int32.Instance;
            int64Decoders[MessagePackCode.UInt8] = Decoders.UInt8Int64.Instance;
            int64Decoders[MessagePackCode.UInt16] = Decoders.UInt16Int64.Instance;
            int64Decoders[MessagePackCode.UInt32] = Decoders.UInt32Int64.Instance;
            int64Decoders[MessagePackCode.UInt64] = Decoders.UInt64Int64.Instance;
            int64Decoders[MessagePackCode.Int8] = Decoders.Int8Int64.Instance;
            int64Decoders[MessagePackCode.Int16] = Decoders.Int16Int64.Instance;
            int64Decoders[MessagePackCode.Int32] = Decoders.Int32Int64.Instance;
            int64Decoders[MessagePackCode.Int64] = Decoders.Int64Int64.Instance;
            uint16Decoders[MessagePackCode.UInt8] = Decoders.UInt8UInt16.Instance;
            uint16Decoders[MessagePackCode.UInt16] = Decoders.UInt16UInt16.Instance;
            uint32Decoders[MessagePackCode.UInt8] = Decoders.UInt8UInt32.Instance;
            uint32Decoders[MessagePackCode.UInt16] = Decoders.UInt16UInt32.Instance;
            uint32Decoders[MessagePackCode.UInt32] = Decoders.UInt32UInt32.Instance;
            uint64Decoders[MessagePackCode.UInt8] = Decoders.UInt8UInt64.Instance;
            uint64Decoders[MessagePackCode.UInt16] = Decoders.UInt16UInt64.Instance;
            uint64Decoders[MessagePackCode.UInt32] = Decoders.UInt32UInt64.Instance;
            uint64Decoders[MessagePackCode.UInt64] = Decoders.UInt64UInt64.Instance;

            singleDecoders[MessagePackCode.Float32] = Decoders.Float32Single.Instance;
            singleDecoders[MessagePackCode.Int8] = Decoders.Int8Single.Instance;
            singleDecoders[MessagePackCode.Int16] = Decoders.Int16Single.Instance;
            singleDecoders[MessagePackCode.Int32] = Decoders.Int32Single.Instance;
            singleDecoders[MessagePackCode.Int64] = Decoders.Int64Single.Instance;
            singleDecoders[MessagePackCode.UInt8] = Decoders.UInt8Single.Instance;
            singleDecoders[MessagePackCode.UInt16] = Decoders.UInt16Single.Instance;
            singleDecoders[MessagePackCode.UInt32] = Decoders.UInt32Single.Instance;
            singleDecoders[MessagePackCode.UInt64] = Decoders.UInt64Single.Instance;

            doubleDecoders[MessagePackCode.Float32] = Decoders.Float32Double.Instance;
            doubleDecoders[MessagePackCode.Float64] = Decoders.Float64Double.Instance;
            doubleDecoders[MessagePackCode.Int8] = Decoders.Int8Double.Instance;
            doubleDecoders[MessagePackCode.Int16] = Decoders.Int16Double.Instance;
            doubleDecoders[MessagePackCode.Int32] = Decoders.Int32Double.Instance;
            doubleDecoders[MessagePackCode.Int64] = Decoders.Int64Double.Instance;
            doubleDecoders[MessagePackCode.UInt8] = Decoders.UInt8Double.Instance;
            doubleDecoders[MessagePackCode.UInt16] = Decoders.UInt16Double.Instance;
            doubleDecoders[MessagePackCode.UInt32] = Decoders.UInt32Double.Instance;
            doubleDecoders[MessagePackCode.UInt64] = Decoders.UInt64Double.Instance;

            readNextDecoders[MessagePackCode.Int8] = Decoders.ReadNext2.Instance;
            readNextDecoders[MessagePackCode.Int16] = Decoders.ReadNext3.Instance;
            readNextDecoders[MessagePackCode.Int32] = Decoders.ReadNext5.Instance;
            readNextDecoders[MessagePackCode.Int64] = Decoders.ReadNext9.Instance;
            readNextDecoders[MessagePackCode.UInt8] = Decoders.ReadNext2.Instance;
            readNextDecoders[MessagePackCode.UInt16] = Decoders.ReadNext3.Instance;
            readNextDecoders[MessagePackCode.UInt32] = Decoders.ReadNext5.Instance;
            readNextDecoders[MessagePackCode.UInt64] = Decoders.ReadNext9.Instance;
            readNextDecoders[MessagePackCode.Float32] = Decoders.ReadNext5.Instance;
            readNextDecoders[MessagePackCode.Float64] = Decoders.ReadNext9.Instance;

            // Map
            for (int i = MessagePackCode.MinFixMap; i <= MessagePackCode.MaxFixMap; i++)
            {
                mapHeaderDecoders[i] = Decoders.FixMapHeader.Instance;
                readNextDecoders[i] = Decoders.ReadNext1.Instance;
            }
            mapHeaderDecoders[MessagePackCode.Map16] = Decoders.Map16Header.Instance;
            mapHeaderDecoders[MessagePackCode.Map32] = Decoders.Map32Header.Instance;
            readNextDecoders[MessagePackCode.Map16] = Decoders.ReadNextMap.Instance;
            readNextDecoders[MessagePackCode.Map32] = Decoders.ReadNextMap.Instance;

            // Array
            for (int i = MessagePackCode.MinFixArray; i <= MessagePackCode.MaxFixArray; i++)
            {
                arrayHeaderDecoders[i] = Decoders.FixArrayHeader.Instance;
                readNextDecoders[i] = Decoders.ReadNext1.Instance;
            }
            arrayHeaderDecoders[MessagePackCode.Array16] = Decoders.Array16Header.Instance;
            arrayHeaderDecoders[MessagePackCode.Array32] = Decoders.Array32Header.Instance;
            readNextDecoders[MessagePackCode.Array16] = Decoders.ReadNextArray.Instance;
            readNextDecoders[MessagePackCode.Array32] = Decoders.ReadNextArray.Instance;

            // Str
            for (int i = MessagePackCode.MinFixStr; i <= MessagePackCode.MaxFixStr; i++)
            {
                stringDecoders[i] = Decoders.FixString.Instance;
                stringSegmentDecoders[i] = Decoders.FixStringSegment.Instance;
                readNextDecoders[i] = Decoders.ReadNextFixStr.Instance;
            }

            stringDecoders[MessagePackCode.Str8] = Decoders.Str8String.Instance;
            stringDecoders[MessagePackCode.Str16] = Decoders.Str16String.Instance;
            stringDecoders[MessagePackCode.Str32] = Decoders.Str32String.Instance;
            stringSegmentDecoders[MessagePackCode.Str8] = Decoders.Str8StringSegment.Instance;
            stringSegmentDecoders[MessagePackCode.Str16] = Decoders.Str16StringSegment.Instance;
            stringSegmentDecoders[MessagePackCode.Str32] = Decoders.Str32StringSegment.Instance;
            readNextDecoders[MessagePackCode.Str8] = Decoders.ReadNextStr8.Instance;
            readNextDecoders[MessagePackCode.Str16] = Decoders.ReadNextStr16.Instance;
            readNextDecoders[MessagePackCode.Str32] = Decoders.ReadNextStr32.Instance;

            // Others
            stringDecoders[MessagePackCode.Nil] = Decoders.NilString.Instance;
            stringSegmentDecoders[MessagePackCode.Nil] = Decoders.NilStringSegment.Instance;
            bytesDecoders[MessagePackCode.Nil] = Decoders.NilBytes.Instance;
            bytesSegmentDecoders[MessagePackCode.Nil] = Decoders.NilBytesSegment.Instance;
            readNextDecoders[MessagePackCode.Nil] = Decoders.ReadNext1.Instance;

            booleanDecoders[MessagePackCode.False] = Decoders.False.Instance;
            booleanDecoders[MessagePackCode.True] = Decoders.True.Instance;
            readNextDecoders[MessagePackCode.False] = Decoders.ReadNext1.Instance;
            readNextDecoders[MessagePackCode.True] = Decoders.ReadNext1.Instance;

            bytesDecoders[MessagePackCode.Bin8] = Decoders.Bin8Bytes.Instance;
            bytesDecoders[MessagePackCode.Bin16] = Decoders.Bin16Bytes.Instance;
            bytesDecoders[MessagePackCode.Bin32] = Decoders.Bin32Bytes.Instance;
            bytesSegmentDecoders[MessagePackCode.Bin8] = Decoders.Bin8BytesSegment.Instance;
            bytesSegmentDecoders[MessagePackCode.Bin16] = Decoders.Bin16BytesSegment.Instance;
            bytesSegmentDecoders[MessagePackCode.Bin32] = Decoders.Bin32BytesSegment.Instance;
            readNextDecoders[MessagePackCode.Bin8] = Decoders.ReadNextBin8.Instance;
            readNextDecoders[MessagePackCode.Bin16] = Decoders.ReadNextBin16.Instance;
            readNextDecoders[MessagePackCode.Bin32] = Decoders.ReadNextBin32.Instance;

            // Ext
            extDecoders[MessagePackCode.FixExt1] = Decoders.FixExt1.Instance;
            extDecoders[MessagePackCode.FixExt2] = Decoders.FixExt2.Instance;
            extDecoders[MessagePackCode.FixExt4] = Decoders.FixExt4.Instance;
            extDecoders[MessagePackCode.FixExt8] = Decoders.FixExt8.Instance;
            extDecoders[MessagePackCode.FixExt16] = Decoders.FixExt16.Instance;
            extDecoders[MessagePackCode.Ext8] = Decoders.Ext8.Instance;
            extDecoders[MessagePackCode.Ext16] = Decoders.Ext16.Instance;
            extDecoders[MessagePackCode.Ext32] = Decoders.Ext32.Instance;

            extHeaderDecoders[MessagePackCode.FixExt1] = Decoders.FixExt1Header.Instance;
            extHeaderDecoders[MessagePackCode.FixExt2] = Decoders.FixExt2Header.Instance;
            extHeaderDecoders[MessagePackCode.FixExt4] = Decoders.FixExt4Header.Instance;
            extHeaderDecoders[MessagePackCode.FixExt8] = Decoders.FixExt8Header.Instance;
            extHeaderDecoders[MessagePackCode.FixExt16] = Decoders.FixExt16Header.Instance;
            extHeaderDecoders[MessagePackCode.Ext8] = Decoders.Ext8Header.Instance;
            extHeaderDecoders[MessagePackCode.Ext16] = Decoders.Ext16Header.Instance;
            extHeaderDecoders[MessagePackCode.Ext32] = Decoders.Ext32Header.Instance;


            readNextDecoders[MessagePackCode.FixExt1] = Decoders.ReadNext3.Instance;
            readNextDecoders[MessagePackCode.FixExt2] = Decoders.ReadNext4.Instance;
            readNextDecoders[MessagePackCode.FixExt4] = Decoders.ReadNext6.Instance;
            readNextDecoders[MessagePackCode.FixExt8] = Decoders.ReadNext10.Instance;
            readNextDecoders[MessagePackCode.FixExt16] = Decoders.ReadNext18.Instance;
            readNextDecoders[MessagePackCode.Ext8] = Decoders.ReadNextExt8.Instance;
            readNextDecoders[MessagePackCode.Ext16] = Decoders.ReadNextExt16.Instance;
            readNextDecoders[MessagePackCode.Ext32] = Decoders.ReadNextExt32.Instance;

            // DateTime
            dateTimeDecoders[MessagePackCode.FixExt4] = Decoders.FixExt4DateTime.Instance;
            dateTimeDecoders[MessagePackCode.FixExt8] = Decoders.FixExt8DateTime.Instance;
            dateTimeDecoders[MessagePackCode.Ext8] = Decoders.Ext8DateTime.Instance;
        }

#if NETSTANDARD || NETFRAMEWORK
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
#endif
        public static MessagePackType GetMessagePackType(ReadOnlySequence<byte> byteSequence)
        {
            return MessagePackCode.ToMessagePackType(byteSequence.First.Span[0]);
        }

#if NETSTANDARD || NETFRAMEWORK
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
#endif
        public static void ReadNext(ref ReadOnlySequence<byte> byteSequence)
        {
            readNextDecoders[byteSequence.First.Span[0]].Read(ref byteSequence);
        }

#if NETSTANDARD || NETFRAMEWORK
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
#endif
        public static void ReadNextBlock(ref ReadOnlySequence<byte> byteSequence)
        {
            switch (GetMessagePackType(byteSequence))
            {
                case MessagePackType.Unknown:
                case MessagePackType.Integer:
                case MessagePackType.Nil:
                case MessagePackType.Boolean:
                case MessagePackType.Float:
                case MessagePackType.String:
                case MessagePackType.Binary:
                case MessagePackType.Extension:
                default:
                    ReadNext(ref byteSequence);
                    break;
                case MessagePackType.Array:
                    {
                        var header = MessagePackBinary.ReadArrayHeader(ref byteSequence);
                        for (int i = 0; i < header; i++)
                        {
                            ReadNextBlock(ref byteSequence);
                        }

                        break;
                    }
                case MessagePackType.Map:
                    {
                        var header = MessagePackBinary.ReadMapHeader(ref byteSequence);
                        for (int i = 0; i < header; i++)
                        {
                            ReadNextBlock(ref byteSequence); // read key block
                            ReadNextBlock(ref byteSequence); // read value block
                        }

                        break;
                    }
            }
        }

#if NETSTANDARD || NETFRAMEWORK
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
#endif
        public static void WriteNil(IBufferWriter<byte> writer)
        {
            var span = writer.GetSpan(1);
            span[0] = MessagePackCode.Nil;
            writer.Advance(1);
        }

#if NETSTANDARD || NETFRAMEWORK
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
#endif
        public static Nil ReadNil(ref ReadOnlySequence<byte> byteSequence)
        {
            if (byteSequence.First.Span[0] == MessagePackCode.Nil)
            {
                byteSequence = byteSequence.Slice(1);
                return Nil.Default;
            }
            else
            {
                throw new InvalidOperationException(string.Format("code is invalid. code:{0} format:{1}", byteSequence.First.Span[0], MessagePackCode.ToFormatName(byteSequence.First.Span[0])));
            }
        }

#if NETSTANDARD || NETFRAMEWORK
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
#endif
        public static bool IsNil(ReadOnlySequence<byte> byteSequence)
        {
            return byteSequence.First.Span[0] == MessagePackCode.Nil;
        }

        public static void WriteRaw(IBufferWriter<byte> writer, ReadOnlySpan<byte> rawMessagePackBlock)
        {
            var span = writer.GetSpan(rawMessagePackBlock.Length);
            rawMessagePackBlock.CopyTo(span);
            writer.Advance(span.Length);
        }

        /// <summary>
        /// Unsafe. If value is guranteed 0 ~ MessagePackRange.MaxFixMapCount(15), can use this method.
        /// </summary>
        /// <returns></returns>
#if NETSTANDARD || NETFRAMEWORK
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
#endif
        public static int WriteFixedMapHeaderUnsafe(IBufferWriter<byte> writer, int count)
        {
            var span = writer.GetSpan(1);
            span[0] = (byte)(MessagePackCode.MinFixMap | count);
            return 1;
        }

        /// <summary>
        /// Write map count.
        /// </summary>
#if NETSTANDARD || NETFRAMEWORK
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
#endif
        public static void WriteMapHeader(IBufferWriter<byte> writer, int count)
        {
            checked
            {
                WriteMapHeader(writer, (uint)count);
            }
        }

        /// <summary>
        /// Write map count.
        /// </summary>
#if NETSTANDARD || NETFRAMEWORK
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
#endif
        public static void WriteMapHeader(IBufferWriter<byte> writer, uint count)
        {
            if (count <= MessagePackRange.MaxFixMapCount)
            {
                var span = writer.GetSpan(1);
                span[0] = (byte)(MessagePackCode.MinFixMap | count);
                writer.Advance(1);
            }
            else if (count <= ushort.MaxValue)
            {
                var span = writer.GetSpan(3);
                unchecked
                {
                    span[0] = MessagePackCode.Map16;
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
                    span[0] = MessagePackCode.Map32;
                    span[1] = (byte)(count >> 24);
                    span[2] = (byte)(count >> 16);
                    span[3] = (byte)(count >> 8);
                    span[4] = (byte)(count);
                }
                writer.Advance(5);
            }
        }

        /// <summary>
        /// Write map format header, always use map32 format(length is fixed, 5).
        /// </summary>
#if NETSTANDARD || NETFRAMEWORK
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
#endif
        public static void WriteMapHeaderForceMap32Block(IBufferWriter<byte> writer, uint count)
        {
            var span = writer.GetSpan(5);
            unchecked
            {
                span[0] = MessagePackCode.Map32;
                span[1] = (byte)(count >> 24);
                span[2] = (byte)(count >> 16);
                span[3] = (byte)(count >> 8);
                span[4] = (byte)(count);
            }
            writer.Advance(5);
        }

        /// <summary>
        /// Return map count.
        /// </summary>
#if NETSTANDARD || NETFRAMEWORK
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
#endif
        public static int ReadMapHeader(ref ReadOnlySequence<byte> byteSequence)
        {
            checked
            {
                return (int)mapHeaderDecoders[byteSequence.First.Span[0]].Read(ref byteSequence);
            }
        }

        /// <summary>
        /// Return map count.
        /// </summary>
#if NETSTANDARD || NETFRAMEWORK
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
#endif
        public static uint ReadMapHeaderRaw(ref ReadOnlySequence<byte> byteSequence)
        {
            return mapHeaderDecoders[byteSequence.First.Span[0]].Read(ref byteSequence);
        }

#if NETSTANDARD || NETFRAMEWORK
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
#endif
        public static int GetArrayHeaderLength(int count)
        {
            if (count <= MessagePackRange.MaxFixArrayCount)
            {
                return 1;
            }
            else if (count <= ushort.MaxValue)
            {
                return 3;
            }
            else
            {
                return 5;
            }
        }

        /// <summary>
        /// Unsafe. If value is guranteed 0 ~ MessagePackRange.MaxFixArrayCount(15), can use this method.
        /// </summary>
        /// <returns></returns>
#if NETSTANDARD || NETFRAMEWORK
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
#endif
        public static void WriteFixedArrayHeaderUnsafe(IBufferWriter<byte> writer, int count)
        {
            var span = writer.GetSpan(1);
            span[0] = (byte)(MessagePackCode.MinFixArray | count);
            writer.Advance(1);
        }

        /// <summary>
        /// Write array count.
        /// </summary>
#if NETSTANDARD || NETFRAMEWORK
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
#endif
        public static void WriteArrayHeader(IBufferWriter<byte> writer, int count)
        {
            checked
            {
                WriteArrayHeader(writer, (uint)count);
            }
        }

        /// <summary>
        /// Write array count.
        /// </summary>
#if NETSTANDARD || NETFRAMEWORK
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
#endif
        public static void WriteArrayHeader(IBufferWriter<byte> writer, uint count)
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
        /// Write array format header, always use array32 format(length is fixed, 5).
        /// </summary>
#if NETSTANDARD || NETFRAMEWORK
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
#endif
        public static int WriteArrayHeaderForceArray32Block(IBufferWriter<byte> writer, uint count)
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
            return 5;
        }

        /// <summary>
        /// Return array count.
        /// </summary>
#if NETSTANDARD || NETFRAMEWORK
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
#endif
        public static int ReadArrayHeader(ref ReadOnlySequence<byte> byteSequence)
        {
            checked
            {
                return (int)arrayHeaderDecoders[byteSequence.First.Span[0]].Read(ref byteSequence);
            }
        }

        /// <summary>
        /// Return array count.
        /// </summary>
#if NETSTANDARD || NETFRAMEWORK
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
#endif
        public static uint ReadArrayHeaderRaw(ref ReadOnlySequence<byte> byteSequence)
        {
            return arrayHeaderDecoders[byteSequence.First.Span[0]].Read(ref byteSequence);
        }

#if NETSTANDARD || NETFRAMEWORK
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
#endif
        public static int WriteBoolean(IBufferWriter<byte> writer, bool value)
        {
            var span = writer.GetSpan(1);

            span[0] = (value ? MessagePackCode.True : MessagePackCode.False);
            return 1;
        }

#if NETSTANDARD || NETFRAMEWORK
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
#endif
        public static bool ReadBoolean(ref ReadOnlySequence<byte> byteSequence)
        {
            bool result = booleanDecoders[byteSequence.First.Span[0]].Read();
            byteSequence = byteSequence.Slice(1);
            return result;
        }

#if NETSTANDARD || NETFRAMEWORK
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
#endif
        public static int WriteByte(IBufferWriter<byte> writer, byte value)
        {
            if (value <= MessagePackCode.MaxFixInt)
            {
                var span = writer.GetSpan(1);
                span[0] = value;
                return 1;
            }
            else
            {
                var span = writer.GetSpan(2);
                span[0] = MessagePackCode.UInt8;
                span[1] = value;
                return 2;
            }
        }

#if NETSTANDARD || NETFRAMEWORK
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
#endif
        public static int WriteByteForceByteBlock(IBufferWriter<byte> writer, byte value)
        {
            var span = writer.GetSpan(2);
            span[0] = MessagePackCode.UInt8;
            span[1] = value;
            return 2;
        }

#if NETSTANDARD || NETFRAMEWORK
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
#endif
        public static byte ReadByte(ref ReadOnlySequence<byte> byteSequence)
        {
            return byteDecoders[byteSequence.First.Span[0]].Read(ref byteSequence);
        }

#if NETSTANDARD || NETFRAMEWORK
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
#endif
        public static void WriteBytes(IBufferWriter<byte> writer, ReadOnlySpan<byte> src)
        {
            if (src == null)
            {
                WriteNil(writer);
            }

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

#if NETSTANDARD || NETFRAMEWORK
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
#endif
        public static byte[] ReadBytes(ref ReadOnlySequence<byte> byteSequence)
        {
            return bytesDecoders[byteSequence.First.Span[0]].Read(ref byteSequence);
        }

#if NETSTANDARD || NETFRAMEWORK
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
#endif
        public static ArraySegment<byte> ReadBytesSegment(ref ReadOnlySequence<byte> byteSequence)
        {
            return bytesSegmentDecoders[byteSequence.First.Span[0]].Read(ref byteSequence);
        }

#if NETSTANDARD || NETFRAMEWORK
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
#endif
        public static int WriteSByte(IBufferWriter<byte> writer, sbyte value)
        {
            if (value < MessagePackRange.MinFixNegativeInt)
            {
                var span = writer.GetSpan(2);
                span[0] = MessagePackCode.Int8;
                span[1] = unchecked((byte)(value));
                return 2;
            }
            else
            {
                var span = writer.GetSpan(1);
                span[0] = unchecked((byte)value);
                return 1;
            }
        }

#if NETSTANDARD || NETFRAMEWORK
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
#endif
        public static int WriteSByteForceSByteBlock(IBufferWriter<byte> writer, sbyte value)
        {
            var span = writer.GetSpan(2);
            span[0] = MessagePackCode.Int8;
            span[1] = unchecked((byte)(value));
            return 2;
        }

#if NETSTANDARD || NETFRAMEWORK
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
#endif
        public static sbyte ReadSByte(ref ReadOnlySequence<byte> byteSequence)
        {
            return sbyteDecoders[byteSequence.First.Span[0]].Read(ref byteSequence);
        }

#if NETSTANDARD || NETFRAMEWORK
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
#endif
        public static int WriteSingle(IBufferWriter<byte> writer, float value)
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

            return 5;
        }

#if NETSTANDARD || NETFRAMEWORK
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
#endif
        public static float ReadSingle(ref ReadOnlySequence<byte> byteSequence)
        {
            return singleDecoders[byteSequence.First.Span[0]].Read(ref byteSequence);
        }

#if NETSTANDARD || NETFRAMEWORK
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
#endif
        public static int WriteDouble(IBufferWriter<byte> writer, double value)
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

            return 9;
        }

#if NETSTANDARD || NETFRAMEWORK
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
#endif
        public static double ReadDouble(ref ReadOnlySequence<byte> byteSequence)
        {
            return doubleDecoders[byteSequence.First.Span[0]].Read(ref byteSequence);
        }

#if NETSTANDARD || NETFRAMEWORK
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
#endif
        public static int WriteInt16(IBufferWriter<byte> writer, short value)
        {
            if (value >= 0)
            {
                // positive int(use uint)
                if (value <= MessagePackRange.MaxFixPositiveInt)
                {
                    var span = writer.GetSpan(1);
                    span[0] = unchecked((byte)value);
                    return 1;
                }
                else if (value <= byte.MaxValue)
                {
                    var span = writer.GetSpan(2);
                    span[0] = MessagePackCode.UInt8;
                    span[1] = unchecked((byte)value);
                    return 2;
                }
                else
                {
                    var span = writer.GetSpan(3);
                    span[0] = MessagePackCode.UInt16;
                    span[1] = unchecked((byte)(value >> 8));
                    span[2] = unchecked((byte)value);
                    return 3;
                }
            }
            else
            {
                // negative int(use int)
                if (MessagePackRange.MinFixNegativeInt <= value)
                {
                    var span = writer.GetSpan(1);
                    span[0] = unchecked((byte)value);
                    return 1;
                }
                else if (sbyte.MinValue <= value)
                {
                    var span = writer.GetSpan(2);
                    span[0] = MessagePackCode.Int8;
                    span[1] = unchecked((byte)value);
                    return 2;
                }
                else
                {
                    var span = writer.GetSpan(3);
                    span[0] = MessagePackCode.Int16;
                    span[1] = unchecked((byte)(value >> 8));
                    span[2] = unchecked((byte)value);
                    return 3;
                }
            }
        }

#if NETSTANDARD || NETFRAMEWORK
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
#endif
        public static int WriteInt16ForceInt16Block(IBufferWriter<byte> writer, short value)
        {
            var span = writer.GetSpan(3);
            span[0] = MessagePackCode.Int16;
            span[1] = unchecked((byte)(value >> 8));
            span[2] = unchecked((byte)value);
            return 3;
        }

#if NETSTANDARD || NETFRAMEWORK
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
#endif
        public static short ReadInt16(ref ReadOnlySequence<byte> byteSequence)
        {
            return int16Decoders[byteSequence.First.Span[0]].Read(ref byteSequence);
        }

        /// <summary>
        /// Unsafe. If value is guranteed 0 ~ MessagePackCode.MaxFixInt(127), can use this method.
        /// </summary>
#if NETSTANDARD || NETFRAMEWORK
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
#endif
        public static int WritePositiveFixedIntUnsafe(IBufferWriter<byte> writer, int value)
        {
            var span = writer.GetSpan(1);
            span[0] = (byte)value;
            return 1;
        }

#if NETSTANDARD || NETFRAMEWORK
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
#endif
        public static void WriteInt32(IBufferWriter<byte> writer, int value)
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
                else if (value <= ushort.MaxValue)
                {
                    var span = writer.GetSpan(3);
                    span[0] = MessagePackCode.UInt16;
                    span[1] = unchecked((byte)(value >> 8));
                    span[2] = unchecked((byte)value);
                    writer.Advance(3);
                }
                else
                {
                    var span = writer.GetSpan(5);
                    span[0] = MessagePackCode.UInt32;
                    span[1] = unchecked((byte)(value >> 24));
                    span[2] = unchecked((byte)(value >> 16));
                    span[3] = unchecked((byte)(value >> 8));
                    span[4] = unchecked((byte)value);
                    writer.Advance(5);
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
                else if (short.MinValue <= value)
                {
                    var span = writer.GetSpan(3);
                    span[0] = MessagePackCode.Int16;
                    span[1] = unchecked((byte)(value >> 8));
                    span[2] = unchecked((byte)value);
                    writer.Advance(3);
                }
                else
                {
                    var span = writer.GetSpan(5);
                    span[0] = MessagePackCode.Int32;
                    span[1] = unchecked((byte)(value >> 24));
                    span[2] = unchecked((byte)(value >> 16));
                    span[3] = unchecked((byte)(value >> 8));
                    span[4] = unchecked((byte)value);
                    writer.Advance(5);
                }
            }
        }

        /// <summary>
        /// Acquire static message block(always 5 bytes).
        /// </summary>
#if NETSTANDARD || NETFRAMEWORK
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
#endif
        public static int WriteInt32ForceInt32Block(IBufferWriter<byte> writer, int value)
        {
            var span = writer.GetSpan(5);
            span[0] = MessagePackCode.Int32;
            span[1] = unchecked((byte)(value >> 24));
            span[2] = unchecked((byte)(value >> 16));
            span[3] = unchecked((byte)(value >> 8));
            span[4] = unchecked((byte)value);
            return 5;
        }

#if NETSTANDARD || NETFRAMEWORK
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
#endif
        public static int ReadInt32(ref ReadOnlySequence<byte> byteSequence)
        {
            return int32Decoders[byteSequence.First.Span[0]].Read(ref byteSequence);
        }

#if NETSTANDARD || NETFRAMEWORK
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
#endif
        public static void WriteInt64(IBufferWriter<byte> writer, long value)
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
                else if (value <= ushort.MaxValue)
                {
                    var span = writer.GetSpan(3);
                    span[0] = MessagePackCode.UInt16;
                    span[1] = unchecked((byte)(value >> 8));
                    span[2] = unchecked((byte)value);
                    writer.Advance(3);
                }
                else if (value <= uint.MaxValue)
                {
                    var span = writer.GetSpan(5);
                    span[0] = MessagePackCode.UInt32;
                    span[1] = unchecked((byte)(value >> 24));
                    span[2] = unchecked((byte)(value >> 16));
                    span[3] = unchecked((byte)(value >> 8));
                    span[4] = unchecked((byte)value);
                    writer.Advance(5);
                }
                else
                {
                    var span = writer.GetSpan(9);
                    span[0] = MessagePackCode.UInt64;
                    span[1] = unchecked((byte)(value >> 56));
                    span[2] = unchecked((byte)(value >> 48));
                    span[3] = unchecked((byte)(value >> 40));
                    span[4] = unchecked((byte)(value >> 32));
                    span[5] = unchecked((byte)(value >> 24));
                    span[6] = unchecked((byte)(value >> 16));
                    span[7] = unchecked((byte)(value >> 8));
                    span[8] = unchecked((byte)value);
                    writer.Advance(9);
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
                else if (short.MinValue <= value)
                {
                    var span = writer.GetSpan(3);
                    span[0] = MessagePackCode.Int16;
                    span[1] = unchecked((byte)(value >> 8));
                    span[2] = unchecked((byte)value);
                    writer.Advance(3);
                }
                else if (int.MinValue <= value)
                {
                    var span = writer.GetSpan(5);
                    span[0] = MessagePackCode.Int32;
                    span[1] = unchecked((byte)(value >> 24));
                    span[2] = unchecked((byte)(value >> 16));
                    span[3] = unchecked((byte)(value >> 8));
                    span[4] = unchecked((byte)value);
                    writer.Advance(5);
                }
                else
                {
                    var span = writer.GetSpan(9);
                    span[0] = MessagePackCode.Int64;
                    span[1] = unchecked((byte)(value >> 56));
                    span[2] = unchecked((byte)(value >> 48));
                    span[3] = unchecked((byte)(value >> 40));
                    span[4] = unchecked((byte)(value >> 32));
                    span[5] = unchecked((byte)(value >> 24));
                    span[6] = unchecked((byte)(value >> 16));
                    span[7] = unchecked((byte)(value >> 8));
                    span[8] = unchecked((byte)value);
                    writer.Advance(9);
                }
            }
        }

#if NETSTANDARD || NETFRAMEWORK
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
#endif
        public static void WriteInt64ForceInt64Block(IBufferWriter<byte> writer, long value)
        {
            var span = writer.GetSpan(9);
            span[0] = MessagePackCode.Int64;
            span[1] = unchecked((byte)(value >> 56));
            span[2] = unchecked((byte)(value >> 48));
            span[3] = unchecked((byte)(value >> 40));
            span[4] = unchecked((byte)(value >> 32));
            span[5] = unchecked((byte)(value >> 24));
            span[6] = unchecked((byte)(value >> 16));
            span[7] = unchecked((byte)(value >> 8));
            span[8] = unchecked((byte)value);
            writer.Advance(9);
        }

#if NETSTANDARD || NETFRAMEWORK
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
#endif
        public static long ReadInt64(ref ReadOnlySequence<byte> byteSequence)
        {
            return int64Decoders[byteSequence.First.Span[0]].Read(ref byteSequence);
        }

#if NETSTANDARD || NETFRAMEWORK
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
#endif
        public static void WriteUInt16(IBufferWriter<byte> writer, ushort value)
        {
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

#if NETSTANDARD || NETFRAMEWORK
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
#endif
        public static void WriteUInt16ForceUInt16Block(IBufferWriter<byte> writer, ushort value)
        {
            var span = writer.GetSpan(3);
            span[0] = MessagePackCode.UInt16;
            span[1] = unchecked((byte)(value >> 8));
            span[2] = unchecked((byte)value);
            writer.Advance(3);
        }

#if NETSTANDARD || NETFRAMEWORK
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
#endif
        public static ushort ReadUInt16(ref ReadOnlySequence<byte> byteSequence)
        {
            return uint16Decoders[byteSequence.First.Span[0]].Read(ref byteSequence);
        }

#if NETSTANDARD || NETFRAMEWORK
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
#endif
        public static int WriteUInt32(IBufferWriter<byte> writer, uint value)
        {
            if (value <= MessagePackRange.MaxFixPositiveInt)
            {
                var span = writer.GetSpan(1);
                span[0] = unchecked((byte)value);
                return 1;
            }
            else if (value <= byte.MaxValue)
            {
                var span = writer.GetSpan(2);
                span[0] = MessagePackCode.UInt8;
                span[1] = unchecked((byte)value);
                return 2;
            }
            else if (value <= ushort.MaxValue)
            {
                var span = writer.GetSpan(3);
                span[0] = MessagePackCode.UInt16;
                span[1] = unchecked((byte)(value >> 8));
                span[2] = unchecked((byte)value);
                return 3;
            }
            else
            {
                var span = writer.GetSpan(5);
                span[0] = MessagePackCode.UInt32;
                span[1] = unchecked((byte)(value >> 24));
                span[2] = unchecked((byte)(value >> 16));
                span[3] = unchecked((byte)(value >> 8));
                span[4] = unchecked((byte)value);
                return 5;
            }
        }

#if NETSTANDARD || NETFRAMEWORK
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
#endif
        public static int WriteUInt32ForceUInt32Block(IBufferWriter<byte> writer, uint value)
        {
            var span = writer.GetSpan(5);
            span[0] = MessagePackCode.UInt32;
            span[1] = unchecked((byte)(value >> 24));
            span[2] = unchecked((byte)(value >> 16));
            span[3] = unchecked((byte)(value >> 8));
            span[4] = unchecked((byte)value);
            return 5;
        }

#if NETSTANDARD || NETFRAMEWORK
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
#endif
        public static uint ReadUInt32(ref ReadOnlySequence<byte> byteSequence)
        {
            return uint32Decoders[byteSequence.First.Span[0]].Read(ref byteSequence);
        }

#if NETSTANDARD || NETFRAMEWORK
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
#endif
        public static void WriteUInt64(IBufferWriter<byte> writer, ulong value)
        {
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
            else if (value <= ushort.MaxValue)
            {
                var span = writer.GetSpan(3);
                span[0] = MessagePackCode.UInt16;
                span[1] = unchecked((byte)(value >> 8));
                span[2] = unchecked((byte)value);
                writer.Advance(3);
            }
            else if (value <= uint.MaxValue)
            {
                var span = writer.GetSpan(5);
                span[0] = MessagePackCode.UInt32;
                span[1] = unchecked((byte)(value >> 24));
                span[2] = unchecked((byte)(value >> 16));
                span[3] = unchecked((byte)(value >> 8));
                span[4] = unchecked((byte)value);
                writer.Advance(5);
            }
            else
            {
                var span = writer.GetSpan(9);
                span[0] = MessagePackCode.UInt64;
                span[1] = unchecked((byte)(value >> 56));
                span[2] = unchecked((byte)(value >> 48));
                span[3] = unchecked((byte)(value >> 40));
                span[4] = unchecked((byte)(value >> 32));
                span[5] = unchecked((byte)(value >> 24));
                span[6] = unchecked((byte)(value >> 16));
                span[7] = unchecked((byte)(value >> 8));
                span[8] = unchecked((byte)value);
                writer.Advance(9);
            }
        }

#if NETSTANDARD || NETFRAMEWORK
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
#endif
        public static void WriteUInt64ForceUInt64Block(IBufferWriter<byte> writer, ulong value)
        {
            var span = writer.GetSpan(9);
            span[0] = MessagePackCode.UInt64;
            span[1] = unchecked((byte)(value >> 56));
            span[2] = unchecked((byte)(value >> 48));
            span[3] = unchecked((byte)(value >> 40));
            span[4] = unchecked((byte)(value >> 32));
            span[5] = unchecked((byte)(value >> 24));
            span[6] = unchecked((byte)(value >> 16));
            span[7] = unchecked((byte)(value >> 8));
            span[8] = unchecked((byte)value);
            writer.Advance(9);
        }

#if NETSTANDARD || NETFRAMEWORK
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
#endif
        public static ulong ReadUInt64(ref ReadOnlySequence<byte> byteSequence)
        {
            return uint64Decoders[byteSequence.First.Span[0]].Read(ref byteSequence);
        }

#if NETSTANDARD || NETFRAMEWORK
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
#endif
        public static void WriteChar(IBufferWriter<byte> writer, char value)
        {
            WriteUInt16(writer, value);
        }

#if NETSTANDARD || NETFRAMEWORK
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
#endif
        public static char ReadChar(ref ReadOnlySequence<byte> byteSequence)
        {
            return (char)ReadUInt16(ref byteSequence);
        }

        /// <summary>
        /// Unsafe. If value is guaranteed length is 0 ~ 31, can use this method.
        /// </summary>
#if NETSTANDARD || NETFRAMEWORK
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
#endif
        public static unsafe void WriteFixedStringUnsafe(IBufferWriter<byte> writer, string value, int byteCount)
        {
            var span = writer.GetSpan(byteCount + 1);
            span[0] = (byte)(MessagePackCode.MinFixStr | byteCount);
            writer.Advance(StringEncoding.UTF8.GetBytes(value, span.Slice(1)) + 1);
        }

        /// <summary>
        /// Unsafe. If pre-calculated byteCount of target string, can use this method.
        /// </summary>
#if NETSTANDARD || NETFRAMEWORK
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
#endif
        public static int WriteStringUnsafe(IBufferWriter<byte> writer, string value, int byteCount)
        {
            if (byteCount <= MessagePackRange.MaxFixStringLength)
            {
                var span = writer.GetSpan(byteCount + 1);
                span[0] = (byte)(MessagePackCode.MinFixStr | byteCount);
                StringEncoding.UTF8.GetBytes(value, span.Slice(1));
                return byteCount + 1;
            }
            else if (byteCount <= byte.MaxValue)
            {
                var span = writer.GetSpan(byteCount + 2);
                span[0] = MessagePackCode.Str8;
                span[1] = unchecked((byte)byteCount);
                StringEncoding.UTF8.GetBytes(value, span.Slice(2));
                return byteCount + 2;
            }
            else if (byteCount <= ushort.MaxValue)
            {
                var span = writer.GetSpan(byteCount + 3);
                span[0] = MessagePackCode.Str16;
                span[1] = unchecked((byte)(byteCount >> 8));
                span[2] = unchecked((byte)byteCount);
                StringEncoding.UTF8.GetBytes(value, span.Slice(3));
                return byteCount + 3;
            }
            else
            {
                var span = writer.GetSpan(byteCount + 5);
                span[0] = MessagePackCode.Str32;
                span[1] = unchecked((byte)(byteCount >> 24));
                span[2] = unchecked((byte)(byteCount >> 16));
                span[3] = unchecked((byte)(byteCount >> 8));
                span[4] = unchecked((byte)byteCount);
                StringEncoding.UTF8.GetBytes(value, span.Slice(5));
                return byteCount + 5;
            }
        }

#if NETSTANDARD || NETFRAMEWORK
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
#endif
        public static void WriteStringBytes(IBufferWriter<byte> writer, ReadOnlySpan<byte> utf8stringBytes)
        {
            var byteCount = utf8stringBytes.Length;
            if (byteCount <= MessagePackRange.MaxFixStringLength)
            {
                var span = writer.GetSpan(byteCount + 1);
                span[0] = (byte)(MessagePackCode.MinFixStr | byteCount);
                utf8stringBytes.CopyTo(span.Slice(1));
                writer.Advance(1);
            }
            else if (byteCount <= byte.MaxValue)
            {
                var span = writer.GetSpan(byteCount + 2);
                span[0] = MessagePackCode.Str8;
                span[1] = unchecked((byte)byteCount);
                utf8stringBytes.CopyTo(span.Slice(2));
                writer.Advance(2);
            }
            else if (byteCount <= ushort.MaxValue)
            {
                var span = writer.GetSpan(byteCount + 3);
                span[0] = MessagePackCode.Str16;
                span[1] = unchecked((byte)(byteCount >> 8));
                span[2] = unchecked((byte)byteCount);
                utf8stringBytes.CopyTo(span.Slice(3));
                writer.Advance(3);
            }
            else
            {
                var span = writer.GetSpan(byteCount + 5);
                span[0] = MessagePackCode.Str32;
                span[1] = unchecked((byte)(byteCount >> 24));
                span[2] = unchecked((byte)(byteCount >> 16));
                span[3] = unchecked((byte)(byteCount >> 8));
                span[4] = unchecked((byte)byteCount);
                utf8stringBytes.CopyTo(span.Slice(5));
                writer.Advance(5);
            }
        }

        public static byte[] GetEncodedStringBytes(string value)
        {
            var byteCount = StringEncoding.UTF8.GetByteCount(value);
            if (byteCount <= MessagePackRange.MaxFixStringLength)
            {
                var bytes = new byte[byteCount + 1];
                bytes[0] = (byte)(MessagePackCode.MinFixStr | byteCount);
                StringEncoding.UTF8.GetBytes(value, 0, value.Length, bytes, 1);
                return bytes;
            }
            else if (byteCount <= byte.MaxValue)
            {
                var bytes = new byte[byteCount + 2];
                bytes[0] = MessagePackCode.Str8;
                bytes[1] = unchecked((byte)byteCount);
                StringEncoding.UTF8.GetBytes(value, 0, value.Length, bytes, 2);
                return bytes;
            }
            else if (byteCount <= ushort.MaxValue)
            {
                var bytes = new byte[byteCount + 3];
                bytes[0] = MessagePackCode.Str16;
                bytes[1] = unchecked((byte)(byteCount >> 8));
                bytes[2] = unchecked((byte)byteCount);
                StringEncoding.UTF8.GetBytes(value, 0, value.Length, bytes, 3);
                return bytes;
            }
            else
            {
                var bytes = new byte[byteCount + 5];
                bytes[0] = MessagePackCode.Str32;
                bytes[1] = unchecked((byte)(byteCount >> 24));
                bytes[2] = unchecked((byte)(byteCount >> 16));
                bytes[3] = unchecked((byte)(byteCount >> 8));
                bytes[4] = unchecked((byte)byteCount);
                StringEncoding.UTF8.GetBytes(value, 0, value.Length, bytes, 5);
                return bytes;
            }
        }

        public static void WriteString(IBufferWriter<byte> writer, string value)
        {
            if (value == null)
            {
                WriteNil(writer);
                return;
            }

            // MaxByteCount -> WritePrefix -> GetBytes has some overheads of `MaxByteCount`
            // solves heuristic length check

            // ensure buffer by MaxByteCount(faster than GetByteCount)
            var span = writer.GetSpan(StringEncoding.UTF8.GetMaxByteCount(value.Length) + 5);

            int useOffset;
            if (value.Length <= MessagePackRange.MaxFixStringLength)
            {
                useOffset = 1;
            }
            else if (value.Length <= byte.MaxValue)
            {
                useOffset = 2;
            }
            else if (value.Length <= ushort.MaxValue)
            {
                useOffset = 3;
            }
            else
            {
                useOffset = 5;
            }

            // skip length area
            var byteCount = StringEncoding.UTF8.GetBytes(value, span.Slice(useOffset));

            // move body and write prefix
            if (byteCount <= MessagePackRange.MaxFixStringLength)
            {
                if (useOffset != 1)
                {
                    span.Slice(useOffset, byteCount).CopyTo(span.Slice(1));
                }
                span[0] = (byte)(MessagePackCode.MinFixStr | byteCount);
                writer.Advance(byteCount + 1);
            }
            else if (byteCount <= byte.MaxValue)
            {
                if (useOffset != 2)
                {
                    span.Slice(useOffset, byteCount).CopyTo(span.Slice(2));
                }

                span[0] = MessagePackCode.Str8;
                span[1] = unchecked((byte)byteCount);
                writer.Advance(byteCount + 2);
            }
            else if (byteCount <= ushort.MaxValue)
            {
                if (useOffset != 3)
                {
                    span.Slice(useOffset, byteCount).CopyTo(span.Slice(3));
                }

                span[0] = MessagePackCode.Str16;
                span[1] = unchecked((byte)(byteCount >> 8));
                span[2] = unchecked((byte)byteCount);
                writer.Advance(byteCount + 3);
            }
            else
            {
                if (useOffset != 5)
                {
                    span.Slice(useOffset, byteCount).CopyTo(span.Slice(5));
                }

                span[0] = MessagePackCode.Str32;
                span[1] = unchecked((byte)(byteCount >> 24));
                span[2] = unchecked((byte)(byteCount >> 16));
                span[3] = unchecked((byte)(byteCount >> 8));
                span[4] = unchecked((byte)byteCount);
                writer.Advance(byteCount + 5);
            }
        }

        public static void WriteStringForceStr32Block(IBufferWriter<byte> writer, string value)
        {
            if (value == null)
            {
                WriteNil(writer);
                return;
            }

            var span = writer.GetSpan(StringEncoding.UTF8.GetMaxByteCount(value.Length) + 5);

            var byteCount = StringEncoding.UTF8.GetBytes(value, span.Slice(5));

            span[0] = MessagePackCode.Str32;
            span[1] = unchecked((byte)(byteCount >> 24));
            span[2] = unchecked((byte)(byteCount >> 16));
            span[3] = unchecked((byte)(byteCount >> 8));
            span[4] = unchecked((byte)byteCount);
            writer.Advance(byteCount + 5);
        }

#if NETSTANDARD || NETFRAMEWORK
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
#endif
        public static string ReadString(ref ReadOnlySequence<byte> byteSequence)
        {
            return stringDecoders[byteSequence.First.Span[0]].Read(ref byteSequence);
        }

#if NETSTANDARD || NETFRAMEWORK
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
#endif
        public static ArraySegment<byte> ReadStringSegment(ref ReadOnlySequence<byte> byteSequence)
        {
            return stringSegmentDecoders[byteSequence.First.Span[0]].Read(ref byteSequence);
        }

#if NETSTANDARD || NETFRAMEWORK
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
#endif
        public static int WriteExtensionFormatHeader(IBufferWriter<byte> writer, sbyte typeCode, int dataLength)
        {
            switch (dataLength)
            {
                case 1:
                    var span = writer.GetSpan(3);
                    span[0] = MessagePackCode.FixExt1;
                    span[1] = unchecked((byte)typeCode);
                    return 2;
                case 2:
                    span = writer.GetSpan(4);
                    span[0] = MessagePackCode.FixExt2;
                    span[1] = unchecked((byte)typeCode);
                    return 2;
                case 4:
                    span = writer.GetSpan(6);
                    span[0] = MessagePackCode.FixExt4;
                    span[1] = unchecked((byte)typeCode);
                    return 2;
                case 8:
                    span = writer.GetSpan(10);
                    span[0] = MessagePackCode.FixExt8;
                    span[1] = unchecked((byte)typeCode);
                    return 2;
                case 16:
                    span = writer.GetSpan(18);
                    span[0] = MessagePackCode.FixExt16;
                    span[1] = unchecked((byte)typeCode);
                    return 2;
                default:
                    unchecked
                    {
                        if (dataLength <= byte.MaxValue)
                        {
                            span = writer.GetSpan(dataLength + 3);
                            span[0] = MessagePackCode.Ext8;
                            span[1] = unchecked((byte)(dataLength));
                            span[2] = unchecked((byte)typeCode);
                            return 3;
                        }
                        else if (dataLength <= UInt16.MaxValue)
                        {
                            span = writer.GetSpan(dataLength + 4);
                            span[0] = MessagePackCode.Ext16;
                            span[1] = unchecked((byte)(dataLength >> 8));
                            span[2] = unchecked((byte)(dataLength));
                            span[3] = unchecked((byte)typeCode);
                            return 4;
                        }
                        else
                        {
                            span = writer.GetSpan(dataLength + 6);
                            span[0] = MessagePackCode.Ext32;
                            span[1] = unchecked((byte)(dataLength >> 24));
                            span[2] = unchecked((byte)(dataLength >> 16));
                            span[3] = unchecked((byte)(dataLength >> 8));
                            span[4] = unchecked((byte)dataLength);
                            span[5] = unchecked((byte)typeCode);
                            return 6;
                        }
                    }
            }
        }

        /// <summary>
        /// Write extension format header, always use ext32 format(length is fixed, 6).
        /// </summary>
#if NETSTANDARD || NETFRAMEWORK
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
#endif
        public static int WriteExtensionFormatHeaderForceExt32Block(IBufferWriter<byte> writer, sbyte typeCode, int dataLength)
        {
            var span = writer.GetSpan(dataLength + 6);
            span[0] = MessagePackCode.Ext32;
            span[1] = unchecked((byte)(dataLength >> 24));
            span[2] = unchecked((byte)(dataLength >> 16));
            span[3] = unchecked((byte)(dataLength >> 8));
            span[4] = unchecked((byte)dataLength);
            span[5] = unchecked((byte)typeCode);
            return 6;
        }

#if NETSTANDARD || NETFRAMEWORK
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
#endif
        public static void WriteExtensionFormat(IBufferWriter<byte> writer, sbyte typeCode, ReadOnlySpan<byte> data)
        {
            var length = data.Length;
            switch (length)
            {
                case 1:
                    var span = writer.GetSpan(3);
                    span[0] = MessagePackCode.FixExt1;
                    span[1] = unchecked((byte)typeCode);
                    span[2] = data[0];
                    writer.Advance(3);
                    return;
                case 2:
                    span = writer.GetSpan(4);
                    span[0] = MessagePackCode.FixExt2;
                    span[1] = unchecked((byte)typeCode);
                    span[2] = data[0];
                    span[3] = data[1];
                    writer.Advance(4);
                    return;
                case 4:
                    span = writer.GetSpan(6);
                    span[0] = MessagePackCode.FixExt4;
                    span[1] = unchecked((byte)typeCode);
                    span[2] = data[0];
                    span[3] = data[1];
                    span[4] = data[2];
                    span[5] = data[3];
                    writer.Advance(6);
                    return;
                case 8:
                    span = writer.GetSpan(10);
                    span[0] = MessagePackCode.FixExt8;
                    span[1] = unchecked((byte)typeCode);
                    span[2] = data[0];
                    span[3] = data[1];
                    span[4] = data[2];
                    span[5] = data[3];
                    span[6] = data[4];
                    span[7] = data[5];
                    span[8] = data[6];
                    span[9] = data[7];
                    writer.Advance(10);
                    return;
                case 16:
                    span = writer.GetSpan(18);
                    span[0] = MessagePackCode.FixExt16;
                    span[1] = unchecked((byte)typeCode);
                    span[2] = data[0];
                    span[3] = data[1];
                    span[4] = data[2];
                    span[5] = data[3];
                    span[6] = data[4];
                    span[7] = data[5];
                    span[8] = data[6];
                    span[9] = data[7];
                    span[10] = data[8];
                    span[11] = data[9];
                    span[12] = data[10];
                    span[13] = data[11];
                    span[14] = data[12];
                    span[15] = data[13];
                    span[16] = data[14];
                    span[17] = data[15];
                    writer.Advance(18);
                    return;
                default:
                    unchecked
                    {
                        if (data.Length <= byte.MaxValue)
                        {
                            span = writer.GetSpan(length + 3);
                            span[0] = MessagePackCode.Ext8;
                            span[1] = unchecked((byte)(length));
                            span[2] = unchecked((byte)typeCode);
                            data.Slice(0, length).CopyTo(span.Slice(3));
                            writer.Advance(length + 3);
                            return;
                        }
                        else if (data.Length <= UInt16.MaxValue)
                        {
                            span = writer.GetSpan(length + 4);
                            span[0] = MessagePackCode.Ext16;
                            span[1] = unchecked((byte)(length >> 8));
                            span[2] = unchecked((byte)(length));
                            span[3] = unchecked((byte)typeCode);
                            data.Slice(0, length).CopyTo(span.Slice(4));
                            writer.Advance(length + 4);
                            return;
                        }
                        else
                        {
                            span = writer.GetSpan(length + 6);
                            span[0] = MessagePackCode.Ext32;
                            span[1] = unchecked((byte)(length >> 24));
                            span[2] = unchecked((byte)(length >> 16));
                            span[3] = unchecked((byte)(length >> 8));
                            span[4] = unchecked((byte)length);
                            span[5] = unchecked((byte)typeCode);
                            data.Slice(0, length).CopyTo(span.Slice(6));
                            writer.Advance(length + 6);
                            return;
                        }
                    }
            }
        }

#if NETSTANDARD || NETFRAMEWORK
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
#endif
        public static ExtensionResult ReadExtensionFormat(ref ReadOnlySequence<byte> byteSequence)
        {
            return extDecoders[byteSequence.First.Span[0]].Read(ref byteSequence);
        }

        /// <summary>
        /// return byte length of ExtensionFormat.
        /// </summary>
#if NETSTANDARD || NETFRAMEWORK
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
#endif
        public static ExtensionHeader ReadExtensionFormatHeader(ref ReadOnlySequence<byte> byteSequence)
        {
            return extHeaderDecoders[byteSequence.First.Span[0]].Read(ref byteSequence);
        }

#if NETSTANDARD || NETFRAMEWORK
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
#endif
        public static int GetExtensionFormatHeaderLength(int dataLength)
        {
            switch (dataLength)
            {
                case 1:
                case 2:
                case 4:
                case 8:
                case 16:
                    return 2;
                default:
                    if (dataLength <= byte.MaxValue)
                    {
                        return 3;
                    }
                    else if (dataLength <= UInt16.MaxValue)
                    {
                        return 4;
                    }
                    else
                    {
                        return 6;
                    }
            }
        }

        // Timestamp spec
        // https://github.com/msgpack/msgpack/pull/209
        // FixExt4(-1) => seconds |  [1970-01-01 00:00:00 UTC, 2106-02-07 06:28:16 UTC) range
        // FixExt8(-1) => nanoseconds + seconds | [1970-01-01 00:00:00.000000000 UTC, 2514-05-30 01:53:04.000000000 UTC) range
        // Ext8(12,-1) => nanoseconds + seconds | [-584554047284-02-23 16:59:44 UTC, 584554051223-11-09 07:00:16.000000000 UTC) range

#if NETSTANDARD || NETFRAMEWORK
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
#endif
        public static int WriteDateTime(IBufferWriter<byte> writer, DateTime dateTime)
        {
            dateTime = dateTime.ToUniversalTime();

            var secondsSinceBclEpoch = dateTime.Ticks / TimeSpan.TicksPerSecond;
            var seconds = secondsSinceBclEpoch - DateTimeConstants.BclSecondsAtUnixEpoch;
            var nanoseconds = (dateTime.Ticks % TimeSpan.TicksPerSecond) * DateTimeConstants.NanosecondsPerTick;

            // reference pseudo code.
            /*
            struct timespec {
                long tv_sec;  // seconds
                long tv_nsec; // nanoseconds
            } time;
            if ((time.tv_sec >> 34) == 0)
            {
                uint64_t data64 = (time.tv_nsec << 34) | time.tv_sec;
                if (data & 0xffffffff00000000L == 0)
                {
                    // timestamp 32
                    uint32_t data32 = data64;
                    serialize(0xd6, -1, data32)
                }
                else
                {
                    // timestamp 64
                    serialize(0xd7, -1, data64)
                }
            }
            else
            {
                // timestamp 96
                serialize(0xc7, 12, -1, time.tv_nsec, time.tv_sec)
            }
            */

            if ((seconds >> 34) == 0)
            {
                var data64 = unchecked((ulong)((nanoseconds << 34) | seconds));
                if ((data64 & 0xffffffff00000000L) == 0)
                {
                    // timestamp 32(seconds in 32-bit unsigned int)
                    var data32 = (UInt32)data64;
                    var span = writer.GetSpan(6);
                    span[0] = MessagePackCode.FixExt4;
                    span[1] = unchecked((byte)ReservedMessagePackExtensionTypeCode.DateTime);
                    span[2] = unchecked((byte)(data32 >> 24));
                    span[3] = unchecked((byte)(data32 >> 16));
                    span[4] = unchecked((byte)(data32 >> 8));
                    span[5] = unchecked((byte)data32);
                    return 6;
                }
                else
                {
                    // timestamp 64(nanoseconds in 30-bit unsigned int | seconds in 34-bit unsigned int)
                    var span = writer.GetSpan(10);
                    span[0] = MessagePackCode.FixExt8;
                    span[1] = unchecked((byte)ReservedMessagePackExtensionTypeCode.DateTime);
                    span[2] = unchecked((byte)(data64 >> 56));
                    span[3] = unchecked((byte)(data64 >> 48));
                    span[4] = unchecked((byte)(data64 >> 40));
                    span[5] = unchecked((byte)(data64 >> 32));
                    span[6] = unchecked((byte)(data64 >> 24));
                    span[7] = unchecked((byte)(data64 >> 16));
                    span[8] = unchecked((byte)(data64 >> 8));
                    span[9] = unchecked((byte)data64);
                    return 10;
                }
            }
            else
            {
                // timestamp 96( nanoseconds in 32-bit unsigned int | seconds in 64-bit signed int )
                var span = writer.GetSpan(15);
                span[0] = MessagePackCode.Ext8;
                span[1] = 12;
                span[2] = unchecked((byte)ReservedMessagePackExtensionTypeCode.DateTime);
                span[3] = unchecked((byte)(nanoseconds >> 24));
                span[4] = unchecked((byte)(nanoseconds >> 16));
                span[5] = unchecked((byte)(nanoseconds >> 8));
                span[6] = unchecked((byte)nanoseconds);
                span[7] = unchecked((byte)(seconds >> 56));
                span[8] = unchecked((byte)(seconds >> 48));
                span[9] = unchecked((byte)(seconds >> 40));
                span[10] = unchecked((byte)(seconds >> 32));
                span[11] = unchecked((byte)(seconds >> 24));
                span[12] = unchecked((byte)(seconds >> 16));
                span[13] = unchecked((byte)(seconds >> 8));
                span[14] = unchecked((byte)seconds);
                return 15;
            }
        }

#if NETSTANDARD || NETFRAMEWORK
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
#endif
        public static DateTime ReadDateTime(ref ReadOnlySequence<byte> byteSequence)
        {
            return dateTimeDecoders[byteSequence.First.Span[0]].Read(ref byteSequence);
        }

        internal delegate T ContiguousMemoryReader<T>(ReadOnlySpan<byte> span);

        internal static T Read<T>(ref ReadOnlySequence<byte> byteSequence, int length, ContiguousMemoryReader<T> reader)
        {
            const int StackAllocLimit = 64 * 1024;
            T result;
            if (byteSequence.First.Length >= length)
            {
                result = reader(byteSequence.First.Span);
            }
            else if (length <= StackAllocLimit)
            {
                Span<byte> span = stackalloc byte[length];
                byteSequence.Slice(0, length).CopyTo(span);
                result = reader(span);
            }
            else
            {
                using (var rental = MemoryPool<byte>.Shared.Rent(length))
                {
                    byteSequence.Slice(0, length).CopyTo(rental.Memory.Span);
                    result = reader(rental.Memory.Span.Slice(0, length));
                }
            }

            byteSequence = byteSequence.Slice(length);
            return result;
        }
    }

    public struct ExtensionResult
    {
        public sbyte TypeCode { get; private set; }
        public byte[] Data { get; private set; }

        public ExtensionResult(sbyte typeCode, byte[] data)
        {
            TypeCode = typeCode;
            Data = data;
        }
    }

    public struct ExtensionHeader
    {
        public sbyte TypeCode { get; private set; }
        public uint Length { get; private set; }

        public ExtensionHeader(sbyte typeCode, uint length)
        {
            TypeCode = typeCode;
            Length = length;
        }
    }
}

namespace MessagePack.Internal
{
    internal static class DateTimeConstants
    {
        internal static readonly DateTime UnixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        internal const long BclSecondsAtUnixEpoch = 62135596800;
        internal const int NanosecondsPerTick = 100;
    }
}

namespace MessagePack.Decoders
{
    internal interface IMapHeaderDecoder
    {
        uint Read(ref ReadOnlySequence<byte> byteSequence);
    }

    internal sealed class FixMapHeader : IMapHeaderDecoder
    {
        internal static readonly IMapHeaderDecoder Instance = new FixMapHeader();

        private FixMapHeader()
        {

        }

        public uint Read(ref ReadOnlySequence<byte> byteSequence)
        {
            uint result = (uint)(byteSequence.First.Span[0] & 0xF);
            byteSequence = byteSequence.Slice(1);
            return result;
        }
    }

    internal sealed class Map16Header : IMapHeaderDecoder
    {
        internal static readonly IMapHeaderDecoder Instance = new Map16Header();

        private Map16Header()
        {

        }

        public uint Read(ref ReadOnlySequence<byte> byteSequence)
        {
            return MessagePackBinary.Read(ref byteSequence, 3, span => unchecked((uint)((span[1] << 8) | (span[2]))));
        }
    }

    internal sealed class Map32Header : IMapHeaderDecoder
    {
        internal static readonly IMapHeaderDecoder Instance = new Map32Header();

        private Map32Header()
        {

        }

        public uint Read(ref ReadOnlySequence<byte> byteSequence)
        {
            return MessagePackBinary.Read(ref byteSequence, 5, span => unchecked((uint)((span[1] << 24) | (span[2] << 16) | (span[3] << 8) | span[4])));
        }
    }

    internal sealed class InvalidMapHeader : IMapHeaderDecoder
    {
        internal static readonly IMapHeaderDecoder Instance = new InvalidMapHeader();

        private InvalidMapHeader()
        {

        }

        public uint Read(ref ReadOnlySequence<byte> byteSequence)
        {
            throw new InvalidOperationException(string.Format("code is invalid. code:{0} format:{1}", byteSequence.First.Span[0], MessagePackCode.ToFormatName(byteSequence.First.Span[0])));
        }
    }

    internal interface IArrayHeaderDecoder
    {
        uint Read(ref ReadOnlySequence<byte> byteSequence);
    }

    internal sealed class FixArrayHeader : IArrayHeaderDecoder
    {
        internal static readonly IArrayHeaderDecoder Instance = new FixArrayHeader();

        private FixArrayHeader()
        {

        }

        public uint Read(ref ReadOnlySequence<byte> byteSequence)
        {
            uint result = (uint)(byteSequence.First.Span[0] & 0xF);
            byteSequence = byteSequence.Slice(1);
            return result;
        }
    }

    internal sealed class Array16Header : IArrayHeaderDecoder
    {
        internal static readonly IArrayHeaderDecoder Instance = new Array16Header();

        private Array16Header()
        {

        }

        public uint Read(ref ReadOnlySequence<byte> byteSequence)
        {
            return MessagePackBinary.Read(ref byteSequence, 3, span => unchecked((uint)((span[1] << 8) | (span[2]))));
        }
    }

    internal sealed class Array32Header : IArrayHeaderDecoder
    {
        internal static readonly IArrayHeaderDecoder Instance = new Array32Header();

        private Array32Header()
        {

        }

        public uint Read(ref ReadOnlySequence<byte> byteSequence)
        {
            return MessagePackBinary.Read(ref byteSequence, 5, span => unchecked((uint)((span[1] << 24) | (span[2] << 16) | (span[3] << 8) | span[4])));
        }
    }

    internal sealed class InvalidArrayHeader : IArrayHeaderDecoder
    {
        internal static readonly IArrayHeaderDecoder Instance = new InvalidArrayHeader();

        private InvalidArrayHeader()
        {

        }

        public uint Read(ref ReadOnlySequence<byte> byteSequence)
        {
            throw new InvalidOperationException(string.Format("code is invalid. code:{0} format:{1}", byteSequence.First.Span[0], MessagePackCode.ToFormatName(byteSequence.First.Span[0])));
        }
    }

    internal interface IBooleanDecoder
    {
        bool Read();
    }

    internal sealed class True : IBooleanDecoder
    {
        internal static IBooleanDecoder Instance = new True();

        private True() { }

        public bool Read()
        {
            return true;
        }
    }

    internal sealed class False : IBooleanDecoder
    {
        internal static IBooleanDecoder Instance = new False();

        private False() { }

        public bool Read()
        {
            return false;
        }
    }

    internal sealed class InvalidBoolean : IBooleanDecoder
    {
        internal static IBooleanDecoder Instance = new InvalidBoolean();

        private InvalidBoolean() { }

        public bool Read()
        {
            throw new InvalidOperationException("code is invalid.");
        }
    }

    internal interface IByteDecoder
    {
        byte Read(ref ReadOnlySequence<byte> byteSequence);
    }

    internal sealed class FixByte : IByteDecoder
    {
        internal static readonly IByteDecoder Instance = new FixByte();

        private FixByte()
        {

        }

        public byte Read(ref ReadOnlySequence<byte> byteSequence)
        {
            byte result = byteSequence.First.Span[0];
            byteSequence = byteSequence.Slice(1);
            return result;
        }
    }

    internal sealed class UInt8Byte : IByteDecoder
    {
        internal static readonly IByteDecoder Instance = new UInt8Byte();

        private UInt8Byte()
        {

        }

        public byte Read(ref ReadOnlySequence<byte> byteSequence)
        {
            return MessagePackBinary.Read(ref byteSequence, 2, span => span[1]);
        }
    }

    internal sealed class InvalidByte : IByteDecoder
    {
        internal static readonly IByteDecoder Instance = new InvalidByte();

        private InvalidByte()
        {

        }

        public byte Read(ref ReadOnlySequence<byte> byteSequence)
        {
            throw new InvalidOperationException(string.Format("code is invalid. code:{0} format:{1}", byteSequence.First.Span[0], MessagePackCode.ToFormatName(byteSequence.First.Span[0])));
        }
    }

    internal interface IBytesDecoder
    {
        byte[] Read(ref ReadOnlySequence<byte> byteSequence);
    }

    internal sealed class NilBytes : IBytesDecoder
    {
        internal static readonly IBytesDecoder Instance = new NilBytes();

        private NilBytes()
        {

        }

        public byte[] Read(ref ReadOnlySequence<byte> byteSequence)
        {
            byteSequence = byteSequence.Slice(1);
            return null;
        }
    }

    internal sealed class Bin8Bytes : IBytesDecoder
    {
        internal static readonly IBytesDecoder Instance = new Bin8Bytes();

        private Bin8Bytes()
        {

        }

        public byte[] Read(ref ReadOnlySequence<byte> byteSequence)
        {
            var length = MessagePackBinary.Read(ref byteSequence, 2, span => span[1]);
            var newBytes = new byte[length];
            byteSequence.Slice(0, length).CopyTo(newBytes);
            byteSequence = byteSequence.Slice(length);
            return newBytes;
        }
    }

    internal sealed class Bin16Bytes : IBytesDecoder
    {
        internal static readonly IBytesDecoder Instance = new Bin16Bytes();

        private Bin16Bytes()
        {

        }

        public byte[] Read(ref ReadOnlySequence<byte> byteSequence)
        {
            var length = MessagePackBinary.Read(ref byteSequence, 3, span => (span[1] << 8) + (span[2]));
            var newBytes = new byte[length];
            byteSequence.Slice(0, length).CopyTo(newBytes);
            byteSequence = byteSequence.Slice(length);
            return newBytes;
        }
    }

    internal sealed class Bin32Bytes : IBytesDecoder
    {
        internal static readonly IBytesDecoder Instance = new Bin32Bytes();

        private Bin32Bytes()
        {

        }

        public byte[] Read(ref ReadOnlySequence<byte> byteSequence)
        {
            var length = MessagePackBinary.Read(ref byteSequence, 5, span => (span[1] << 24) | (span[2] << 16) | (span[3] << 8) | (span[4]));
            var newBytes = new byte[length];
            byteSequence.Slice(0, length).CopyTo(newBytes);
            byteSequence = byteSequence.Slice(length);
            return newBytes;
        }
    }

    internal sealed class InvalidBytes : IBytesDecoder
    {
        internal static readonly IBytesDecoder Instance = new InvalidBytes();

        private InvalidBytes()
        {

        }

        public byte[] Read(ref ReadOnlySequence<byte> byteSequence)
        {
            throw new InvalidOperationException(string.Format("code is invalid. code:{0} format:{1}", byteSequence.First.Span[0], MessagePackCode.ToFormatName(byteSequence.First.Span[0])));
        }
    }

    internal interface IBytesSegmentDecoder
    {
        ArraySegment<byte> Read(ref ReadOnlySequence<byte> byteSequence);
    }

    internal sealed class NilBytesSegment : IBytesSegmentDecoder
    {
        internal static readonly IBytesSegmentDecoder Instance = new NilBytesSegment();

        private NilBytesSegment()
        {

        }

        public ArraySegment<byte> Read(ref ReadOnlySequence<byte> byteSequence)
        {
            byteSequence = byteSequence.Slice(1);
            return default(ArraySegment<byte>);
        }
    }

    internal sealed class Bin8BytesSegment : IBytesSegmentDecoder
    {
        internal static readonly IBytesSegmentDecoder Instance = new Bin8BytesSegment();

        private Bin8BytesSegment()
        {

        }

        public ArraySegment<byte> Read(ref ReadOnlySequence<byte> byteSequence)
        {
            return Read(ref byteSequence, 2, span => span[1]);
        }

        internal delegate int SegmentLengthReader(ReadOnlySpan<byte> span);

        internal static ArraySegment<byte> Read(ref ReadOnlySequence<byte> byteSequence, int lengthOfLengthHeader, SegmentLengthReader readLength)
        {
            Span<byte> lengthHeaderSpan = stackalloc byte[lengthOfLengthHeader];
            byteSequence.Slice(0, lengthOfLengthHeader).CopyTo(lengthHeaderSpan);
            byteSequence = byteSequence.Slice(lengthOfLengthHeader);

            int length = readLength(lengthHeaderSpan);
            ArraySegment<byte> result;
            if (byteSequence.First.Length >= length && MemoryMarshal.TryGetArray(byteSequence.First, out ArraySegment<byte> segment))
            {
                // Everything we need to return happens to already be in a single array. Return a segment into that same array.
                result = new ArraySegment<byte>(segment.Array, segment.Offset, length);
            }
            else
            {
                // We need to create a new array in order to return a continguous segment in our result.
                result = new ArraySegment<byte>(new byte[length]);
                byteSequence.Slice(0, length).CopyTo(result.Array);
            }

            byteSequence = byteSequence.Slice(length);
            return result;
        }
    }

    internal sealed class Bin16BytesSegment : IBytesSegmentDecoder
    {
        internal static readonly IBytesSegmentDecoder Instance = new Bin16BytesSegment();

        private Bin16BytesSegment()
        {

        }

        public ArraySegment<byte> Read(ref ReadOnlySequence<byte> byteSequence)
        {
            return Bin8BytesSegment.Read(ref byteSequence, 3, span => (span[1] << 8) + (span[2]));
        }

    }

    internal sealed class Bin32BytesSegment : IBytesSegmentDecoder
    {
        internal static readonly IBytesSegmentDecoder Instance = new Bin32BytesSegment();

        private Bin32BytesSegment()
        {

        }

        public ArraySegment<byte> Read(ref ReadOnlySequence<byte> byteSequence)
        {
            return Bin8BytesSegment.Read(ref byteSequence, 5, span => (span[1] << 24) | (span[2] << 16) | (span[3] << 8) | (span[4]));
        }
    }

    internal sealed class InvalidBytesSegment : IBytesSegmentDecoder
    {
        internal static readonly IBytesSegmentDecoder Instance = new InvalidBytesSegment();

        private InvalidBytesSegment()
        {

        }

        public ArraySegment<byte> Read(ref ReadOnlySequence<byte> byteSequence)
        {
            throw new InvalidOperationException(string.Format("code is invalid. code:{0} format:{1}", byteSequence.First.Span[0], MessagePackCode.ToFormatName(byteSequence.First.Span[0])));
        }
    }

    internal interface ISByteDecoder
    {
        sbyte Read(ref ReadOnlySequence<byte> byteSequence);
    }

    internal sealed class FixSByte : ISByteDecoder
    {
        internal static readonly ISByteDecoder Instance = new FixSByte();

        private FixSByte()
        {

        }

        public sbyte Read(ref ReadOnlySequence<byte> byteSequence)
        {
            sbyte result = unchecked((sbyte)byteSequence.First.Span[0]);
            byteSequence = byteSequence.Slice(1);
            return result;
        }
    }

    internal sealed class Int8SByte : ISByteDecoder
    {
        internal static readonly ISByteDecoder Instance = new Int8SByte();

        private Int8SByte()
        {

        }

        public sbyte Read(ref ReadOnlySequence<byte> byteSequence)
        {
            return MessagePackBinary.Read(ref byteSequence, 2, span => (sbyte)span[1]);
        }
    }

    internal sealed class InvalidSByte : ISByteDecoder
    {
        internal static readonly ISByteDecoder Instance = new InvalidSByte();

        private InvalidSByte()
        {

        }

        public sbyte Read(ref ReadOnlySequence<byte> byteSequence)
        {
            throw new InvalidOperationException(string.Format("code is invalid. code:{0} format:{1}", byteSequence.First.Span[0], MessagePackCode.ToFormatName(byteSequence.First.Span[0])));
        }
    }

    internal interface ISingleDecoder
    {
        float Read(ref ReadOnlySequence<byte> byteSequence);
    }

    internal sealed class FixNegativeFloat : ISingleDecoder
    {
        internal static readonly ISingleDecoder Instance = new FixNegativeFloat();

        private FixNegativeFloat()
        {

        }

        public Single Read(ref ReadOnlySequence<byte> byteSequence)
        {
            return FixSByte.Instance.Read(ref byteSequence);
        }
    }

    internal sealed class FixFloat : ISingleDecoder
    {
        internal static readonly ISingleDecoder Instance = new FixFloat();

        private FixFloat()
        {

        }

        public Single Read(ref ReadOnlySequence<byte> byteSequence)
        {
            return FixByte.Instance.Read(ref byteSequence);
        }
    }

    internal sealed class Int8Single : ISingleDecoder
    {
        internal static readonly ISingleDecoder Instance = new Int8Single();

        private Int8Single()
        {

        }

        public Single Read(ref ReadOnlySequence<byte> byteSequence)
        {
            return Int8SByte.Instance.Read(ref byteSequence);
        }
    }

    internal sealed class Int16Single : ISingleDecoder
    {
        internal static readonly ISingleDecoder Instance = new Int16Single();

        private Int16Single()
        {

        }

        public Single Read(ref ReadOnlySequence<byte> byteSequence)
        {
            return Int16Int16.Instance.Read(ref byteSequence);
        }
    }

    internal sealed class Int32Single : ISingleDecoder
    {
        internal static readonly ISingleDecoder Instance = new Int32Single();

        private Int32Single()
        {

        }

        public Single Read(ref ReadOnlySequence<byte> byteSequence)
        {
            return Int32Int32.Instance.Read(ref byteSequence);
        }
    }

    internal sealed class Int64Single : ISingleDecoder
    {
        internal static readonly ISingleDecoder Instance = new Int64Single();

        private Int64Single()
        {

        }

        public Single Read(ref ReadOnlySequence<byte> byteSequence)
        {
            return Int64Int64.Instance.Read(ref byteSequence);
        }
    }


    internal sealed class UInt8Single : ISingleDecoder
    {
        internal static readonly ISingleDecoder Instance = new UInt8Single();

        private UInt8Single()
        {

        }

        public Single Read(ref ReadOnlySequence<byte> byteSequence)
        {
            return UInt8Byte.Instance.Read(ref byteSequence);
        }
    }

    internal sealed class UInt16Single : ISingleDecoder
    {
        internal static readonly ISingleDecoder Instance = new UInt16Single();

        private UInt16Single()
        {

        }

        public Single Read(ref ReadOnlySequence<byte> byteSequence)
        {
            return UInt16UInt16.Instance.Read(ref byteSequence);
        }
    }

    internal sealed class UInt32Single : ISingleDecoder
    {
        internal static readonly ISingleDecoder Instance = new UInt32Single();

        private UInt32Single()
        {

        }

        public Single Read(ref ReadOnlySequence<byte> byteSequence)
        {
            return UInt32UInt32.Instance.Read(ref byteSequence);
        }
    }

    internal sealed class UInt64Single : ISingleDecoder
    {
        internal static readonly ISingleDecoder Instance = new UInt64Single();

        private UInt64Single()
        {

        }

        public Single Read(ref ReadOnlySequence<byte> byteSequence)
        {
            return UInt64UInt64.Instance.Read(ref byteSequence);
        }
    }

    internal sealed class Float32Single : ISingleDecoder
    {
        internal static readonly ISingleDecoder Instance = new Float32Single();

        private Float32Single()
        {

        }

        public Single Read(ref ReadOnlySequence<byte> byteSequence)
        {
            return MessagePackBinary.Read(ref byteSequence, 5, span => new Float32Bits(span.Slice(1)).Value);
        }
    }

    internal sealed class InvalidSingle : ISingleDecoder
    {
        internal static readonly ISingleDecoder Instance = new InvalidSingle();

        private InvalidSingle()
        {

        }

        public Single Read(ref ReadOnlySequence<byte> byteSequence)
        {
            throw new InvalidOperationException(string.Format("code is invalid. code:{0} format:{1}", byteSequence.First.Span[0], MessagePackCode.ToFormatName(byteSequence.First.Span[0])));
        }
    }

    internal interface IDoubleDecoder
    {
        double Read(ref ReadOnlySequence<byte> byteSequence);
    }

    internal sealed class FixNegativeDouble : IDoubleDecoder
    {
        internal static readonly IDoubleDecoder Instance = new FixNegativeDouble();

        private FixNegativeDouble()
        {

        }

        public Double Read(ref ReadOnlySequence<byte> byteSequence)
        {
            return FixSByte.Instance.Read(ref byteSequence);
        }
    }

    internal sealed class FixDouble : IDoubleDecoder
    {
        internal static readonly IDoubleDecoder Instance = new FixDouble();

        private FixDouble()
        {

        }

        public Double Read(ref ReadOnlySequence<byte> byteSequence)
        {
            return FixByte.Instance.Read(ref byteSequence);
        }
    }

    internal sealed class Int8Double : IDoubleDecoder
    {
        internal static readonly IDoubleDecoder Instance = new Int8Double();

        private Int8Double()
        {

        }

        public Double Read(ref ReadOnlySequence<byte> byteSequence)
        {
            return Int8SByte.Instance.Read(ref byteSequence);
        }
    }

    internal sealed class Int16Double : IDoubleDecoder
    {
        internal static readonly IDoubleDecoder Instance = new Int16Double();

        private Int16Double()
        {

        }

        public Double Read(ref ReadOnlySequence<byte> byteSequence)
        {
            return Int16Int16.Instance.Read(ref byteSequence);
        }
    }

    internal sealed class Int32Double : IDoubleDecoder
    {
        internal static readonly IDoubleDecoder Instance = new Int32Double();

        private Int32Double()
        {

        }

        public Double Read(ref ReadOnlySequence<byte> byteSequence)
        {
            return Int32Int32.Instance.Read(ref byteSequence);
        }
    }

    internal sealed class Int64Double : IDoubleDecoder
    {
        internal static readonly IDoubleDecoder Instance = new Int64Double();

        private Int64Double()
        {

        }

        public Double Read(ref ReadOnlySequence<byte> byteSequence)
        {
            return Int64Int64.Instance.Read(ref byteSequence);
        }
    }


    internal sealed class UInt8Double : IDoubleDecoder
    {
        internal static readonly IDoubleDecoder Instance = new UInt8Double();

        private UInt8Double()
        {

        }

        public Double Read(ref ReadOnlySequence<byte> byteSequence)
        {
            return UInt8Byte.Instance.Read(ref byteSequence);
        }
    }

    internal sealed class UInt16Double : IDoubleDecoder
    {
        internal static readonly IDoubleDecoder Instance = new UInt16Double();

        private UInt16Double()
        {

        }

        public Double Read(ref ReadOnlySequence<byte> byteSequence)
        {
            return UInt16UInt16.Instance.Read(ref byteSequence);
        }
    }

    internal sealed class UInt32Double : IDoubleDecoder
    {
        internal static readonly IDoubleDecoder Instance = new UInt32Double();

        private UInt32Double()
        {

        }

        public Double Read(ref ReadOnlySequence<byte> byteSequence)
        {
            return UInt32UInt32.Instance.Read(ref byteSequence);
        }
    }

    internal sealed class UInt64Double : IDoubleDecoder
    {
        internal static readonly IDoubleDecoder Instance = new UInt64Double();

        private UInt64Double()
        {

        }

        public Double Read(ref ReadOnlySequence<byte> byteSequence)
        {
            return UInt64UInt64.Instance.Read(ref byteSequence);
        }
    }

    internal sealed class Float32Double : IDoubleDecoder
    {
        internal static readonly IDoubleDecoder Instance = new Float32Double();

        private Float32Double()
        {

        }

        public Double Read(ref ReadOnlySequence<byte> byteSequence)
        {
            return MessagePackBinary.Read(ref byteSequence, 5, span => new Float32Bits(span.Slice(1)).Value);
        }
    }

    internal sealed class Float64Double : IDoubleDecoder
    {
        internal static readonly IDoubleDecoder Instance = new Float64Double();

        private Float64Double()
        {

        }

        public Double Read(ref ReadOnlySequence<byte> byteSequence)
        {
            return MessagePackBinary.Read(ref byteSequence, 9, span => new Float64Bits(span.Slice(1)).Value);
        }
    }

    internal sealed class InvalidDouble : IDoubleDecoder
    {
        internal static readonly IDoubleDecoder Instance = new InvalidDouble();

        private InvalidDouble()
        {

        }

        public Double Read(ref ReadOnlySequence<byte> byteSequence)
        {
            throw new InvalidOperationException(string.Format("code is invalid. code:{0} format:{1}", byteSequence.First.Span[0], MessagePackCode.ToFormatName(byteSequence.First.Span[0])));
        }
    }

    internal interface IInt16Decoder
    {
        Int16 Read(ref ReadOnlySequence<byte> byteSequence);
    }

    internal sealed class FixNegativeInt16 : IInt16Decoder
    {
        internal static readonly IInt16Decoder Instance = new FixNegativeInt16();

        private FixNegativeInt16()
        {

        }

        public Int16 Read(ref ReadOnlySequence<byte> byteSequence)
        {
            Int16 result = unchecked((sbyte)byteSequence.First.Span[0]);
            byteSequence = byteSequence.Slice(1);
            return result;
        }
    }

    internal sealed class FixInt16 : IInt16Decoder
    {
        internal static readonly IInt16Decoder Instance = new FixInt16();

        private FixInt16()
        {

        }

        public Int16 Read(ref ReadOnlySequence<byte> byteSequence)
        {
            var result = unchecked(byteSequence.First.Span[0]);
            byteSequence = byteSequence.Slice(1);
            return result;
        }
    }

    internal sealed class UInt8Int16 : IInt16Decoder
    {
        internal static readonly IInt16Decoder Instance = new UInt8Int16();

        private UInt8Int16()
        {

        }

        public Int16 Read(ref ReadOnlySequence<byte> byteSequence)
        {
            return MessagePackBinary.Read(ref byteSequence, 2, span => unchecked(span[1]));
        }
    }

    internal sealed class UInt16Int16 : IInt16Decoder
    {
        internal static readonly IInt16Decoder Instance = new UInt16Int16();

        private UInt16Int16()
        {

        }

        public Int16 Read(ref ReadOnlySequence<byte> byteSequence)
        {
            return MessagePackBinary.Read(ref byteSequence, 5, span => checked((Int16)((span[1] << 8) + (span[2]))));
        }
    }

    internal sealed class Int8Int16 : IInt16Decoder
    {
        internal static readonly IInt16Decoder Instance = new Int8Int16();

        private Int8Int16()
        {

        }

        public Int16 Read(ref ReadOnlySequence<byte> byteSequence)
        {
            return MessagePackBinary.Read(ref byteSequence, 2, span => unchecked((sbyte)(span[1])));
        }
    }

    internal sealed class Int16Int16 : IInt16Decoder
    {
        internal static readonly IInt16Decoder Instance = new Int16Int16();

        private Int16Int16()
        {

        }

        public Int16 Read(ref ReadOnlySequence<byte> byteSequence)
        {
            return MessagePackBinary.Read(ref byteSequence, 3, span => unchecked((short)((span[1] << 8) | (span[2]))));
        }
    }

    internal sealed class InvalidInt16 : IInt16Decoder
    {
        internal static readonly IInt16Decoder Instance = new InvalidInt16();

        private InvalidInt16()
        {

        }

        public Int16 Read(ref ReadOnlySequence<byte> byteSequence)
        {
            throw new InvalidOperationException(string.Format("code is invalid. code:{0} format:{1}", byteSequence.First.Span[0], MessagePackCode.ToFormatName(byteSequence.First.Span[0])));
        }
    }

    internal interface IInt32Decoder
    {
        Int32 Read(ref ReadOnlySequence<byte> byteSequence);
    }

    internal sealed class FixNegativeInt32 : IInt32Decoder
    {
        internal static readonly IInt32Decoder Instance = new FixNegativeInt32();

        private FixNegativeInt32()
        {

        }

        public Int32 Read(ref ReadOnlySequence<byte> byteSequence)
        {
            var result = unchecked((sbyte)byteSequence.First.Span[0]);
            byteSequence = byteSequence.Slice(1);
            return result;
        }
    }

    internal sealed class FixInt32 : IInt32Decoder
    {
        internal static readonly IInt32Decoder Instance = new FixInt32();

        private FixInt32()
        {

        }

        public Int32 Read(ref ReadOnlySequence<byte> byteSequence)
        {
            var result = unchecked(byteSequence.First.Span[0]);
            byteSequence = byteSequence.Slice(1);
            return result;
        }
    }

    internal sealed class UInt8Int32 : IInt32Decoder
    {
        internal static readonly IInt32Decoder Instance = new UInt8Int32();

        private UInt8Int32()
        {

        }

        public Int32 Read(ref ReadOnlySequence<byte> byteSequence)
        {
            return MessagePackBinary.Read(ref byteSequence, 2, span => unchecked(span[1]));
        }
    }
    internal sealed class UInt16Int32 : IInt32Decoder
    {
        internal static readonly IInt32Decoder Instance = new UInt16Int32();

        private UInt16Int32()
        {

        }

        public Int32 Read(ref ReadOnlySequence<byte> byteSequence)
        {
            return MessagePackBinary.Read(ref byteSequence, 3, span => (span[1] << 8) | (span[2]));
        }
    }

    internal sealed class UInt32Int32 : IInt32Decoder
    {
        internal static readonly IInt32Decoder Instance = new UInt32Int32();

        private UInt32Int32()
        {

        }

        public Int32 Read(ref ReadOnlySequence<byte> byteSequence)
        {
            return MessagePackBinary.Read(ref byteSequence, 5, span => checked((Int32)((UInt32)(span[1] << 24) | (UInt32)(span[2] << 16) | (UInt32)(span[3] << 8) | span[4])));
        }
    }

    internal sealed class Int8Int32 : IInt32Decoder
    {
        internal static readonly IInt32Decoder Instance = new Int8Int32();

        private Int8Int32()
        {

        }

        public Int32 Read(ref ReadOnlySequence<byte> byteSequence)
        {
            return MessagePackBinary.Read(ref byteSequence, 2, span => unchecked((sbyte)(span[1])));
        }
    }

    internal sealed class Int16Int32 : IInt32Decoder
    {
        internal static readonly IInt32Decoder Instance = new Int16Int32();

        private Int16Int32()
        {

        }

        public Int32 Read(ref ReadOnlySequence<byte> byteSequence)
        {
            return MessagePackBinary.Read(ref byteSequence, 3, span => unchecked((short)((span[1] << 8) | (span[2]))));
        }
    }

    internal sealed class Int32Int32 : IInt32Decoder
    {
        internal static readonly IInt32Decoder Instance = new Int32Int32();

        private Int32Int32()
        {

        }

        public Int32 Read(ref ReadOnlySequence<byte> byteSequence)
        {
            return MessagePackBinary.Read(ref byteSequence, 5, span => unchecked((span[1] << 24) | (span[2] << 16) | (span[3] << 8) | span[4]));
        }
    }

    internal sealed class InvalidInt32 : IInt32Decoder
    {
        internal static readonly IInt32Decoder Instance = new InvalidInt32();

        private InvalidInt32()
        {
        }

        public Int32 Read(ref ReadOnlySequence<byte> byteSequence)
        {
            throw new InvalidOperationException(string.Format("code is invalid. code:{0} format:{1}", byteSequence.First.Span[0], MessagePackCode.ToFormatName(byteSequence.First.Span[0])));
        }
    }

    internal interface IInt64Decoder
    {
        Int64 Read(ref ReadOnlySequence<byte> byteSequence);
    }

    internal sealed class FixNegativeInt64 : IInt64Decoder
    {
        internal static readonly IInt64Decoder Instance = new FixNegativeInt64();

        private FixNegativeInt64()
        {

        }

        public Int64 Read(ref ReadOnlySequence<byte> byteSequence)
        {
            var result = unchecked((sbyte)byteSequence.First.Span[0]);
            byteSequence = byteSequence.Slice(1);
            return result;
        }
    }

    internal sealed class FixInt64 : IInt64Decoder
    {
        internal static readonly IInt64Decoder Instance = new FixInt64();

        private FixInt64()
        {

        }

        public Int64 Read(ref ReadOnlySequence<byte> byteSequence)
        {
            var result = unchecked(byteSequence.First.Span[0]);
            byteSequence = byteSequence.Slice(1);
            return result;
        }
    }

    internal sealed class UInt8Int64 : IInt64Decoder
    {
        internal static readonly IInt64Decoder Instance = new UInt8Int64();

        private UInt8Int64()
        {

        }

        public Int64 Read(ref ReadOnlySequence<byte> byteSequence)
        {
            readSize = 2;
            return unchecked(span[1]);
        }
    }
    internal sealed class UInt16Int64 : IInt64Decoder
    {
        internal static readonly IInt64Decoder Instance = new UInt16Int64();

        private UInt16Int64()
        {

        }

        public Int64 Read(ref ReadOnlySequence<byte> byteSequence)
        {
            readSize = 3;
            return (span[1] << 8) | (span[2]);
        }
    }

    internal sealed class UInt32Int64 : IInt64Decoder
    {
        internal static readonly IInt64Decoder Instance = new UInt32Int64();

        private UInt32Int64()
        {

        }

        public Int64 Read(ref ReadOnlySequence<byte> byteSequence)
        {
            readSize = 5;
            return unchecked((uint)(span[1] << 24) | ((uint)span[2] << 16) | ((uint)span[3] << 8) | span[4]);
        }
    }

    internal sealed class UInt64Int64 : IInt64Decoder
    {
        internal static readonly IInt64Decoder Instance = new UInt64Int64();

        private UInt64Int64()
        {

        }

        public Int64 Read(ref ReadOnlySequence<byte> byteSequence)
        {
            readSize = 9;
            checked
            {
                return (Int64)span[1] << 56 | (Int64)span[2] << 48 | (Int64)span[3] << 40 | (Int64)span[4] << 32
                     | (Int64)span[5] << 24 | (Int64)span[6] << 16 | (Int64)span[7] << 8 | span[8];
            }
        }
    }


    internal sealed class Int8Int64 : IInt64Decoder
    {
        internal static readonly IInt64Decoder Instance = new Int8Int64();

        private Int8Int64()
        {

        }

        public Int64 Read(ref ReadOnlySequence<byte> byteSequence)
        {
            readSize = 2;
            return unchecked((sbyte)(span[1]));
        }
    }

    internal sealed class Int16Int64 : IInt64Decoder
    {
        internal static readonly IInt64Decoder Instance = new Int16Int64();

        private Int16Int64()
        {

        }

        public Int64 Read(ref ReadOnlySequence<byte> byteSequence)
        {
            readSize = 3;
            unchecked
            {
                return (short)((span[1] << 8) | (span[2]));
            }
        }
    }

    internal sealed class Int32Int64 : IInt64Decoder
    {
        internal static readonly IInt64Decoder Instance = new Int32Int64();

        private Int32Int64()
        {

        }

        public Int64 Read(ref ReadOnlySequence<byte> byteSequence)
        {
            readSize = 5;
            unchecked
            {
                return (span[1] << 24) + (long)(span[2] << 16) + (span[3] << 8) + span[4];
            }
        }
    }

    internal sealed class Int64Int64 : IInt64Decoder
    {
        internal static readonly IInt64Decoder Instance = new Int64Int64();

        private Int64Int64()
        {

        }

        public Int64 Read(ref ReadOnlySequence<byte> byteSequence)
        {
            readSize = 9;
            unchecked
            {
                return (long)span[1] << 56 | (long)span[2] << 48 | (long)span[3] << 40 | (long)span[4] << 32
                     | (long)span[5] << 24 | (long)span[6] << 16 | (long)span[7] << 8 | span[8];
            }
        }
    }

    internal sealed class InvalidInt64 : IInt64Decoder
    {
        internal static readonly IInt64Decoder Instance = new InvalidInt64();

        private InvalidInt64()
        {

        }

        public Int64 Read(ref ReadOnlySequence<byte> byteSequence)
        {
            throw new InvalidOperationException(string.Format("code is invalid. code:{0} format:{1}", byteSequence.First.Span[0], MessagePackCode.ToFormatName(byteSequence.First.Span[0])));
        }
    }

    internal interface IUInt16Decoder
    {
        UInt16 Read(ref ReadOnlySequence<byte> byteSequence);
    }

    internal sealed class FixUInt16 : IUInt16Decoder
    {
        internal static readonly IUInt16Decoder Instance = new FixUInt16();

        private FixUInt16()
        {

        }

        public UInt16 Read(ref ReadOnlySequence<byte> byteSequence)
        {
            var result = unchecked(byteSequence.First.Span[0]);
            byteSequence = byteSequence.Slice(1);
            return result;
        }
    }

    internal sealed class UInt8UInt16 : IUInt16Decoder
    {
        internal static readonly IUInt16Decoder Instance = new UInt8UInt16();

        private UInt8UInt16()
        {

        }

        public UInt16 Read(ref ReadOnlySequence<byte> byteSequence)
        {
            readSize = 2;
            return unchecked(span[1]);
        }
    }

    internal sealed class UInt16UInt16 : IUInt16Decoder
    {
        internal static readonly IUInt16Decoder Instance = new UInt16UInt16();

        private UInt16UInt16()
        {

        }

        public UInt16 Read(ref ReadOnlySequence<byte> byteSequence)
        {
            readSize = 3;
            unchecked
            {
                return (UInt16)((span[1] << 8) | (span[2]));
            }
        }
    }

    internal sealed class InvalidUInt16 : IUInt16Decoder
    {
        internal static readonly IUInt16Decoder Instance = new InvalidUInt16();

        private InvalidUInt16()
        {

        }

        public UInt16 Read(ref ReadOnlySequence<byte> byteSequence)
        {
            throw new InvalidOperationException(string.Format("code is invalid. code:{0} format:{1}", byteSequence.First.Span[0], MessagePackCode.ToFormatName(byteSequence.First.Span[0])));
        }
    }

    internal interface IUInt32Decoder
    {
        UInt32 Read(ref ReadOnlySequence<byte> byteSequence);
    }

    internal sealed class FixUInt32 : IUInt32Decoder
    {
        internal static readonly IUInt32Decoder Instance = new FixUInt32();

        private FixUInt32()
        {

        }

        public UInt32 Read(ref ReadOnlySequence<byte> byteSequence)
        {
            var result = unchecked(byteSequence.First.Span[0]);
            byteSequence = byteSequence.Slice(1);
            return result;
        }
    }

    internal sealed class UInt8UInt32 : IUInt32Decoder
    {
        internal static readonly IUInt32Decoder Instance = new UInt8UInt32();

        private UInt8UInt32()
        {

        }

        public UInt32 Read(ref ReadOnlySequence<byte> byteSequence)
        {
            readSize = 2;
            return unchecked(span[1]);
        }
    }

    internal sealed class UInt16UInt32 : IUInt32Decoder
    {
        internal static readonly IUInt32Decoder Instance = new UInt16UInt32();

        private UInt16UInt32()
        {

        }

        public UInt32 Read(ref ReadOnlySequence<byte> byteSequence)
        {
            readSize = 3;
            unchecked
            {
                return (UInt32)((span[1] << 8) | (span[2]));
            }
        }
    }

    internal sealed class UInt32UInt32 : IUInt32Decoder
    {
        internal static readonly IUInt32Decoder Instance = new UInt32UInt32();

        private UInt32UInt32()
        {

        }

        public UInt32 Read(ref ReadOnlySequence<byte> byteSequence)
        {
            readSize = 5;
            unchecked
            {
                return (UInt32)(span[1] << 24) | (UInt32)(span[2] << 16) | (UInt32)(span[3] << 8) | span[4];
            }
        }
    }

    internal sealed class InvalidUInt32 : IUInt32Decoder
    {
        internal static readonly IUInt32Decoder Instance = new InvalidUInt32();

        private InvalidUInt32()
        {

        }

        public UInt32 Read(ref ReadOnlySequence<byte> byteSequence)
        {
            throw new InvalidOperationException(string.Format("code is invalid. code:{0} format:{1}", byteSequence.First.Span[0], MessagePackCode.ToFormatName(byteSequence.First.Span[0])));
        }
    }

    internal interface IUInt64Decoder
    {
        UInt64 Read(ref ReadOnlySequence<byte> byteSequence);
    }

    internal sealed class FixUInt64 : IUInt64Decoder
    {
        internal static readonly IUInt64Decoder Instance = new FixUInt64();

        private FixUInt64()
        {

        }

        public UInt64 Read(ref ReadOnlySequence<byte> byteSequence)
        {
            var result = unchecked(byteSequence.First.Span[0]);
            byteSequence = byteSequence.Slice(1);
            return result;
        }
    }

    internal sealed class UInt8UInt64 : IUInt64Decoder
    {
        internal static readonly IUInt64Decoder Instance = new UInt8UInt64();

        private UInt8UInt64()
        {

        }

        public UInt64 Read(ref ReadOnlySequence<byte> byteSequence)
        {
            readSize = 2;
            return unchecked(span[1]);
        }
    }

    internal sealed class UInt16UInt64 : IUInt64Decoder
    {
        internal static readonly IUInt64Decoder Instance = new UInt16UInt64();

        private UInt16UInt64()
        {

        }

        public UInt64 Read(ref ReadOnlySequence<byte> byteSequence)
        {
            readSize = 3;
            unchecked
            {
                return (UInt64)((span[1] << 8) | (span[2]));
            }
        }
    }

    internal sealed class UInt32UInt64 : IUInt64Decoder
    {
        internal static readonly IUInt64Decoder Instance = new UInt32UInt64();

        private UInt32UInt64()
        {

        }

        public UInt64 Read(ref ReadOnlySequence<byte> byteSequence)
        {
            readSize = 5;
            unchecked
            {
                return ((UInt64)span[1] << 24) + (ulong)(span[2] << 16) + (UInt64)(span[3] << 8) + span[4];
            }
        }
    }

    internal sealed class UInt64UInt64 : IUInt64Decoder
    {
        internal static readonly IUInt64Decoder Instance = new UInt64UInt64();

        private UInt64UInt64()
        {

        }

        public UInt64 Read(ref ReadOnlySequence<byte> byteSequence)
        {
            readSize = 9;
            unchecked
            {
                return (UInt64)span[1] << 56 | (UInt64)span[2] << 48 | (UInt64)span[3] << 40 | (UInt64)span[4] << 32
                     | (UInt64)span[5] << 24 | (UInt64)span[6] << 16 | (UInt64)span[7] << 8 | span[8];
            }
        }
    }

    internal sealed class InvalidUInt64 : IUInt64Decoder
    {
        internal static readonly IUInt64Decoder Instance = new InvalidUInt64();

        private InvalidUInt64()
        {

        }

        public UInt64 Read(ref ReadOnlySequence<byte> byteSequence)
        {
            throw new InvalidOperationException(string.Format("code is invalid. code:{0} format:{1}", byteSequence.First.Span[0], MessagePackCode.ToFormatName(byteSequence.First.Span[0])));
        }
    }

    internal interface IStringDecoder
    {
        String Read(ref ReadOnlySequence<byte> byteSequence);
    }

    internal sealed class NilString : IStringDecoder
    {
        internal static readonly IStringDecoder Instance = new NilString();

        private NilString()
        {

        }

        public String Read(ref ReadOnlySequence<byte> byteSequence)
        {
            readSize = 1;
            return null;
        }
    }

    internal sealed class FixString : IStringDecoder
    {
        internal static readonly IStringDecoder Instance = new FixString();

        private FixString()
        {

        }

        public String Read(ref ReadOnlySequence<byte> byteSequence)
        {
            var length = span[0] & 0x1F;
            readSize = length + 1;
            return StringEncoding.UTF8.GetString(bytes, offset + 1, length);
        }
    }

    internal sealed class Str8String : IStringDecoder
    {
        internal static readonly IStringDecoder Instance = new Str8String();

        private Str8String()
        {

        }

        public String Read(ref ReadOnlySequence<byte> byteSequence)
        {
            var length = (int)span[1];
            readSize = length + 2;
            return StringEncoding.UTF8.GetString(bytes, offset + 2, length);
        }
    }

    internal sealed class Str16String : IStringDecoder
    {
        internal static readonly IStringDecoder Instance = new Str16String();

        private Str16String()
        {

        }

        public String Read(ref ReadOnlySequence<byte> byteSequence)
        {
            unchecked
            {
                var length = (span[1] << 8) + (span[2]);
                readSize = length + 3;
                return StringEncoding.UTF8.GetString(bytes, offset + 3, length);
            }
        }
    }

    internal sealed class Str32String : IStringDecoder
    {
        internal static readonly IStringDecoder Instance = new Str32String();

        private Str32String()
        {

        }

        public String Read(ref ReadOnlySequence<byte> byteSequence)
        {
            unchecked
            {
                var length = (int)((uint)(span[1] << 24) | (uint)(span[2] << 16) | (uint)(span[3] << 8) | span[4]);
                readSize = length + 5;
                return StringEncoding.UTF8.GetString(bytes, offset + 5, length);
            }
        }
    }

    internal sealed class InvalidString : IStringDecoder
    {
        internal static readonly IStringDecoder Instance = new InvalidString();

        private InvalidString()
        {

        }

        public String Read(ref ReadOnlySequence<byte> byteSequence)
        {
            throw new InvalidOperationException(string.Format("code is invalid. code:{0} format:{1}", byteSequence.First.Span[0], MessagePackCode.ToFormatName(byteSequence.First.Span[0])));
        }
    }

    internal interface IStringSegmentDecoder
    {
        ArraySegment<byte> Read(ref ReadOnlySequence<byte> byteSequence);
    }

    internal sealed class NilStringSegment : IStringSegmentDecoder
    {
        internal static readonly IStringSegmentDecoder Instance = new NilStringSegment();

        private NilStringSegment()
        {

        }

        public ArraySegment<byte> Read(ref ReadOnlySequence<byte> byteSequence)
        {
            readSize = 1;
            return new ArraySegment<byte>(bytes, offset, 1);
        }
    }

    internal sealed class FixStringSegment : IStringSegmentDecoder
    {
        internal static readonly IStringSegmentDecoder Instance = new FixStringSegment();

        private FixStringSegment()
        {

        }

        public ArraySegment<byte> Read(ref ReadOnlySequence<byte> byteSequence)
        {
            var length = span[0] & 0x1F;
            readSize = length + 1;
            return new ArraySegment<byte>(bytes, offset + 1, length);
        }
    }

    internal sealed class Str8StringSegment : IStringSegmentDecoder
    {
        internal static readonly IStringSegmentDecoder Instance = new Str8StringSegment();

        private Str8StringSegment()
        {

        }

        public ArraySegment<byte> Read(ref ReadOnlySequence<byte> byteSequence)
        {
            var length = (int)span[1];
            readSize = length + 2;
            return new ArraySegment<byte>(bytes, offset + 2, length);
        }
    }

    internal sealed class Str16StringSegment : IStringSegmentDecoder
    {
        internal static readonly IStringSegmentDecoder Instance = new Str16StringSegment();

        private Str16StringSegment()
        {

        }

        public ArraySegment<byte> Read(ref ReadOnlySequence<byte> byteSequence)
        {
            unchecked
            {
                var length = (span[1] << 8) + (span[2]);
                readSize = length + 3;
                return new ArraySegment<byte>(bytes, offset + 3, length);
            }
        }
    }

    internal sealed class Str32StringSegment : IStringSegmentDecoder
    {
        internal static readonly IStringSegmentDecoder Instance = new Str32StringSegment();

        private Str32StringSegment()
        {

        }

        public ArraySegment<byte> Read(ref ReadOnlySequence<byte> byteSequence)
        {
            unchecked
            {
                var length = (int)((uint)(span[1] << 24) | (uint)(span[2] << 16) | (uint)(span[3] << 8) | span[4]);
                readSize = length + 5;
                return new ArraySegment<byte>(bytes, offset + 5, length);
            }
        }
    }

    internal sealed class InvalidStringSegment : IStringSegmentDecoder
    {
        internal static readonly IStringSegmentDecoder Instance = new InvalidStringSegment();

        private InvalidStringSegment()
        {

        }

        public ArraySegment<byte> Read(ref ReadOnlySequence<byte> byteSequence)
        {
            throw new InvalidOperationException(string.Format("code is invalid. code:{0} format:{1}", byteSequence.First.Span[0], MessagePackCode.ToFormatName(byteSequence.First.Span[0])));
        }
    }

    internal interface IExtDecoder
    {
        ExtensionResult Read(ref ReadOnlySequence<byte> byteSequence);
    }

    internal sealed class FixExt1 : IExtDecoder
    {
        internal static readonly IExtDecoder Instance = new FixExt1();

        private FixExt1()
        {

        }

        public ExtensionResult Read(ref ReadOnlySequence<byte> byteSequence)
        {
            readSize = 3;
            var typeCode = unchecked((sbyte)span[1]);
            var body = new byte[1] { span[2] }; // make new bytes is overhead?
            return new ExtensionResult(typeCode, body);
        }
    }

    internal sealed class FixExt2 : IExtDecoder
    {
        internal static readonly IExtDecoder Instance = new FixExt2();

        private FixExt2()
        {

        }

        public ExtensionResult Read(ref ReadOnlySequence<byte> byteSequence)
        {
            readSize = 4;
            var typeCode = unchecked((sbyte)span[1]);
            var body = new byte[2]
            {
                span[2],
                span[3],
            };
            return new ExtensionResult(typeCode, body);
        }
    }

    internal sealed class FixExt4 : IExtDecoder
    {
        internal static readonly IExtDecoder Instance = new FixExt4();

        private FixExt4()
        {

        }

        public ExtensionResult Read(ref ReadOnlySequence<byte> byteSequence)
        {
            readSize = 6;
            var typeCode = unchecked((sbyte)span[1]);
            var body = new byte[4]
            {
                span[2],
                span[3],
                span[4],
                span[5],
            };
            return new ExtensionResult(typeCode, body);
        }
    }

    internal sealed class FixExt8 : IExtDecoder
    {
        internal static readonly IExtDecoder Instance = new FixExt8();

        private FixExt8()
        {

        }

        public ExtensionResult Read(ref ReadOnlySequence<byte> byteSequence)
        {
            readSize = 10;
            var typeCode = unchecked((sbyte)span[1]);
            var body = new byte[8]
            {
                span[2],
                span[3],
                span[4],
                span[5],
                span[6],
                span[7],
                span[8],
                span[9],
            };
            return new ExtensionResult(typeCode, body);
        }
    }

    internal sealed class FixExt16 : IExtDecoder
    {
        internal static readonly IExtDecoder Instance = new FixExt16();

        private FixExt16()
        {

        }

        public ExtensionResult Read(ref ReadOnlySequence<byte> byteSequence)
        {
            readSize = 18;
            var typeCode = unchecked((sbyte)span[1]);
            var body = new byte[16]
            {
                span[2],
                span[3],
                span[4],
                span[5],
                span[6],
                span[7],
                span[8],
                span[9],
                span[10],
                span[11],
                span[12],
                span[13],
                span[14],
                span[15],
                span[16],
                span[17]
            };
            return new ExtensionResult(typeCode, body);
        }
    }

    internal sealed class Ext8 : IExtDecoder
    {
        internal static readonly IExtDecoder Instance = new Ext8();

        private Ext8()
        {

        }

        public ExtensionResult Read(ref ReadOnlySequence<byte> byteSequence)
        {
            unchecked
            {
                var length = span[1];
                var typeCode = unchecked((sbyte)span[2]);

                var body = new byte[length];
                readSize = length + 3;
                Buffer.BlockCopy(bytes, offset + 3, body, 0, length);
                return new ExtensionResult(typeCode, body);
            }
        }
    }

    internal sealed class Ext16 : IExtDecoder
    {
        internal static readonly IExtDecoder Instance = new Ext16();

        private Ext16()
        {

        }

        public ExtensionResult Read(ref ReadOnlySequence<byte> byteSequence)
        {
            unchecked
            {
                var length = (UInt16)(span[1] << 8) | span[2];
                var typeCode = unchecked((sbyte)span[3]);

                var body = new byte[length];
                readSize = length + 4;
                Buffer.BlockCopy(bytes, offset + 4, body, 0, length);
                return new ExtensionResult(typeCode, body);
            }
        }
    }

    internal sealed class Ext32 : IExtDecoder
    {
        internal static readonly IExtDecoder Instance = new Ext32();

        private Ext32()
        {

        }

        public ExtensionResult Read(ref ReadOnlySequence<byte> byteSequence)
        {
            unchecked
            {
                var length = (UInt32)(span[1] << 24) | (UInt32)(span[2] << 16) | (UInt32)(span[3] << 8) | span[4];
                var typeCode = unchecked((sbyte)span[5]);

                var body = new byte[length];
                checked
                {
                    readSize = (int)length + 6;
                    Buffer.BlockCopy(bytes, offset + 6, body, 0, (int)length);
                }
                return new ExtensionResult(typeCode, body);
            }
        }
    }

    internal sealed class InvalidExt : IExtDecoder
    {
        internal static readonly IExtDecoder Instance = new InvalidExt();

        private InvalidExt()
        {

        }

        public ExtensionResult Read(ref ReadOnlySequence<byte> byteSequence)
        {
            throw new InvalidOperationException(string.Format("code is invalid. code:{0} format:{1}", byteSequence.First.Span[0], MessagePackCode.ToFormatName(byteSequence.First.Span[0])));
        }
    }






    internal interface IExtHeaderDecoder
    {
        ExtensionHeader Read(ref ReadOnlySequence<byte> byteSequence);
    }

    internal sealed class FixExt1Header : IExtHeaderDecoder
    {
        internal static readonly IExtHeaderDecoder Instance = new FixExt1Header();

        private FixExt1Header()
        {

        }

        public ExtensionHeader Read(ref ReadOnlySequence<byte> byteSequence)
        {
            readSize = 2;
            var typeCode = unchecked((sbyte)span[1]);
            return new ExtensionHeader(typeCode, 1);
        }
    }

    internal sealed class FixExt2Header : IExtHeaderDecoder
    {
        internal static readonly IExtHeaderDecoder Instance = new FixExt2Header();

        private FixExt2Header()
        {

        }

        public ExtensionHeader Read(ref ReadOnlySequence<byte> byteSequence)
        {
            readSize = 2;
            var typeCode = unchecked((sbyte)span[1]);
            return new ExtensionHeader(typeCode, 2);
        }
    }

    internal sealed class FixExt4Header : IExtHeaderDecoder
    {
        internal static readonly IExtHeaderDecoder Instance = new FixExt4Header();

        private FixExt4Header()
        {

        }

        public ExtensionHeader Read(ref ReadOnlySequence<byte> byteSequence)
        {
            readSize = 2;
            var typeCode = unchecked((sbyte)span[1]);
            return new ExtensionHeader(typeCode, 4);
        }
    }

    internal sealed class FixExt8Header : IExtHeaderDecoder
    {
        internal static readonly IExtHeaderDecoder Instance = new FixExt8Header();

        private FixExt8Header()
        {

        }

        public ExtensionHeader Read(ref ReadOnlySequence<byte> byteSequence)
        {
            readSize = 2;
            var typeCode = unchecked((sbyte)span[1]);
            return new ExtensionHeader(typeCode, 8);
        }
    }

    internal sealed class FixExt16Header : IExtHeaderDecoder
    {
        internal static readonly IExtHeaderDecoder Instance = new FixExt16Header();

        private FixExt16Header()
        {

        }

        public ExtensionHeader Read(ref ReadOnlySequence<byte> byteSequence)
        {
            readSize = 2;
            var typeCode = unchecked((sbyte)span[1]);
            return new ExtensionHeader(typeCode, 16);
        }
    }

    internal sealed class Ext8Header : IExtHeaderDecoder
    {
        internal static readonly IExtHeaderDecoder Instance = new Ext8Header();

        private Ext8Header()
        {

        }

        public ExtensionHeader Read(ref ReadOnlySequence<byte> byteSequence)
        {
            unchecked
            {
                var length = span[1];
                var typeCode = unchecked((sbyte)span[2]);

                readSize = 3;
                return new ExtensionHeader(typeCode, length);
            }
        }
    }

    internal sealed class Ext16Header : IExtHeaderDecoder
    {
        internal static readonly IExtHeaderDecoder Instance = new Ext16Header();

        private Ext16Header()
        {

        }

        public ExtensionHeader Read(ref ReadOnlySequence<byte> byteSequence)
        {
            unchecked
            {
                var length = (UInt32)((UInt16)(span[1] << 8) | span[2]);
                var typeCode = unchecked((sbyte)span[3]);

                readSize = 4;
                return new ExtensionHeader(typeCode, length);
            }
        }
    }

    internal sealed class Ext32Header : IExtHeaderDecoder
    {
        internal static readonly IExtHeaderDecoder Instance = new Ext32Header();

        private Ext32Header()
        {

        }

        public ExtensionHeader Read(ref ReadOnlySequence<byte> byteSequence)
        {
            unchecked
            {
                var length = (UInt32)(span[1] << 24) | (UInt32)(span[2] << 16) | (UInt32)(span[3] << 8) | span[4];
                var typeCode = unchecked((sbyte)span[5]);

                readSize = 6;
                return new ExtensionHeader(typeCode, length);
            }
        }
    }

    internal sealed class InvalidExtHeader : IExtHeaderDecoder
    {
        internal static readonly IExtHeaderDecoder Instance = new InvalidExtHeader();

        private InvalidExtHeader()
        {

        }

        public ExtensionHeader Read(ref ReadOnlySequence<byte> byteSequence)
        {
            throw new InvalidOperationException(string.Format("code is invalid. code:{0} format:{1}", byteSequence.First.Span[0], MessagePackCode.ToFormatName(byteSequence.First.Span[0])));
        }
    }

    internal interface IDateTimeDecoder
    {
        DateTime Read(ref ReadOnlySequence<byte> byteSequence);
    }

    internal sealed class FixExt4DateTime : IDateTimeDecoder
    {
        internal static readonly IDateTimeDecoder Instance = new FixExt4DateTime();

        private FixExt4DateTime()
        {

        }

        public DateTime Read(ref ReadOnlySequence<byte> byteSequence)
        {
            var typeCode = unchecked((sbyte)span[1]);
            if (typeCode != ReservedMessagePackExtensionTypeCode.DateTime)
            {
                throw new InvalidOperationException(string.Format("typeCode is invalid. typeCode:{0}", typeCode));
            }

            unchecked
            {
                var seconds = (UInt32)(span[2] << 24) | (UInt32)(span[3] << 16) | (UInt32)(span[4] << 8) | span[5];

                readSize = 6;
                return DateTimeConstants.UnixEpoch.AddSeconds(seconds);
            }
        }
    }

    internal sealed class FixExt8DateTime : IDateTimeDecoder
    {
        internal static readonly IDateTimeDecoder Instance = new FixExt8DateTime();

        private FixExt8DateTime()
        {

        }

        public DateTime Read(ref ReadOnlySequence<byte> byteSequence)
        {
            var typeCode = unchecked((sbyte)span[1]);
            if (typeCode != ReservedMessagePackExtensionTypeCode.DateTime)
            {
                throw new InvalidOperationException(string.Format("typeCode is invalid. typeCode:{0}", typeCode));
            }

            var data64 = (UInt64)span[2] << 56 | (UInt64)span[3] << 48 | (UInt64)span[4] << 40 | (UInt64)span[5] << 32
                       | (UInt64)span[6] << 24 | (UInt64)span[7] << 16 | (UInt64)span[8] << 8 | span[9];

            var nanoseconds = (long)(data64 >> 34);
            var seconds = data64 & 0x00000003ffffffffL;

            readSize = 10;
            return DateTimeConstants.UnixEpoch.AddSeconds(seconds).AddTicks(nanoseconds / DateTimeConstants.NanosecondsPerTick);
        }
    }

    internal sealed class Ext8DateTime : IDateTimeDecoder
    {
        internal static readonly IDateTimeDecoder Instance = new Ext8DateTime();

        private Ext8DateTime()
        {

        }

        public DateTime Read(ref ReadOnlySequence<byte> byteSequence)
        {
            var length = checked(span[1]);
            var typeCode = unchecked((sbyte)span[2]);
            if (length != 12 || typeCode != ReservedMessagePackExtensionTypeCode.DateTime)
            {
                throw new InvalidOperationException(string.Format("typeCode is invalid. typeCode:{0}", typeCode));
            }

            var nanoseconds = (UInt32)(span[3] << 24) | (UInt32)(span[4] << 16) | (UInt32)(span[5] << 8) | span[6];
            unchecked
            {
                var seconds = (long)span[7] << 56 | (long)span[8] << 48 | (long)span[9] << 40 | (long)span[10] << 32
                            | (long)span[11] << 24 | (long)span[12] << 16 | (long)span[13] << 8 | span[14];

                readSize = 15;
                return DateTimeConstants.UnixEpoch.AddSeconds(seconds).AddTicks(nanoseconds / DateTimeConstants.NanosecondsPerTick);
            }
        }
    }

    internal sealed class InvalidDateTime : IDateTimeDecoder
    {
        internal static readonly IDateTimeDecoder Instance = new InvalidDateTime();

        private InvalidDateTime()
        {

        }

        public DateTime Read(ref ReadOnlySequence<byte> byteSequence)
        {
            throw new InvalidOperationException(string.Format("code is invalid. code:{0} format:{1}", byteSequence.First.Span[0], MessagePackCode.ToFormatName(byteSequence.First.Span[0])));
        }
    }

    internal interface IReadNextDecoder
    {
        void Read(ref ReadOnlySequence<byte> byteSequence);
    }

    internal sealed class ReadNext1 : IReadNextDecoder
    {
        internal static readonly IReadNextDecoder Instance = new ReadNext1();

        private ReadNext1()
        {

        }
        public int Read(byte[] bytes, int offset) { return 1; }
    }

    internal sealed class ReadNext2 : IReadNextDecoder
    {
        internal static readonly IReadNextDecoder Instance = new ReadNext2();

        private ReadNext2()
        {

        }
        public int Read(byte[] bytes, int offset) { return 2; }

    }
    internal sealed class ReadNext3 : IReadNextDecoder
    {
        internal static readonly IReadNextDecoder Instance = new ReadNext3();

        private ReadNext3()
        {

        }
        public int Read(byte[] bytes, int offset) { return 3; }
    }
    internal sealed class ReadNext4 : IReadNextDecoder
    {
        internal static readonly IReadNextDecoder Instance = new ReadNext4();

        private ReadNext4()
        {

        }
        public int Read(byte[] bytes, int offset) { return 4; }
    }
    internal sealed class ReadNext5 : IReadNextDecoder
    {
        internal static readonly IReadNextDecoder Instance = new ReadNext5();

        private ReadNext5()
        {

        }
        public int Read(byte[] bytes, int offset) { return 5; }
    }
    internal sealed class ReadNext6 : IReadNextDecoder
    {
        internal static readonly IReadNextDecoder Instance = new ReadNext6();

        private ReadNext6()
        {

        }
        public int Read(byte[] bytes, int offset) { return 6; }
    }

    internal sealed class ReadNext9 : IReadNextDecoder
    {
        internal static readonly IReadNextDecoder Instance = new ReadNext9();

        private ReadNext9()
        {

        }
        public int Read(byte[] bytes, int offset) { return 9; }
    }
    internal sealed class ReadNext10 : IReadNextDecoder
    {
        internal static readonly IReadNextDecoder Instance = new ReadNext10();

        private ReadNext10()
        {

        }
        public int Read(byte[] bytes, int offset) { return 10; }
    }
    internal sealed class ReadNext18 : IReadNextDecoder
    {
        internal static readonly IReadNextDecoder Instance = new ReadNext18();

        private ReadNext18()
        {

        }
        public int Read(byte[] bytes, int offset) { return 18; }
    }

    internal sealed class ReadNextMap : IReadNextDecoder
    {
        internal static readonly IReadNextDecoder Instance = new ReadNextMap();

        private ReadNextMap()
        {

        }
        public int Read(byte[] bytes, int offset)
        {
            var startOffset = offset;
            int readSize;
            var length = MessagePackBinary.ReadMapHeader(ref byteSequence);
            offset += readSize;
            for (int i = 0; i < length; i++)
            {
                offset += MessagePackBinary.ReadNext(bytes, offset); // key
                offset += MessagePackBinary.ReadNext(bytes, offset); // value
            }
            return offset - startOffset;
        }
    }

    internal sealed class ReadNextArray : IReadNextDecoder
    {
        internal static readonly IReadNextDecoder Instance = new ReadNextArray();

        private ReadNextArray()
        {

        }
        public int Read(byte[] bytes, int offset)
        {
            var startOffset = offset;
            int readSize;
            var length = MessagePackBinary.ReadArrayHeader(ref byteSequence);
            offset += readSize;
            for (int i = 0; i < length; i++)
            {
                offset += MessagePackBinary.ReadNext(bytes, offset);
            }
            return offset - startOffset;
        }
    }

    internal sealed class ReadNextFixStr : IReadNextDecoder
    {
        internal static readonly IReadNextDecoder Instance = new ReadNextFixStr();

        private ReadNextFixStr()
        {

        }
        public int Read(byte[] bytes, int offset)
        {
            var length = span[0] & 0x1F;
            return length + 1;
        }
    }

    internal sealed class ReadNextStr8 : IReadNextDecoder
    {
        internal static readonly IReadNextDecoder Instance = new ReadNextStr8();

        private ReadNextStr8()
        {

        }
        public int Read(byte[] bytes, int offset)
        {
            var length = (int)span[1];
            return length + 2;
        }
    }

    internal sealed class ReadNextStr16 : IReadNextDecoder
    {
        internal static readonly IReadNextDecoder Instance = new ReadNextStr16();

        private ReadNextStr16()
        {

        }
        public int Read(byte[] bytes, int offset)
        {

            var length = (span[1] << 8) | (span[2]);
            return length + 3;
        }
    }

    internal sealed class ReadNextStr32 : IReadNextDecoder
    {
        internal static readonly IReadNextDecoder Instance = new ReadNextStr32();

        private ReadNextStr32()
        {

        }
        public int Read(byte[] bytes, int offset)
        {
            var length = (int)((uint)(span[1] << 24) | (uint)(span[2] << 16) | (uint)(span[3] << 8) | span[4]);
            return length + 5;
        }
    }

    internal sealed class ReadNextBin8 : IReadNextDecoder
    {
        internal static readonly IReadNextDecoder Instance = new ReadNextBin8();

        private ReadNextBin8()
        {

        }
        public int Read(byte[] bytes, int offset)
        {
            var length = span[1];
            return length + 2;
        }
    }

    internal sealed class ReadNextBin16 : IReadNextDecoder
    {
        internal static readonly IReadNextDecoder Instance = new ReadNextBin16();

        private ReadNextBin16()
        {

        }
        public int Read(byte[] bytes, int offset)
        {

            var length = (span[1] << 8) | (span[2]);
            return length + 3;
        }
    }

    internal sealed class ReadNextBin32 : IReadNextDecoder
    {
        internal static readonly IReadNextDecoder Instance = new ReadNextBin32();

        private ReadNextBin32()
        {

        }
        public int Read(byte[] bytes, int offset)
        {
            var length = (span[1] << 24) | (span[2] << 16) | (span[3] << 8) | (span[4]);
            return length + 5;
        }
    }

    internal sealed class ReadNextExt8 : IReadNextDecoder
    {
        internal static readonly IReadNextDecoder Instance = new ReadNextExt8();

        private ReadNextExt8()
        {

        }
        public int Read(byte[] bytes, int offset)
        {
            var length = span[1];
            return length + 3;
        }
    }

    internal sealed class ReadNextExt16 : IReadNextDecoder
    {
        internal static readonly IReadNextDecoder Instance = new ReadNextExt16();

        private ReadNextExt16()
        {

        }
        public int Read(byte[] bytes, int offset)
        {
            var length = (UInt16)(span[1] << 8) | span[2];
            return length + 4;
        }
    }

    internal sealed class ReadNextExt32 : IReadNextDecoder
    {
        internal static readonly IReadNextDecoder Instance = new ReadNextExt32();

        private ReadNextExt32()
        {

        }
        public int Read(byte[] bytes, int offset)
        {
            var length = (UInt32)(span[1] << 24) | (UInt32)(span[2] << 16) | (UInt32)(span[3] << 8) | span[4];
            return (int)length + 6;
        }
    }
}
