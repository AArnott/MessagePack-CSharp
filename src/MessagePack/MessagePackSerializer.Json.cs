using System;
using System.Buffers;
using System.Globalization;
using System.IO;
using System.Text;
using MessagePack.Formatters;
using MessagePack.Internal;
using Nerdbank.Streams;

namespace MessagePack
{
    // JSON API
    public partial class MessagePackSerializer
    {
        /// <summary>
        /// Serialize an object to JSON string.
        /// </summary>
        public string ToJson<T>(T obj, IFormatterResolver resolver = null)
        {
            resolver = resolver ?? this.DefaultResolver;

            using (var sequence = new Sequence<byte>())
            {
                var writer = new BufferWriter(sequence);
                Serialize(ref writer, obj, resolver);
                writer.Commit();
                return ToJson(sequence.AsReadOnlySequence);
            }
        }

        /// <summary>
        /// Dump message-pack binary to JSON string.
        /// </summary>
        public string ToJson(byte[] bytes) => this.ToJson(new ReadOnlySequence<byte>(bytes));

        /// <summary>
        /// Dump message-pack binary to JSON string.
        /// </summary>
        public string ToJson(ReadOnlyMemory<byte> bytes) => this.ToJson(new ReadOnlySequence<byte>(bytes));

        /// <summary>
        /// Dump message-pack binary to JSON string.
        /// </summary>
        public virtual string ToJson(ReadOnlySequence<byte> byteSequence)
        {
            if (byteSequence.Length == 0)
            {
                return "";
            }

            var sb = new StringWriter();
            ToJsonCore(ref byteSequence, sb);
            return sb.ToString();
        }

        public void FromJson(string str, IBufferWriter<byte> writer)
        {
            using (var sr = new StringReader(str))
            {
                FromJson(sr, writer);
            }
        }

        /// <summary>
        /// From Json String to MessagePack binary
        /// </summary>
        public void FromJson(TextReader reader, IBufferWriter<byte> writer)
        {
            var fastWriter = new BufferWriter(writer);
            this.FromJson(reader, ref fastWriter);
            fastWriter.Commit();
        }

        /// <summary>
        /// From Json String to MessagePack binary
        /// </summary>
        protected virtual void FromJson(TextReader reader, ref BufferWriter writer)
        {
            using (var jr = new TinyJsonReader(reader, false))
            {
                FromJsonCore(jr, ref writer);
            }
        }

        private static uint FromJsonCore(TinyJsonReader jr, ref BufferWriter writer)
        {
            uint count = 0;
            while (jr.Read())
            {
                switch (jr.TokenType)
                {
                    case TinyJsonToken.None:
                        break;
                    case TinyJsonToken.StartObject:
                        {
                            using (var scratch = new Sequence<byte>())
                            {
                                var scratchWriter = new BufferWriter(scratch);
                                var mapCount = FromJsonCore(jr, ref scratchWriter);
                                scratchWriter.Commit();
                                mapCount = mapCount / 2; // remove propertyname string count.

                                MessagePackBinary.WriteMapHeaderForceMap32Block(ref writer, mapCount);
                                scratch.AsReadOnlySequence.CopyTo(ref writer);
                            }

                            count++;
                            break;
                        }
                    case TinyJsonToken.EndObject:
                        return count; // break
                    case TinyJsonToken.StartArray:
                        {
                            // Reserve space for the header.
                            var headerSpan = writer.GetSpan(5);
                            writer.Advance(5);

                            var arrayCount = FromJsonCore(jr, ref writer);
                            MessagePackBinary.WriteArrayHeaderForceArray32Block(headerSpan, arrayCount);
                            count++;
                            break;
                        }
                    case TinyJsonToken.EndArray:
                        return count; // break
                    case TinyJsonToken.Number:
                        var v = jr.ValueType;
                        if (v == ValueType.Double)
                        {
                            MessagePackBinary.WriteDouble(ref writer, jr.DoubleValue);
                        }
                        else if (v == ValueType.Long)
                        {
                            MessagePackBinary.WriteInt64(ref writer, jr.LongValue);
                        }
                        else if (v == ValueType.ULong)
                        {
                            MessagePackBinary.WriteUInt64(ref writer, jr.ULongValue);
                        }
                        else if (v == ValueType.Decimal)
                        {
                            DecimalFormatter.Instance.Serialize(ref writer, jr.DecimalValue, null);
                        }
                        count++;
                        break;
                    case TinyJsonToken.String:
                        MessagePackBinary.WriteString(ref writer, jr.StringValue);
                        count++;
                        break;
                    case TinyJsonToken.True:
                        MessagePackBinary.WriteBoolean(ref writer, true);
                        count++;
                        break;
                    case TinyJsonToken.False:
                        MessagePackBinary.WriteBoolean(ref writer, false);
                        count++;
                        break;
                    case TinyJsonToken.Null:
                        MessagePackBinary.WriteNil(ref writer);
                        count++;
                        break;
                    default:
                        break;
                }
            }
            return count;
        }

