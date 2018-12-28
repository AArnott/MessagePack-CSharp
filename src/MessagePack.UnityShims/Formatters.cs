using System;
using System.Buffers;
using MessagePack;
namespace MessagePack.Unity
{
    public sealed class Vector2Formatter : global::MessagePack.Formatters.IMessagePackFormatter<global::UnityEngine.Vector2>
    {
        public void Serialize(IBufferWriter<byte> writer, global::UnityEngine.Vector2 value, global::MessagePack.IFormatterResolver formatterResolver)
        {
            global::MessagePack.MessagePackBinary.WriteFixedArrayHeaderUnsafe(writer, 2);
            MessagePackBinary.WriteSingle(writer, value.x);
            MessagePackBinary.WriteSingle(writer, value.y);
        }
        public global::UnityEngine.Vector2 Deserialize(ref ReadOnlySequence<byte> byteSequence, global::MessagePack.IFormatterResolver formatterResolver)
        {
            if (global::MessagePack.MessagePackBinary.IsNil(byteSequence))
            {
                throw new InvalidOperationException("typecode is null, struct not supported");
            }
            var length = global::MessagePack.MessagePackBinary.ReadArrayHeader(ref byteSequence);
            var x = default(float);
            var y = default(float);
            for (int i = 0; i < length; i++)
            {
                var key = i;
                switch (key)
                {
                    case 0:
                        x = MessagePackBinary.ReadSingle(ref byteSequence);
                        break;
                    case 1:
                        y = MessagePackBinary.ReadSingle(ref byteSequence);
                        break;
                    default:
                        global::MessagePack.MessagePackBinary.ReadNextBlock(ref byteSequence);
                        break;
                }
            }

            var result = new global::UnityEngine.Vector2(x, y);
            return result;
        }
    }

    public sealed class Vector3Formatter : global::MessagePack.Formatters.IMessagePackFormatter<global::UnityEngine.Vector3>
    {
        public void Serialize(IBufferWriter<byte> writer, global::UnityEngine.Vector3 value, global::MessagePack.IFormatterResolver formatterResolver)
        {
            global::MessagePack.MessagePackBinary.WriteFixedArrayHeaderUnsafe(writer, 3);
            MessagePackBinary.WriteSingle(writer, value.x);
            MessagePackBinary.WriteSingle(writer, value.y);
            MessagePackBinary.WriteSingle(writer, value.z);
        }
        public global::UnityEngine.Vector3 Deserialize(ref ReadOnlySequence<byte> byteSequence, global::MessagePack.IFormatterResolver formatterResolver)
        {
            if (global::MessagePack.MessagePackBinary.IsNil(byteSequence))
            {
                throw new InvalidOperationException("typecode is null, struct not supported");
            }
            var length = global::MessagePack.MessagePackBinary.ReadArrayHeader(ref byteSequence);
            var x = default(float);
            var y = default(float);
            var z = default(float);
            for (int i = 0; i < length; i++)
            {
                var key = i;
                switch (key)
                {
                    case 0:
                        x = MessagePackBinary.ReadSingle(ref byteSequence);
                        break;
                    case 1:
                        y = MessagePackBinary.ReadSingle(ref byteSequence);
                        break;
                    case 2:
                        z = MessagePackBinary.ReadSingle(ref byteSequence);
                        break;
                    default:
                        global::MessagePack.MessagePackBinary.ReadNextBlock(ref byteSequence);
                        break;
                }
            }

            var result = new global::UnityEngine.Vector3(x, y, z);
            return result;
        }
    }

    public sealed class Vector4Formatter : global::MessagePack.Formatters.IMessagePackFormatter<global::UnityEngine.Vector4>
    {
        public void Serialize(IBufferWriter<byte> writer, global::UnityEngine.Vector4 value, global::MessagePack.IFormatterResolver formatterResolver)
        {
            global::MessagePack.MessagePackBinary.WriteFixedArrayHeaderUnsafe(writer, 4);
            MessagePackBinary.WriteSingle(writer, value.x);
            MessagePackBinary.WriteSingle(writer, value.y);
            MessagePackBinary.WriteSingle(writer, value.z);
            MessagePackBinary.WriteSingle(writer, value.w);
        }
        public global::UnityEngine.Vector4 Deserialize(ref ReadOnlySequence<byte> byteSequence, global::MessagePack.IFormatterResolver formatterResolver)
        {
            if (global::MessagePack.MessagePackBinary.IsNil(byteSequence))
            {
                throw new InvalidOperationException("typecode is null, struct not supported");
            }
            var length = global::MessagePack.MessagePackBinary.ReadArrayHeader(ref byteSequence);
            var x = default(float);
            var y = default(float);
            var z = default(float);
            var w = default(float);
            for (int i = 0; i < length; i++)
            {
                var key = i;
                switch (key)
                {
                    case 0:
                        x = MessagePackBinary.ReadSingle(ref byteSequence);
                        break;
                    case 1:
                        y = MessagePackBinary.ReadSingle(ref byteSequence);
                        break;
                    case 2:
                        z = MessagePackBinary.ReadSingle(ref byteSequence);
                        break;
                    case 3:
                        w = MessagePackBinary.ReadSingle(ref byteSequence);
                        break;
                    default:
                        global::MessagePack.MessagePackBinary.ReadNextBlock(ref byteSequence);
                        break;
                }
            }

            var result = new global::UnityEngine.Vector4(x, y, z, w);
            return result;
        }
    }

