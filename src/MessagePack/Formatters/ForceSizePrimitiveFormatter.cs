using System;
using System.Buffers;

namespace MessagePack.Formatters
{
    public sealed class ForceInt16BlockFormatter : IMessagePackFormatter<Int16>
    {
        public static readonly ForceInt16BlockFormatter Instance = new ForceInt16BlockFormatter();

        ForceInt16BlockFormatter()
        {
        }

        public void Serialize(IBufferWriter<byte> writer, Int16 value, IFormatterResolver formatterResolver)
        {
            return MessagePackBinary.WriteInt16ForceInt16Block(writer, value);
        }

        public Int16 Deserialize(ref ReadOnlySequence<byte> byteSequence, IFormatterResolver formatterResolver)
        {
            return MessagePackBinary.ReadInt16(ref byteSequence);
        }
    }

    public sealed class NullableForceInt16BlockFormatter : IMessagePackFormatter<Int16?>
    {
        public static readonly NullableForceInt16BlockFormatter Instance = new NullableForceInt16BlockFormatter();

        NullableForceInt16BlockFormatter()
        {
        }

        public void Serialize(IBufferWriter<byte> writer, Int16? value, IFormatterResolver formatterResolver)
        {
            if (value == null)
            {
                MessagePackBinary.WriteNil(writer);
            }
            else
            {
                MessagePackBinary.WriteInt16ForceInt16Block(writer, value.Value);
            }
        }

        public Int16? Deserialize(ref ReadOnlySequence<byte> byteSequence, IFormatterResolver formatterResolver)
        {
            if (MessagePackBinary.IsNil(byteSequence))
            {
                byteSequence = byteSequence.Slice(1);
                return null;
            }
            else
            {
                return MessagePackBinary.ReadInt16(ref byteSequence);
            }
        }
    }

    public sealed class ForceInt16BlockArrayFormatter : IMessagePackFormatter<Int16[]>
    {
        public static readonly ForceInt16BlockArrayFormatter Instance = new ForceInt16BlockArrayFormatter();

        ForceInt16BlockArrayFormatter()
        {

        }

