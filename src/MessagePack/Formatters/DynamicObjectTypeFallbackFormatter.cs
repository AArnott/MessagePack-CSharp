#if NETSTANDARD || NETFRAMEWORK

using MessagePack.Resolvers;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace MessagePack.Formatters
{
    public sealed class DynamicObjectTypeFallbackFormatter : IMessagePackFormatter<object>
    {
        delegate int SerializeMethod(object dynamicFormatter, IBufferWriter<byte> writer, object value, IFormatterResolver formatterResolver);

        readonly MessagePack.Internal.ThreadsafeTypeKeyHashTable<KeyValuePair<object, SerializeMethod>> serializers = new Internal.ThreadsafeTypeKeyHashTable<KeyValuePair<object, SerializeMethod>>();

        readonly IFormatterResolver[] innerResolvers;

        public DynamicObjectTypeFallbackFormatter(params IFormatterResolver[] innerResolvers)
        {
            this.innerResolvers = innerResolvers;
        }

        public void Serialize(IBufferWriter<byte> writer, object value, IFormatterResolver formatterResolver)
        {
            if (value == null)
            {
                MessagePackBinary.WriteNil(writer);
            }

            var type = value.GetType();
            var ti = type.GetTypeInfo();

            if (type == typeof(object))
            {
                // serialize to empty map
                MessagePackBinary.WriteMapHeader(writer, 0);
                return;
            }

            KeyValuePair<object, SerializeMethod> formatterAndDelegate;
            if (!serializers.TryGetValue(type, out formatterAndDelegate))
            {
                lock (serializers)
                {
                    if (!serializers.TryGetValue(type, out formatterAndDelegate))
                    {
                        object formatter = null;
                        foreach (var innerResolver in innerResolvers)
                        {
                            formatter = innerResolver.GetFormatterDynamic(type);
                            if (formatter != null) break;
                        }
                        if (formatter == null)
                        {
                            throw new FormatterNotRegisteredException(type.FullName + " is not registered in this resolver. resolvers:" + string.Join(", ", innerResolvers.Select(x => x.GetType().Name).ToArray()));
                        }

                        var t = type;
                        {
                            var formatterType = typeof(IMessagePackFormatter<>).MakeGenericType(t);
                            var param0 = Expression.Parameter(typeof(object), "formatter");
                            var param1 = Expression.Parameter(typeof(IBufferWriter<byte>), "byteSequence");
                            var param2 = Expression.Parameter(typeof(object), "value");
                            var param3 = Expression.Parameter(typeof(IFormatterResolver), "formatterResolver");

                            var serializeMethodInfo = formatterType.GetRuntimeMethod("Serialize", new[] { typeof(IBufferWriter<byte>), t, typeof(IFormatterResolver) });

                            var body = Expression.Call(
                                Expression.Convert(param0, formatterType),
                                serializeMethodInfo,
                                param1,
                                ti.IsValueType ? Expression.Unbox(param2, t) : Expression.Convert(param2, t),
                                param3);

                            var lambda = Expression.Lambda<SerializeMethod>(body, param0, param1, param2, param3).Compile();

                            formatterAndDelegate = new KeyValuePair<object, SerializeMethod>(formatter, lambda);
                        }

                        serializers.TryAdd(t, formatterAndDelegate);
                    }
                }
            }

            formatterAndDelegate.Value(formatterAndDelegate.Key, writer, value, formatterResolver);
        }

        public object Deserialize(ref ReadOnlySequence<byte> byteSequence, IFormatterResolver formatterResolver)
        {
            return PrimitiveObjectFormatter.Instance.Deserialize(ref byteSequence, formatterResolver);
        }
    }
}

#endif