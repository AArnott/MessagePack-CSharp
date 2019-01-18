using System;
using System.Buffers;
using System.Reflection;
using System.Collections.Generic;

namespace MessagePack.Formatters
{
    public sealed class PrimitiveObjectFormatter : IMessagePackFormatter<object>
    {
        public static readonly IMessagePackFormatter<object> Instance = new PrimitiveObjectFormatter();

        static readonly Dictionary<Type, int> typeToJumpCode = new Dictionary<Type, int>()
        {
            { typeof(Boolean), 0 },
            { typeof(Char), 1 },
            { typeof(SByte), 2 },
            { typeof(Byte), 3 },
            { typeof(Int16), 4 },
            { typeof(UInt16), 5 },
            { typeof(Int32), 6 },
            { typeof(UInt32), 7 },
            { typeof(Int64), 8 },
            { typeof(UInt64),9  },
            { typeof(Single), 10 },
            { typeof(Double), 11 },
            { typeof(DateTime), 12 },
            { typeof(string), 13 },
            { typeof(byte[]), 14 }
        };

        PrimitiveObjectFormatter()
        {

        }

#if !UNITY_WSA

        public static bool IsSupportedType(Type type, TypeInfo typeInfo, object value)
        {
            if (value == null) return true;
            if (typeToJumpCode.ContainsKey(type)) return true;
            if (typeInfo.IsEnum) return true;

            if (value is System.Collections.IDictionary) return true;
            if (value is System.Collections.ICollection) return true;

            return false;
        }

#endif

        public void Serialize(IBufferWriter<byte> writer, object value, IFormatterResolver formatterResolver)
        {
            if (value == null)
            {
                MessagePackBinary.WriteNil(writer);
                return;
            }

            var t = value.GetType();

            int code;
            if (typeToJumpCode.TryGetValue(t, out code))
            {
                switch (code)
                {
                    case 0:
                        MessagePackBinary.WriteBoolean(writer, (bool)value);
                        return;
                    case 1:
                        MessagePackBinary.WriteChar(writer, (char)value);
                        return;
                    case 2:
                        MessagePackBinary.WriteSByteForceSByteBlock(writer, (sbyte)value);
                        return;
                    case 3:
                        MessagePackBinary.WriteByteForceByteBlock(writer, (byte)value);
                        return;
                    case 4:
                        MessagePackBinary.WriteInt16ForceInt16Block(writer, (Int16)value);
                        return;
                    case 5:
                        MessagePackBinary.WriteUInt16ForceUInt16Block(writer, (UInt16)value);
                        return;
                    case 6:
                        MessagePackBinary.WriteInt32ForceInt32Block(writer, (Int32)value);
                        return;
                    case 7:
                        MessagePackBinary.WriteUInt32ForceUInt32Block(writer, (UInt32)value);
                        return;
                    case 8:
                        MessagePackBinary.WriteInt64ForceInt64Block(writer, (Int64)value);
                        return;
                    case 9:
                        MessagePackBinary.WriteUInt64ForceUInt64Block(writer, (UInt64)value);
                        return;
                    case 10:
                        MessagePackBinary.WriteSingle(writer, (Single)value);
                        return;
                    case 11:
                        MessagePackBinary.WriteDouble(writer, (double)value);
                        return;
                    case 12:
                        MessagePackBinary.WriteDateTime(writer, (DateTime)value);
                        return;
                    case 13:
                        MessagePackBinary.WriteString(writer, (string)value);
                        return;
                    case 14:
                        MessagePackBinary.WriteBytes(writer, (byte[])value);
                        return;
                    default:
                        throw new InvalidOperationException("Not supported primitive object resolver. type:" + t.Name);
                }
            }
            else
            {
#if UNITY_WSA && !NETFX_CORE
                if (t.IsEnum)
#else
                if (t.GetTypeInfo().IsEnum)
#endif
                {
                    var underlyingType = Enum.GetUnderlyingType(t);
                    var code2 = typeToJumpCode[underlyingType];
                    switch (code2)
                    {
                        case 2:
                            MessagePackBinary.WriteSByteForceSByteBlock(writer, (sbyte)value);
                            return;
                        case 3:
                            MessagePackBinary.WriteByteForceByteBlock(writer, (byte)value);
                            return;
                        case 4:
                            MessagePackBinary.WriteInt16ForceInt16Block(writer, (Int16)value);
                            return;
                        case 5:
                            MessagePackBinary.WriteUInt16ForceUInt16Block(writer, (UInt16)value);
                            return;
                        case 6:
                            MessagePackBinary.WriteInt32ForceInt32Block(writer, (Int32)value);
                            return;
                        case 7:
                            MessagePackBinary.WriteUInt32ForceUInt32Block(writer, (UInt32)value);
                            return;
                        case 8:
                            MessagePackBinary.WriteInt64ForceInt64Block(writer, (Int64)value);
                            return;
                        case 9:
                            MessagePackBinary.WriteUInt64ForceUInt64Block(writer, (UInt64)value);
                            return;
                        default:
                            break;
                    }
                }
                else if (value is System.Collections.IDictionary) // check IDictionary first
                {
                    var d = value as System.Collections.IDictionary;
                    MessagePackBinary.WriteMapHeader(writer, d.Count);
                    foreach (System.Collections.DictionaryEntry item in d)
                    {
                        Serialize(writer, item.Key, formatterResolver);
                        Serialize(writer, item.Value, formatterResolver);
                    }
                    return;
                }
                else if (value is System.Collections.ICollection)
                {
                    var c = value as System.Collections.ICollection;
                    MessagePackBinary.WriteArrayHeader(writer, c.Count);
                    foreach (var item in c)
                    {
                        Serialize(writer, item, formatterResolver);
                    }
                    return;
                }
            }

            throw new InvalidOperationException("Not supported primitive object resolver. type:" + t.Name);
        }

