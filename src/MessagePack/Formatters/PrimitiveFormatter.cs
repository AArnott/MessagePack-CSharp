using System;

namespace MessagePack.Formatters
{
    public sealed class Int16Formatter : IMessagePackFormatter<Int16>
    {
        public static readonly Int16Formatter Instance = new Int16Formatter();

        Int16Formatter()
        {
        }

        public void Serialize(IBufferWriter<byte> writer, Int16 value, IFormatterResolver formatterResolver)
        {
            return MessagePackBinary.WriteInt16(ref bytes, offset, value);
        }

        public Int16 Deserialize(ref ReadOnlySequence<byte> byteSequence, IFormatterResolver formatterResolver)
        {
            return MessagePackBinary.ReadInt16(bytes, offset, out readSize);
        }
    }

    public sealed class NullableInt16Formatter : IMessagePackFormatter<Int16?>
    {
        public static readonly NullableInt16Formatter Instance = new NullableInt16Formatter();

        NullableInt16Formatter()
        {
        }

        public void Serialize(IBufferWriter<byte> writer, Int16? value, IFormatterResolver formatterResolver)
        {
            if (value == null)
            {
                return MessagePackBinary.WriteNil(ref bytes, offset);
            }
            else
            {
                return MessagePackBinary.WriteInt16(ref bytes, offset, value.Value);
            }
        }

        public Int16? Deserialize(ref ReadOnlySequence<byte> byteSequence, IFormatterResolver formatterResolver)
        {
            if (MessagePackBinary.IsNil(bytes, offset))
            {
                readSize = 1;
                return null;
            }
            else
            {
                return MessagePackBinary.ReadInt16(bytes, offset, out readSize);
            }
        }
    }

    public sealed class Int16ArrayFormatter : IMessagePackFormatter<Int16[]>
    {
        public static readonly Int16ArrayFormatter Instance = new Int16ArrayFormatter();

        Int16ArrayFormatter()
        {

        }

        public void Serialize(IBufferWriter<byte> writer, Int16[] value, IFormatterResolver formatterResolver)
        {
            if (value == null)
            {
                return MessagePackBinary.WriteNil(ref bytes, offset);
            }
            else
            {
                var startOffset = offset;
                offset += MessagePackBinary.WriteArrayHeader(ref bytes, offset, value.Length);
                for (int i = 0; i < value.Length; i++)
                {
                    offset += MessagePackBinary.WriteInt16(ref bytes, offset, value[i]);
                }

                return offset - startOffset;
            }
        }

