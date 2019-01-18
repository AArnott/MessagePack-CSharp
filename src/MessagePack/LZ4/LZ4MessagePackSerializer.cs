using System;
using System.Buffers;
using System.Runtime.InteropServices;
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
                serializedData.CopyTo(writer);
            }
            else
            {
                ArraySegment<byte> srcArray;
                bool rentedSourceArray = false;
                if (!serializedData.IsSingleSegment || !MemoryMarshal.TryGetArray(serializedData.First, out srcArray))
                {
                    srcArray = new ArraySegment<byte>(ArrayPool<byte>.Shared.Rent((int)serializedData.Length));
                    serializedData.CopyTo(srcArray);
                    rentedSourceArray = true;
                }

                var maxOutCount = LZ4Codec.MaximumOutputLength((int)serializedData.Length);
                var dstArray = new ArraySegment<byte>(ArrayPool<byte>.Shared.Rent(maxOutCount));

                try
                {
                    int lz4Length = LZ4Codec.Encode(srcArray.Array, srcArray.Offset, (int)serializedData.Length, dstArray.Array, dstArray.Offset, dstArray.Count);

                    const int CompressedStreamLengthLength = 5;
                    MessagePackBinary.WriteExtensionFormatHeaderForceExt32Block(writer, ExtensionTypeCode, lz4Length + CompressedStreamLengthLength);
                    MessagePackBinary.WriteInt32ForceInt32Block(writer, (int)serializedData.Length);
                    writer.Write(dstArray.AsSpan(0, lz4Length));
                }
                finally
                {
                    if (rentedSourceArray)
                    {
                        ArrayPool<byte>.Shared.Return(srcArray.Array);
                    }

                    ArrayPool<byte>.Shared.Return(dstArray.Array);
                }
            }
        }
    }
}
