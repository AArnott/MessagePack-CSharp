// Copyright (c) All contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Buffers;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using MessagePack.LZ4;

namespace MessagePack
{
    internal static class LZ4Utilities
    {
        private const int LZ4NotCompressionSizeInLz4BlockType = 64;

        private delegate int LZ4Transform(ReadOnlySpan<byte> input, Span<byte> output);

        private static readonly LZ4Transform LZ4CodecEncode = LZ4Codec.Encode;
        private static readonly LZ4Transform LZ4CodecDecode = LZ4Codec.Decode;

        /// <summary>
        /// Compresses messagepack data using LZ4.
        /// </summary>
        /// <param name="msgpackUncompressedData">The uncompressed msgpack payload to be compressed.</param>
        /// <param name="writer">The writer to emit the compressed msgpack extension to.</param>
        /// <param name="compression">The compression format to use.</param>
        /// <remarks>
        /// The uncompressed data may be copied verbatim from <paramref name="msgpackUncompressedData"/> to <paramref name="writer"/>
        /// when compression is not likely to produce a smaller result.
        /// </remarks>
        internal static void Compress(in ReadOnlySequence<byte> msgpackUncompressedData, ref MessagePackWriter writer, MessagePackCompression compression)
        {
            if (compression == MessagePackCompression.Lz4Block)
            {
                if (msgpackUncompressedData.Length < LZ4NotCompressionSizeInLz4BlockType)
                {
                    writer.WriteRaw(msgpackUncompressedData);
                    return;
                }

                var maxCompressedLength = LZ4Codec.MaximumOutputLength((int)msgpackUncompressedData.Length);
                var lz4Span = ArrayPool<byte>.Shared.Rent(maxCompressedLength);
                try
                {
                    int lz4Length = LZ4Operation(msgpackUncompressedData, lz4Span, LZ4CodecEncode);

                    const int LengthOfUncompressedDataSizeHeader = 5;
                    writer.WriteExtensionFormatHeader(new ExtensionHeader(ThisLibraryExtensionTypeCodes.Lz4Block, LengthOfUncompressedDataSizeHeader + (uint)lz4Length));
                    writer.WriteInt32((int)msgpackUncompressedData.Length);
                    writer.WriteRaw(lz4Span.AsSpan(0, lz4Length));
                }
                finally
                {
                    ArrayPool<byte>.Shared.Return(lz4Span);
                }
            }
            else if (compression == MessagePackCompression.Lz4BlockArray)
            {
                // Write to [Ext(98:int,int...), bin,bin,bin...]
                var sequenceCount = 0;
                var extHeaderSize = 0;
                foreach (var item in msgpackUncompressedData)
                {
                    sequenceCount++;
                    extHeaderSize += GetUInt32WriteSize((uint)item.Length);
                }

                writer.WriteArrayHeader(sequenceCount + 1);
                writer.WriteExtensionFormatHeader(new ExtensionHeader(ThisLibraryExtensionTypeCodes.Lz4BlockArray, extHeaderSize));
                {
                    foreach (var item in msgpackUncompressedData)
                    {
                        writer.Write(item.Length);
                    }
                }

                foreach (var item in msgpackUncompressedData)
                {
                    var maxCompressedLength = LZ4Codec.MaximumOutputLength(item.Length);
                    var lz4Span = writer.GetSpan(maxCompressedLength + 5);
                    int lz4Length = LZ4Codec.Encode(item.Span, lz4Span.Slice(5, lz4Span.Length - 5));
                    WriteBin32Header((uint)lz4Length, lz4Span);
                    writer.Advance(lz4Length + 5);
                }
            }
            else
            {
                throw new ArgumentException("Invalid MessagePackCompression Code. Code:" + compression);
            }
        }

