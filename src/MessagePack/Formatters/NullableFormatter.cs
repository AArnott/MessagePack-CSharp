using System.Buffers;

namespace MessagePack.Formatters
{
    public sealed class NullableFormatter<T> : IMessagePackFormatter<T?>
        where T : struct
    {
        public void Serialize(IBufferWriter<byte> writer, T? value, IFormatterResolver formatterResolver)
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

        public T? Deserialize(ref ReadOnlySequence<byte> byteSequence, IFormatterResolver formatterResolver)
        {
            if (MessagePackBinary.IsNil(byteSequence))
            {
                byteSequence = byteSequence.Slice(1);
                return null;
            }
            else
            {
                return formatterResolver.GetFormatterWithVerify<T>().Deserialize(ref byteSequence, formatterResolver);
            }
        }
    }

    public sealed class StaticNullableFormatter<T> : IMessagePackFormatter<T?>
        where T : struct
    {
        readonly IMessagePackFormatter<T> underlyingFormatter;

        public StaticNullableFormatter(IMessagePackFormatter<T> underlyingFormatter)
        {
            this.underlyingFormatter = underlyingFormatter;
        }

        public void Serialize(IBufferWriter<byte> writer, T? value, IFormatterResolver formatterResolver)
        {
            if (value == null)
            {
                MessagePackBinary.WriteNil(writer);
            }
            else
            {
                underlyingFormatter.Serialize(writer, value.Value, formatterResolver);
            }
        }

        public T? Deserialize(ref ReadOnlySequence<byte> byteSequence, IFormatterResolver formatterResolver)
        {
            if (MessagePackBinary.IsNil(byteSequence))
            {
                byteSequence = byteSequence.Slice(1);
                return null;
            }
            else
            {
                return underlyingFormatter.Deserialize(ref byteSequence, formatterResolver);
            }
        }
    }
}