    public sealed class QuaternionFormatter : global::MessagePack.Formatters.IMessagePackFormatter<global::UnityEngine.Quaternion>
    {
        public void Serialize(IBufferWriter<byte> writer, global::UnityEngine.Quaternion value, global::MessagePack.IFormatterResolver formatterResolver)
        {
            global::MessagePack.MessagePackBinary.WriteFixedArrayHeaderUnsafe(writer, 4);
            MessagePackBinary.WriteSingle(writer, value.x);
            MessagePackBinary.WriteSingle(writer, value.y);
            MessagePackBinary.WriteSingle(writer, value.z);
            MessagePackBinary.WriteSingle(writer, value.w);
        }
        public global::UnityEngine.Quaternion Deserialize(ref ReadOnlySequence<byte> byteSequence, global::MessagePack.IFormatterResolver formatterResolver)
        {
            if (global::MessagePack.MessagePackBinary.IsNil(byteSequence))
            {
                throw new InvalidOperationException("typecode is null, struct not supported");
            }
            var length = global::MessagePack.MessagePackBinary.ReadArrayHeader(ref byteSequence);
            var x = default(float);
            var y = default(float);
            var z = default(float);
            var w = default(float);
            for (int i = 0; i < length; i++)
            {
                var key = i;
                switch (key)
                {
                    case 0:
                        x = MessagePackBinary.ReadSingle(ref byteSequence);
                        break;
                    case 1:
                        y = MessagePackBinary.ReadSingle(ref byteSequence);
                        break;
                    case 2:
                        z = MessagePackBinary.ReadSingle(ref byteSequence);
                        break;
                    case 3:
                        w = MessagePackBinary.ReadSingle(ref byteSequence);
                        break;
                    default:
                        global::MessagePack.MessagePackBinary.ReadNextBlock(ref byteSequence);
                        break;
                }
            }

            var result = new global::UnityEngine.Quaternion(x, y, z, w);
            return result;
        }
    }

    public sealed class ColorFormatter : global::MessagePack.Formatters.IMessagePackFormatter<global::UnityEngine.Color>
    {
        public void Serialize(IBufferWriter<byte> writer, global::UnityEngine.Color value, global::MessagePack.IFormatterResolver formatterResolver)
        {
            global::MessagePack.MessagePackBinary.WriteFixedArrayHeaderUnsafe(writer, 4);
            MessagePackBinary.WriteSingle(writer, value.r);
            MessagePackBinary.WriteSingle(writer, value.g);
            MessagePackBinary.WriteSingle(writer, value.b);
            MessagePackBinary.WriteSingle(writer, value.a);
        }
        public global::UnityEngine.Color Deserialize(ref ReadOnlySequence<byte> byteSequence, global::MessagePack.IFormatterResolver formatterResolver)
        {
            if (global::MessagePack.MessagePackBinary.IsNil(byteSequence))
            {
                throw new InvalidOperationException("typecode is null, struct not supported");
            }
            var length = global::MessagePack.MessagePackBinary.ReadArrayHeader(ref byteSequence);
            var r = default(float);
            var g = default(float);
            var b = default(float);
            var a = default(float);
            for (int i = 0; i < length; i++)
            {
                var key = i;
                switch (key)
                {
                    case 0:
                        r = MessagePackBinary.ReadSingle(ref byteSequence);
                        break;
                    case 1:
                        g = MessagePackBinary.ReadSingle(ref byteSequence);
                        break;
                    case 2:
                        b = MessagePackBinary.ReadSingle(ref byteSequence);
                        break;
                    case 3:
                        a = MessagePackBinary.ReadSingle(ref byteSequence);
                        break;
                    default:
                        global::MessagePack.MessagePackBinary.ReadNextBlock(ref byteSequence);
                        break;
                }
            }

            var result = new global::UnityEngine.Color(r, g, b, a);
            return result;
        }
    }

    public sealed class BoundsFormatter : global::MessagePack.Formatters.IMessagePackFormatter<global::UnityEngine.Bounds>
    {
        public void Serialize(IBufferWriter<byte> writer, global::UnityEngine.Bounds value, global::MessagePack.IFormatterResolver formatterResolver)
        {
            global::MessagePack.MessagePackBinary.WriteFixedArrayHeaderUnsafe(writer, 2);
            formatterResolver.GetFormatterWithVerify<global::UnityEngine.Vector3>().Serialize(writer, value.center, formatterResolver);
            formatterResolver.GetFormatterWithVerify<global::UnityEngine.Vector3>().Serialize(writer, value.size, formatterResolver);
        }
        public global::UnityEngine.Bounds Deserialize(ref ReadOnlySequence<byte> byteSequence, global::MessagePack.IFormatterResolver formatterResolver)
        {
            if (global::MessagePack.MessagePackBinary.IsNil(byteSequence))
            {
                throw new InvalidOperationException("typecode is null, struct not supported");
            }
            var length = global::MessagePack.MessagePackBinary.ReadArrayHeader(ref byteSequence);
            var center = default(global::UnityEngine.Vector3);
            var size = default(global::UnityEngine.Vector3);
            for (int i = 0; i < length; i++)
            {
                var key = i;
                switch (key)
                {
                    case 0:
                        center = formatterResolver.GetFormatterWithVerify<global::UnityEngine.Vector3>().Deserialize(ref byteSequence, formatterResolver);
                        break;
                    case 1:
                        size = formatterResolver.GetFormatterWithVerify<global::UnityEngine.Vector3>().Deserialize(ref byteSequence, formatterResolver);
                        break;
                    default:
                        global::MessagePack.MessagePackBinary.ReadNextBlock(ref byteSequence);
                        break;
                }
            }

            var result = new global::UnityEngine.Bounds(center, size);
            return result;
        }
    }