        public void Serialize(IBufferWriter<byte> writer, Int16[] value, IFormatterResolver formatterResolver)
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
                    MessagePackBinary.WriteInt16ForceInt16Block(writer, value[i]);
                }
            }
        }

        public Int16[] Deserialize(ref ReadOnlySequence<byte> byteSequence, IFormatterResolver formatterResolver)
        {
            if (MessagePackBinary.IsNil(byteSequence))
            {
                byteSequence = byteSequence.Slice(1);
                return null;
            }
            else
            {
                var len = MessagePackBinary.ReadArrayHeader(ref byteSequence);
                var array = new Int16[len];
                for (int i = 0; i < array.Length; i++)
                {
                    array[i] = MessagePackBinary.ReadInt16(ref byteSequence);
                }
                return array;
            }
        }
    }

    public sealed class ForceInt32BlockFormatter : IMessagePackFormatter<Int32>
    {
        public static readonly ForceInt32BlockFormatter Instance = new ForceInt32BlockFormatter();

        ForceInt32BlockFormatter()
        {
        }

        public void Serialize(IBufferWriter<byte> writer, Int32 value, IFormatterResolver formatterResolver)
        {
            return MessagePackBinary.WriteInt32ForceInt32Block(writer, value);
        }

        public Int32 Deserialize(ref ReadOnlySequence<byte> byteSequence, IFormatterResolver formatterResolver)
        {
            return MessagePackBinary.ReadInt32(ref byteSequence);
        }
    }

    public sealed class NullableForceInt32BlockFormatter : IMessagePackFormatter<Int32?>
    {
        public static readonly NullableForceInt32BlockFormatter Instance = new NullableForceInt32BlockFormatter();

        NullableForceInt32BlockFormatter()
        {
        }

        public void Serialize(IBufferWriter<byte> writer, Int32? value, IFormatterResolver formatterResolver)
        {
            if (value == null)
            {
                MessagePackBinary.WriteNil(writer);
            }
            else
            {
                MessagePackBinary.WriteInt32ForceInt32Block(writer, value.Value);
            }
        }

        public Int32? Deserialize(ref ReadOnlySequence<byte> byteSequence, IFormatterResolver formatterResolver)
        {
            if (MessagePackBinary.IsNil(byteSequence))
            {
                byteSequence = byteSequence.Slice(1);
                return null;
            }
            else
            {
                return MessagePackBinary.ReadInt32(ref byteSequence);
            }
        }
    }

    public sealed class ForceInt32BlockArrayFormatter : IMessagePackFormatter<Int32[]>
    {
        public static readonly ForceInt32BlockArrayFormatter Instance = new ForceInt32BlockArrayFormatter();

        ForceInt32BlockArrayFormatter()
        {

        }

        public void Serialize(IBufferWriter<byte> writer, Int32[] value, IFormatterResolver formatterResolver)
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
                    MessagePackBinary.WriteInt32ForceInt32Block(writer, value[i]);
                }
            }
        }

        public Int32[] Deserialize(ref ReadOnlySequence<byte> byteSequence, IFormatterResolver formatterResolver)
        {
            if (MessagePackBinary.IsNil(byteSequence))
            {
                byteSequence = byteSequence.Slice(1);
                return null;
            }
            else
            {
                var len = MessagePackBinary.ReadArrayHeader(ref byteSequence);
                var array = new Int32[len];
                for (int i = 0; i < array.Length; i++)
                {
                    array[i] = MessagePackBinary.ReadInt32(ref byteSequence);
                }
                return array;
            }
        }
    }

    public sealed class ForceInt64BlockFormatter : IMessagePackFormatter<Int64>
    {
        public static readonly ForceInt64BlockFormatter Instance = new ForceInt64BlockFormatter();

        ForceInt64BlockFormatter()
        {
        }

        public void Serialize(IBufferWriter<byte> writer, Int64 value, IFormatterResolver formatterResolver)
        {
            return MessagePackBinary.WriteInt64ForceInt64Block(writer, value);
        }

        public Int64 Deserialize(ref ReadOnlySequence<byte> byteSequence, IFormatterResolver formatterResolver)
        {
            return MessagePackBinary.ReadInt64(ref byteSequence);
        }
    }

    public sealed class NullableForceInt64BlockFormatter : IMessagePackFormatter<Int64?>
    {
        public static readonly NullableForceInt64BlockFormatter Instance = new NullableForceInt64BlockFormatter();

        NullableForceInt64BlockFormatter()
        {
        }

        public void Serialize(IBufferWriter<byte> writer, Int64? value, IFormatterResolver formatterResolver)
        {
            if (value == null)
            {
                MessagePackBinary.WriteNil(writer);
            }
            else
            {
                MessagePackBinary.WriteInt64ForceInt64Block(writer, value.Value);
            }
        }

        public Int64? Deserialize(ref ReadOnlySequence<byte> byteSequence, IFormatterResolver formatterResolver)
        {
            if (MessagePackBinary.IsNil(byteSequence))
            {
                byteSequence = byteSequence.Slice(1);
                return null;
            }
            else
            {
                return MessagePackBinary.ReadInt64(ref byteSequence);
            }
        }
    }

    public sealed class ForceInt64BlockArrayFormatter : IMessagePackFormatter<Int64[]>
    {
        public static readonly ForceInt64BlockArrayFormatter Instance = new ForceInt64BlockArrayFormatter();

        ForceInt64BlockArrayFormatter()
        {

        }

        public void Serialize(IBufferWriter<byte> writer, Int64[] value, IFormatterResolver formatterResolver)
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
                    MessagePackBinary.WriteInt64ForceInt64Block(writer, value[i]);
                }
            }
        }

        public Int64[] Deserialize(ref ReadOnlySequence<byte> byteSequence, IFormatterResolver formatterResolver)
        {
            if (MessagePackBinary.IsNil(byteSequence))
            {
                byteSequence = byteSequence.Slice(1);
                return null;
            }
            else
            {
                var len = MessagePackBinary.ReadArrayHeader(ref byteSequence);
                var array = new Int64[len];
                for (int i = 0; i < array.Length; i++)
                {
                    array[i] = MessagePackBinary.ReadInt64(ref byteSequence);
                }
                return array;
            }
        }
    }

    public sealed class ForceUInt16BlockFormatter : IMessagePackFormatter<UInt16>
    {
        public static readonly ForceUInt16BlockFormatter Instance = new ForceUInt16BlockFormatter();

        ForceUInt16BlockFormatter()
        {
        }

        public void Serialize(IBufferWriter<byte> writer, UInt16 value, IFormatterResolver formatterResolver)
        {
            return MessagePackBinary.WriteUInt16ForceUInt16Block(writer, value);
        }

        public UInt16 Deserialize(ref ReadOnlySequence<byte> byteSequence, IFormatterResolver formatterResolver)
        {
            return MessagePackBinary.ReadUInt16(ref byteSequence);
        }
    }

    public sealed class NullableForceUInt16BlockFormatter : IMessagePackFormatter<UInt16?>
    {
        public static readonly NullableForceUInt16BlockFormatter Instance = new NullableForceUInt16BlockFormatter();

        NullableForceUInt16BlockFormatter()
        {
        }

        public void Serialize(IBufferWriter<byte> writer, UInt16? value, IFormatterResolver formatterResolver)
        {
            if (value == null)
            {
                MessagePackBinary.WriteNil(writer);
            }
            else
            {
                MessagePackBinary.WriteUInt16ForceUInt16Block(writer, value.Value);
            }
        }

        public UInt16? Deserialize(ref ReadOnlySequence<byte> byteSequence, IFormatterResolver formatterResolver)
        {
            if (MessagePackBinary.IsNil(byteSequence))
            {
                byteSequence = byteSequence.Slice(1);
                return null;
            }
            else
            {
                return MessagePackBinary.ReadUInt16(ref byteSequence);
            }
        }
    }

    public sealed class ForceUInt16BlockArrayFormatter : IMessagePackFormatter<UInt16[]>
    {
        public static readonly ForceUInt16BlockArrayFormatter Instance = new ForceUInt16BlockArrayFormatter();

        ForceUInt16BlockArrayFormatter()
        {

        }

        public void Serialize(IBufferWriter<byte> writer, UInt16[] value, IFormatterResolver formatterResolver)
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
                    MessagePackBinary.WriteUInt16ForceUInt16Block(writer, value[i]);
                }
            }
        }

        public UInt16[] Deserialize(ref ReadOnlySequence<byte> byteSequence, IFormatterResolver formatterResolver)
        {
            if (MessagePackBinary.IsNil(byteSequence))
            {
                byteSequence = byteSequence.Slice(1);
                return null;
            }
            else
            {
                var len = MessagePackBinary.ReadArrayHeader(ref byteSequence);
                var array = new UInt16[len];
                for (int i = 0; i < array.Length; i++)
                {
                    array[i] = MessagePackBinary.ReadUInt16(ref byteSequence);
                }
                return array;
            }
        }
    }

    public sealed class ForceUInt32BlockFormatter : IMessagePackFormatter<UInt32>
    {
        public static readonly ForceUInt32BlockFormatter Instance = new ForceUInt32BlockFormatter();

        ForceUInt32BlockFormatter()
        {
        }

        public void Serialize(IBufferWriter<byte> writer, UInt32 value, IFormatterResolver formatterResolver)
        {
            return MessagePackBinary.WriteUInt32ForceUInt32Block(writer, value);
        }

        public UInt32 Deserialize(ref ReadOnlySequence<byte> byteSequence, IFormatterResolver formatterResolver)
        {
            return MessagePackBinary.ReadUInt32(ref byteSequence);
        }
    }

    public sealed class NullableForceUInt32BlockFormatter : IMessagePackFormatter<UInt32?>
    {
        public static readonly NullableForceUInt32BlockFormatter Instance = new NullableForceUInt32BlockFormatter();

        NullableForceUInt32BlockFormatter()
        {
        }

        public void Serialize(IBufferWriter<byte> writer, UInt32? value, IFormatterResolver formatterResolver)
        {
            if (value == null)
            {
                MessagePackBinary.WriteNil(writer);
            }
            else
            {
                MessagePackBinary.WriteUInt32ForceUInt32Block(writer, value.Value);
            }
        }

        public UInt32? Deserialize(ref ReadOnlySequence<byte> byteSequence, IFormatterResolver formatterResolver)
        {
            if (MessagePackBinary.IsNil(byteSequence))
            {
                byteSequence = byteSequence.Slice(1);
                return null;
            }
            else
            {
                return MessagePackBinary.ReadUInt32(ref byteSequence);
            }
        }
    }

    public sealed class ForceUInt32BlockArrayFormatter : IMessagePackFormatter<UInt32[]>
    {
        public static readonly ForceUInt32BlockArrayFormatter Instance = new ForceUInt32BlockArrayFormatter();

        ForceUInt32BlockArrayFormatter()
        {

        }

        public void Serialize(IBufferWriter<byte> writer, UInt32[] value, IFormatterResolver formatterResolver)
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
                    MessagePackBinary.WriteUInt32ForceUInt32Block(writer, value[i]);
                }
            }
        }

        public UInt32[] Deserialize(ref ReadOnlySequence<byte> byteSequence, IFormatterResolver formatterResolver)
        {
            if (MessagePackBinary.IsNil(byteSequence))
            {
                byteSequence = byteSequence.Slice(1);
                return null;
            }
            else
            {
                var len = MessagePackBinary.ReadArrayHeader(ref byteSequence);
                var array = new UInt32[len];
                for (int i = 0; i < array.Length; i++)
                {
                    array[i] = MessagePackBinary.ReadUInt32(ref byteSequence);
                }
                return array;
            }
        }
    }

    public sealed class ForceUInt64BlockFormatter : IMessagePackFormatter<UInt64>
    {
        public static readonly ForceUInt64BlockFormatter Instance = new ForceUInt64BlockFormatter();

        ForceUInt64BlockFormatter()
        {
        }

        public void Serialize(IBufferWriter<byte> writer, UInt64 value, IFormatterResolver formatterResolver)
        {
            return MessagePackBinary.WriteUInt64ForceUInt64Block(writer, value);
        }

        public UInt64 Deserialize(ref ReadOnlySequence<byte> byteSequence, IFormatterResolver formatterResolver)
        {
            return MessagePackBinary.ReadUInt64(ref byteSequence);
        }
    }

    public sealed class NullableForceUInt64BlockFormatter : IMessagePackFormatter<UInt64?>
    {
        public static readonly NullableForceUInt64BlockFormatter Instance = new NullableForceUInt64BlockFormatter();

        NullableForceUInt64BlockFormatter()
        {
        }

        public void Serialize(IBufferWriter<byte> writer, UInt64? value, IFormatterResolver formatterResolver)
        {
            if (value == null)
            {
                MessagePackBinary.WriteNil(writer);
            }
            else
            {
                MessagePackBinary.WriteUInt64ForceUInt64Block(writer, value.Value);
            }
        }

        public UInt64? Deserialize(ref ReadOnlySequence<byte> byteSequence, IFormatterResolver formatterResolver)
        {
            if (MessagePackBinary.IsNil(byteSequence))
            {
                byteSequence = byteSequence.Slice(1);
                return null;
            }
            else
            {
                return MessagePackBinary.ReadUInt64(ref byteSequence);
            }
        }
    }

    public sealed class ForceUInt64BlockArrayFormatter : IMessagePackFormatter<UInt64[]>
    {
        public static readonly ForceUInt64BlockArrayFormatter Instance = new ForceUInt64BlockArrayFormatter();

        ForceUInt64BlockArrayFormatter()
        {

        }

        public void Serialize(IBufferWriter<byte> writer, UInt64[] value, IFormatterResolver formatterResolver)
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
                    MessagePackBinary.WriteUInt64ForceUInt64Block(writer, value[i]);
                }
            }
        }

        public UInt64[] Deserialize(ref ReadOnlySequence<byte> byteSequence, IFormatterResolver formatterResolver)
        {
            if (MessagePackBinary.IsNil(byteSequence))
            {
                byteSequence = byteSequence.Slice(1);
                return null;
            }
            else
            {
                var len = MessagePackBinary.ReadArrayHeader(ref byteSequence);
                var array = new UInt64[len];
                for (int i = 0; i < array.Length; i++)
                {
                    array[i] = MessagePackBinary.ReadUInt64(ref byteSequence);
                }
                return array;
            }
        }
    }

    public sealed class ForceByteBlockFormatter : IMessagePackFormatter<Byte>
    {
        public static readonly ForceByteBlockFormatter Instance = new ForceByteBlockFormatter();

        ForceByteBlockFormatter()
        {
        }

        public void Serialize(IBufferWriter<byte> writer, Byte value, IFormatterResolver formatterResolver)
        {
            return MessagePackBinary.WriteByteForceByteBlock(writer, value);
        }

        public Byte Deserialize(ref ReadOnlySequence<byte> byteSequence, IFormatterResolver formatterResolver)
        {
            return MessagePackBinary.ReadByte(ref byteSequence);
        }
    }

    public sealed class NullableForceByteBlockFormatter : IMessagePackFormatter<Byte?>
    {
        public static readonly NullableForceByteBlockFormatter Instance = new NullableForceByteBlockFormatter();

        NullableForceByteBlockFormatter()
        {
        }

        public void Serialize(IBufferWriter<byte> writer, Byte? value, IFormatterResolver formatterResolver)
        {
            if (value == null)
            {
                MessagePackBinary.WriteNil(writer);
            }
            else
            {
                MessagePackBinary.WriteByteForceByteBlock(writer, value.Value);
            }
        }

        public Byte? Deserialize(ref ReadOnlySequence<byte> byteSequence, IFormatterResolver formatterResolver)
        {
            if (MessagePackBinary.IsNil(byteSequence))
            {
                byteSequence = byteSequence.Slice(1);
                return null;
            }
            else
            {
                return MessagePackBinary.ReadByte(ref byteSequence);
            }
        }
    }


    public sealed class ForceSByteBlockFormatter : IMessagePackFormatter<SByte>
    {
        public static readonly ForceSByteBlockFormatter Instance = new ForceSByteBlockFormatter();

        ForceSByteBlockFormatter()
        {
        }

        public void Serialize(IBufferWriter<byte> writer, SByte value, IFormatterResolver formatterResolver)
        {
            return MessagePackBinary.WriteSByteForceSByteBlock(writer, value);
        }

        public SByte Deserialize(ref ReadOnlySequence<byte> byteSequence, IFormatterResolver formatterResolver)
        {
            return MessagePackBinary.ReadSByte(ref byteSequence);
        }
    }

    public sealed class NullableForceSByteBlockFormatter : IMessagePackFormatter<SByte?>
    {
        public static readonly NullableForceSByteBlockFormatter Instance = new NullableForceSByteBlockFormatter();

        NullableForceSByteBlockFormatter()
        {
        }

        public void Serialize(IBufferWriter<byte> writer, SByte? value, IFormatterResolver formatterResolver)
        {
            if (value == null)
            {
                MessagePackBinary.WriteNil(writer);
            }
            else
            {
                MessagePackBinary.WriteSByteForceSByteBlock(writer, value.Value);
            }
        }

        public SByte? Deserialize(ref ReadOnlySequence<byte> byteSequence, IFormatterResolver formatterResolver)
        {
            if (MessagePackBinary.IsNil(byteSequence))
            {
                byteSequence = byteSequence.Slice(1);
                return null;
            }
            else
            {
                return MessagePackBinary.ReadSByte(ref byteSequence);
            }
        }
    }

    public sealed class ForceSByteBlockArrayFormatter : IMessagePackFormatter<SByte[]>
    {
        public static readonly ForceSByteBlockArrayFormatter Instance = new ForceSByteBlockArrayFormatter();

        ForceSByteBlockArrayFormatter()
        {

        }

        public void Serialize(IBufferWriter<byte> writer, SByte[] value, IFormatterResolver formatterResolver)
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
                    MessagePackBinary.WriteSByteForceSByteBlock(writer, value[i]);
                }
            }
        }

        public SByte[] Deserialize(ref ReadOnlySequence<byte> byteSequence, IFormatterResolver formatterResolver)
        {
            if (MessagePackBinary.IsNil(byteSequence))
            {
                byteSequence = byteSequence.Slice(1);
                return null;
            }
            else
            {
                var len = MessagePackBinary.ReadArrayHeader(ref byteSequence);
                var array = new SByte[len];
                for (int i = 0; i < array.Length; i++)
                {
                    array[i] = MessagePackBinary.ReadSByte(ref byteSequence);
                }
                return array;
            }
        }
    }

}