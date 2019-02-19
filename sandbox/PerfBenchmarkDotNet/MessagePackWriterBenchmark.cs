extern alias newmsgpack;
extern alias oldmsgpack;

using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using Nerdbank.Streams;

namespace PerfBenchmarkDotNet
{
    [ClrJob(baseline: true), CoreJob]
    //[Config(typeof(BenchmarkConfig))]
    [GroupBenchmarksBy(BenchmarkLogicalGroupRule.ByCategory)]
    [CategoriesColumn]
    public class MessagePackWriterBenchmark
    {
        private const int RepsOverArray = 100 * 1024;
        private readonly Sequence<byte> sequence = new Sequence<byte>();

        private readonly int[] values = new int[newmsgpack::MessagePack.MessagePackCode.MaxFixInt];
        private readonly byte[] byteValues = new byte[newmsgpack::MessagePack.MessagePackCode.MaxFixInt];

        private byte[] bytes;

        [GlobalSetup]
        public void GlobalSetup()
        {
            int bufferSize = 16 + 4 * RepsOverArray * values.Length;
            this.sequence.GetSpan(bufferSize);
            bytes = new byte[bufferSize];

            for (int i = 0; i < values.Length; i++)
            {
                values[i] = i + 1;
            }
            for (int i = 0; i < byteValues.Length; i++)
            {
                byteValues[i] = (byte)(i + 1);
            }
        }

        [IterationSetup]
        public void IterationSetup() => sequence.Reset();

        [Benchmark(OperationsPerInvoke = RepsOverArray * newmsgpack::MessagePack.MessagePackCode.MaxFixInt)]
        [BenchmarkCategory("2.0")]
        public void Write_Byte()
        {
            var writer = new newmsgpack::MessagePack.MessagePackWriter(this.sequence);
            for (int j = 0; j < RepsOverArray; j++)
            {
                for (int i = 0; i < byteValues.Length; i++)
                {
                    writer.Write(byteValues[i]);
                }
            }
        }

        [Benchmark(OperationsPerInvoke = RepsOverArray * newmsgpack::MessagePack.MessagePackCode.MaxFixInt)]
        [BenchmarkCategory("1.x")]
        public void WriteByte()
        {
            int offset = 0;
            for (int j = 0; j < RepsOverArray; j++)
            {
                for (int i = 0; i < byteValues.Length; i++)
                {
                    offset += oldmsgpack::MessagePack.MessagePackBinary.WriteByte(ref bytes, offset, byteValues[i]);
                }
            }
        }

        [Benchmark(OperationsPerInvoke = RepsOverArray * newmsgpack::MessagePack.MessagePackCode.MaxFixInt)]
        [BenchmarkCategory("2.0")]
        public void Write_UInt32()
        {
            var writer = new newmsgpack::MessagePack.MessagePackWriter(this.sequence);
            for (int j = 0; j < RepsOverArray; j++)
            {
                for (int i = 0; i < values.Length; i++)
                {
                    writer.Write((uint)values[i]);
                }
            }
        }

        [Benchmark(OperationsPerInvoke = RepsOverArray * newmsgpack::MessagePack.MessagePackCode.MaxFixInt)]
        [BenchmarkCategory("2.0")]
        public void Write_Int32()
        {
            var writer = new newmsgpack::MessagePack.MessagePackWriter(this.sequence);
            for (int j = 0; j < RepsOverArray; j++)
            {
                for (int i = 0; i < values.Length; i++)
                {
                    writer.Write(values[i]);
                }
            }
        }

        [Benchmark(OperationsPerInvoke = RepsOverArray * newmsgpack::MessagePack.MessagePackCode.MaxFixInt)]
        [BenchmarkCategory("1.x")]
        public void WriteInt32()
        {
            int offset = 0;
            for (int j = 0; j < RepsOverArray; j++)
            {
                for (int i = 0; i < values.Length; i++)
                {
                    offset += oldmsgpack::MessagePack.MessagePackBinary.WriteInt32(ref bytes, offset, values[i]);
                }
            }
        }

        [Benchmark(OperationsPerInvoke = 100000)]
        [BenchmarkCategory("2.0")]
        public void Write_String()
        {
            var writer = new newmsgpack::MessagePack.MessagePackWriter(this.sequence);
            for (int i = 0; i < 100000; i++)
            {
                writer.Write("Hello!");
            }
        }

        [Benchmark(OperationsPerInvoke = 100000)]
        [BenchmarkCategory("1.x")]
        public void WriteString()
        {
            int offset = 0;
            for (int i = 0; i < 100000; i++)
            {
                offset += oldmsgpack::MessagePack.MessagePackBinary.WriteString(ref bytes, offset, "Hello!");
            }
        }
    }
}
