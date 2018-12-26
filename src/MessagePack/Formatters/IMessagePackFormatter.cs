using System;
using System.Buffers;

namespace MessagePack.Formatters
{
    public interface IMessagePackFormatter<T>
    {
        void Serialize(IBufferWriter<byte> writer, T value, IFormatterResolver formatterResolver);

        T Deserialize(ref ReadOnlySequence<byte> byteSequence, IFormatterResolver resolver);
    }
}
