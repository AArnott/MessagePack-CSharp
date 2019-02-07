#if !UNITY

using MessagePack.Internal;
using System;
using System.Buffers;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Text.RegularExpressions;

namespace MessagePack.Formatters
{
    /// <summary>
    /// For `object` field that holds derived from `object` value, ex: var arr = new object[] { 1, "a", new Model() };
    /// </summary>
    public sealed class TypelessFormatter : IMessagePackFormatter<object>
    {
        public const sbyte ExtensionTypeCode = 100;

        static readonly Regex SubtractFullNameRegex = new Regex(@", Version=\d+.\d+.\d+.\d+, Culture=\w+, PublicKeyToken=\w+", RegexOptions.Compiled);

        delegate void SerializeMethod(object dynamicContractlessFormatter, ref BufferWriter writer, object value, IFormatterResolver formatterResolver);
        delegate object DeserializeMethod(object dynamicContractlessFormatter, ref ReadOnlySequence<byte> byteSequence, IFormatterResolver formatterResolver);

        readonly ThreadsafeTypeKeyHashTable<KeyValuePair<object, SerializeMethod>> serializers = new ThreadsafeTypeKeyHashTable<KeyValuePair<object, SerializeMethod>>();
        readonly ThreadsafeTypeKeyHashTable<KeyValuePair<object, DeserializeMethod>> deserializers = new ThreadsafeTypeKeyHashTable<KeyValuePair<object, DeserializeMethod>>();
        readonly ThreadsafeTypeKeyHashTable<byte[]> typeNameCache = new ThreadsafeTypeKeyHashTable<byte[]>();
        readonly AsymmetricKeyHashTable<byte[], ArraySegment<byte>, Type> typeCache = new AsymmetricKeyHashTable<byte[], ArraySegment<byte>, Type>(new StringArraySegmentByteAscymmetricEqualityComparer());

        static readonly HashSet<string> blacklistCheck = new HashSet<string>
        {
            "System.CodeDom.Compiler.TempFileCollection",
            "System.IO.FileSystemInfo",
            "System.Management.IWbemClassObjectFreeThreaded"
        };
        static readonly HashSet<Type> useBuiltinTypes = new HashSet<Type>
        {
            typeof(Boolean),
            // typeof(Char),
            typeof(SByte),
            typeof(Byte),
            typeof(Int16),
            typeof(UInt16),
            typeof(Int32),
            typeof(UInt32),
            typeof(Int64),
            typeof(UInt64),
            typeof(Single),
            typeof(Double),
            typeof(string),
            typeof(byte[]),

            // array should save there types.
            //typeof(Boolean[]),
            //typeof(Char[]),
            //typeof(SByte[]),
            //typeof(Int16[]),
            //typeof(UInt16[]),
            //typeof(Int32[]),
            //typeof(UInt32[]),
            //typeof(Int64[]),
            //typeof(UInt64[]),
            //typeof(Single[]),
            //typeof(Double[]),
            //typeof(string[]),

            typeof(Boolean?),
            // typeof(Char?),
            typeof(SByte?),
            typeof(Byte?),
            typeof(Int16?),
            typeof(UInt16?),
            typeof(Int32?),
            typeof(UInt32?),
            typeof(Int64?),
            typeof(UInt64?),
            typeof(Single?),
            typeof(Double?),
        };

        public Func<string, Type> BindToType { get; set; } = (string typeName) => Type.GetType(typeName, false);

        // mscorlib or System.Private.CoreLib
        static readonly bool isMscorlib = typeof(int).AssemblyQualifiedName.Contains("mscorlib");

        /// <summary>
        /// Gets or sets a value indicating whether to exclude assembly qualifiers from type names.
        /// </summary>
        /// <value>The default value is <c>true</c>.</value>
        public bool RemoveAssemblyVersion { get; set; } = true;

        public TypelessFormatter()
        {
            serializers.TryAdd(typeof(object), _ => new KeyValuePair<object, SerializeMethod>(null, (object p1, ref BufferWriter p2, object p3, IFormatterResolver p4) => { }));
            deserializers.TryAdd(typeof(object), _ => new KeyValuePair<object, DeserializeMethod>(null, (object p1, ref ReadOnlySequence<byte> p2, IFormatterResolver p3) => new object()));
        }

