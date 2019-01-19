using MessagePack.Internal;
using System;
using System.Buffers;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

#if !UNITY
using System.Threading.Tasks;
#endif

namespace MessagePack.Formatters
{
    // NET40 -> BigInteger, Complex, Tuple

    // byte[] is special. represents bin type.
    public sealed class ByteArrayFormatter : IMessagePackFormatter<byte[]>
    {
        public static readonly ByteArrayFormatter Instance = new ByteArrayFormatter();

        ByteArrayFormatter()
        {

        }

        public void Serialize(IBufferWriter<byte> writer, byte[] value, IFormatterResolver formatterResolver)
        {
            MessagePackBinary.WriteBytes(writer, value);
            return;
        }

        public byte[] Deserialize(ref ReadOnlySequence<byte> byteSequence, IFormatterResolver formatterResolver)
        {
            return MessagePackBinary.ReadBytes(ref byteSequence);
        }
    }

    public sealed class NullableStringFormatter : IMessagePackFormatter<String>
    {
        public static readonly NullableStringFormatter Instance = new NullableStringFormatter();

        NullableStringFormatter()
        {

        }

        public void Serialize(IBufferWriter<byte> writer, String value, IFormatterResolver typeResolver)
        {
            MessagePackBinary.WriteString(writer, value);
        }

        public String Deserialize(ref ReadOnlySequence<byte> byteSequence, IFormatterResolver formatterResolver)
        {
            return MessagePackBinary.ReadString(ref byteSequence);
        }
    }

    public sealed class NullableStringArrayFormatter : IMessagePackFormatter<String[]>
    {
        public static readonly NullableStringArrayFormatter Instance = new NullableStringArrayFormatter();

        NullableStringArrayFormatter()
        {

        }

        public void Serialize(IBufferWriter<byte> writer, String[] value, IFormatterResolver typeResolver)
        {
            if (value == null)
            {
                MessagePackBinary.WriteNil(writer);
            }
            else
            {
                MessagePackBinary.WriteArrayHeader(writer, value.Length);
                for (int i = 0; i < value.Length; i++)
                {
                    MessagePackBinary.WriteString(writer, value[i]);
                }
            }
        }

        public String[] Deserialize(ref ReadOnlySequence<byte> byteSequence, IFormatterResolver formatterResolver)
        {
            if (MessagePackBinary.IsNil(byteSequence))
            {
                byteSequence = byteSequence.Slice(1);
                return null;
            }
            else
            {
                var len = MessagePackBinary.ReadArrayHeader(ref byteSequence);
                var array = new String[len];
                for (int i = 0; i < array.Length; i++)
                {
                    array[i] = MessagePackBinary.ReadString(ref byteSequence);
                }
                return array;
            }
        }
    }

    public sealed class DecimalFormatter : IMessagePackFormatter<Decimal>
    {
        public static readonly DecimalFormatter Instance = new DecimalFormatter();

        DecimalFormatter()
        {

        }

        public void Serialize(IBufferWriter<byte> writer, decimal value, IFormatterResolver formatterResolver)
        {
            MessagePackBinary.WriteString(writer, value.ToString(CultureInfo.InvariantCulture));
            return;
        }

        public decimal Deserialize(ref ReadOnlySequence<byte> byteSequence, IFormatterResolver formatterResolver)
        {
            return decimal.Parse(MessagePackBinary.ReadString(ref byteSequence), CultureInfo.InvariantCulture);
        }
    }

    public sealed class TimeSpanFormatter : IMessagePackFormatter<TimeSpan>
    {
        public static readonly IMessagePackFormatter<TimeSpan> Instance = new TimeSpanFormatter();

        TimeSpanFormatter()
        {

        }

        public void Serialize(IBufferWriter<byte> writer, TimeSpan value, IFormatterResolver formatterResolver)
        {
            MessagePackBinary.WriteInt64(writer, value.Ticks);
            return;
        }

        public TimeSpan Deserialize(ref ReadOnlySequence<byte> byteSequence, IFormatterResolver formatterResolver)
        {
            return new TimeSpan(MessagePackBinary.ReadInt64(ref byteSequence));
        }
    }

    public sealed class DateTimeOffsetFormatter : IMessagePackFormatter<DateTimeOffset>
    {
        public static readonly IMessagePackFormatter<DateTimeOffset> Instance = new DateTimeOffsetFormatter();

