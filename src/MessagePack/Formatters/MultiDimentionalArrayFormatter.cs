using System;
using System.Buffers;
using System.Collections.Generic;
using System.Text;

namespace MessagePack.Formatters
{
    // multi dimentional array serialize to [i, j, [seq]]

    public sealed class TwoDimentionalArrayFormatter<T> : IMessagePackFormatter<T[,]>
    {
        const int ArrayLength = 3;

        public void Serialize(IBufferWriter<byte> writer, T[,] value, IFormatterResolver formatterResolver)
        {
            if (value == null)
            {
                MessagePackBinary.WriteNil(writer);
            }
            else
            {
                var i = value.GetLength(0);
                var j = value.GetLength(1);

                var formatter = formatterResolver.GetFormatterWithVerify<T>();

                MessagePackBinary.WriteArrayHeader(writer, ArrayLength);
                MessagePackBinary.WriteInt32(writer, i);
                MessagePackBinary.WriteInt32(writer, j);

                MessagePackBinary.WriteArrayHeader(writer, value.Length);
                foreach (var item in value)
                {
                    formatter.Serialize(writer, item, formatterResolver);
                }
            }
        }

        public T[,] Deserialize(ref ReadOnlySequence<byte> byteSequence, IFormatterResolver formatterResolver)
        {
            if (MessagePackBinary.IsNil(byteSequence))
            {
                byteSequence = byteSequence.Slice(1);
                return null;
            }
            else
            {
                var formatter = formatterResolver.GetFormatterWithVerify<T>();

                var len = MessagePackBinary.ReadArrayHeader(ref byteSequence);
                if (len != ArrayLength) throw new InvalidOperationException("Invalid T[,] format");

                var iLength = MessagePackBinary.ReadInt32(ref byteSequence);
                var jLength = MessagePackBinary.ReadInt32(ref byteSequence);
                var maxLen = MessagePackBinary.ReadArrayHeader(ref byteSequence);
                
                var array = new T[iLength, jLength];

                var i = 0;
                var j = -1;
                for (int loop = 0; loop < maxLen; loop++)
                {
                    if (j < jLength - 1)
                    {
                        j++;
                    }
                    else
                    {
                        j = 0;
                        i++;
                    }

                    array[i, j] = formatter.Deserialize(bytes, offset, formatterResolver, out readSize);
                    offset += readSize;
                }

                readSize = offset - startOffset;
                return array;
            }
        }
    }

    public sealed class ThreeDimentionalArrayFormatter<T> : IMessagePackFormatter<T[,,]>
    {
        const int ArrayLength = 4;

        public void Serialize(IBufferWriter<byte> writer, T[,,] value, IFormatterResolver formatterResolver)
        {
            if (value == null)
            {
                return MessagePackBinary.WriteNil(ref bytes, offset);
            }
            else
            {
                var i = value.GetLength(0);
                var j = value.GetLength(1);
                var k = value.GetLength(2);

                var startOffset = offset;
                var formatter = formatterResolver.GetFormatterWithVerify<T>();

                offset += MessagePackBinary.WriteArrayHeader(ref bytes, offset, ArrayLength);
                offset += MessagePackBinary.WriteInt32(ref bytes, offset, i);
                offset += MessagePackBinary.WriteInt32(ref bytes, offset, j);
                offset += MessagePackBinary.WriteInt32(ref bytes, offset, k);

                offset += MessagePackBinary.WriteArrayHeader(ref bytes, offset, value.Length);
                foreach (var item in value)
                {
                    offset += formatter.Serialize(ref bytes, offset, item, formatterResolver);
                }

                return offset - startOffset;
            }
        }