    public sealed class RectFormatter : global::MessagePack.Formatters.IMessagePackFormatter<global::UnityEngine.Rect>
    {
        public void Serialize(IBufferWriter<byte> writer, global::UnityEngine.Rect value, global::MessagePack.IFormatterResolver formatterResolver)
        {
            global::MessagePack.MessagePackBinary.WriteFixedArrayHeaderUnsafe(writer, 4);
            MessagePackBinary.WriteSingle(writer, value.x);
            MessagePackBinary.WriteSingle(writer, value.y);
            MessagePackBinary.WriteSingle(writer, value.width);
            MessagePackBinary.WriteSingle(writer, value.height);
        }
        public global::UnityEngine.Rect Deserialize(ref ReadOnlySequence<byte> byteSequence, global::MessagePack.IFormatterResolver formatterResolver)
        {
            if (global::MessagePack.MessagePackBinary.IsNil(byteSequence))
            {
                throw new InvalidOperationException("typecode is null, struct not supported");
            }
            var length = global::MessagePack.MessagePackBinary.ReadArrayHeader(ref byteSequence);
            var x = default(float);
            var y = default(float);
            var width = default(float);
            var height = default(float);
            for (int i = 0; i < length; i++)
            {
                var key = i;
                switch (key)
                {
                    case 0:
                        x = MessagePackBinary.ReadSingle(ref byteSequence);
                        break;
                    case 1:
                        y = MessagePackBinary.ReadSingle(ref byteSequence);
                        break;
                    case 2:
                        width = MessagePackBinary.ReadSingle(ref byteSequence);
                        break;
                    case 3:
                        height = MessagePackBinary.ReadSingle(ref byteSequence);
                        break;
                    default:
                        global::MessagePack.MessagePackBinary.ReadNextBlock(ref byteSequence);
                        break;
                }
            }

            var result = new global::UnityEngine.Rect(x, y, width, height);
            return result;
        }
    }
    // additional
    public sealed class WrapModeFormatter : global::MessagePack.Formatters.IMessagePackFormatter<global::UnityEngine.WrapMode>
    {
        public void Serialize(IBufferWriter<byte> writer, global::UnityEngine.WrapMode value, global::MessagePack.IFormatterResolver formatterResolver)
        {
            MessagePackBinary.WriteInt32(writer, (Int32)value);
        }
        public global::UnityEngine.WrapMode Deserialize(ref ReadOnlySequence<byte> byteSequence, global::MessagePack.IFormatterResolver formatterResolver)
        {
            return (global::UnityEngine.WrapMode)MessagePackBinary.ReadInt32(ref byteSequence);
        }
    }
    public sealed class GradientModeFormatter : global::MessagePack.Formatters.IMessagePackFormatter<global::UnityEngine.GradientMode>
    {
        public void Serialize(IBufferWriter<byte> writer, global::UnityEngine.GradientMode value, global::MessagePack.IFormatterResolver formatterResolver)
        {
            MessagePackBinary.WriteInt32(writer, (Int32)value);
        }
        public global::UnityEngine.GradientMode Deserialize(ref ReadOnlySequence<byte> byteSequence, global::MessagePack.IFormatterResolver formatterResolver)
        {
            return (global::UnityEngine.GradientMode)MessagePackBinary.ReadInt32(ref byteSequence);
        }
    }
    public sealed class KeyframeFormatter : global::MessagePack.Formatters.IMessagePackFormatter<global::UnityEngine.Keyframe>
    {
        public void Serialize(IBufferWriter<byte> writer, global::UnityEngine.Keyframe value, global::MessagePack.IFormatterResolver formatterResolver)
        {
            global::MessagePack.MessagePackBinary.WriteFixedArrayHeaderUnsafe(writer, 4);
            MessagePackBinary.WriteSingle(writer, value.time);
            MessagePackBinary.WriteSingle(writer, value.value);
            MessagePackBinary.WriteSingle(writer, value.inTangent);
            MessagePackBinary.WriteSingle(writer, value.outTangent);
        }
        public global::UnityEngine.Keyframe Deserialize(ref ReadOnlySequence<byte> byteSequence, global::MessagePack.IFormatterResolver formatterResolver)
        {
            if (global::MessagePack.MessagePackBinary.IsNil(byteSequence))
            {
                throw new InvalidOperationException("typecode is null, struct not supported");
            }
            var length = global::MessagePack.MessagePackBinary.ReadArrayHeader(ref byteSequence);
            var __time__ = default(float);
            var __value__ = default(float);
            var __inTangent__ = default(float);
            var __outTangent__ = default(float);
            for (int i = 0; i < length; i++)
            {
                var key = i;
                switch (key)
                {
                    case 0:
                        __time__ = MessagePackBinary.ReadSingle(ref byteSequence);
                        break;
                    case 1:
                        __value__ = MessagePackBinary.ReadSingle(ref byteSequence);
                        break;
                    case 2:
                        __inTangent__ = MessagePackBinary.ReadSingle(ref byteSequence);
                        break;
                    case 3:
                        __outTangent__ = MessagePackBinary.ReadSingle(ref byteSequence);
                        break;
                    default:
                        global::MessagePack.MessagePackBinary.ReadNextBlock(ref byteSequence);
                        break;
                }
            }

            var ____result = new global::UnityEngine.Keyframe(__time__, __value__, __inTangent__, __outTangent__);
            ____result.time = __time__;
            ____result.value = __value__;
            ____result.inTangent = __inTangent__;
            ____result.outTangent = __outTangent__;
            return ____result;
        }
    }

