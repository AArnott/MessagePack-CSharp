using System;
using System.Buffers;

namespace MessagePack.Formatters
{
    /// <summary>
    /// Serialize by .NET native DateTime binary format.
    /// </summary>
    public sealed class NativeDateTimeFormatter : IMessagePackFormatter<DateTime>
    {
        public static readonly NativeDateTimeFormatter Instance = new NativeDateTimeFormatter();

        public void Serialize(IBufferWriter<byte> writer, DateTime value, IFormatterResolver formatterResolver)
        {
            var dateData = value.ToBinary();
            MessagePackBinary.WriteInt64(writer, dateData);
        }

        public DateTime Deserialize(ref ReadOnlySequence<byte> byteSequence, IFormatterResolver formatterResolver)
        {
            if (MessagePackBinary.GetMessagePackType(byteSequence) == MessagePackType.Extension)
            {
                return DateTimeFormatter.Instance.Deserialize(ref byteSequence, formatterResolver);
            }

            var dateData = MessagePackBinary.ReadInt64(ref byteSequence);
            return DateTime.FromBinary(dateData);
        }
    }

    public sealed class NativeDateTimeArrayFormatter : IMessagePackFormatter<DateTime[]>
    {
        public static readonly NativeDateTimeArrayFormatter Instance = new NativeDateTimeArrayFormatter();