        DateTimeOffsetFormatter()
        {

        }

        public void Serialize(IBufferWriter<byte> writer, DateTimeOffset value, IFormatterResolver formatterResolver)
        {
            MessagePackBinary.WriteArrayHeader(writer, 2);
            MessagePackBinary.WriteDateTime(writer, new DateTime(value.Ticks, DateTimeKind.Utc)); // current ticks as is
            MessagePackBinary.WriteInt16(writer, (short)value.Offset.TotalMinutes); // offset is normalized in minutes
            return;
        }

        public DateTimeOffset Deserialize(ref ReadOnlySequence<byte> byteSequence, IFormatterResolver formatterResolver)
        {
            var count = MessagePackBinary.ReadArrayHeader(ref byteSequence);

            if (count != 2) throw new InvalidOperationException("Invalid DateTimeOffset format.");

            var utc = MessagePackBinary.ReadDateTime(ref byteSequence);

            var dtOffsetMinutes = MessagePackBinary.ReadInt16(ref byteSequence);


            return new DateTimeOffset(utc.Ticks, TimeSpan.FromMinutes(dtOffsetMinutes));
        }
    }

    public sealed class GuidFormatter : IMessagePackFormatter<Guid>
    {
        public static readonly IMessagePackFormatter<Guid> Instance = new GuidFormatter();


        GuidFormatter()
        {

        }

        public void Serialize(IBufferWriter<byte> writer, Guid value, IFormatterResolver formatterResolver)
        {
            var bytes = writer.GetSpan(38);
            bytes[0] = MessagePackCode.Str8;
            bytes[1] = unchecked((byte)36);
            new GuidBits(ref value).Write(bytes.Slice(2));
            writer.Advance(38);
        }

        public Guid Deserialize(ref ReadOnlySequence<byte> byteSequence, IFormatterResolver formatterResolver)
        {
            var segment = MessagePackBinary.ReadStringSegment(ref byteSequence);
            return new GuidBits(segment).Value;
        }
    }

    public sealed class UriFormatter : IMessagePackFormatter<Uri>
    {
        public static readonly IMessagePackFormatter<Uri> Instance = new UriFormatter();


        UriFormatter()
        {

        }

        public void Serialize(IBufferWriter<byte> writer, Uri value, IFormatterResolver formatterResolver)
        {
            if (value == null)
            {
                MessagePackBinary.WriteNil(writer);
            }
            else
            {
                MessagePackBinary.WriteString(writer, value.ToString());
            }
        }

        public Uri Deserialize(ref ReadOnlySequence<byte> byteSequence, IFormatterResolver formatterResolver)
        {
            if (MessagePackBinary.IsNil(byteSequence))
            {
                byteSequence = byteSequence.Slice(1);
                return null;
            }
            else
            {
                return new Uri(MessagePackBinary.ReadString(ref byteSequence), UriKind.RelativeOrAbsolute);
            }
        }
    }

    public sealed class VersionFormatter : IMessagePackFormatter<Version>
    {
        public static readonly IMessagePackFormatter<Version> Instance = new VersionFormatter();

        VersionFormatter()
        {

        }

        public void Serialize(IBufferWriter<byte> writer, Version value, IFormatterResolver formatterResolver)
        {
            if (value == null)
            {
                MessagePackBinary.WriteNil(writer);
            }
            else
            {
                MessagePackBinary.WriteString(writer, value.ToString());
            }
        }

        public Version Deserialize(ref ReadOnlySequence<byte> byteSequence, IFormatterResolver formatterResolver)
        {
            if (MessagePackBinary.IsNil(byteSequence))
            {
                byteSequence = byteSequence.Slice(1);
                return null;
            }
            else
            {
                return new Version(MessagePackBinary.ReadString(ref byteSequence));
            }
        }
    }

    public sealed class KeyValuePairFormatter<TKey, TValue> : IMessagePackFormatter<KeyValuePair<TKey, TValue>>
    {
        public void Serialize(IBufferWriter<byte> writer, KeyValuePair<TKey, TValue> value, IFormatterResolver formatterResolver)
        {
            MessagePackBinary.WriteArrayHeader(writer, 2);
            formatterResolver.GetFormatterWithVerify<TKey>().Serialize(writer, value.Key, formatterResolver);
            formatterResolver.GetFormatterWithVerify<TValue>().Serialize(writer, value.Value, formatterResolver);
            return;
        }