    public sealed class AnimationCurveFormatter : global::MessagePack.Formatters.IMessagePackFormatter<global::UnityEngine.AnimationCurve>
    {
        public void Serialize(IBufferWriter<byte> writer, global::UnityEngine.AnimationCurve value, global::MessagePack.IFormatterResolver formatterResolver)
        {
            if (value == null)
            {
                global::MessagePack.MessagePackBinary.WriteNil(writer);
                return;
            }
            global::MessagePack.MessagePackBinary.WriteFixedArrayHeaderUnsafe(writer, 3);
            formatterResolver.GetFormatterWithVerify<global::UnityEngine.Keyframe[]>().Serialize(writer, value.keys, formatterResolver);
            formatterResolver.GetFormatterWithVerify<global::UnityEngine.WrapMode>().Serialize(writer, value.postWrapMode, formatterResolver);
            formatterResolver.GetFormatterWithVerify<global::UnityEngine.WrapMode>().Serialize(writer, value.preWrapMode, formatterResolver);
        }
        public global::UnityEngine.AnimationCurve Deserialize(ref ReadOnlySequence<byte> byteSequence, global::MessagePack.IFormatterResolver formatterResolver)
        {
            if (global::MessagePack.MessagePackBinary.IsNil(byteSequence))
            {
                byteSequence = byteSequence.Slice(1);
                return null;
            }
            var length = global::MessagePack.MessagePackBinary.ReadArrayHeader(ref byteSequence);
            var __keys__ = default(global::UnityEngine.Keyframe[]);
            var __postWrapMode__ = default(global::UnityEngine.WrapMode);
            var __preWrapMode__ = default(global::UnityEngine.WrapMode);
            for (int i = 0; i < length; i++)
            {
                var key = i;
                switch (key)
                {
                    case 0:
                        __keys__ = formatterResolver.GetFormatterWithVerify<global::UnityEngine.Keyframe[]>().Deserialize(ref byteSequence, formatterResolver);
                        break;
                    case 1:
                        __postWrapMode__ = formatterResolver.GetFormatterWithVerify<global::UnityEngine.WrapMode>().Deserialize(ref byteSequence, formatterResolver);
                        break;
                    case 2:
                        __preWrapMode__ = formatterResolver.GetFormatterWithVerify<global::UnityEngine.WrapMode>().Deserialize(ref byteSequence, formatterResolver);
                        break;
                    default:
                        global::MessagePack.MessagePackBinary.ReadNextBlock(ref byteSequence);
                        break;
                }
            }

            var ____result = new global::UnityEngine.AnimationCurve();
            ____result.keys = __keys__;
            ____result.postWrapMode = __postWrapMode__;
            ____result.preWrapMode = __preWrapMode__;
            return ____result;
        }
    }
    public sealed class Matrix4x4Formatter : global::MessagePack.Formatters.IMessagePackFormatter<global::UnityEngine.Matrix4x4>
    {
        public void Serialize(IBufferWriter<byte> writer, global::UnityEngine.Matrix4x4 value, global::MessagePack.IFormatterResolver formatterResolver)
        {
            global::MessagePack.MessagePackBinary.WriteArrayHeader(writer, 16);
            MessagePackBinary.WriteSingle(writer, value.m00);
            MessagePackBinary.WriteSingle(writer, value.m10);
            MessagePackBinary.WriteSingle(writer, value.m20);
            MessagePackBinary.WriteSingle(writer, value.m30);
            MessagePackBinary.WriteSingle(writer, value.m01);
            MessagePackBinary.WriteSingle(writer, value.m11);
            MessagePackBinary.WriteSingle(writer, value.m21);
            MessagePackBinary.WriteSingle(writer, value.m31);
            MessagePackBinary.WriteSingle(writer, value.m02);
            MessagePackBinary.WriteSingle(writer, value.m12);
            MessagePackBinary.WriteSingle(writer, value.m22);
            MessagePackBinary.WriteSingle(writer, value.m32);
            MessagePackBinary.WriteSingle(writer, value.m03);
            MessagePackBinary.WriteSingle(writer, value.m13);
            MessagePackBinary.WriteSingle(writer, value.m23);
            MessagePackBinary.WriteSingle(writer, value.m33);
        }
        public global::UnityEngine.Matrix4x4 Deserialize(ref ReadOnlySequence<byte> byteSequence, global::MessagePack.IFormatterResolver formatterResolver)
        {
            if (global::MessagePack.MessagePackBinary.IsNil(byteSequence))
            {
                throw new InvalidOperationException("typecode is null, struct not supported");
            }
            var length = global::MessagePack.MessagePackBinary.ReadArrayHeader(ref byteSequence);
            var __m00__ = default(float);
            var __m10__ = default(float);
            var __m20__ = default(float);
            var __m30__ = default(float);
            var __m01__ = default(float);
            var __m11__ = default(float);
            var __m21__ = default(float);
            var __m31__ = default(float);
            var __m02__ = default(float);
            var __m12__ = default(float);
            var __m22__ = default(float);
            var __m32__ = default(float);
            var __m03__ = default(float);
            var __m13__ = default(float);
            var __m23__ = default(float);
            var __m33__ = default(float);
            for (int i = 0; i < length; i++)
            {
                var key = i;
                switch (key)
                {
                    case 0:
                        __m00__ = MessagePackBinary.ReadSingle(ref byteSequence);
                        break;
                    case 1:
                        __m10__ = MessagePackBinary.ReadSingle(ref byteSequence);
                        break;
                    case 2:
                        __m20__ = MessagePackBinary.ReadSingle(ref byteSequence);
                        break;
                    case 3:
                        __m30__ = MessagePackBinary.ReadSingle(ref byteSequence);
                        break;
                    case 4:
                        __m01__ = MessagePackBinary.ReadSingle(ref byteSequence);
                        break;
                    case 5:
                        __m11__ = MessagePackBinary.ReadSingle(ref byteSequence);
                        break;
                    case 6:
                        __m21__ = MessagePackBinary.ReadSingle(ref byteSequence);
                        break;
                    case 7:
                        __m31__ = MessagePackBinary.ReadSingle(ref byteSequence);
                        break;
                    case 8:
                        __m02__ = MessagePackBinary.ReadSingle(ref byteSequence);
                        break;
                    case 9:
                        __m12__ = MessagePackBinary.ReadSingle(ref byteSequence);
                        break;
                    case 10:
                        __m22__ = MessagePackBinary.ReadSingle(ref byteSequence);
                        break;
                    case 11:
                        __m32__ = MessagePackBinary.ReadSingle(ref byteSequence);
                        break;
                    case 12:
                        __m03__ = MessagePackBinary.ReadSingle(ref byteSequence);
                        break;
                    case 13:
                        __m13__ = MessagePackBinary.ReadSingle(ref byteSequence);
                        break;
                    case 14:
                        __m23__ = MessagePackBinary.ReadSingle(ref byteSequence);
                        break;
                    case 15:
                        __m33__ = MessagePackBinary.ReadSingle(ref byteSequence);
                        break;
                    default:
                        global::MessagePack.MessagePackBinary.ReadNextBlock(ref byteSequence);
                        break;
                }
            }

            var ____result = new global::UnityEngine.Matrix4x4();
            ____result.m00 = __m00__;
            ____result.m10 = __m10__;
            ____result.m20 = __m20__;
            ____result.m30 = __m30__;
            ____result.m01 = __m01__;
            ____result.m11 = __m11__;
            ____result.m21 = __m21__;
            ____result.m31 = __m31__;
            ____result.m02 = __m02__;
            ____result.m12 = __m12__;
            ____result.m22 = __m22__;
            ____result.m32 = __m32__;
            ____result.m03 = __m03__;
            ____result.m13 = __m13__;
            ____result.m23 = __m23__;
            ____result.m33 = __m33__;
            return ____result;
        }
    }

