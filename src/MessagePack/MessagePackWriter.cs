using MessagePack.Formatters;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.Text;

namespace MessagePack
{
    public ref struct MessagePackWriter<T> where T : IBufferWriter<byte>
    {
        private BufferWriter writer;

        public MessagePackWriter(IBufferWriter<byte> writer)
        {
            this.writer = new BufferWriter(writer);
        }

        /// <summary>
        /// Write the length of the next array to be written.
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
    }
}