        private static void ToJsonCore(ref ReadOnlySequence<byte> byteSequence, TextWriter writer)
        {
            var type = MessagePackBinary.GetMessagePackType(byteSequence);
            switch (type)
            {
                case MessagePackType.Integer:
                    var code = byteSequence.First.Span[0];
                    if (MessagePackCode.MinNegativeFixInt <= code && code <= MessagePackCode.MaxNegativeFixInt)
                    {
                        writer.Write(MessagePackBinary.ReadSByte(ref byteSequence).ToString(System.Globalization.CultureInfo.InvariantCulture));
                    }
                    else if (MessagePackCode.MinFixInt <= code && code <= MessagePackCode.MaxFixInt)
                    {
                        writer.Write(MessagePackBinary.ReadByte(ref byteSequence).ToString(System.Globalization.CultureInfo.InvariantCulture));
                    }
                    else if (code == MessagePackCode.Int8)
                    {
                        writer.Write(MessagePackBinary.ReadSByte(ref byteSequence).ToString(System.Globalization.CultureInfo.InvariantCulture));
                    }
                    else if (code == MessagePackCode.Int16)
                    {
                        writer.Write(MessagePackBinary.ReadInt16(ref byteSequence).ToString(System.Globalization.CultureInfo.InvariantCulture));
                    }
                    else if (code == MessagePackCode.Int32)
                    {
                        writer.Write(MessagePackBinary.ReadInt32(ref byteSequence).ToString(System.Globalization.CultureInfo.InvariantCulture));
                    }
                    else if (code == MessagePackCode.Int64)
                    {
                        writer.Write(MessagePackBinary.ReadInt64(ref byteSequence).ToString(System.Globalization.CultureInfo.InvariantCulture));
                    }
                    else if (code == MessagePackCode.UInt8)
                    {
                        writer.Write(MessagePackBinary.ReadByte(ref byteSequence).ToString(System.Globalization.CultureInfo.InvariantCulture));
                    }
                    else if (code == MessagePackCode.UInt16)
                    {
                        writer.Write(MessagePackBinary.ReadUInt16(ref byteSequence).ToString(System.Globalization.CultureInfo.InvariantCulture));
                    }
                    else if (code == MessagePackCode.UInt32)
                    {
                        writer.Write(MessagePackBinary.ReadUInt32(ref byteSequence).ToString(System.Globalization.CultureInfo.InvariantCulture));
                    }
                    else if (code == MessagePackCode.UInt64)
                    {
                        writer.Write(MessagePackBinary.ReadUInt64(ref byteSequence).ToString(System.Globalization.CultureInfo.InvariantCulture));
                    }

                    break;
                case MessagePackType.Boolean:
                    writer.Write(MessagePackBinary.ReadBoolean(ref byteSequence) ? "true" : "false");
                    break;
                case MessagePackType.Float:
                    var floatCode = byteSequence.First.Span[0];
                    if (floatCode == MessagePackCode.Float32)
                    {
                        writer.Write(MessagePackBinary.ReadSingle(ref byteSequence).ToString(System.Globalization.CultureInfo.InvariantCulture));
                    }
                    else
                    {
                        writer.Write(MessagePackBinary.ReadDouble(ref byteSequence).ToString(System.Globalization.CultureInfo.InvariantCulture));
                    }
                    break;
                case MessagePackType.String:
                    WriteJsonString(MessagePackBinary.ReadString(ref byteSequence), writer);
                    break;
                case MessagePackType.Binary:
                    writer.Write("\"" + Convert.ToBase64String(MessagePackBinary.ReadBytes(ref byteSequence)) + "\"");
                    break;
                case MessagePackType.Array:
                    {
                        var length = MessagePackBinary.ReadArrayHeaderRaw(ref byteSequence);
                        writer.Write("[");
                        for (int i = 0; i < length; i++)
                        {
                            ToJsonCore(ref byteSequence, writer);

                            if (i != length - 1)
                            {
                                writer.Write(",");
                            }
                        }
                        writer.Write("]");
                        return;
                    }
                case MessagePackType.Map:
                    {
                        var length = MessagePackBinary.ReadMapHeaderRaw(ref byteSequence);
                        writer.Write("{");
                        for (int i = 0; i < length; i++)
                        {
                            // write key
                            {
                                var keyType = MessagePackBinary.GetMessagePackType(byteSequence);
                                if (keyType == MessagePackType.String || keyType == MessagePackType.Binary)
                                {
                                    ToJsonCore(ref byteSequence, writer);
                                }
                                else
                                {
                                    writer.Write("\"");
                                    ToJsonCore(ref byteSequence, writer);
                                    writer.Write("\"");
                                }
                            }

                            writer.Write(":");

                            // write body
                            {
                                ToJsonCore(ref byteSequence, writer);
                            }

                            if (i != length - 1)
                            {
                                writer.Write(",");
                            }
                        }
                        writer.Write("}");

                        return;
                    }
                case MessagePackType.Extension:
                    var extHeader = MessagePackBinary.ReadExtensionFormatHeader(ref byteSequence);
                    if (extHeader.TypeCode == ReservedMessagePackExtensionTypeCode.DateTime)
                    {
                        var dt = MessagePackBinary.ReadDateTime(ref byteSequence);
                        writer.Write("\"");
                        writer.Write(dt.ToString("o", CultureInfo.InvariantCulture));
                        writer.Write("\"");
                    }
#if !UNITY
                    else if (extHeader.TypeCode == TypelessFormatter.ExtensionTypeCode)
                    {
                        // prepare type name token
                        var privateBuilder = new StringBuilder();
                        var typeNameTokenBuilder = new StringBuilder();
                        var byteSequenceBeforeTypeNameRead = byteSequence;
                        ToJsonCore(ref byteSequence, new StringWriter(typeNameTokenBuilder));
                        var typeNameReadSize = byteSequenceBeforeTypeNameRead.Length - byteSequence.Length;
                        if (extHeader.Length > typeNameReadSize)
                        {
                            // object map or array
                            var typeInside = MessagePackBinary.GetMessagePackType(byteSequence);
                            if (typeInside != MessagePackType.Array && typeInside != MessagePackType.Map)
                            {
                                privateBuilder.Append("{");
                            }

                            ToJsonCore(ref byteSequence, new StringWriter(privateBuilder));
                            // insert type name token to start of object map or array
                            if (typeInside != MessagePackType.Array)
                            {
                                typeNameTokenBuilder.Insert(0, "\"$type\":");
                            }

                            if (typeInside != MessagePackType.Array && typeInside != MessagePackType.Map)
                            {
                                privateBuilder.Append("}");
                            }

                            if (privateBuilder.Length > 2)
                            {
                                typeNameTokenBuilder.Append(",");
                            }

                            privateBuilder.Insert(1, typeNameTokenBuilder.ToString());

                            writer.Write(privateBuilder.ToString());
                        }
                        else
                        {
                            writer.Write("{\"$type\":\"" + typeNameTokenBuilder.ToString() + "}");
                        }
                    }
#endif
                    else
                    {
                        var ext = MessagePackBinary.ReadExtensionFormat(ref byteSequence);
                        writer.Write("[");
                        writer.Write(ext.TypeCode);
                        writer.Write(",");
                        writer.Write("\"");
                        writer.Write(Convert.ToBase64String(ext.Data.ToArray()));
                        writer.Write("\"");
                        writer.Write("]");
                    }
                    break;
                case MessagePackType.Unknown:
                case MessagePackType.Nil:
                default:
                    byteSequence = byteSequence.Slice(1);
                    writer.Write("null");
                    break;
            }
        }

        // escape string
        private static void WriteJsonString(string value, TextWriter builder)
        {
            builder.Write('\"');

            var len = value.Length;
            for (int i = 0; i < len; i++)
            {
                var c = value[i];
                switch (c)
                {
                    case '"':
                        builder.Write("\\\"");
                        break;
                    case '\\':
                        builder.Write("\\\\");
                        break;
                    case '\b':
                        builder.Write("\\b");
                        break;
                    case '\f':
                        builder.Write("\\f");
                        break;
                    case '\n':
                        builder.Write("\\n");
                        break;
                    case '\r':
                        builder.Write("\\r");
                        break;
                    case '\t':
                        builder.Write("\\t");
                        break;
                    default:
                        builder.Write(c);
                        break;
                }
            }

            builder.Write('\"');
        }
    }
}