    public sealed class GradientColorKeyFormatter : global::MessagePack.Formatters.IMessagePackFormatter<global::UnityEngine.GradientColorKey>
    {
        public void Serialize(IBufferWriter<byte> writer, global::UnityEngine.GradientColorKey value, global::MessagePack.IFormatterResolver formatterResolver)
        {
            global::MessagePack.MessagePackBinary.WriteFixedArrayHeaderUnsafe(writer, 2);
            formatterResolver.GetFormatterWithVerify<global::UnityEngine.Color>().Serialize(writer, value.color, formatterResolver);
            MessagePackBinary.WriteSingle(writer, value.time);
        }
        public global::UnityEngine.GradientColorKey Deserialize(ref ReadOnlySequence<byte> byteSequence, global::MessagePack.IFormatterResolver formatterResolver)
        {
            if (global::MessagePack.MessagePackBinary.IsNil(byteSequence))
            {
                throw new InvalidOperationException("typecode is null, struct not supported");
            }
            var length = global::MessagePack.MessagePackBinary.ReadArrayHeader(ref byteSequence);
            var __color__ = default(global::UnityEngine.Color);
            var __time__ = default(float);
            for (int i = 0; i < length; i++)
            {
                var key = i;
                switch (key)
                {
                    case 0:
                        __color__ = formatterResolver.GetFormatterWithVerify<global::UnityEngine.Color>().Deserialize(ref byteSequence, formatterResolver);
                        break;
                    case 1:
                        __time__ = MessagePackBinary.ReadSingle(ref byteSequence);
                        break;
                    default:
                        global::MessagePack.MessagePackBinary.ReadNextBlock(ref byteSequence);
                        break;
                }
            }

            var ____result = new global::UnityEngine.GradientColorKey(__color__, __time__);
            ____result.color = __color__;
            ____result.time = __time__;
            return ____result;
        }
    }

    public sealed class GradientAlphaKeyFormatter : global::MessagePack.Formatters.IMessagePackFormatter<global::UnityEngine.GradientAlphaKey>
    {
        public void Serialize(IBufferWriter<byte> writer, global::UnityEngine.GradientAlphaKey value, global::MessagePack.IFormatterResolver formatterResolver)
        {
            global::MessagePack.MessagePackBinary.WriteFixedArrayHeaderUnsafe(writer, 2);
            MessagePackBinary.WriteSingle(writer, value.alpha);
            MessagePackBinary.WriteSingle(writer, value.time);
        }
        public global::UnityEngine.GradientAlphaKey Deserialize(ref ReadOnlySequence<byte> byteSequence, global::MessagePack.IFormatterResolver formatterResolver)
        {
            if (global::MessagePack.MessagePackBinary.IsNil(byteSequence))
            {
                throw new InvalidOperationException("typecode is null, struct not supported");
            }
            var length = global::MessagePack.MessagePackBinary.ReadArrayHeader(ref byteSequence);
            var __alpha__ = default(float);
            var __time__ = default(float);
            for (int i = 0; i < length; i++)
            {
                var key = i;
                switch (key)
                {
                    case 0:
                        __alpha__ = MessagePackBinary.ReadSingle(ref byteSequence);
                        break;
                    case 1:
                        __time__ = MessagePackBinary.ReadSingle(ref byteSequence);
                        break;
                    default:
                        global::MessagePack.MessagePackBinary.ReadNextBlock(ref byteSequence);
                        break;
                }
            }

            var ____result = new global::UnityEngine.GradientAlphaKey(__alpha__, __time__);
            ____result.alpha = __alpha__;
            ____result.time = __time__;
            return ____result;
        }
    }