        public T[,,] Deserialize(ref ReadOnlySequence<byte> byteSequence, IFormatterResolver formatterResolver)
        {
            if (MessagePackBinary.IsNil(bytes, offset))
            {
                readSize = 1;
                return null;
            }
            else
            {
                var startOffset = offset;
                var formatter = formatterResolver.GetFormatterWithVerify<T>();

                var len = MessagePackBinary.ReadArrayHeader(bytes, offset, out readSize);
                offset += readSize;
                if (len != ArrayLength) throw new InvalidOperationException("Invalid T[,,] format");

                var iLength = MessagePackBinary.ReadInt32(bytes, offset, out readSize);
                offset += readSize;

                var jLength = MessagePackBinary.ReadInt32(bytes, offset, out readSize);
                offset += readSize;

                var kLength = MessagePackBinary.ReadInt32(bytes, offset, out readSize);
                offset += readSize;

                var maxLen = MessagePackBinary.ReadArrayHeader(bytes, offset, out readSize);
                offset += readSize;

                var array = new T[iLength, jLength, kLength];

                var i = 0;
                var j = 0;
                var k = -1;
                for (int loop = 0; loop < maxLen; loop++)
                {
                    if (k < kLength - 1)
                    {
                        k++;
                    }
                    else if (j < jLength - 1)
                    {
                        k = 0;
                        j++;
                    }
                    else
                    {
                        k = 0;
                        j = 0;
                        i++;
                    }

                    array[i, j, k] = formatter.Deserialize(bytes, offset, formatterResolver, out readSize);
                    offset += readSize;
                }

                readSize = offset - startOffset;
                return array;
            }
        }
    }

    public sealed class FourDimentionalArrayFormatter<T> : IMessagePackFormatter<T[,,,]>
    {
        const int ArrayLength = 5;

        public void Serialize(IBufferWriter<byte> writer, T[,,,] value, IFormatterResolver formatterResolver)
        {
            if (value == null)
            {
                return MessagePackBinary.WriteNil(ref bytes, offset);
            }
            else
            {
                var i = value.GetLength(0);
                var j = value.GetLength(1);
                var k = value.GetLength(2);
                var l = value.GetLength(3);

                var startOffset = offset;
                var formatter = formatterResolver.GetFormatterWithVerify<T>();

                offset += MessagePackBinary.WriteArrayHeader(ref bytes, offset, ArrayLength);
                offset += MessagePackBinary.WriteInt32(ref bytes, offset, i);
                offset += MessagePackBinary.WriteInt32(ref bytes, offset, j);
                offset += MessagePackBinary.WriteInt32(ref bytes, offset, k);
                offset += MessagePackBinary.WriteInt32(ref bytes, offset, l);

                offset += MessagePackBinary.WriteArrayHeader(ref bytes, offset, value.Length);
                foreach (var item in value)
                {
                    offset += formatter.Serialize(ref bytes, offset, item, formatterResolver);
                }

                return offset - startOffset;
            }
        }

        public T[,,,] Deserialize(ref ReadOnlySequence<byte> byteSequence, IFormatterResolver formatterResolver)
        {
            if (MessagePackBinary.IsNil(bytes, offset))
            {
                readSize = 1;
                return null;
            }
            else
            {
                var startOffset = offset;
                var formatter = formatterResolver.GetFormatterWithVerify<T>();

                var len = MessagePackBinary.ReadArrayHeader(bytes, offset, out readSize);
                offset += readSize;
                if (len != ArrayLength) throw new InvalidOperationException("Invalid T[,,,] format");

                var iLength = MessagePackBinary.ReadInt32(bytes, offset, out readSize);
                offset += readSize;

                var jLength = MessagePackBinary.ReadInt32(bytes, offset, out readSize);
                offset += readSize;

                var kLength = MessagePackBinary.ReadInt32(bytes, offset, out readSize);
                offset += readSize;

                var lLength = MessagePackBinary.ReadInt32(bytes, offset, out readSize);
                offset += readSize;

                var maxLen = MessagePackBinary.ReadArrayHeader(bytes, offset, out readSize);
                offset += readSize;

                var array = new T[iLength, jLength, kLength, lLength];

                var i = 0;
                var j = 0;
                var k = 0;
                var l = -1;
                for (int loop = 0; loop < maxLen; loop++)
                {
                    if (l < lLength - 1)
                    {
                        l++;
                    }
                    else if (k < kLength - 1)
                    {
                        l = 0;
                        k++;
                    }
                    else if (j < jLength - 1)
                    {
                        l = 0;
                        k = 0;
                        j++;
                    }
                    else
                    {
                        l = 0;
                        k = 0;
                        j = 0;
                        i++;
                    }

                    array[i, j, k, l] = formatter.Deserialize(bytes, offset, formatterResolver, out readSize);
                    offset += readSize;
                }

                readSize = offset - startOffset;
                return array;
            }
        }
    }
}