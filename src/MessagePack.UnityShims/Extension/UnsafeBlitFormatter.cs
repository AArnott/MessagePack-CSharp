#if ENABLE_UNSAFE_MSGPACK

using MessagePack.Formatters;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using UnityEngine;

namespace MessagePack.Unity.Extension
{
    public static class ReservedUnityExtensionTypeCode
    {
        public const sbyte Vector2 = 30;
        public const sbyte Vector3 = 31;
        public const sbyte Vector4 = 32;
        public const sbyte Quaternion = 33;
        public const sbyte Color = 34;
        public const sbyte Bounds = 35;
        public const sbyte Rect = 36;
        public const sbyte Int = 37;
        public const sbyte Float = 38;
        public const sbyte Double = 39;
    }

    // use ext instead of ArrayFormatter to extremely boost up performance.
    // Layout: [extHeader, byteSize(integer), isLittlEendian(bool), bytes()]

    // Used Ext:30~36

    public abstract class UnsafeBlitFormatterBase<T> : IMessagePackFormatter<T[]>
        where T : struct
    {
        protected abstract sbyte TypeCode { get; }
        protected void CopyDeserializeUnsafe(ReadOnlySpan<byte> src, Span<T> dest) => src.CopyTo(MemoryMarshal.Cast<T, byte>(dest));

        public void Serialize(IBufferWriter<byte> writer, T[] value, IFormatterResolver formatterResolver)
        {
            if (value == null)
            {
                MessagePackBinary.WriteNil(writer);
                return;
            }

            var byteLen = value.Length * Marshal.SizeOf<T>();

            MessagePackBinary.WriteExtensionFormatHeader(writer, TypeCode, byteLen);
            MessagePackBinary.WriteInt32(writer, byteLen); // write original header(not array header)
            MessagePackBinary.WriteBoolean(writer, BitConverter.IsLittleEndian);

            var span = writer.GetSpan(byteLen);
            MemoryMarshal.Cast<T, byte>(value).CopyTo(span);
            writer.Advance(byteLen);
        }

        public T[] Deserialize(ref ReadOnlySequence<byte> byteSequence, IFormatterResolver formatterResolver)
        {
            if (MessagePackBinary.IsNil(byteSequence))
            {
                byteSequence = byteSequence.Slice(1);
                return null;
            }

            var header = MessagePackBinary.ReadExtensionFormatHeader(ref byteSequence);
            if (header.TypeCode != TypeCode) throw new InvalidOperationException("Invalid typeCode.");

            var byteLength = MessagePackBinary.ReadInt32(ref byteSequence);
            var isLittleEndian = MessagePackBinary.ReadBoolean(ref byteSequence);

            return MessagePackBinary.Parse(ref byteSequence, byteLength, span =>
            {
                if (isLittleEndian != BitConverter.IsLittleEndian)
                {
                    var span2 = new byte[span.Length];
                    for (int i = 0; i < span.Length; i++)
                    {
                        span2[span.Length - i - 1] = span[i];
                    }

                    span = span2;
                }

                var result = new T[byteLength / Marshal.SizeOf<T>()];
                MemoryMarshal.Cast<byte, T>(span).CopyTo(result);
                return result;
            });
        }
    }

    public class Vector2ArrayBlitFormatter : UnsafeBlitFormatterBase<Vector2>
    {
        protected override sbyte TypeCode
        {
            get
            {
                return ReservedUnityExtensionTypeCode.Vector2;
            }
        }
    }

    public class Vector3ArrayBlitFormatter : UnsafeBlitFormatterBase<Vector3>
    {
        protected override sbyte TypeCode
        {
            get
            {
                return ReservedUnityExtensionTypeCode.Vector3;
            }
        }
    }

    public class Vector4ArrayBlitFormatter : UnsafeBlitFormatterBase<Vector4>
    {
        protected override sbyte TypeCode
        {
            get
            {
                return ReservedUnityExtensionTypeCode.Vector4;
            }
        }
    }

    public class QuaternionArrayBlitFormatter : UnsafeBlitFormatterBase<Quaternion>
    {
        protected override sbyte TypeCode
        {
            get
            {
                return ReservedUnityExtensionTypeCode.Quaternion;
            }
        }
    }

    public class ColorArrayBlitFormatter : UnsafeBlitFormatterBase<Color>
    {
        protected override sbyte TypeCode
        {
            get
            {
                return ReservedUnityExtensionTypeCode.Color;
            }
        }
    }

    public class BoundsArrayBlitFormatter : UnsafeBlitFormatterBase<Bounds>
    {
        protected override sbyte TypeCode
        {
            get
            {
                return ReservedUnityExtensionTypeCode.Bounds;
            }
        }
    }

    public class RectArrayBlitFormatter : UnsafeBlitFormatterBase<Rect>
    {
        protected override sbyte TypeCode
        {
            get
            {
                return ReservedUnityExtensionTypeCode.Rect;
            }
        }
    }

    public class IntArrayBlitFormatter : UnsafeBlitFormatterBase<int>
    {
        protected override sbyte TypeCode { get { return ReservedUnityExtensionTypeCode.Int; } }
    }

    public class FloatArrayBlitFormatter : UnsafeBlitFormatterBase<float>
    {
        protected override sbyte TypeCode { get { return ReservedUnityExtensionTypeCode.Float; } }
    }

    public class DoubleArrayBlitFormatter : UnsafeBlitFormatterBase<double>
    {
        protected override sbyte TypeCode { get { return ReservedUnityExtensionTypeCode.Double; } }
    }
}

#endif