    public sealed class GradientFormatter : global::MessagePack.Formatters.IMessagePackFormatter<global::UnityEngine.Gradient>
    {
        public void Serialize(IBufferWriter<byte> writer, global::UnityEngine.Gradient value, global::MessagePack.IFormatterResolver formatterResolver)
        {
            if (value == null)
            {
                global::MessagePack.MessagePackBinary.WriteNil(writer);
                return;
            }
            global::MessagePack.MessagePackBinary.WriteFixedArrayHeaderUnsafe(writer, 3);
            formatterResolver.GetFormatterWithVerify<global::UnityEngine.GradientColorKey[]>().Serialize(writer, value.colorKeys, formatterResolver);
            formatterResolver.GetFormatterWithVerify<global::UnityEngine.GradientAlphaKey[]>().Serialize(writer, value.alphaKeys, formatterResolver);
            formatterResolver.GetFormatterWithVerify<global::UnityEngine.GradientMode>().Serialize(writer, value.mode, formatterResolver);
        }
        public global::UnityEngine.Gradient Deserialize(ref ReadOnlySequence<byte> byteSequence, global::MessagePack.IFormatterResolver formatterResolver)
        {
            if (global::MessagePack.MessagePackBinary.IsNil(byteSequence))
            {
                byteSequence = byteSequence.Slice(1);
                return null;
            }
            var length = global::MessagePack.MessagePackBinary.ReadArrayHeader(ref byteSequence);
            var __colorKeys__ = default(global::UnityEngine.GradientColorKey[]);
            var __alphaKeys__ = default(global::UnityEngine.GradientAlphaKey[]);
            var __mode__ = default(global::UnityEngine.GradientMode);
            for (int i = 0; i < length; i++)
            {
                var key = i;
                switch (key)
                {
                    case 0:
                        __colorKeys__ = formatterResolver.GetFormatterWithVerify<global::UnityEngine.GradientColorKey[]>().Deserialize(ref byteSequence, formatterResolver);
                        break;
                    case 1:
                        __alphaKeys__ = formatterResolver.GetFormatterWithVerify<global::UnityEngine.GradientAlphaKey[]>().Deserialize(ref byteSequence, formatterResolver);
                        break;
                    case 2:
                        __mode__ = formatterResolver.GetFormatterWithVerify<global::UnityEngine.GradientMode>().Deserialize(ref byteSequence, formatterResolver);
                        break;
                    default:
                        global::MessagePack.MessagePackBinary.ReadNextBlock(ref byteSequence);
                        break;
                }
            }

            var ____result = new global::UnityEngine.Gradient();
            ____result.colorKeys = __colorKeys__;
            ____result.alphaKeys = __alphaKeys__;
            ____result.mode = __mode__;
            return ____result;
        }
    }

    public sealed class Color32Formatter : global::MessagePack.Formatters.IMessagePackFormatter<global::UnityEngine.Color32>
    {
        public void Serialize(IBufferWriter<byte> writer, global::UnityEngine.Color32 value, global::MessagePack.IFormatterResolver formatterResolver)
        {
            global::MessagePack.MessagePackBinary.WriteFixedArrayHeaderUnsafe(writer, 4);
            MessagePackBinary.WriteByte(writer, value.r);
            MessagePackBinary.WriteByte(writer, value.g);
            MessagePackBinary.WriteByte(writer, value.b);
            MessagePackBinary.WriteByte(writer, value.a);
        }
        public global::UnityEngine.Color32 Deserialize(ref ReadOnlySequence<byte> byteSequence, global::MessagePack.IFormatterResolver formatterResolver)
        {
            if (global::MessagePack.MessagePackBinary.IsNil(byteSequence))
            {
                throw new InvalidOperationException("typecode is null, struct not supported");
            }
            var length = global::MessagePack.MessagePackBinary.ReadArrayHeader(ref byteSequence);
            var __r__ = default(byte);
            var __g__ = default(byte);
            var __b__ = default(byte);
            var __a__ = default(byte);
            for (int i = 0; i < length; i++)
            {
                var key = i;
                switch (key)
                {
                    case 0:
                        __r__ = MessagePackBinary.ReadByte(ref byteSequence);
                        break;
                    case 1:
                        __g__ = MessagePackBinary.ReadByte(ref byteSequence);
                        break;
                    case 2:
                        __b__ = MessagePackBinary.ReadByte(ref byteSequence);
                        break;
                    case 3:
                        __a__ = MessagePackBinary.ReadByte(ref byteSequence);
                        break;
                    default:
                        global::MessagePack.MessagePackBinary.ReadNextBlock(ref byteSequence);
                        break;
                }
            }

            var ____result = new global::UnityEngine.Color32(__r__, __g__, __b__, __a__);
            ____result.r = __r__;
            ____result.g = __g__;
            ____result.b = __b__;
            ____result.a = __a__;
            return ____result;
        }
    }

    public sealed class RectOffsetFormatter : global::MessagePack.Formatters.IMessagePackFormatter<global::UnityEngine.RectOffset>
    {
        public void Serialize(IBufferWriter<byte> writer, global::UnityEngine.RectOffset value, global::MessagePack.IFormatterResolver formatterResolver)
        {
            if (value == null)
            {
                global::MessagePack.MessagePackBinary.WriteNil(writer);
                return;
            }
            global::MessagePack.MessagePackBinary.WriteFixedArrayHeaderUnsafe(writer, 4);
            MessagePackBinary.WriteInt32(writer, value.left);
            MessagePackBinary.WriteInt32(writer, value.right);
            MessagePackBinary.WriteInt32(writer, value.top);
            MessagePackBinary.WriteInt32(writer, value.bottom);
        }
        public global::UnityEngine.RectOffset Deserialize(ref ReadOnlySequence<byte> byteSequence, global::MessagePack.IFormatterResolver formatterResolver)
        {
            if (global::MessagePack.MessagePackBinary.IsNil(byteSequence))
            {
                byteSequence = byteSequence.Slice(1);
                return null;
            }
            var length = global::MessagePack.MessagePackBinary.ReadArrayHeader(ref byteSequence);
            var __left__ = default(int);
            var __right__ = default(int);
            var __top__ = default(int);
            var __bottom__ = default(int);
            for (int i = 0; i < length; i++)
            {
                var key = i;
                switch (key)
                {
                    case 0:
                        __left__ = MessagePackBinary.ReadInt32(ref byteSequence);
                        break;
                    case 1:
                        __right__ = MessagePackBinary.ReadInt32(ref byteSequence);
                        break;
                    case 2:
                        __top__ = MessagePackBinary.ReadInt32(ref byteSequence);
                        break;
                    case 3:
                        __bottom__ = MessagePackBinary.ReadInt32(ref byteSequence);
                        break;
                    default:
                        global::MessagePack.MessagePackBinary.ReadNextBlock(ref byteSequence);
                        break;
                }
            }

            var ____result = new global::UnityEngine.RectOffset();
            ____result.left = __left__;
            ____result.right = __right__;
            ____result.top = __top__;
            ____result.bottom = __bottom__;
            return ____result;
        }
    }