        public object Deserialize(ref ReadOnlySequence<byte> byteSequence, IFormatterResolver formatterResolver)
        {
            var type = MessagePackBinary.GetMessagePackType(byteSequence);
            switch (type)
            {
                case MessagePackType.Integer:
                    var code = byteSequence.First.Span[0];
                    if (MessagePackCode.MinNegativeFixInt <= code && code <= MessagePackCode.MaxNegativeFixInt) return MessagePackBinary.ReadSByte(ref byteSequence);
                    else if (MessagePackCode.MinFixInt <= code && code <= MessagePackCode.MaxFixInt) return MessagePackBinary.ReadByte(ref byteSequence);
                    else if (code == MessagePackCode.Int8) return MessagePackBinary.ReadSByte(ref byteSequence);
                    else if (code == MessagePackCode.Int16) return MessagePackBinary.ReadInt16(ref byteSequence);
                    else if (code == MessagePackCode.Int32) return MessagePackBinary.ReadInt32(ref byteSequence);
                    else if (code == MessagePackCode.Int64) return MessagePackBinary.ReadInt64(ref byteSequence);
                    else if (code == MessagePackCode.UInt8) return MessagePackBinary.ReadByte(ref byteSequence);
                    else if (code == MessagePackCode.UInt16) return MessagePackBinary.ReadUInt16(ref byteSequence);
                    else if (code == MessagePackCode.UInt32) return MessagePackBinary.ReadUInt32(ref byteSequence);
                    else if (code == MessagePackCode.UInt64) return MessagePackBinary.ReadUInt64(ref byteSequence);
                    throw new InvalidOperationException("Invalid primitive bytes.");
                case MessagePackType.Boolean:
                    return MessagePackBinary.ReadBoolean(ref byteSequence);
                case MessagePackType.Float:
                    if (MessagePackCode.Float32 == byteSequence.First.Span[0])
                    {
                        return MessagePackBinary.ReadSingle(ref byteSequence);
                    }
                    else
                    {
                        return MessagePackBinary.ReadDouble(ref byteSequence);
                    }
                case MessagePackType.String:
                    return MessagePackBinary.ReadString(ref byteSequence);
                case MessagePackType.Binary:
                    return MessagePackBinary.ReadBytes(ref byteSequence);
                case MessagePackType.Extension:
                    var byteSequence2 = byteSequence;
                    var ext = MessagePackBinary.ReadExtensionFormatHeader(ref byteSequence2);
                    if (ext.TypeCode == ReservedMessagePackExtensionTypeCode.DateTime)
                    {
                        return MessagePackBinary.ReadDateTime(ref byteSequence);
                    }
                    throw new InvalidOperationException("Invalid primitive bytes.");
                case MessagePackType.Array:
                    {
                        var length = MessagePackBinary.ReadArrayHeader(ref byteSequence);

                        var objectFormatter = formatterResolver.GetFormatter<object>();
                        var array = new object[length];
                        for (int i = 0; i < length; i++)
                        {
                            array[i] = objectFormatter.Deserialize(ref byteSequence, formatterResolver);
                        }

                        return array;
                    }
                case MessagePackType.Map:
                    {
                        var length = MessagePackBinary.ReadMapHeader(ref byteSequence);

                        var objectFormatter = formatterResolver.GetFormatter<object>();
                        var hash = new Dictionary<object, object>(length);
                        for (int i = 0; i < length; i++)
                        {
                            var key = objectFormatter.Deserialize(ref byteSequence, formatterResolver);

                            var value = objectFormatter.Deserialize(ref byteSequence, formatterResolver);

                            hash.Add(key, value);
                        }

                        return hash;
                    }
                case MessagePackType.Nil:
                    byteSequence = byteSequence.Slice(1);
                    return null;
                default:
                    throw new InvalidOperationException("Invalid primitive bytes.");
            }
        }
    }
}