        /// <summary>
        /// Decompresses msgpack from an LZ4 compression extension written with <see cref="Compress(in ReadOnlySequence{byte}, ref MessagePackWriter, MessagePackCompression)"/>.
        /// </summary>
        /// <param name="reader">The reader positioned on what may be an LZ4 extension header.</param>
        /// <param name="writer">The writer to copy the uncompressed msgpack data to.</param>
        /// <returns>
        /// <see langword="true"/> if a compression extension header was found and the uncompressed extension payload was written to <paramref name="writer"/>;
        /// <see langword="false"/> if the msgpack is not compressed and the caller may read naturally from the <paramref name="reader"/>.
        /// </returns>
        internal static bool TryDecompress(ref MessagePackReader reader, IBufferWriter<byte> writer)
        {
            if (!reader.End)
            {
                // Try to find LZ4Block
                if (reader.NextMessagePackType == MessagePackType.Extension)
                {
                    MessagePackReader peekReader = reader.CreatePeekReader();
                    ExtensionHeader header = peekReader.ReadExtensionFormatHeader();
                    if (header.TypeCode == ThisLibraryExtensionTypeCodes.Lz4Block)
                    {
                        // Read the extension using the original reader, so we "consume" it.
                        ExtensionResult extension = reader.ReadExtensionFormat();
                        var extReader = new MessagePackReader(extension.Data);

                        // The first part of the extension payload is a MessagePack-encoded Int32 that
                        // tells us the length the data will be AFTER decompression.
                        int uncompressedLength = extReader.ReadInt32();

                        // The rest of the payload is the compressed data itself.
                        ReadOnlySequence<byte> compressedData = extReader.Sequence.Slice(extReader.Position);

                        Span<byte> uncompressedSpan = writer.GetSpan(uncompressedLength).Slice(0, uncompressedLength);
                        int actualUncompressedLength = LZ4Operation(compressedData, uncompressedSpan, LZ4CodecDecode);
                        Debug.Assert(actualUncompressedLength == uncompressedLength, "Unexpected length of uncompressed data.");
                        writer.Advance(actualUncompressedLength);
                        return true;
                    }
                }

                // Try to find LZ4BlockArray
                if (reader.NextMessagePackType == MessagePackType.Array)
                {
                    MessagePackReader peekReader = reader.CreatePeekReader();
                    var arrayLength = peekReader.ReadArrayHeader();
                    if (arrayLength != 0 && peekReader.NextMessagePackType == MessagePackType.Extension)
                    {
                        ExtensionHeader header = peekReader.ReadExtensionFormatHeader();
                        if (header.TypeCode == ThisLibraryExtensionTypeCodes.Lz4BlockArray)
                        {
                            // switch peekReader as original reader.
                            reader = peekReader;

                            // Read from [Ext(98:int,int...), bin,bin,bin...]
                            var sequenceCount = arrayLength - 1;
                            var uncompressedLengths = ArrayPool<int>.Shared.Rent(sequenceCount);
                            try
                            {
                                for (int i = 0; i < sequenceCount; i++)
                                {
                                    uncompressedLengths[i] = reader.ReadInt32();
                                }

                                for (int i = 0; i < sequenceCount; i++)
                                {
                                    var uncompressedLength = uncompressedLengths[i];
                                    var lz4Block = reader.ReadBytes();
                                    Span<byte> uncompressedSpan = writer.GetSpan(uncompressedLength).Slice(0, uncompressedLength);
                                    var actualUncompressedLength = LZ4Operation(lz4Block.Value, uncompressedSpan, LZ4CodecDecode);
                                    Debug.Assert(actualUncompressedLength == uncompressedLength, "Unexpected length of uncompressed data.");
                                    writer.Advance(actualUncompressedLength);
                                }

                                return true;
                            }
                            finally
                            {
                                ArrayPool<int>.Shared.Return(uncompressedLengths);
                            }
                        }
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Performs LZ4 compression or decompression.
        /// </summary>
        /// <param name="input">The input for the operation.</param>
        /// <param name="output">The buffer to write the result of the operation.</param>
        /// <param name="lz4Operation">The LZ4 codec transformation.</param>
        /// <returns>The number of bytes written to the <paramref name="output"/>.</returns>
        private static int LZ4Operation(in ReadOnlySequence<byte> input, Span<byte> output, LZ4Transform lz4Operation)
        {
            ReadOnlySpan<byte> inputSpan;
            byte[] rentedInputArray = null;
            if (input.IsSingleSegment)
            {
                inputSpan = input.First.Span;
            }
            else
            {
                rentedInputArray = ArrayPool<byte>.Shared.Rent((int)input.Length);
                input.CopyTo(rentedInputArray);
                inputSpan = rentedInputArray.AsSpan(0, (int)input.Length);
            }

            try
            {
                return lz4Operation(inputSpan, output);
            }
            finally
            {
                if (rentedInputArray != null)
                {
                    ArrayPool<byte>.Shared.Return(rentedInputArray);
                }
            }
        }

        private static int GetUInt32WriteSize(uint value)
        {
            if (value <= MessagePackRange.MaxFixPositiveInt)
            {
                return 1;
            }
            else if (value <= byte.MaxValue)
            {
                return 2;
            }
            else if (value <= ushort.MaxValue)
            {
                return 3;
            }
            else
            {
                return 5;
            }
        }

        private static void WriteBin32Header(uint value, Span<byte> span)
        {
            unchecked
            {
                span[0] = MessagePackCode.Bin32;

                // Write to highest index first so the JIT skips bounds checks on subsequent writes.
                span[4] = (byte)value;
                span[3] = (byte)(value >> 8);
                span[2] = (byte)(value >> 16);
                span[1] = (byte)(value >> 24);
            }
        }
    }
}