        public KeyValuePair<TKey, TValue> Deserialize(ref ReadOnlySequence<byte> byteSequence, IFormatterResolver formatterResolver)
        {
            var count = MessagePackBinary.ReadArrayHeader(ref byteSequence);

            if (count != 2) throw new InvalidOperationException("Invalid KeyValuePair format.");

            var key = formatterResolver.GetFormatterWithVerify<TKey>().Deserialize(ref byteSequence, formatterResolver);
            var value = formatterResolver.GetFormatterWithVerify<TValue>().Deserialize(ref byteSequence, formatterResolver);
            return new KeyValuePair<TKey, TValue>(key, value);
        }
    }

    public sealed class StringBuilderFormatter : IMessagePackFormatter<StringBuilder>
    {
        public static readonly IMessagePackFormatter<StringBuilder> Instance = new StringBuilderFormatter();

        StringBuilderFormatter()
        {

        }

        public void Serialize(IBufferWriter<byte> writer, StringBuilder value, IFormatterResolver formatterResolver)
        {
            if (value == null)
            {
                MessagePackBinary.WriteNil(writer);
            }
            else
            {
                MessagePackBinary.WriteString(writer, value.ToString());
            }
        }

        public StringBuilder Deserialize(ref ReadOnlySequence<byte> byteSequence, IFormatterResolver formatterResolver)
        {
            if (MessagePackBinary.IsNil(byteSequence))
            {
                byteSequence = byteSequence.Slice(1);
                return null;
            }
            else
            {
                return new StringBuilder(MessagePackBinary.ReadString(ref byteSequence));
            }
        }
    }

    public sealed class BitArrayFormatter : IMessagePackFormatter<BitArray>
    {
        public static readonly IMessagePackFormatter<BitArray> Instance = new BitArrayFormatter();

        BitArrayFormatter()
        {

        }

        public void Serialize(IBufferWriter<byte> writer, BitArray value, IFormatterResolver formatterResolver)
        {
            if (value == null)
            {
                MessagePackBinary.WriteNil(writer);
            }
            else
            {
                var len = value.Length;
                MessagePackBinary.WriteArrayHeader(writer, len);
                for (int i = 0; i < len; i++)
                {
                    MessagePackBinary.WriteBoolean(writer, value.Get(i));
                }

                return;
            }
        }

        public BitArray Deserialize(ref ReadOnlySequence<byte> byteSequence, IFormatterResolver formatterResolver)
        {
            if (MessagePackBinary.IsNil(byteSequence))
            {
                byteSequence = byteSequence.Slice(1);
                return null;
            }
            else
            {
                var len = MessagePackBinary.ReadArrayHeader(ref byteSequence);

                var array = new BitArray(len);
                for (int i = 0; i < len; i++)
                {
                    array[i] = MessagePackBinary.ReadBoolean(ref byteSequence);
                }

                return array;
            }
        }
    }

#if !UNITY

    public sealed class BigIntegerFormatter : IMessagePackFormatter<System.Numerics.BigInteger>
    {
        public static readonly IMessagePackFormatter<System.Numerics.BigInteger> Instance = new BigIntegerFormatter();

        BigIntegerFormatter()
        {

        }

        public void Serialize(IBufferWriter<byte> writer, System.Numerics.BigInteger value, IFormatterResolver formatterResolver)
        {
            MessagePackBinary.WriteBytes(writer, value.ToByteArray());
            return;
        }

        public System.Numerics.BigInteger Deserialize(ref ReadOnlySequence<byte> byteSequence, IFormatterResolver formatterResolver)
        {
            return new System.Numerics.BigInteger(MessagePackBinary.ReadBytes(ref byteSequence));
        }
    }

    public sealed class ComplexFormatter : IMessagePackFormatter<System.Numerics.Complex>
    {
        public static readonly IMessagePackFormatter<System.Numerics.Complex> Instance = new ComplexFormatter();

        ComplexFormatter()
        {

        }

        public void Serialize(IBufferWriter<byte> writer, System.Numerics.Complex value, IFormatterResolver formatterResolver)
        {
            MessagePackBinary.WriteArrayHeader(writer, 2);
            MessagePackBinary.WriteDouble(writer, value.Real);
            MessagePackBinary.WriteDouble(writer, value.Imaginary);
            return;
        }