    public sealed class LayerMaskFormatter : global::MessagePack.Formatters.IMessagePackFormatter<global::UnityEngine.LayerMask>
    {
        public void Serialize(IBufferWriter<byte> writer, global::UnityEngine.LayerMask value, global::MessagePack.IFormatterResolver formatterResolver)
        {
            global::MessagePack.MessagePackBinary.WriteFixedArrayHeaderUnsafe(writer, 1);
            MessagePackBinary.WriteInt32(writer, value.value);
        }
        public global::UnityEngine.LayerMask Deserialize(ref ReadOnlySequence<byte> byteSequence, global::MessagePack.IFormatterResolver formatterResolver)
        {
            if (global::MessagePack.MessagePackBinary.IsNil(byteSequence))
            {
                throw new InvalidOperationException("typecode is null, struct not supported");
            }
            var length = global::MessagePack.MessagePackBinary.ReadArrayHeader(ref byteSequence);
            var __value__ = default(int);
            for (int i = 0; i < length; i++)
            {
                var key = i;
                switch (key)
                {
                    case 0:
                        __value__ = MessagePackBinary.ReadInt32(ref byteSequence);
                        break;
                    default:
                        global::MessagePack.MessagePackBinary.ReadNextBlock(ref byteSequence);
                        break;
                }
            }

            var ____result = new global::UnityEngine.LayerMask();
            ____result.value = __value__;
            return ____result;
        }
    }
#if UNITY_2017_2_OR_NEWER
    public sealed class Vector2IntFormatter : global::MessagePack.Formatters.IMessagePackFormatter<global::UnityEngine.Vector2Int>
    {
        public void Serialize(IBufferWriter<byte> writerglobal::UnityEngine.Vector2Int value, global::MessagePack.IFormatterResolver formatterResolver)
        {
            global::MessagePack.MessagePackBinary.WriteFixedArrayHeaderUnsafe(writer, 2);
            MessagePackBinary.WriteInt32(writer, value.x);
            MessagePackBinary.WriteInt32(writer, value.y);
        }
        public global::UnityEngine.Vector2Int Deserialize(ref ReadOnlySequence<byte> byteSequenceglobal::MessagePack.IFormatterResolver formatterResolver)
        {
            if (global::MessagePack.MessagePackBinary.IsNil(byteSequence))
            {
                throw new InvalidOperationException("typecode is null, struct not supported");
            }
            var length = global::MessagePack.MessagePackBinary.ReadArrayHeader(ref byteSequence);
            var __x__ = default(int);
            var __y__ = default(int);
            for (int i = 0; i < length; i++)
            {
                var key = i;
                switch (key)
                {
                    case 0:
                        __x__ = MessagePackBinary.ReadInt32(ref byteSequence);
                        break;
                    case 1:
                        __y__ = MessagePackBinary.ReadInt32(ref byteSequence);
                        break;
                    default:
                        global::MessagePack.MessagePackBinary.ReadNextBlock(ref byteSequence);
                        break;
                }
            }

            var ____result = new global::UnityEngine.Vector2Int(__x__, __y__);
            ____result.x = __x__;
            ____result.y = __y__;
            return ____result;
        }
    }

    public sealed class Vector3IntFormatter : global::MessagePack.Formatters.IMessagePackFormatter<global::UnityEngine.Vector3Int>
    {
        public void Serialize(IBufferWriter<byte> writerglobal::UnityEngine.Vector3Int value, global::MessagePack.IFormatterResolver formatterResolver)
        {
            global::MessagePack.MessagePackBinary.WriteFixedArrayHeaderUnsafe(writer, 3);
            MessagePackBinary.WriteInt32(writer, value.x);
            MessagePackBinary.WriteInt32(writer, value.y);
            MessagePackBinary.WriteInt32(writer, value.z);
        }
        public global::UnityEngine.Vector3Int Deserialize(ref ReadOnlySequence<byte> byteSequenceglobal::MessagePack.IFormatterResolver formatterResolver)
        {
            if (global::MessagePack.MessagePackBinary.IsNil(byteSequence))
            {
                throw new InvalidOperationException("typecode is null, struct not supported");
            }
            var length = global::MessagePack.MessagePackBinary.ReadArrayHeader(ref byteSequence);
            var __x__ = default(int);
            var __y__ = default(int);
            var __z__ = default(int);
            for (int i = 0; i < length; i++)
            {
                var key = i;
                switch (key)
                {
                    case 0:
                        __x__ = MessagePackBinary.ReadInt32(ref byteSequence);
                        break;
                    case 1:
                        __y__ = MessagePackBinary.ReadInt32(ref byteSequence);
                        break;
                    case 2:
                        __z__ = MessagePackBinary.ReadInt32(ref byteSequence);
                        break;
                    default:
                        global::MessagePack.MessagePackBinary.ReadNextBlock(ref byteSequence);
                        break;
                }
            }

            var ____result = new global::UnityEngine.Vector3Int(__x__, __y__, __z__);
            ____result.x = __x__;
            ____result.y = __y__;
            ____result.z = __z__;
            return ____result;
        }
    }