        public Int16[] Deserialize(ref ReadOnlySequence<byte> byteSequence, IFormatterResolver formatterResolver)
        {
            if (MessagePackBinary.IsNil(bytes, offset))
            {
                readSize = 1;
                return null;
            }
            else
            {
                var startOffset = offset;

                var len = MessagePackBinary.ReadArrayHeader(bytes, offset, out readSize);
                offset += readSize;
                var array = new Int16[len];
                for (int i = 0; i < array.Length; i++)
                {
                    array[i] = MessagePackBinary.ReadInt16(bytes, offset, out readSize);
                    offset += readSize;
                }
                readSize = offset - startOffset;
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

        public void Serialize(IBufferWriter<byte> writer, Int32 value, IFormatterResolver formatterResolver)
        {
            return MessagePackBinary.WriteInt32(ref bytes, offset, value);
        }

        public Int32 Deserialize(ref ReadOnlySequence<byte> byteSequence, IFormatterResolver formatterResolver)
        {
            return MessagePackBinary.ReadInt32(bytes, offset, out readSize);
        }
    }

    public sealed class NullableInt32Formatter : IMessagePackFormatter<Int32?>
    {
        public static readonly NullableInt32Formatter Instance = new NullableInt32Formatter();

        NullableInt32Formatter()
        {
        }

        public void Serialize(IBufferWriter<byte> writer, Int32? value, IFormatterResolver formatterResolver)
        {
            if (value == null)
            {
                return MessagePackBinary.WriteNil(ref bytes, offset);
            }
            else
            {
                return MessagePackBinary.WriteInt32(ref bytes, offset, value.Value);
            }
        }

        public Int32? Deserialize(ref ReadOnlySequence<byte> byteSequence, IFormatterResolver formatterResolver)
        {
            if (MessagePackBinary.IsNil(bytes, offset))
            {
                readSize = 1;
                return null;
            }
            else
            {
                return MessagePackBinary.ReadInt32(bytes, offset, out readSize);
            }
        }
    }

    public sealed class Int32ArrayFormatter : IMessagePackFormatter<Int32[]>
    {
        public static readonly Int32ArrayFormatter Instance = new Int32ArrayFormatter();

        Int32ArrayFormatter()
        {

        }

        public void Serialize(IBufferWriter<byte> writer, Int32[] value, IFormatterResolver formatterResolver)
        {
            if (value == null)
            {
                return MessagePackBinary.WriteNil(ref bytes, offset);
            }
            else
            {
                var startOffset = offset;
                offset += MessagePackBinary.WriteArrayHeader(ref bytes, offset, value.Length);
                for (int i = 0; i < value.Length; i++)
                {
                    offset += MessagePackBinary.WriteInt32(ref bytes, offset, value[i]);
                }

                return offset - startOffset;
            }
        }

        public Int32[] Deserialize(ref ReadOnlySequence<byte> byteSequence, IFormatterResolver formatterResolver)
        {
            if (MessagePackBinary.IsNil(bytes, offset))
            {
                readSize = 1;
                return null;
            }
            else
            {
                var startOffset = offset;

                var len = MessagePackBinary.ReadArrayHeader(bytes, offset, out readSize);
                offset += readSize;
                var array = new Int32[len];
                for (int i = 0; i < array.Length; i++)
                {
                    array[i] = MessagePackBinary.ReadInt32(bytes, offset, out readSize);
                    offset += readSize;
                }
                readSize = offset - startOffset;
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

        public void Serialize(IBufferWriter<byte> writer, Int64 value, IFormatterResolver formatterResolver)
        {
            return MessagePackBinary.WriteInt64(ref bytes, offset, value);
        }

        public Int64 Deserialize(ref ReadOnlySequence<byte> byteSequence, IFormatterResolver formatterResolver)
        {
            return MessagePackBinary.ReadInt64(bytes, offset, out readSize);
        }
    }

    public sealed class NullableInt64Formatter : IMessagePackFormatter<Int64?>
    {
        public static readonly NullableInt64Formatter Instance = new NullableInt64Formatter();

        NullableInt64Formatter()
        {
        }

        public void Serialize(IBufferWriter<byte> writer, Int64? value, IFormatterResolver formatterResolver)
        {
            if (value == null)
            {
                return MessagePackBinary.WriteNil(ref bytes, offset);
            }
            else
            {
                return MessagePackBinary.WriteInt64(ref bytes, offset, value.Value);
            }
        }

        public Int64? Deserialize(ref ReadOnlySequence<byte> byteSequence, IFormatterResolver formatterResolver)
        {
            if (MessagePackBinary.IsNil(bytes, offset))
            {
                readSize = 1;
                return null;
            }
            else
            {
                return MessagePackBinary.ReadInt64(bytes, offset, out readSize);
            }
        }
    }

    public sealed class Int64ArrayFormatter : IMessagePackFormatter<Int64[]>
    {
        public static readonly Int64ArrayFormatter Instance = new Int64ArrayFormatter();

        Int64ArrayFormatter()
        {

        }

        public void Serialize(IBufferWriter<byte> writer, Int64[] value, IFormatterResolver formatterResolver)
        {
            if (value == null)
            {
                return MessagePackBinary.WriteNil(ref bytes, offset);
            }
            else
            {
                var startOffset = offset;
                offset += MessagePackBinary.WriteArrayHeader(ref bytes, offset, value.Length);
                for (int i = 0; i < value.Length; i++)
                {
                    offset += MessagePackBinary.WriteInt64(ref bytes, offset, value[i]);
                }

                return offset - startOffset;
            }
        }

        public Int64[] Deserialize(ref ReadOnlySequence<byte> byteSequence, IFormatterResolver formatterResolver)
        {
            if (MessagePackBinary.IsNil(bytes, offset))
            {
                readSize = 1;
                return null;
            }
            else
            {
                var startOffset = offset;

                var len = MessagePackBinary.ReadArrayHeader(bytes, offset, out readSize);
                offset += readSize;
                var array = new Int64[len];
                for (int i = 0; i < array.Length; i++)
                {
                    array[i] = MessagePackBinary.ReadInt64(bytes, offset, out readSize);
                    offset += readSize;
                }
                readSize = offset - startOffset;
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

        public void Serialize(IBufferWriter<byte> writer, UInt16 value, IFormatterResolver formatterResolver)
        {
            return MessagePackBinary.WriteUInt16(ref bytes, offset, value);
        }

        public UInt16 Deserialize(ref ReadOnlySequence<byte> byteSequence, IFormatterResolver formatterResolver)
        {
            return MessagePackBinary.ReadUInt16(bytes, offset, out readSize);
        }
    }

    public sealed class NullableUInt16Formatter : IMessagePackFormatter<UInt16?>
    {
        public static readonly NullableUInt16Formatter Instance = new NullableUInt16Formatter();

        NullableUInt16Formatter()
        {
        }

        public void Serialize(IBufferWriter<byte> writer, UInt16? value, IFormatterResolver formatterResolver)
        {
            if (value == null)
            {
                return MessagePackBinary.WriteNil(ref bytes, offset);
            }
            else
            {
                return MessagePackBinary.WriteUInt16(ref bytes, offset, value.Value);
            }
        }

        public UInt16? Deserialize(ref ReadOnlySequence<byte> byteSequence, IFormatterResolver formatterResolver)
        {
            if (MessagePackBinary.IsNil(bytes, offset))
            {
                readSize = 1;
                return null;
            }
            else
            {
                return MessagePackBinary.ReadUInt16(bytes, offset, out readSize);
            }
        }
    }

    public sealed class UInt16ArrayFormatter : IMessagePackFormatter<UInt16[]>
    {
        public static readonly UInt16ArrayFormatter Instance = new UInt16ArrayFormatter();

        UInt16ArrayFormatter()
        {

        }

        public void Serialize(IBufferWriter<byte> writer, UInt16[] value, IFormatterResolver formatterResolver)
        {
            if (value == null)
            {
                return MessagePackBinary.WriteNil(ref bytes, offset);
            }
            else
            {
                var startOffset = offset;
                offset += MessagePackBinary.WriteArrayHeader(ref bytes, offset, value.Length);
                for (int i = 0; i < value.Length; i++)
                {
                    offset += MessagePackBinary.WriteUInt16(ref bytes, offset, value[i]);
                }

                return offset - startOffset;
            }
        }

        public UInt16[] Deserialize(ref ReadOnlySequence<byte> byteSequence, IFormatterResolver formatterResolver)
        {
            if (MessagePackBinary.IsNil(bytes, offset))
            {
                readSize = 1;
                return null;
            }
            else
            {
                var startOffset = offset;

                var len = MessagePackBinary.ReadArrayHeader(bytes, offset, out readSize);
                offset += readSize;
                var array = new UInt16[len];
                for (int i = 0; i < array.Length; i++)
                {
                    array[i] = MessagePackBinary.ReadUInt16(bytes, offset, out readSize);
                    offset += readSize;
                }
                readSize = offset - startOffset;
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

        public void Serialize(IBufferWriter<byte> writer, UInt32 value, IFormatterResolver formatterResolver)
        {
            return MessagePackBinary.WriteUInt32(ref bytes, offset, value);
        }

        public UInt32 Deserialize(ref ReadOnlySequence<byte> byteSequence, IFormatterResolver formatterResolver)
        {
            return MessagePackBinary.ReadUInt32(bytes, offset, out readSize);
        }
    }

    public sealed class NullableUInt32Formatter : IMessagePackFormatter<UInt32?>
    {
        public static readonly NullableUInt32Formatter Instance = new NullableUInt32Formatter();

        NullableUInt32Formatter()
        {
        }

        public void Serialize(IBufferWriter<byte> writer, UInt32? value, IFormatterResolver formatterResolver)
        {
            if (value == null)
            {
                return MessagePackBinary.WriteNil(ref bytes, offset);
            }
            else
            {
                return MessagePackBinary.WriteUInt32(ref bytes, offset, value.Value);
            }
        }

        public UInt32? Deserialize(ref ReadOnlySequence<byte> byteSequence, IFormatterResolver formatterResolver)
        {
            if (MessagePackBinary.IsNil(bytes, offset))
            {
                readSize = 1;
                return null;
            }
            else
            {
                return MessagePackBinary.ReadUInt32(bytes, offset, out readSize);
            }
        }
    }

    public sealed class UInt32ArrayFormatter : IMessagePackFormatter<UInt32[]>
    {
        public static readonly UInt32ArrayFormatter Instance = new UInt32ArrayFormatter();

        UInt32ArrayFormatter()
        {

        }

        public void Serialize(IBufferWriter<byte> writer, UInt32[] value, IFormatterResolver formatterResolver)
        {
            if (value == null)
            {
                return MessagePackBinary.WriteNil(ref bytes, offset);
            }
            else
            {
                var startOffset = offset;
                offset += MessagePackBinary.WriteArrayHeader(ref bytes, offset, value.Length);
                for (int i = 0; i < value.Length; i++)
                {
                    offset += MessagePackBinary.WriteUInt32(ref bytes, offset, value[i]);
                }

                return offset - startOffset;
            }
        }

        public UInt32[] Deserialize(ref ReadOnlySequence<byte> byteSequence, IFormatterResolver formatterResolver)
        {
            if (MessagePackBinary.IsNil(bytes, offset))
            {
                readSize = 1;
                return null;
            }
            else
            {
                var startOffset = offset;

                var len = MessagePackBinary.ReadArrayHeader(bytes, offset, out readSize);
                offset += readSize;
                var array = new UInt32[len];
                for (int i = 0; i < array.Length; i++)
                {
                    array[i] = MessagePackBinary.ReadUInt32(bytes, offset, out readSize);
                    offset += readSize;
                }
                readSize = offset - startOffset;
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

        public void Serialize(IBufferWriter<byte> writer, UInt64 value, IFormatterResolver formatterResolver)
        {
            return MessagePackBinary.WriteUInt64(ref bytes, offset, value);
        }

        public UInt64 Deserialize(ref ReadOnlySequence<byte> byteSequence, IFormatterResolver formatterResolver)
        {
            return MessagePackBinary.ReadUInt64(bytes, offset, out readSize);
        }
    }

    public sealed class NullableUInt64Formatter : IMessagePackFormatter<UInt64?>
    {
        public static readonly NullableUInt64Formatter Instance = new NullableUInt64Formatter();

        NullableUInt64Formatter()
        {
        }

        public void Serialize(IBufferWriter<byte> writer, UInt64? value, IFormatterResolver formatterResolver)
        {
            if (value == null)
            {
                return MessagePackBinary.WriteNil(ref bytes, offset);
            }
            else
            {
                return MessagePackBinary.WriteUInt64(ref bytes, offset, value.Value);
            }
        }

        public UInt64? Deserialize(ref ReadOnlySequence<byte> byteSequence, IFormatterResolver formatterResolver)
        {
            if (MessagePackBinary.IsNil(bytes, offset))
            {
                readSize = 1;
                return null;
            }
            else
            {
                return MessagePackBinary.ReadUInt64(bytes, offset, out readSize);
            }
        }
    }

    public sealed class UInt64ArrayFormatter : IMessagePackFormatter<UInt64[]>
    {
        public static readonly UInt64ArrayFormatter Instance = new UInt64ArrayFormatter();

        UInt64ArrayFormatter()
        {

        }

        public void Serialize(IBufferWriter<byte> writer, UInt64[] value, IFormatterResolver formatterResolver)
        {
            if (value == null)
            {
                return MessagePackBinary.WriteNil(ref bytes, offset);
            }
            else
            {
                var startOffset = offset;
                offset += MessagePackBinary.WriteArrayHeader(ref bytes, offset, value.Length);
                for (int i = 0; i < value.Length; i++)
                {
                    offset += MessagePackBinary.WriteUInt64(ref bytes, offset, value[i]);
                }

                return offset - startOffset;
            }
        }

        public UInt64[] Deserialize(ref ReadOnlySequence<byte> byteSequence, IFormatterResolver formatterResolver)
        {
            if (MessagePackBinary.IsNil(bytes, offset))
            {
                readSize = 1;
                return null;
            }
            else
            {
                var startOffset = offset;

                var len = MessagePackBinary.ReadArrayHeader(bytes, offset, out readSize);
                offset += readSize;
                var array = new UInt64[len];
                for (int i = 0; i < array.Length; i++)
                {
                    array[i] = MessagePackBinary.ReadUInt64(bytes, offset, out readSize);
                    offset += readSize;
                }
                readSize = offset - startOffset;
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

        public void Serialize(IBufferWriter<byte> writer, Single value, IFormatterResolver formatterResolver)
        {
            return MessagePackBinary.WriteSingle(ref bytes, offset, value);
        }

        public Single Deserialize(ref ReadOnlySequence<byte> byteSequence, IFormatterResolver formatterResolver)
        {
            return MessagePackBinary.ReadSingle(bytes, offset, out readSize);
        }
    }

    public sealed class NullableSingleFormatter : IMessagePackFormatter<Single?>
    {
        public static readonly NullableSingleFormatter Instance = new NullableSingleFormatter();

        NullableSingleFormatter()
        {
        }

        public void Serialize(IBufferWriter<byte> writer, Single? value, IFormatterResolver formatterResolver)
        {
            if (value == null)
            {
                return MessagePackBinary.WriteNil(ref bytes, offset);
            }
            else
            {
                return MessagePackBinary.WriteSingle(ref bytes, offset, value.Value);
            }
        }

        public Single? Deserialize(ref ReadOnlySequence<byte> byteSequence, IFormatterResolver formatterResolver)
        {
            if (MessagePackBinary.IsNil(bytes, offset))
            {
                readSize = 1;
                return null;
            }
            else
            {
                return MessagePackBinary.ReadSingle(bytes, offset, out readSize);
            }
        }
    }

    public sealed class SingleArrayFormatter : IMessagePackFormatter<Single[]>
    {
        public static readonly SingleArrayFormatter Instance = new SingleArrayFormatter();

        SingleArrayFormatter()
        {

        }

        public void Serialize(IBufferWriter<byte> writer, Single[] value, IFormatterResolver formatterResolver)
        {
            if (value == null)
            {
                return MessagePackBinary.WriteNil(ref bytes, offset);
            }
            else
            {
                var startOffset = offset;
                offset += MessagePackBinary.WriteArrayHeader(ref bytes, offset, value.Length);
                for (int i = 0; i < value.Length; i++)
                {
                    offset += MessagePackBinary.WriteSingle(ref bytes, offset, value[i]);
                }

                return offset - startOffset;
            }
        }

        public Single[] Deserialize(ref ReadOnlySequence<byte> byteSequence, IFormatterResolver formatterResolver)
        {
            if (MessagePackBinary.IsNil(bytes, offset))
            {
                readSize = 1;
                return null;
            }
            else
            {
                var startOffset = offset;

                var len = MessagePackBinary.ReadArrayHeader(bytes, offset, out readSize);
                offset += readSize;
                var array = new Single[len];
                for (int i = 0; i < array.Length; i++)
                {
                    array[i] = MessagePackBinary.ReadSingle(bytes, offset, out readSize);
                    offset += readSize;
                }
                readSize = offset - startOffset;
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

        public void Serialize(IBufferWriter<byte> writer, Double value, IFormatterResolver formatterResolver)
        {
            return MessagePackBinary.WriteDouble(ref bytes, offset, value);
        }

        public Double Deserialize(ref ReadOnlySequence<byte> byteSequence, IFormatterResolver formatterResolver)
        {
            return MessagePackBinary.ReadDouble(bytes, offset, out readSize);
        }
    }

    public sealed class NullableDoubleFormatter : IMessagePackFormatter<Double?>
    {
        public static readonly NullableDoubleFormatter Instance = new NullableDoubleFormatter();

        NullableDoubleFormatter()
        {
        }

        public void Serialize(IBufferWriter<byte> writer, Double? value, IFormatterResolver formatterResolver)
        {
            if (value == null)
            {
                return MessagePackBinary.WriteNil(ref bytes, offset);
            }
            else
            {
                return MessagePackBinary.WriteDouble(ref bytes, offset, value.Value);
            }
        }

        public Double? Deserialize(ref ReadOnlySequence<byte> byteSequence, IFormatterResolver formatterResolver)
        {
            if (MessagePackBinary.IsNil(bytes, offset))
            {
                readSize = 1;
                return null;
            }
            else
            {
                return MessagePackBinary.ReadDouble(bytes, offset, out readSize);
            }
        }
    }

    public sealed class DoubleArrayFormatter : IMessagePackFormatter<Double[]>
    {
        public static readonly DoubleArrayFormatter Instance = new DoubleArrayFormatter();

        DoubleArrayFormatter()
        {

        }

        public void Serialize(IBufferWriter<byte> writer, Double[] value, IFormatterResolver formatterResolver)
        {
            if (value == null)
            {
                return MessagePackBinary.WriteNil(ref bytes, offset);
            }
            else
            {
                var startOffset = offset;
                offset += MessagePackBinary.WriteArrayHeader(ref bytes, offset, value.Length);
                for (int i = 0; i < value.Length; i++)
                {
                    offset += MessagePackBinary.WriteDouble(ref bytes, offset, value[i]);
                }

                return offset - startOffset;
            }
        }

        public Double[] Deserialize(ref ReadOnlySequence<byte> byteSequence, IFormatterResolver formatterResolver)
        {
            if (MessagePackBinary.IsNil(bytes, offset))
            {
                readSize = 1;
                return null;
            }
            else
            {
                var startOffset = offset;

                var len = MessagePackBinary.ReadArrayHeader(bytes, offset, out readSize);
                offset += readSize;
                var array = new Double[len];
                for (int i = 0; i < array.Length; i++)
                {
                    array[i] = MessagePackBinary.ReadDouble(bytes, offset, out readSize);
                    offset += readSize;
                }
                readSize = offset - startOffset;
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

        public void Serialize(IBufferWriter<byte> writer, Boolean value, IFormatterResolver formatterResolver)
        {
            return MessagePackBinary.WriteBoolean(ref bytes, offset, value);
        }

        public Boolean Deserialize(ref ReadOnlySequence<byte> byteSequence, IFormatterResolver formatterResolver)
        {
            return MessagePackBinary.ReadBoolean(bytes, offset, out readSize);
        }
    }

    public sealed class NullableBooleanFormatter : IMessagePackFormatter<Boolean?>
    {
        public static readonly NullableBooleanFormatter Instance = new NullableBooleanFormatter();

        NullableBooleanFormatter()
        {
        }

        public void Serialize(IBufferWriter<byte> writer, Boolean? value, IFormatterResolver formatterResolver)
        {
            if (value == null)
            {
                return MessagePackBinary.WriteNil(ref bytes, offset);
            }
            else
            {
                return MessagePackBinary.WriteBoolean(ref bytes, offset, value.Value);
            }
        }

        public Boolean? Deserialize(ref ReadOnlySequence<byte> byteSequence, IFormatterResolver formatterResolver)
        {
            if (MessagePackBinary.IsNil(bytes, offset))
            {
                readSize = 1;
                return null;
            }
            else
            {
                return MessagePackBinary.ReadBoolean(bytes, offset, out readSize);
            }
        }
    }

    public sealed class BooleanArrayFormatter : IMessagePackFormatter<Boolean[]>
    {
        public static readonly BooleanArrayFormatter Instance = new BooleanArrayFormatter();

        BooleanArrayFormatter()
        {

        }

        public void Serialize(IBufferWriter<byte> writer, Boolean[] value, IFormatterResolver formatterResolver)
        {
            if (value == null)
            {
                return MessagePackBinary.WriteNil(ref bytes, offset);
            }
            else
            {
                var startOffset = offset;
                offset += MessagePackBinary.WriteArrayHeader(ref bytes, offset, value.Length);
                for (int i = 0; i < value.Length; i++)
                {
                    offset += MessagePackBinary.WriteBoolean(ref bytes, offset, value[i]);
                }

                return offset - startOffset;
            }
        }

        public Boolean[] Deserialize(ref ReadOnlySequence<byte> byteSequence, IFormatterResolver formatterResolver)
        {
            if (MessagePackBinary.IsNil(bytes, offset))
            {
                readSize = 1;
                return null;
            }
            else
            {
                var startOffset = offset;

                var len = MessagePackBinary.ReadArrayHeader(bytes, offset, out readSize);
                offset += readSize;
                var array = new Boolean[len];
                for (int i = 0; i < array.Length; i++)
                {
                    array[i] = MessagePackBinary.ReadBoolean(bytes, offset, out readSize);
                    offset += readSize;
                }
                readSize = offset - startOffset;
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

        public void Serialize(IBufferWriter<byte> writer, Byte value, IFormatterResolver formatterResolver)
        {
            return MessagePackBinary.WriteByte(ref bytes, offset, value);
        }

        public Byte Deserialize(ref ReadOnlySequence<byte> byteSequence, IFormatterResolver formatterResolver)
        {
            return MessagePackBinary.ReadByte(bytes, offset, out readSize);
        }
    }

    public sealed class NullableByteFormatter : IMessagePackFormatter<Byte?>
    {
        public static readonly NullableByteFormatter Instance = new NullableByteFormatter();

        NullableByteFormatter()
        {
        }

        public void Serialize(IBufferWriter<byte> writer, Byte? value, IFormatterResolver formatterResolver)
        {
            if (value == null)
            {
                return MessagePackBinary.WriteNil(ref bytes, offset);
            }
            else
            {
                return MessagePackBinary.WriteByte(ref bytes, offset, value.Value);
            }
        }

        public Byte? Deserialize(ref ReadOnlySequence<byte> byteSequence, IFormatterResolver formatterResolver)
        {
            if (MessagePackBinary.IsNil(bytes, offset))
            {
                readSize = 1;
                return null;
            }
            else
            {
                return MessagePackBinary.ReadByte(bytes, offset, out readSize);
            }
        }
    }


    public sealed class SByteFormatter : IMessagePackFormatter<SByte>
    {
        public static readonly SByteFormatter Instance = new SByteFormatter();

        SByteFormatter()
        {
        }

        public void Serialize(IBufferWriter<byte> writer, SByte value, IFormatterResolver formatterResolver)
        {
            return MessagePackBinary.WriteSByte(ref bytes, offset, value);
        }

        public SByte Deserialize(ref ReadOnlySequence<byte> byteSequence, IFormatterResolver formatterResolver)
        {
            return MessagePackBinary.ReadSByte(bytes, offset, out readSize);
        }
    }

    public sealed class NullableSByteFormatter : IMessagePackFormatter<SByte?>
    {
        public static readonly NullableSByteFormatter Instance = new NullableSByteFormatter();

        NullableSByteFormatter()
        {
        }

        public void Serialize(IBufferWriter<byte> writer, SByte? value, IFormatterResolver formatterResolver)
        {
            if (value == null)
            {
                return MessagePackBinary.WriteNil(ref bytes, offset);
            }
            else
            {
                return MessagePackBinary.WriteSByte(ref bytes, offset, value.Value);
            }
        }

        public SByte? Deserialize(ref ReadOnlySequence<byte> byteSequence, IFormatterResolver formatterResolver)
        {
            if (MessagePackBinary.IsNil(bytes, offset))
            {
                readSize = 1;
                return null;
            }
            else
            {
                return MessagePackBinary.ReadSByte(bytes, offset, out readSize);
            }
        }
    }

    public sealed class SByteArrayFormatter : IMessagePackFormatter<SByte[]>
    {
        public static readonly SByteArrayFormatter Instance = new SByteArrayFormatter();

        SByteArrayFormatter()
        {

        }

        public void Serialize(IBufferWriter<byte> writer, SByte[] value, IFormatterResolver formatterResolver)
        {
            if (value == null)
            {
                return MessagePackBinary.WriteNil(ref bytes, offset);
            }
            else
            {
                var startOffset = offset;
                offset += MessagePackBinary.WriteArrayHeader(ref bytes, offset, value.Length);
                for (int i = 0; i < value.Length; i++)
                {
                    offset += MessagePackBinary.WriteSByte(ref bytes, offset, value[i]);
                }

                return offset - startOffset;
            }
        }

        public SByte[] Deserialize(ref ReadOnlySequence<byte> byteSequence, IFormatterResolver formatterResolver)
        {
            if (MessagePackBinary.IsNil(bytes, offset))
            {
                readSize = 1;
                return null;
            }
            else
            {
                var startOffset = offset;

                var len = MessagePackBinary.ReadArrayHeader(bytes, offset, out readSize);
                offset += readSize;
                var array = new SByte[len];
                for (int i = 0; i < array.Length; i++)
                {
                    array[i] = MessagePackBinary.ReadSByte(bytes, offset, out readSize);
                    offset += readSize;
                }
                readSize = offset - startOffset;
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

        public void Serialize(IBufferWriter<byte> writer, Char value, IFormatterResolver formatterResolver)
        {
            return MessagePackBinary.WriteChar(ref bytes, offset, value);
        }

        public Char Deserialize(ref ReadOnlySequence<byte> byteSequence, IFormatterResolver formatterResolver)
        {
            return MessagePackBinary.ReadChar(bytes, offset, out readSize);
        }
    }

    public sealed class NullableCharFormatter : IMessagePackFormatter<Char?>
    {
        public static readonly NullableCharFormatter Instance = new NullableCharFormatter();

        NullableCharFormatter()
        {
        }

        public void Serialize(IBufferWriter<byte> writer, Char? value, IFormatterResolver formatterResolver)
        {
            if (value == null)
            {
                return MessagePackBinary.WriteNil(ref bytes, offset);
            }
            else
            {
                return MessagePackBinary.WriteChar(ref bytes, offset, value.Value);
            }
        }

        public Char? Deserialize(ref ReadOnlySequence<byte> byteSequence, IFormatterResolver formatterResolver)
        {
            if (MessagePackBinary.IsNil(bytes, offset))
            {
                readSize = 1;
                return null;
            }
            else
            {
                return MessagePackBinary.ReadChar(bytes, offset, out readSize);
            }
        }
    }

    public sealed class CharArrayFormatter : IMessagePackFormatter<Char[]>
    {
        public static readonly CharArrayFormatter Instance = new CharArrayFormatter();

        CharArrayFormatter()
        {

        }

        public void Serialize(IBufferWriter<byte> writer, Char[] value, IFormatterResolver formatterResolver)
        {
            if (value == null)
            {
                return MessagePackBinary.WriteNil(ref bytes, offset);
            }
            else
            {
                var startOffset = offset;
                offset += MessagePackBinary.WriteArrayHeader(ref bytes, offset, value.Length);
                for (int i = 0; i < value.Length; i++)
                {
                    offset += MessagePackBinary.WriteChar(ref bytes, offset, value[i]);
                }

                return offset - startOffset;
            }
        }

        public Char[] Deserialize(ref ReadOnlySequence<byte> byteSequence, IFormatterResolver formatterResolver)
        {
            if (MessagePackBinary.IsNil(bytes, offset))
            {
                readSize = 1;
                return null;
            }
            else
            {
                var startOffset = offset;

                var len = MessagePackBinary.ReadArrayHeader(bytes, offset, out readSize);
                offset += readSize;
                var array = new Char[len];
                for (int i = 0; i < array.Length; i++)
                {
                    array[i] = MessagePackBinary.ReadChar(bytes, offset, out readSize);
                    offset += readSize;
                }
                readSize = offset - startOffset;
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

        public void Serialize(IBufferWriter<byte> writer, DateTime value, IFormatterResolver formatterResolver)
        {
            return MessagePackBinary.WriteDateTime(ref bytes, offset, value);
        }

        public DateTime Deserialize(ref ReadOnlySequence<byte> byteSequence, IFormatterResolver formatterResolver)
        {
            return MessagePackBinary.ReadDateTime(bytes, offset, out readSize);
        }
    }

    public sealed class NullableDateTimeFormatter : IMessagePackFormatter<DateTime?>
    {
        public static readonly NullableDateTimeFormatter Instance = new NullableDateTimeFormatter();

        NullableDateTimeFormatter()
        {
        }

        public void Serialize(IBufferWriter<byte> writer, DateTime? value, IFormatterResolver formatterResolver)
        {
            if (value == null)
            {
                return MessagePackBinary.WriteNil(ref bytes, offset);
            }
            else
            {
                return MessagePackBinary.WriteDateTime(ref bytes, offset, value.Value);
            }
        }

        public DateTime? Deserialize(ref ReadOnlySequence<byte> byteSequence, IFormatterResolver formatterResolver)
        {
            if (MessagePackBinary.IsNil(bytes, offset))
            {
                readSize = 1;
                return null;
            }
            else
            {
                return MessagePackBinary.ReadDateTime(bytes, offset, out readSize);
            }
        }
    }

    public sealed class DateTimeArrayFormatter : IMessagePackFormatter<DateTime[]>
    {
        public static readonly DateTimeArrayFormatter Instance = new DateTimeArrayFormatter();

        DateTimeArrayFormatter()
        {

        }

        public void Serialize(IBufferWriter<byte> writer, DateTime[] value, IFormatterResolver formatterResolver)
        {
            if (value == null)
            {
                return MessagePackBinary.WriteNil(ref bytes, offset);
            }
            else
            {
                var startOffset = offset;
                offset += MessagePackBinary.WriteArrayHeader(ref bytes, offset, value.Length);
                for (int i = 0; i < value.Length; i++)
                {
                    offset += MessagePackBinary.WriteDateTime(ref bytes, offset, value[i]);
                }

                return offset - startOffset;
            }
        }

        public DateTime[] Deserialize(ref ReadOnlySequence<byte> byteSequence, IFormatterResolver formatterResolver)
        {
            if (MessagePackBinary.IsNil(bytes, offset))
            {
                readSize = 1;
                return null;
            }
            else
            {
                var startOffset = offset;

                var len = MessagePackBinary.ReadArrayHeader(bytes, offset, out readSize);
                offset += readSize;
                var array = new DateTime[len];
                for (int i = 0; i < array.Length; i++)
                {
                    array[i] = MessagePackBinary.ReadDateTime(bytes, offset, out readSize);
                    offset += readSize;
                }
                readSize = offset - startOffset;
                return array;
            }
        }
    }

}