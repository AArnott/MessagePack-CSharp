﻿using MessagePack.Formatters;
using MessagePack.Internal;
using Nerdbank.Streams;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace MessagePack.Tests
{
    public class UnsafeMemoryTest
    {
        private MessagePackSerializer serializer = new MessagePackSerializer();

        delegate void WriteDelegate(ref BufferWriter writer, ReadOnlySpan<byte> ys);

        [Theory]
        [InlineData('a', 1)]
        [InlineData('b', 10)]
        [InlineData('c', 100)]
        [InlineData('d', 1000)]
        [InlineData('e', 10000)]
        [InlineData('f', 100000)]
        public void GetEncodedStringBytes(char c, int count)
        {
            var s = new string(c, count);
            var bin1 = MessagePackBinary.GetEncodedStringBytes(s);
            var bin2 = serializer.Serialize(s);
            var bin3 = new Sequence<byte>();
            var bin3Writer = new BufferWriter(bin3);
            MessagePackBinary.WriteRaw(ref bin3Writer, bin1);
            bin3Writer.Commit();

            MessagePack.Internal.ByteArrayComparer.Equals(bin1, 0, bin1.Length, bin2).IsTrue();
            MessagePack.Internal.ByteArrayComparer.Equals(bin1, 0, bin1.Length, bin3.AsReadOnlySequence.ToArray()).IsTrue();
        }

        [Fact]
        public void WriteRaw()
        {
            // x86
            for (int i = 1; i <= MessagePackRange.MaxFixStringLength; i++)
            {
                var src = Enumerable.Range(0, i).Select(x => (byte)x).ToArray();
                var dst = new Sequence<byte>();
                var dstWriter = new BufferWriter(dst);
                ((typeof(UnsafeMemory32).GetMethod("WriteRaw" + i)).CreateDelegate(typeof(WriteDelegate)) as WriteDelegate).Invoke(ref dstWriter, src);
                dstWriter.Commit();
                dst.Length.Is(i);
                MessagePack.Internal.ByteArrayComparer.Equals(src, 0, src.Length, dst.AsReadOnlySequence.ToArray()).IsTrue();
            }
            // x64
            for (int i = 1; i <= MessagePackRange.MaxFixStringLength; i++)
            {
                var src = Enumerable.Range(0, i).Select(x => (byte)x).ToArray();
                var dst = new Sequence<byte>();
                var dstWriter = new BufferWriter(dst);
                ((typeof(UnsafeMemory64).GetMethod("WriteRaw" + i)).CreateDelegate(typeof(WriteDelegate)) as WriteDelegate).Invoke(ref dstWriter, src);
                dstWriter.Commit();
                dst.Length.Is(i);
                MessagePack.Internal.ByteArrayComparer.Equals(src, 0, src.Length, dst.AsReadOnlySequence.ToArray()).IsTrue();
            }
        }
    }
}