        // see:http://msdn.microsoft.com/en-us/library/w3f99sx1.aspx
        // subtract Version, Culture and PublicKeyToken from AssemblyQualifiedName 
        private string BuildTypeName(Type type)
        {
            if (RemoveAssemblyVersion)
            {
                string full = type.AssemblyQualifiedName;

                var shortened = SubtractFullNameRegex.Replace(full, "");
                if (Type.GetType(shortened, false) == null)
                {
                    // if type cannot be found with shortened name - use full name
                    shortened = full;
                }

                return shortened;
            }
            else
            {
                return type.AssemblyQualifiedName;
            }
        }

        public void Serialize(ref BufferWriter writer, object value, IFormatterResolver formatterResolver)
        {
            if (value == null)
            {
                MessagePackBinary.WriteNil(ref writer);
                return;
            }

            var type = value.GetType();

            byte[] typeName;
            if (!typeNameCache.TryGetValue(type, out typeName))
            {
                if (blacklistCheck.Contains(type.FullName))
                {
                    throw new InvalidOperationException("Type is in blacklist:" + type.FullName);
                }

                var ti = type.GetTypeInfo();
                if (ti.IsAnonymous() || useBuiltinTypes.Contains(type))
                {
                    typeName = null;
                }
                else
                {
                    typeName = StringEncoding.UTF8.GetBytes(BuildTypeName(type));
                }
                typeNameCache.TryAdd(type, typeName);
            }

            if (typeName == null)
            {
                Resolvers.TypelessFormatterFallbackResolver.Instance.GetFormatter<object>().Serialize(ref writer, value, formatterResolver);
                return;
            }

            // don't use GetOrAdd for avoid closure capture.
            KeyValuePair<object, SerializeMethod> formatterAndDelegate;
            if (!serializers.TryGetValue(type, out formatterAndDelegate))
            {
                lock (serializers) // double check locking...
                {
                    if (!serializers.TryGetValue(type, out formatterAndDelegate))
                    {
                        var ti = type.GetTypeInfo();

                        var formatter = formatterResolver.GetFormatterDynamic(type);
                        if (formatter == null)
                        {
                            throw new FormatterNotRegisteredException(type.FullName + " is not registered in this resolver. resolver:" + formatterResolver.GetType().Name);
                        }

                        var formatterType = typeof(IMessagePackFormatter<>).MakeGenericType(type);
                        var param0 = Expression.Parameter(typeof(object), "formatter");
                        var param1 = Expression.Parameter(typeof(BufferWriter).MakeByRefType(), "writer");
                        var param2 = Expression.Parameter(typeof(object), "value");
                        var param3 = Expression.Parameter(typeof(IFormatterResolver), "formatterResolver");

                        var serializeMethodInfo = formatterType.GetRuntimeMethod("Serialize", new[] { typeof(BufferWriter).MakeByRefType(), type, typeof(IFormatterResolver) });

                        var body = Expression.Call(
                            Expression.Convert(param0, formatterType),
                            serializeMethodInfo,
                            param1,
                            ti.IsValueType ? Expression.Unbox(param2, type) : Expression.Convert(param2, type),
                            param3);

                        var lambda = Expression.Lambda<SerializeMethod>(body, param0, param1, param2, param3).Compile();

                        formatterAndDelegate = new KeyValuePair<object, SerializeMethod>(formatter, lambda);
                        serializers.TryAdd(type, formatterAndDelegate);
                    }
                }
            }

            // mark will be written at the end, when size is known
            using (var sequence = new Nerdbank.Streams.Sequence<byte>())
            {
                var sequenceWriter = new BufferWriter(sequence);
                MessagePackBinary.WriteStringBytes(ref sequenceWriter, typeName);
                formatterAndDelegate.Value(formatterAndDelegate.Key, ref sequenceWriter, value, formatterResolver);
                sequenceWriter.Commit();

                // mark as extension with code 100
                MessagePackBinary.WriteExtensionFormatHeaderForceExt32Block(ref writer, (sbyte)TypelessFormatter.ExtensionTypeCode, (int)sequence.Length);
                var copySpan = writer.GetSpan((int)sequence.Length);
                sequence.AsReadOnlySequence.CopyTo(copySpan);
                writer.Advance((int)sequence.Length);
            }
        }