        public System.Numerics.Complex Deserialize(ref ReadOnlySequence<byte> byteSequence, IFormatterResolver formatterResolver)
        {
            var count = MessagePackBinary.ReadArrayHeader(ref byteSequence);

            if (count != 2) throw new InvalidOperationException("Invalid Complex format.");

            var real = MessagePackBinary.ReadDouble(ref byteSequence);

            var imaginary = MessagePackBinary.ReadDouble(ref byteSequence);

            return new System.Numerics.Complex(real, imaginary);
        }
    }

    public sealed class LazyFormatter<T> : IMessagePackFormatter<Lazy<T>>
    {
        public void Serialize(IBufferWriter<byte> writer, Lazy<T> value, IFormatterResolver formatterResolver)
        {
            if (value == null)
            {
                MessagePackBinary.WriteNil(writer);
            }
            else
            {
                formatterResolver.GetFormatterWithVerify<T>().Serialize(writer, value.Value, formatterResolver);
            }
        }

        public Lazy<T> Deserialize(ref ReadOnlySequence<byte> byteSequence, IFormatterResolver formatterResolver)
        {
            if (MessagePackBinary.IsNil(byteSequence))
            {
                byteSequence = byteSequence.Slice(1);
                return null;
            }
            else
            {
                // deserialize immediately(no delay, because capture byte[] causes memory leak)
                var v = formatterResolver.GetFormatterWithVerify<T>().Deserialize(ref byteSequence, formatterResolver);
                return new Lazy<T>(() => v);
            }
        }
    }

#pragma warning disable VSTHRD002 // This section will be removed when https://github.com/AArnott/MessagePack-CSharp/issues/29 is fixed

    public sealed class TaskUnitFormatter : IMessagePackFormatter<Task>
    {
        public static readonly IMessagePackFormatter<Task> Instance = new TaskUnitFormatter();
        static readonly Task CompletedTask = Task.FromResult<object>(null);

        TaskUnitFormatter()
        {

        }

        public void Serialize(IBufferWriter<byte> writer, Task value, IFormatterResolver formatterResolver)
        {
            if (value == null)
            {
                MessagePackBinary.WriteNil(writer);
            }
            else
            {
                value.Wait(); // wait...!
                MessagePackBinary.WriteNil(writer);
            }
        }

        public Task Deserialize(ref ReadOnlySequence<byte> byteSequence, IFormatterResolver formatterResolver)
        {
            if (!MessagePackBinary.IsNil(byteSequence))
            {
                throw new InvalidOperationException("Invalid input");
            }
            else
            {
                byteSequence = byteSequence.Slice(1);
                return CompletedTask;
            }
        }
    }

    public sealed class TaskValueFormatter<T> : IMessagePackFormatter<Task<T>>
    {
        public void Serialize(IBufferWriter<byte> writer, Task<T> value, IFormatterResolver formatterResolver)
        {
            if (value == null)
            {
                MessagePackBinary.WriteNil(writer);
            }
            else
            {
                // value.Result -> wait...!
                formatterResolver.GetFormatterWithVerify<T>().Serialize(writer, value.Result, formatterResolver);
            }
        }

        public Task<T> Deserialize(ref ReadOnlySequence<byte> byteSequence, IFormatterResolver formatterResolver)
        {
            if (MessagePackBinary.IsNil(byteSequence))
            {
                byteSequence = byteSequence.Slice(1);
                return null;
            }
            else
            {
                var v = formatterResolver.GetFormatterWithVerify<T>().Deserialize(ref byteSequence, formatterResolver);
                return Task.FromResult(v);
            }
        }
    }

    public sealed class ValueTaskFormatter<T> : IMessagePackFormatter<ValueTask<T>>
    {
        public void Serialize(IBufferWriter<byte> writer, ValueTask<T> value, IFormatterResolver formatterResolver)
        {
            formatterResolver.GetFormatterWithVerify<T>().Serialize(writer, value.Result, formatterResolver);
        }

        public ValueTask<T> Deserialize(ref ReadOnlySequence<byte> byteSequence, IFormatterResolver formatterResolver)
        {
            var v = formatterResolver.GetFormatterWithVerify<T>().Deserialize(ref byteSequence, formatterResolver);
            return new ValueTask<T>(v);
        }
    }

#pragma warning restore VSTHRD002

#endif
}