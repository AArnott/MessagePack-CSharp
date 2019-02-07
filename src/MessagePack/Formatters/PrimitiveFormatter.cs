using System;
using System.Buffers;

namespace MessagePack.Formatters
{
    public sealed class Int16Formatter : IMessagePackFormatter<Int16>
    {
        public static readonly Int16Formatter Instance = new Int16Formatter();

        Int16Formatter()
        {
        }

        public void Serialize(ref BufferWriter writer, Int16 value, IFormatterResolver formatterResolver)
        {
            MessagePackBinary.WriteInt16(ref writer, value);
        }

        public Int16 Deserialize(ref ReadOnlySequence<byte> byteSequence, IFormatterResolver formatterResolver)
        {
            return MessagePackBinary.ReadInt16(ref byteSequence);
        }
    }

    public sealed class NullableInt16Formatter : IMessagePackFormatter<Int16?>
    {
        public static readonly NullableInt16Formatter Instance = new NullableInt16Formatter();

        NullableInt16Formatter()
        {
        }

        public void Serialize(ref BufferWriter writer, Int16? value, IFormatterResolver formatterResolver)
        {
            if (value == null)
            {
                MessagePackBinary.WriteNil(ref writer);
            }
            else
            {
                MessagePackBinary.WriteInt16(ref writer, value.Value);
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

    public sealed class Int16ArrayFormatter : IMessagePackFormatter<Int16[]>
    {
        public static readonly Int16ArrayFormatter Instance = new Int16ArrayFormatter();

        Int16ArrayFormatter()
        {

        }

        public void Serialize(ref BufferWriter writer, Int16[] value, IFormatterResolver formatterResolver)
        {
            if (value == null)
            {
                MessagePackBinary.WriteNil(ref writer);
            }
            else
            {
                MessagePackBinary.WriteArrayHeader(ref writer, value.Length);
                for (int i = 0; i < value.Length; i++)
                {
                    MessagePackBinary.WriteInt16(ref writer, value[i]);
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

    public sealed class Int32Formatter : IMessagePackFormatter<Int32>
    {
        public static readonly Int32Formatter Instance = new Int32Formatter();

        Int32Formatter()
        {
        }

        public void Serialize(ref BufferWriter writer, Int32 value, IFormatterResolver formatterResolver)
        {
            MessagePackBinary.WriteInt32(ref writer, value);
        }

        public Int32 Deserialize(ref ReadOnlySequence<byte> byteSequence, IFormatterResolver formatterResolver)
        {
            return MessagePackBinary.ReadInt32(ref byteSequence);
        }
    }

    public sealed class NullableInt32Formatter : IMessagePackFormatter<Int32?>
    {
        public static readonly NullableInt32Formatter Instance = new NullableInt32Formatter();

        NullableInt32Formatter()
        {
        }

        public void Serialize(ref BufferWriter writer, Int32? value, IFormatterResolver formatterResolver)
        {
            if (value == null)
            {
                MessagePackBinary.WriteNil(ref writer);
            }
            else
            {
                MessagePackBinary.WriteInt32(ref writer, value.Value);
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

    public sealed class Int32ArrayFormatter : IMessagePackFormatter<Int32[]>
    {
        public static readonly Int32ArrayFormatter Instance = new Int32ArrayFormatter();

        Int32ArrayFormatter()
        {

        }

        public void Serialize(ref BufferWriter writer, Int32[] value, IFormatterResolver formatterResolver)
        {
            if (value == null)
            {
                MessagePackBinary.WriteNil(ref writer);
            }
            else
            {
                MessagePackBinary.WriteArrayHeader(ref writer, value.Length);
                for (int i = 0; i < value.Length; i++)
                {
                    MessagePackBinary.WriteInt32(ref writer, value[i]);
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

    public sealed class Int64Formatter : IMessagePackFormatter<Int64>
    {
        public static readonly Int64Formatter Instance = new Int64Formatter();

        Int64Formatter()
        {
        }

        public void Serialize(ref BufferWriter writer, Int64 value, IFormatterResolver formatterResolver)
        {
            MessagePackBinary.WriteInt64(ref writer, value);
        }

        public Int64 Deserialize(ref ReadOnlySequence<byte> byteSequence, IFormatterResolver formatterResolver)
        {
            return MessagePackBinary.ReadInt64(ref byteSequence);
        }
    }

    public sealed class NullableInt64Formatter : IMessagePackFormatter<Int64?>
    {
        public static readonly NullableInt64Formatter Instance = new NullableInt64Formatter();

        NullableInt64Formatter()
        {
        }

        public void Serialize(ref BufferWriter writer, Int64? value, IFormatterResolver formatterResolver)
        {
            if (value == null)
            {
                MessagePackBinary.WriteNil(ref writer);
            }
            else
            {
                MessagePackBinary.WriteInt64(ref writer, value.Value);
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

    public sealed class Int64ArrayFormatter : IMessagePackFormatter<Int64[]>
    {
        public static readonly Int64ArrayFormatter Instance = new Int64ArrayFormatter();

        Int64ArrayFormatter()
        {

        }

        public void Serialize(ref BufferWriter writer, Int64[] value, IFormatterResolver formatterResolver)
        {
            if (value == null)
            {
                MessagePackBinary.WriteNil(ref writer);
            }
            else
            {
                MessagePackBinary.WriteArrayHeader(ref writer, value.Length);
                for (int i = 0; i < value.Length; i++)
                {
                    MessagePackBinary.WriteInt64(ref writer, value[i]);
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

    public sealed class UInt16Formatter : IMessagePackFormatter<UInt16>
    {
        public static readonly UInt16Formatter Instance = new UInt16Formatter();

        UInt16Formatter()
        {
        }

        public void Serialize(ref BufferWriter writer, UInt16 value, IFormatterResolver formatterResolver)
        {
            MessagePackBinary.WriteUInt16(ref writer, value);
        }

        public UInt16 Deserialize(ref ReadOnlySequence<byte> byteSequence, IFormatterResolver formatterResolver)
        {
            return MessagePackBinary.ReadUInt16(ref byteSequence);
        }
    }

    public sealed class NullableUInt16Formatter : IMessagePackFormatter<UInt16?>
    {
        public static readonly NullableUInt16Formatter Instance = new NullableUInt16Formatter();

        NullableUInt16Formatter()
        {
        }

        public void Serialize(ref BufferWriter writer, UInt16? value, IFormatterResolver formatterResolver)
        {
            if (value == null)
            {
                MessagePackBinary.WriteNil(ref writer);
            }
            else
            {
                MessagePackBinary.WriteUInt16(ref writer, value.Value);
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

    public sealed class UInt16ArrayFormatter : IMessagePackFormatter<UInt16[]>
    {
        public static readonly UInt16ArrayFormatter Instance = new UInt16ArrayFormatter();

        UInt16ArrayFormatter()
        {

        }

        public void Serialize(ref BufferWriter writer, UInt16[] value, IFormatterResolver formatterResolver)
        {
            if (value == null)
            {
                MessagePackBinary.WriteNil(ref writer);
            }
            else
            {
                MessagePackBinary.WriteArrayHeader(ref writer, value.Length);
                for (int i = 0; i < value.Length; i++)
                {
                    MessagePackBinary.WriteUInt16(ref writer, value[i]);
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

    public sealed class UInt32Formatter : IMessagePackFormatter<UInt32>
    {
        public static readonly UInt32Formatter Instance = new UInt32Formatter();

        UInt32Formatter()
        {
        }

        public void Serialize(ref BufferWriter writer, UInt32 value, IFormatterResolver formatterResolver)
        {
            MessagePackBinary.WriteUInt32(ref writer, value);
        }

        public UInt32 Deserialize(ref ReadOnlySequence<byte> byteSequence, IFormatterResolver formatterResolver)
        {
            return MessagePackBinary.ReadUInt32(ref byteSequence);
        }
    }

    public sealed class NullableUInt32Formatter : IMessagePackFormatter<UInt32?>
    {
        public static readonly NullableUInt32Formatter Instance = new NullableUInt32Formatter();

        NullableUInt32Formatter()
        {
        }

        public void Serialize(ref BufferWriter writer, UInt32? value, IFormatterResolver formatterResolver)
        {
            if (value == null)
            {
                MessagePackBinary.WriteNil(ref writer);
            }
            else
            {
                MessagePackBinary.WriteUInt32(ref writer, value.Value);
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

    public sealed class UInt32ArrayFormatter : IMessagePackFormatter<UInt32[]>
    {
        public static readonly UInt32ArrayFormatter Instance = new UInt32ArrayFormatter();

        UInt32ArrayFormatter()
        {

        }

        public void Serialize(ref BufferWriter writer, UInt32[] value, IFormatterResolver formatterResolver)
        {
            if (value == null)
            {
                MessagePackBinary.WriteNil(ref writer);
            }
            else
            {
                MessagePackBinary.WriteArrayHeader(ref writer, value.Length);
                for (int i = 0; i < value.Length; i++)
                {
                    MessagePackBinary.WriteUInt32(ref writer, value[i]);
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

    public sealed class UInt64Formatter : IMessagePackFormatter<UInt64>
    {
        public static readonly UInt64Formatter Instance = new UInt64Formatter();

        UInt64Formatter()
        {
        }

        public void Serialize(ref BufferWriter writer, UInt64 value, IFormatterResolver formatterResolver)
        {
            MessagePackBinary.WriteUInt64(ref writer, value);
        }

        public UInt64 Deserialize(ref ReadOnlySequence<byte> byteSequence, IFormatterResolver formatterResolver)
        {
            return MessagePackBinary.ReadUInt64(ref byteSequence);
        }
    }

    public sealed class NullableUInt64Formatter : IMessagePackFormatter<UInt64?>
    {
        public static readonly NullableUInt64Formatter Instance = new NullableUInt64Formatter();

        NullableUInt64Formatter()
        {
        }

        public void Serialize(ref BufferWriter writer, UInt64? value, IFormatterResolver formatterResolver)
        {
            if (value == null)
            {
                MessagePackBinary.WriteNil(ref writer);
            }
            else
            {
                MessagePackBinary.WriteUInt64(ref writer, value.Value);
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

    public sealed class UInt64ArrayFormatter : IMessagePackFormatter<UInt64[]>
    {
        public static readonly UInt64ArrayFormatter Instance = new UInt64ArrayFormatter();

        UInt64ArrayFormatter()
        {

        }

        public void Serialize(ref BufferWriter writer, UInt64[] value, IFormatterResolver formatterResolver)
        {
            if (value == null)
            {
                MessagePackBinary.WriteNil(ref writer);
            }
            else
            {
                MessagePackBinary.WriteArrayHeader(ref writer, value.Length);
                for (int i = 0; i < value.Length; i++)
                {
                    MessagePackBinary.WriteUInt64(ref writer, value[i]);
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

    public sealed class SingleFormatter : IMessagePackFormatter<Single>
    {
        public static readonly SingleFormatter Instance = new SingleFormatter();

        SingleFormatter()
        {
        }

        public void Serialize(ref BufferWriter writer, Single value, IFormatterResolver formatterResolver)
        {
            MessagePackBinary.WriteSingle(ref writer, value);
        }

        public Single Deserialize(ref ReadOnlySequence<byte> byteSequence, IFormatterResolver formatterResolver)
        {
            return MessagePackBinary.ReadSingle(ref byteSequence);
        }
    }

    public sealed class NullableSingleFormatter : IMessagePackFormatter<Single?>
    {
        public static readonly NullableSingleFormatter Instance = new NullableSingleFormatter();

        NullableSingleFormatter()
        {
        }

        public void Serialize(ref BufferWriter writer, Single? value, IFormatterResolver formatterResolver)
        {
            if (value == null)
            {
                MessagePackBinary.WriteNil(ref writer);
            }
            else
            {
                MessagePackBinary.WriteSingle(ref writer, value.Value);
            }
        }

        public Single? Deserialize(ref ReadOnlySequence<byte> byteSequence, IFormatterResolver formatterResolver)
        {
            if (MessagePackBinary.IsNil(byteSequence))
            {
                byteSequence = byteSequence.Slice(1);
                return null;
            }
            else
            {
                return MessagePackBinary.ReadSingle(ref byteSequence);
            }
        }
    }

    public sealed class SingleArrayFormatter : IMessagePackFormatter<Single[]>
    {
        public static readonly SingleArrayFormatter Instance = new SingleArrayFormatter();

        SingleArrayFormatter()
        {

        }

        public void Serialize(ref BufferWriter writer, Single[] value, IFormatterResolver formatterResolver)
        {
            if (value == null)
            {
                MessagePackBinary.WriteNil(ref writer);
            }
            else
            {
                MessagePackBinary.WriteArrayHeader(ref writer, value.Length);
                for (int i = 0; i < value.Length; i++)
                {
                    MessagePackBinary.WriteSingle(ref writer, value[i]);
                }
            }
        }

        public Single[] Deserialize(ref ReadOnlySequence<byte> byteSequence, IFormatterResolver formatterResolver)
        {
            if (MessagePackBinary.IsNil(byteSequence))
            {
                byteSequence = byteSequence.Slice(1);
                return null;
            }
            else
            {
                var len = MessagePackBinary.ReadArrayHeader(ref byteSequence);
                var array = new Single[len];
                for (int i = 0; i < array.Length; i++)
                {
                    array[i] = MessagePackBinary.ReadSingle(ref byteSequence);
                }
                return array;
            }
        }
    }

    public sealed class DoubleFormatter : IMessagePackFormatter<Double>
    {
        public static readonly DoubleFormatter Instance = new DoubleFormatter();

        DoubleFormatter()
        {
        }

        public void Serialize(ref BufferWriter writer, Double value, IFormatterResolver formatterResolver)
        {
            MessagePackBinary.WriteDouble(ref writer, value);
        }

        public Double Deserialize(ref ReadOnlySequence<byte> byteSequence, IFormatterResolver formatterResolver)
        {
            return MessagePackBinary.ReadDouble(ref byteSequence);
        }
    }

    public sealed class NullableDoubleFormatter : IMessagePackFormatter<Double?>
    {
        public static readonly NullableDoubleFormatter Instance = new NullableDoubleFormatter();

        NullableDoubleFormatter()
        {
        }

        public void Serialize(ref BufferWriter writer, Double? value, IFormatterResolver formatterResolver)
        {
            if (value == null)
            {
                MessagePackBinary.WriteNil(ref writer);
            }
            else
            {
                MessagePackBinary.WriteDouble(ref writer, value.Value);
            }
        }

        public Double? Deserialize(ref ReadOnlySequence<byte> byteSequence, IFormatterResolver formatterResolver)
        {
            if (MessagePackBinary.IsNil(byteSequence))
            {
                byteSequence = byteSequence.Slice(1);
                return null;
            }
            else
            {
                return MessagePackBinary.ReadDouble(ref byteSequence);
            }
        }
    }

    public sealed class DoubleArrayFormatter : IMessagePackFormatter<Double[]>
    {
        public static readonly DoubleArrayFormatter Instance = new DoubleArrayFormatter();

        DoubleArrayFormatter()
        {

        }

        public void Serialize(ref BufferWriter writer, Double[] value, IFormatterResolver formatterResolver)
        {
            if (value == null)
            {
                MessagePackBinary.WriteNil(ref writer);
            }
            else
            {
                MessagePackBinary.WriteArrayHeader(ref writer, value.Length);
                for (int i = 0; i < value.Length; i++)
                {
                    MessagePackBinary.WriteDouble(ref writer, value[i]);
                }
            }
        }

        public Double[] Deserialize(ref ReadOnlySequence<byte> byteSequence, IFormatterResolver formatterResolver)
        {
            if (MessagePackBinary.IsNil(byteSequence))
            {
                byteSequence = byteSequence.Slice(1);
                return null;
            }
            else
            {
                var len = MessagePackBinary.ReadArrayHeader(ref byteSequence);
                var array = new Double[len];
                for (int i = 0; i < array.Length; i++)
                {
                    array[i] = MessagePackBinary.ReadDouble(ref byteSequence);
                }
                return array;
            }
        }
    }

    public sealed class BooleanFormatter : IMessagePackFormatter<Boolean>
    {
        public static readonly BooleanFormatter Instance = new BooleanFormatter();

        BooleanFormatter()
        {
        }

        public void Serialize(ref BufferWriter writer, Boolean value, IFormatterResolver formatterResolver)
        {
            MessagePackBinary.WriteBoolean(ref writer, value);
        }

        public Boolean Deserialize(ref ReadOnlySequence<byte> byteSequence, IFormatterResolver formatterResolver)
        {
            return MessagePackBinary.ReadBoolean(ref byteSequence);
        }
    }

    public sealed class NullableBooleanFormatter : IMessagePackFormatter<Boolean?>
    {
        public static readonly NullableBooleanFormatter Instance = new NullableBooleanFormatter();

        NullableBooleanFormatter()
        {
        }

        public void Serialize(ref BufferWriter writer, Boolean? value, IFormatterResolver formatterResolver)
        {
            if (value == null)
            {
                MessagePackBinary.WriteNil(ref writer);
            }
            else
            {
                MessagePackBinary.WriteBoolean(ref writer, value.Value);
            }
        }

        public Boolean? Deserialize(ref ReadOnlySequence<byte> byteSequence, IFormatterResolver formatterResolver)
        {
            if (MessagePackBinary.IsNil(byteSequence))
            {
                byteSequence = byteSequence.Slice(1);
                return null;
            }
            else
            {
                return MessagePackBinary.ReadBoolean(ref byteSequence);
            }
        }
    }

    public sealed class BooleanArrayFormatter : IMessagePackFormatter<Boolean[]>
    {
        public static readonly BooleanArrayFormatter Instance = new BooleanArrayFormatter();

        BooleanArrayFormatter()
        {

        }

        public void Serialize(ref BufferWriter writer, Boolean[] value, IFormatterResolver formatterResolver)
        {
            if (value == null)
            {
                MessagePackBinary.WriteNil(ref writer);
            }
            else
            {
                MessagePackBinary.WriteArrayHeader(ref writer, value.Length);
                for (int i = 0; i < value.Length; i++)
                {
                    MessagePackBinary.WriteBoolean(ref writer, value[i]);
                }
            }
        }

        public Boolean[] Deserialize(ref ReadOnlySequence<byte> byteSequence, IFormatterResolver formatterResolver)
        {
            if (MessagePackBinary.IsNil(byteSequence))
            {
                byteSequence = byteSequence.Slice(1);
                return null;
            }
            else
            {
                var len = MessagePackBinary.ReadArrayHeader(ref byteSequence);
                var array = new Boolean[len];
                for (int i = 0; i < array.Length; i++)
                {
                    array[i] = MessagePackBinary.ReadBoolean(ref byteSequence);
                }
                return array;
            }
        }
    }

    public sealed class ByteFormatter : IMessagePackFormatter<Byte>
    {
        public static readonly ByteFormatter Instance = new ByteFormatter();

        ByteFormatter()
        {
        }

        public void Serialize(ref BufferWriter writer, Byte value, IFormatterResolver formatterResolver)
        {
            MessagePackBinary.WriteByte(ref writer, value);
        }

        public Byte Deserialize(ref ReadOnlySequence<byte> byteSequence, IFormatterResolver formatterResolver)
        {
            return MessagePackBinary.ReadByte(ref byteSequence);
        }
    }

    public sealed class NullableByteFormatter : IMessagePackFormatter<Byte?>
    {
        public static readonly NullableByteFormatter Instance = new NullableByteFormatter();

        NullableByteFormatter()
        {
        }

        public void Serialize(ref BufferWriter writer, Byte? value, IFormatterResolver formatterResolver)
        {
            if (value == null)
            {
                MessagePackBinary.WriteNil(ref writer);
            }
            else
            {
                MessagePackBinary.WriteByte(ref writer, value.Value);
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


    public sealed class SByteFormatter : IMessagePackFormatter<SByte>
    {
        public static readonly SByteFormatter Instance = new SByteFormatter();

        SByteFormatter()
        {
        }

        public void Serialize(ref BufferWriter writer, SByte value, IFormatterResolver formatterResolver)
        {
            MessagePackBinary.WriteSByte(ref writer, value);
        }

        public SByte Deserialize(ref ReadOnlySequence<byte> byteSequence, IFormatterResolver formatterResolver)
        {
            return MessagePackBinary.ReadSByte(ref byteSequence);
        }
    }

    public sealed class NullableSByteFormatter : IMessagePackFormatter<SByte?>
    {
        public static readonly NullableSByteFormatter Instance = new NullableSByteFormatter();

        NullableSByteFormatter()
        {
        }

        public void Serialize(ref BufferWriter writer, SByte? value, IFormatterResolver formatterResolver)
        {
            if (value == null)
            {
                MessagePackBinary.WriteNil(ref writer);
            }
            else
            {
                MessagePackBinary.WriteSByte(ref writer, value.Value);
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

    public sealed class SByteArrayFormatter : IMessagePackFormatter<SByte[]>
    {
        public static readonly SByteArrayFormatter Instance = new SByteArrayFormatter();

        SByteArrayFormatter()
        {

        }

        public void Serialize(ref BufferWriter writer, SByte[] value, IFormatterResolver formatterResolver)
        {
            if (value == null)
            {
                MessagePackBinary.WriteNil(ref writer);
            }
            else
            {
                MessagePackBinary.WriteArrayHeader(ref writer, value.Length);
                for (int i = 0; i < value.Length; i++)
                {
                    MessagePackBinary.WriteSByte(ref writer, value[i]);
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

    public sealed class CharFormatter : IMessagePackFormatter<Char>
    {
        public static readonly CharFormatter Instance = new CharFormatter();

        CharFormatter()
        {
        }

        public void Serialize(ref BufferWriter writer, Char value, IFormatterResolver formatterResolver)
        {
            MessagePackBinary.WriteChar(ref writer, value);
        }

        public Char Deserialize(ref ReadOnlySequence<byte> byteSequence, IFormatterResolver formatterResolver)
        {
            return MessagePackBinary.ReadChar(ref byteSequence);
        }
    }

    public sealed class NullableCharFormatter : IMessagePackFormatter<Char?>
    {
        public static readonly NullableCharFormatter Instance = new NullableCharFormatter();

        NullableCharFormatter()
        {
        }

        public void Serialize(ref BufferWriter writer, Char? value, IFormatterResolver formatterResolver)
        {
            if (value == null)
            {
                MessagePackBinary.WriteNil(ref writer);
            }
            else
            {
                MessagePackBinary.WriteChar(ref writer, value.Value);
            }
        }

        public Char? Deserialize(ref ReadOnlySequence<byte> byteSequence, IFormatterResolver formatterResolver)
        {
            if (MessagePackBinary.IsNil(byteSequence))
            {
                byteSequence = byteSequence.Slice(1);
                return null;
            }
            else
            {
                return MessagePackBinary.ReadChar(ref byteSequence);
            }
        }
    }

    public sealed class CharArrayFormatter : IMessagePackFormatter<Char[]>
    {
        public static readonly CharArrayFormatter Instance = new CharArrayFormatter();

        CharArrayFormatter()
        {

        }

        public void Serialize(ref BufferWriter writer, Char[] value, IFormatterResolver formatterResolver)
        {
            if (value == null)
            {
                MessagePackBinary.WriteNil(ref writer);
            }
            else
            {
                MessagePackBinary.WriteArrayHeader(ref writer, value.Length);
                for (int i = 0; i < value.Length; i++)
                {
                    MessagePackBinary.WriteChar(ref writer, value[i]);
                }
            }
        }

        public Char[] Deserialize(ref ReadOnlySequence<byte> byteSequence, IFormatterResolver formatterResolver)
        {
            if (MessagePackBinary.IsNil(byteSequence))
            {
                byteSequence = byteSequence.Slice(1);
                return null;
            }
            else
            {
                var len = MessagePackBinary.ReadArrayHeader(ref byteSequence);
                var array = new Char[len];
                for (int i = 0; i < array.Length; i++)
                {
                    array[i] = MessagePackBinary.ReadChar(ref byteSequence);
                }
                return array;
            }
        }
    }

    public sealed class DateTimeFormatter : IMessagePackFormatter<DateTime>
    {
        public static readonly DateTimeFormatter Instance = new DateTimeFormatter();

        DateTimeFormatter()
        {
        }

        public void Serialize(ref BufferWriter writer, DateTime value, IFormatterResolver formatterResolver)
        {
            MessagePackBinary.WriteDateTime(ref writer, value);
        }

        public DateTime Deserialize(ref ReadOnlySequence<byte> byteSequence, IFormatterResolver formatterResolver)
        {
            return MessagePackBinary.ReadDateTime(ref byteSequence);
        }
    }

    public sealed class NullableDateTimeFormatter : IMessagePackFormatter<DateTime?>
    {
        public static readonly NullableDateTimeFormatter Instance = new NullableDateTimeFormatter();

        NullableDateTimeFormatter()
        {
        }

        public void Serialize(ref BufferWriter writer, DateTime? value, IFormatterResolver formatterResolver)
        {
            if (value == null)
            {
                MessagePackBinary.WriteNil(ref writer);
            }
            else
            {
                MessagePackBinary.WriteDateTime(ref writer, value.Value);
            }
        }

        public DateTime? Deserialize(ref ReadOnlySequence<byte> byteSequence, IFormatterResolver formatterResolver)
        {
            if (MessagePackBinary.IsNil(byteSequence))
            {
                byteSequence = byteSequence.Slice(1);
                return null;
            }
            else
            {
                return MessagePackBinary.ReadDateTime(ref byteSequence);
            }
        }
    }

    public sealed class DateTimeArrayFormatter : IMessagePackFormatter<DateTime[]>
    {
        public static readonly DateTimeArrayFormatter Instance = new DateTimeArrayFormatter();

        DateTimeArrayFormatter()
        {

        }

        public void Serialize(ref BufferWriter writer, DateTime[] value, IFormatterResolver formatterResolver)
        {
            if (value == null)
            {
                MessagePackBinary.WriteNil(ref writer);
            }
            else
            {
                MessagePackBinary.WriteArrayHeader(ref writer, value.Length);
                for (int i = 0; i < value.Length; i++)
                {
                    MessagePackBinary.WriteDateTime(ref writer, value[i]);
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
                    array[i] = MessagePackBinary.ReadDateTime(ref byteSequence);
                }
                return array;
            }
        }
    }

}