        public void Serialize(IBufferWriter<byte> writer, DateTime[] value, IFormatterResolver formatterResolver)
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
                    MessagePackBinary.WriteInt64(writer, value[i].ToBinary());
                }
            }
        }

        public DateTime[] Deserialize(ref ReadOnlySequence<byte> byteSequence, IFormatterResolver formatterResolver)
        {
            if (MessagePackBinary.IsNil(byteSequence))
            {
                byteSequence = byteSequence.Slice(1);
                return null;
            }
            else
            {
                var len = MessagePackBinary.ReadArrayHeader(ref byteSequence);
                var array = new DateTime[len];
                for (int i = 0; i < array.Length; i++)
                {
                    var dateData = MessagePackBinary.ReadInt64(ref byteSequence);
                    array[i] = DateTime.FromBinary(dateData);
                }

                return array;
            }
        }
    }

    // Old-Spec
    // bin 8, bin 16, bin 32, str 8, str 16, str 32 -> fixraw or raw 16 or raw 32
    // fixraw -> fixstr, raw16 -> str16, raw32 -> str32
    // https://github.com/msgpack/msgpack/blob/master/spec-old.md

    /// <summary>
    /// Old-MessagePack spec's string formatter.
    /// </summary>
    public sealed class OldSpecStringFormatter : IMessagePackFormatter<string>
    {
        public static readonly OldSpecStringFormatter Instance = new OldSpecStringFormatter();

        // Old spec does not exists str 8 format.
        public void Serialize(IBufferWriter<byte> writer, string value, IFormatterResolver formatterResolver)
        {
            if (value == null)
            {
                MessagePackBinary.WriteNil(writer);
                return;
            }

            int byteCount = StringEncoding.UTF8.GetByteCount(value);
            int headerLength;
            Span<byte> span;
            if (byteCount <= MessagePackRange.MaxFixStringLength)
            {
                headerLength = 1;
                span = writer.GetSpan(headerLength + byteCount);
                span[0] = (byte)(MessagePackCode.MinFixStr | byteCount);
            }
            else if (byteCount <= ushort.MaxValue)
            {
                headerLength = 3;
                span = writer.GetSpan(headerLength + byteCount);
                span[0] = MessagePackCode.Str16;
                span[1] = unchecked((byte)(byteCount >> 8));
                span[2] = unchecked((byte)byteCount);
            }
            else
            {
                headerLength = 5;
                span = writer.GetSpan(headerLength + byteCount);
                span[0] = MessagePackCode.Str32;
                span[1] = unchecked((byte)(byteCount >> 24));
                span[2] = unchecked((byte)(byteCount >> 16));
                span[3] = unchecked((byte)(byteCount >> 8));
                span[4] = unchecked((byte)byteCount);
            }

            StringEncoding.UTF8.GetBytes(value, span.Slice(headerLength));
            writer.Advance(headerLength + byteCount);
        }

        public string Deserialize(ref ReadOnlySequence<byte> byteSequence, IFormatterResolver formatterResolver)
        {
            return MessagePackBinary.ReadString(ref byteSequence);
        }
    }

    /// <summary>
    /// Old-MessagePack spec's binary formatter.
    /// </summary>
    public sealed class OldSpecBinaryFormatter : IMessagePackFormatter<byte[]>
    {
        public static readonly OldSpecBinaryFormatter Instance = new OldSpecBinaryFormatter();

        public void Serialize(IBufferWriter<byte> writer, byte[] value, IFormatterResolver formatterResolver)
        {
            if (value == null)
            {
                 MessagePackBinary.WriteNil(writer);
                 return;
            }

            var byteCount = value.Length;

            if (byteCount <= MessagePackRange.MaxFixStringLength)
            {
                var span = writer.GetSpan(byteCount + 1);
                span[0] = (byte)(MessagePackCode.MinFixStr | byteCount);
                value.CopyTo(span.Slice(1));
                writer.Advance(byteCount + 1);
            }
            else if (byteCount <= ushort.MaxValue)
            {
                var span = writer.GetSpan(byteCount + 3);
                span[0] = MessagePackCode.Str16;
                span[1] = unchecked((byte)(byteCount >> 8));
                span[2] = unchecked((byte)byteCount);
                value.CopyTo(span.Slice(3));
                writer.Advance(byteCount + 3);
            }
            else
            {
                var span = writer.GetSpan(byteCount + 5);
                span[0] = MessagePackCode.Str32;
                span[1] = unchecked((byte)(byteCount >> 24));
                span[2] = unchecked((byte)(byteCount >> 16));
                span[3] = unchecked((byte)(byteCount >> 8));
                span[4] = unchecked((byte)byteCount);
                value.CopyTo(span.Slice(5));
                writer.Advance(byteCount + 5);
            }
        }

        public byte[] Deserialize(ref ReadOnlySequence<byte> byteSequence, IFormatterResolver formatterResolver)
        {
            var type = MessagePackBinary.GetMessagePackType(byteSequence);
            if (type == MessagePackType.Nil)
            {
                byteSequence = byteSequence.Slice(1);
                return null;
            }
            else if (type == MessagePackType.Binary)
            {
                return MessagePackBinary.ReadBytes(ref byteSequence);
            }
            else if (type == MessagePackType.String)
            {
                var code = byteSequence.First.Span[0];
                unchecked
                {
                    if (MessagePackCode.MinFixStr <= code && code <= MessagePackCode.MaxFixStr)
                    {
                        return MessagePackBinary.Parse(ref byteSequence, 1, lengthSpan => lengthSpan[0] & 0x1F, span => span.ToArray());
                    }
                    else if (code == MessagePackCode.Str8)
                    {
                        return MessagePackBinary.Parse(ref byteSequence, 2, lengthSpan => lengthSpan[1], span => span.ToArray());
                    }
                    else if (code == MessagePackCode.Str16)
                    {
                        return MessagePackBinary.Parse(
                            ref byteSequence,
                            3,
                            lengthSpan => (lengthSpan[1] << 8) + (lengthSpan[2]),
                            span => span.ToArray());
                    }
                    else if (code == MessagePackCode.Str32)
                    {
                        return MessagePackBinary.Parse(
                            ref byteSequence,
                            5,
                            lengthSpan => (int)((uint)(lengthSpan[1] << 24) | (uint)(lengthSpan[2] << 16) | (uint)(lengthSpan[3] << 8) | (uint)lengthSpan[4]),
                            span => span.ToArray());
                    }
                }
            }

            throw new InvalidOperationException(string.Format("code is invalid. code:{0} format:{1}", byteSequence.First.Span[0], MessagePackCode.ToFormatName(byteSequence.First.Span[0])));
        }
    }
}