    public sealed class RangeIntFormatter : global::MessagePack.Formatters.IMessagePackFormatter<global::UnityEngine.RangeInt>
    {
        public void Serialize(IBufferWriter<byte> writerglobal::UnityEngine.RangeInt value, global::MessagePack.IFormatterResolver formatterResolver)
        {
            global::MessagePack.MessagePackBinary.WriteFixedArrayHeaderUnsafe(writer, 2);
            MessagePackBinary.WriteInt32(writer, value.start);
            MessagePackBinary.WriteInt32(writer, value.length);
        }
        public global::UnityEngine.RangeInt Deserialize(ref ReadOnlySequence<byte> byteSequenceglobal::MessagePack.IFormatterResolver formatterResolver)
        {
            if (global::MessagePack.MessagePackBinary.IsNil(byteSequence))
            {
                throw new InvalidOperationException("typecode is null, struct not supported");
            }
            var length = global::MessagePack.MessagePackBinary.ReadArrayHeader(ref byteSequence);
            var __start__ = default(int);
            var __length__ = default(int);
            for (int i = 0; i < length; i++)
            {
                var key = i;
                switch (key)
                {
                    case 0:
                        __start__ = MessagePackBinary.ReadInt32(ref byteSequence);
                        break;
                    case 1:
                        __length__ = MessagePackBinary.ReadInt32(ref byteSequence);
                        break;
                    default:
                        global::MessagePack.MessagePackBinary.ReadNextBlock(ref byteSequence);
                        break;
                }
            }

            var ____result = new global::UnityEngine.RangeInt(__start__, __length__);
            ____result.start = __start__;
            ____result.length = __length__;
            return ____result;
        }
    }

    public sealed class RectIntFormatter : global::MessagePack.Formatters.IMessagePackFormatter<global::UnityEngine.RectInt>
    {
        public void Serialize(IBufferWriter<byte> writerglobal::UnityEngine.RectInt value, global::MessagePack.IFormatterResolver formatterResolver)
        {
            global::MessagePack.MessagePackBinary.WriteFixedArrayHeaderUnsafe(writer, 4);
            MessagePackBinary.WriteInt32(writer, value.x);
            MessagePackBinary.WriteInt32(writer, value.y);
            MessagePackBinary.WriteInt32(writer, value.width);
            MessagePackBinary.WriteInt32(writer, value.height);
        }
        public global::UnityEngine.RectInt Deserialize(ref ReadOnlySequence<byte> byteSequenceglobal::MessagePack.IFormatterResolver formatterResolver)
        {
            if (global::MessagePack.MessagePackBinary.IsNil(byteSequence))
            {
                throw new InvalidOperationException("typecode is null, struct not supported");
            }
            var length = global::MessagePack.MessagePackBinary.ReadArrayHeader(ref byteSequence);
            var __x__ = default(int);
            var __y__ = default(int);
            var __width__ = default(int);
            var __height__ = default(int);
            for (int i = 0; i < length; i++)
            {
                var key = i;
                switch (key)
                {
                    case 0:
                        __x__ = MessagePackBinary.ReadInt32(ref byteSequence);
                        break;
                    case 1:
                        __y__ = MessagePackBinary.ReadInt32(ref byteSequence);
                        break;
                    case 2:
                        __width__ = MessagePackBinary.ReadInt32(ref byteSequence);
                        break;
                    case 3:
                        __height__ = MessagePackBinary.ReadInt32(ref byteSequence);
                        break;
                    default:
                        global::MessagePack.MessagePackBinary.ReadNextBlock(ref byteSequence);
                        break;
                }
            }

            var ____result = new global::UnityEngine.RectInt(__x__, __y__, __width__, __height__);
            ____result.x = __x__;
            ____result.y = __y__;
            ____result.width = __width__;
            ____result.height = __height__;
            return ____result;
        }
    }

    public sealed class BoundsIntFormatter : global::MessagePack.Formatters.IMessagePackFormatter<global::UnityEngine.BoundsInt>
    {
        public void Serialize(IBufferWriter<byte> writerglobal::UnityEngine.BoundsInt value, global::MessagePack.IFormatterResolver formatterResolver)
        {
            global::MessagePack.MessagePackBinary.WriteFixedArrayHeaderUnsafe(writer, 2);
            formatterResolver.GetFormatterWithVerify<global::UnityEngine.Vector3Int>().Serialize(writer, value.position, formatterResolver);
            formatterResolver.GetFormatterWithVerify<global::UnityEngine.Vector3Int>().Serialize(writer, value.size, formatterResolver);
        }
        public global::UnityEngine.BoundsInt Deserialize(ref ReadOnlySequence<byte> byteSequenceglobal::MessagePack.IFormatterResolver formatterResolver)
        {
            if (global::MessagePack.MessagePackBinary.IsNil(byteSequence))
            {
                throw new InvalidOperationException("typecode is null, struct not supported");
            }
            var length = global::MessagePack.MessagePackBinary.ReadArrayHeader(ref byteSequence);
            var __position__ = default(global::UnityEngine.Vector3Int);
            var __size__ = default(global::UnityEngine.Vector3Int);
            for (int i = 0; i < length; i++)
            {
                var key = i;
                switch (key)
                {
                    case 0:
                        __position__ = formatterResolver.GetFormatterWithVerify<global::UnityEngine.Vector3Int>().Deserialize(ref byteSequence, formatterResolver);
                        break;
                    case 1:
                        __size__ = formatterResolver.GetFormatterWithVerify<global::UnityEngine.Vector3Int>().Deserialize(ref byteSequence, formatterResolver);
                        break;
                    default:
                        global::MessagePack.MessagePackBinary.ReadNextBlock(ref byteSequence);
                        break;
                }
            }

            var ____result = new global::UnityEngine.BoundsInt(__position__, __size__);
            ____result.position = __position__;
            ____result.size = __size__;
            return ____result;
        }
    }
#endif
}