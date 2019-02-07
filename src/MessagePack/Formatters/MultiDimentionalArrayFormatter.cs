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

        public void Serialize(ref BufferWriter writer, T[,] value, IFormatterResolver formatterResolver)
        {
            if (value == null)
            {
                MessagePackBinary.WriteNil(ref writer);
            }
            else
            {
                var i = value.GetLength(0);
                var j = value.GetLength(1);

                var formatter = formatterResolver.GetFormatterWithVerify<T>();

                MessagePackBinary.WriteArrayHeader(ref writer, ArrayLength);
                MessagePackBinary.WriteInt32(ref writer, i);
                MessagePackBinary.WriteInt32(ref writer, j);

                MessagePackBinary.WriteArrayHeader(ref writer, value.Length);
                foreach (var item in value)
                {
                    formatter.Serialize(ref writer, item, formatterResolver);
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

                    array[i, j] = formatter.Deserialize(ref byteSequence, formatterResolver);
                }

                return array;
            }
        }
    }

    public sealed class ThreeDimentionalArrayFormatter<T> : IMessagePackFormatter<T[,,]>
    {
        const int ArrayLength = 4;

        public void Serialize(ref BufferWriter writer, T[,,] value, IFormatterResolver formatterResolver)
        {
            if (value == null)
            {
                MessagePackBinary.WriteNil(ref writer);
            }
            else
            {
                var i = value.GetLength(0);
                var j = value.GetLength(1);
                var k = value.GetLength(2);

                var formatter = formatterResolver.GetFormatterWithVerify<T>();

                MessagePackBinary.WriteArrayHeader(ref writer, ArrayLength);
                MessagePackBinary.WriteInt32(ref writer, i);
                MessagePackBinary.WriteInt32(ref writer, j);
                MessagePackBinary.WriteInt32(ref writer, k);

                MessagePackBinary.WriteArrayHeader(ref writer, value.Length);
                foreach (var item in value)
                {
                    formatter.Serialize(ref writer, item, formatterResolver);
                }
            }
        }

        public T[,,] Deserialize(ref ReadOnlySequence<byte> byteSequence, IFormatterResolver formatterResolver)
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
                if (len != ArrayLength) throw new InvalidOperationException("Invalid T[,,] format");

                var iLength = MessagePackBinary.ReadInt32(ref byteSequence);
                var jLength = MessagePackBinary.ReadInt32(ref byteSequence);
                var kLength = MessagePackBinary.ReadInt32(ref byteSequence);
                var maxLen = MessagePackBinary.ReadArrayHeader(ref byteSequence);

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

                    array[i, j, k] = formatter.Deserialize(ref byteSequence, formatterResolver);
                }

                return array;
            }
        }
    }

    public sealed class FourDimentionalArrayFormatter<T> : IMessagePackFormatter<T[,,,]>
    {
        const int ArrayLength = 5;

        public void Serialize(ref BufferWriter writer, T[,,,] value, IFormatterResolver formatterResolver)
        {
            if (value == null)
            {
                MessagePackBinary.WriteNil(ref writer);
            }
            else
            {
                var i = value.GetLength(0);
                var j = value.GetLength(1);
                var k = value.GetLength(2);
                var l = value.GetLength(3);

                var formatter = formatterResolver.GetFormatterWithVerify<T>();

                MessagePackBinary.WriteArrayHeader(ref writer, ArrayLength);
                MessagePackBinary.WriteInt32(ref writer, i);
                MessagePackBinary.WriteInt32(ref writer, j);
                MessagePackBinary.WriteInt32(ref writer, k);
                MessagePackBinary.WriteInt32(ref writer, l);

                MessagePackBinary.WriteArrayHeader(ref writer, value.Length);
                foreach (var item in value)
                {
                    formatter.Serialize(ref writer, item, formatterResolver);
                }
            }
        }

        public T[,,,] Deserialize(ref ReadOnlySequence<byte> byteSequence, IFormatterResolver formatterResolver)
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
                if (len != ArrayLength) throw new InvalidOperationException("Invalid T[,,,] format");
                var iLength = MessagePackBinary.ReadInt32(ref byteSequence);
                var jLength = MessagePackBinary.ReadInt32(ref byteSequence);
                var kLength = MessagePackBinary.ReadInt32(ref byteSequence);
                var lLength = MessagePackBinary.ReadInt32(ref byteSequence);
                var maxLen = MessagePackBinary.ReadArrayHeader(ref byteSequence);
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

                    array[i, j, k, l] = formatter.Deserialize(ref byteSequence, formatterResolver);
                }

                return array;
            }
        }
    }
}