        public object Deserialize(ref ReadOnlySequence<byte> byteSequence, IFormatterResolver formatterResolver)
        {
            if (MessagePackBinary.IsNil(byteSequence))
            {
                byteSequence = byteSequence.Slice(1);
                return null;
            }

            var packType = MessagePackBinary.GetMessagePackType(byteSequence);
            if (packType == MessagePackType.Extension)
            {
                var ext = MessagePackBinary.ReadExtensionFormatHeader(ref byteSequence);
                if (ext.TypeCode == TypelessFormatter.ExtensionTypeCode)
                {
                    // it has type name serialized
                    var typeName = MessagePackBinary.ReadStringSegment(ref byteSequence);
                    var result = DeserializeByTypeName(typeName, ref byteSequence, formatterResolver);
                    return result;
                }
            }

            // fallback
            return Resolvers.TypelessFormatterFallbackResolver.Instance.GetFormatter<object>().Deserialize(ref byteSequence, formatterResolver);
        }

        /// <summary>
        /// Does not support deserializing of anonymous types
        /// Type should be covered by preceeding resolvers in complex/standard resolver
        /// </summary>
        private object DeserializeByTypeName(ArraySegment<byte> typeName, ref ReadOnlySequence<byte> byteSequence, IFormatterResolver formatterResolver)
        {
            // try get type with assembly name, throw if not found
            Type type;
            if (!typeCache.TryGetValue(typeName, out type))
            {
                var buffer = new byte[typeName.Count];
                Buffer.BlockCopy(typeName.Array, typeName.Offset, buffer, 0, buffer.Length);
                var str = StringEncoding.UTF8.GetString(buffer);
                type = BindToType(str);
                if (type == null)
                {
                    if (isMscorlib && str.Contains("System.Private.CoreLib"))
                    {
                        str = str.Replace("System.Private.CoreLib", "mscorlib");
                        type = Type.GetType(str, true); // throw
                    }
                    else if (!isMscorlib && str.Contains("mscorlib"))
                    {
                        str = str.Replace("mscorlib", "System.Private.CoreLib");
                        type = Type.GetType(str, true); // throw
                    }
                    else
                    {
                        type = Type.GetType(str, true); // re-throw
                    }
                }
                typeCache.TryAdd(buffer, type);
            }

            KeyValuePair<object, DeserializeMethod> formatterAndDelegate;
            if (!deserializers.TryGetValue(type, out formatterAndDelegate))
            {
                lock (deserializers)
                {
                    if (!deserializers.TryGetValue(type, out formatterAndDelegate))
                    {
                        var ti = type.GetTypeInfo();

                        var formatter = formatterResolver.GetFormatterDynamic(type);
                        if (formatter == null)
                        {
                            throw new FormatterNotRegisteredException(type.FullName + " is not registered in this resolver. resolver:" + formatterResolver.GetType().Name);
                        }

                        var formatterType = typeof(IMessagePackFormatter<>).MakeGenericType(type);
                        var param0 = Expression.Parameter(typeof(object), "formatter");
                        var param1 = Expression.Parameter(typeof(ReadOnlySequence<byte>).MakeByRefType(), "byteSequence");
                        var param2 = Expression.Parameter(typeof(IFormatterResolver), "formatterResolver");

                        var deserializeMethodInfo = formatterType.GetRuntimeMethod("Deserialize", new[] { typeof(ReadOnlySequence<byte>).MakeByRefType(), typeof(IFormatterResolver) });

                        var deserialize = Expression.Call(
                            Expression.Convert(param0, formatterType),
                            deserializeMethodInfo,
                            param1,
                            param2);

                        Expression body = deserialize;
                        if (ti.IsValueType)
                            body = Expression.Convert(deserialize, typeof(object));
                        var lambda = Expression.Lambda<DeserializeMethod>(body, param0, param1, param2).Compile();

                        formatterAndDelegate = new KeyValuePair<object, DeserializeMethod>(formatter, lambda);
                        deserializers.TryAdd(type, formatterAndDelegate);
                    }
                }
            }

            return formatterAndDelegate.Value(formatterAndDelegate.Key, ref byteSequence, formatterResolver);
        }
    }

}

#endif