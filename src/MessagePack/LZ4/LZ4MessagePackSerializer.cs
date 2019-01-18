using System;
using System.Buffers;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using MessagePack.Internal;
using MessagePack.LZ4;
using Microsoft;

namespace MessagePack
{
    /// <summary>
    /// LZ4 Compressed special serializer.
    /// </summary>
    public partial class LZ4MessagePackSerializer : MessagePackSerializer
    {
        public const sbyte ExtensionTypeCode = 99;

        public const int NotCompressionSize = 64;

        /// <summary>
        /// Initializes a new instance of the <see cref="LZ4MessagePackSerializer"/> class
        /// initialized with the <see cref="Resolvers.StandardResolver"/>.
        /// </summary>
        public LZ4MessagePackSerializer()
            : this(Resolvers.StandardResolver.Instance)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LZ4MessagePackSerializer"/> class
        /// </summary>
        /// <param name="defaultResolver">The resolver to use.</param>
        public LZ4MessagePackSerializer(IFormatterResolver defaultResolver)
            : base(defaultResolver)
        {
        }

        /// <summary>
        /// Serialize to binary with default resolver.
        /// </summary>
        public override void Serialize<T>(IBufferWriter<byte> writer, T value, IFormatterResolver resolver)
        {
            if (resolver == null) resolver = DefaultResolver;
            var formatter = resolver.GetFormatterWithVerify<T>();

            using (var innerWriter = new Nerdbank.Streams.Sequence<byte>())
            {
                formatter.Serialize(innerWriter, value, resolver);
                ToLZ4BinaryCore(innerWriter.AsReadOnlySequence, writer);
            }
        }

        public override T Deserialize<T>(ReadOnlySequence<byte> byteSequence, IFormatterResolver resolver, out SequencePosition endPosition)
        {
            if (resolver == null)
            {
                resolver = DefaultResolver;
            }

            var formatter = resolver.GetFormatterWithVerify<T>();

            using (var uncompressedSequence = new Nerdbank.Streams.Sequence<byte>())
            {
                if (this.TryDecompress(ref byteSequence, uncompressedSequence))
                {
                    endPosition = byteSequence.End;
                    var uncompressedReadOnlySequence = uncompressedSequence.AsReadOnlySequence;
                    return formatter.Deserialize(ref uncompressedReadOnlySequence, resolver);
                }
                else
                {
                    T result = formatter.Deserialize(ref byteSequence, resolver);
                    endPosition = byteSequence.End;
                    return result;
                }
            }
        }

        private bool TryDecompress(ref ReadOnlySequence<byte> byteSequence, IBufferWriter<byte> writer)
        {
            if (MessagePackBinary.GetMessagePackType(byteSequence) == MessagePackType.Extension)
            {
                var header = MessagePackBinary.ReadExtensionFormatHeader(ref byteSequence);
                if (header.TypeCode == ExtensionTypeCode)
                {
                    int compressedLength = (int)header.Length - 5;
                    int uncompressedLength = MessagePackBinary.ReadInt32(ref byteSequence);

                    var uncompressedMemory = writer.GetMemory(uncompressedLength);
                    if (!MemoryMarshal.TryGetArray(uncompressedMemory, out ArraySegment<byte> uncompressedSegment))
                    {
                        throw new InvalidOperationException("Unable to get ArraySegment to write to.");
                    }

                    var compressedSegment = MessagePackBinary.ReadArraySegment(ref byteSequence, compressedLength);
                    int actualUncompressedLength = LZ4Codec.Decode(compressedSegment.Array, compressedSegment.Offset, compressedSegment.Count, uncompressedSegment.Array, uncompressedSegment.Offset, uncompressedLength);
                    Assumes.True(actualUncompressedLength == uncompressedLength);
                    writer.Advance(actualUncompressedLength);
                    return true;
                }
            }

            return false;
        }

        private static void ToLZ4BinaryCore(ReadOnlySequence<byte> serializedData, IBufferWriter<byte> writer)
        {
            if (serializedData.Length < NotCompressionSize)
            {
                var span = writer.GetSpan((int)serializedData.Length);
                serializedData.CopyTo(span);
                writer.Advance((int)serializedData.Length);
            }
            else
            {
                // Reserve space for the extension header.
                const int ExtensionHeaderLength = 6;
                const int CompressedStreamLengthLength = 5;
                var headerSpan = writer.GetSpan(ExtensionHeaderLength + CompressedStreamLengthLength);
                writer.Advance(ExtensionHeaderLength + CompressedStreamLengthLength);
                headerSpan = headerSpan.Slice(0, ExtensionHeaderLength + CompressedStreamLengthLength); // trim it to just what we promised to use

                // write body
                ArraySegment<byte> srcArray, dstArray;
                bool rentedSourceArray = false, rentedTargetArray = false;
                if (!serializedData.IsSingleSegment || !MemoryMarshal.TryGetArray(serializedData.First, out srcArray))
                {
                    srcArray = new ArraySegment<byte>(ArrayPool<byte>.Shared.Rent((int)serializedData.Length));
                    serializedData.CopyTo(srcArray);
                    rentedSourceArray = true;
                }

                var maxOutCount = LZ4Codec.MaximumOutputLength((int)serializedData.Length);
                var compressedMemory = writer.GetMemory(maxOutCount);
                if (!MemoryMarshal.TryGetArray(compressedMemory, out dstArray))
                {
                    dstArray = new ArraySegment<byte>(ArrayPool<byte>.Shared.Rent(maxOutCount));
                    rentedTargetArray = true;
                }

                int lz4Length;
                try
                {
                    lz4Length = LZ4Codec.Encode(srcArray.Array, srcArray.Offset, (int)serializedData.Length, dstArray.Array, dstArray.Offset, dstArray.Count);
                    if (rentedTargetArray)
                    {
                        dstArray.AsSpan(0, lz4Length).CopyTo(compressedMemory.Span);
                    }

                    writer.Advance(lz4Length);
                }
                finally
                {
                    if (rentedSourceArray)
                    {
                        ArrayPool<byte>.Shared.Return(srcArray.Array);
                    }

                    if (rentedTargetArray)
                    {
                        ArrayPool<byte>.Shared.Return(dstArray.Array);
                    }
                }

                // write extension header (always 6 bytes)
                MessagePackBinary.WriteExtensionFormatHeaderForceExt32Block(headerSpan, ExtensionTypeCode, lz4Length + CompressedStreamLengthLength);

                // write length of uncompressed stream (always 5 bytes)
                MessagePackBinary.WriteInt32ForceInt32Block(headerSpan.Slice(ExtensionHeaderLength), (int)serializedData.Length);
            }
        }
    }
}
