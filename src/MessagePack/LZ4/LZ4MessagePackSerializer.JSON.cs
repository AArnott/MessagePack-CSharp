using MessagePack.Formatters;
using MessagePack.Internal;
using MessagePack.LZ4;
using System;
using System.Buffers;
using System.Globalization;
using System.IO;
using System.Text;

namespace MessagePack
{
    // JSON API
    public partial class LZ4MessagePackSerializer
    {
        /// <summary>
        /// Dump message-pack binary to JSON string.
        /// </summary>
        public override string ToJson(ReadOnlySequence<byte> byteSequence)
        {
            if (byteSequence.Length == 0) return "";

            using (var uncompressedSequence = new Nerdbank.Streams.Sequence<byte>())
            {
                if (this.TryDecompress(ref byteSequence, uncompressedSequence))
                {
                    return base.ToJson(uncompressedSequence.AsReadOnlySequence);
                }
                else
                {
                    return base.ToJson(byteSequence);
                }
            }
        }

        /// <summary>
        /// From Json String to LZ4MessagePack binary
        /// </summary>
        public override void FromJson(TextReader reader, IBufferWriter<byte> writer)
        {
            using (var sequence = new Nerdbank.Streams.Sequence<byte>())
            {
                base.FromJson(reader, sequence);
                ToLZ4BinaryCore(sequence, writer);
            }
        }
    }
}
