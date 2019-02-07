using System.Buffers;

namespace MessagePack.Formatters
{
    public sealed class IgnoreFormatter<T> : IMessagePackFormatter<T>
    {
        public void Serialize(ref BufferWriter writer, T value, IFormatterResolver formatterResolver)
        {
            MessagePackBinary.WriteNil(ref writer);
        }

        public T Deserialize(ref ReadOnlySequence<byte> byteSequence, IFormatterResolver formatterResolver)
        {
            MessagePackBinary.ReadNextBlock(ref byteSequence);
            return default(T);
        }
    }
}