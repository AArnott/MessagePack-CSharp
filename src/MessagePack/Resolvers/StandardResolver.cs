using MessagePack.Formatters;
using System.Linq;
using MessagePack.Internal;
using MessagePack.Resolvers;

namespace MessagePack.Resolvers
{
    /// <summary>
    /// Default composited resolver, builtin -> attribute -> dynamic enum -> dynamic generic -> dynamic union -> dynamic object -> primitive.
    /// </summary>
    public sealed class StandardResolver : IFormatterResolver
    {
        public static readonly IFormatterResolver Instance = new StandardResolver();

        public static readonly IMessagePackFormatter<object> ObjectFallbackFormatter = new DynamicObjectTypeFallbackFormatter(StandardResolverCore.Instance);

        StandardResolver()
        {
        }

        public IMessagePackFormatter<T> GetFormatter<T>()
        {
            return FormatterCache<T>.formatter;
        }

        static class FormatterCache<T>
        {
            public static readonly IMessagePackFormatter<T> formatter;

            static FormatterCache()
            {
                if (typeof(T) == typeof(object))
                {
                    // final fallback
                    formatter = (IMessagePackFormatter<T>)ObjectFallbackFormatter;
                }
                else
                {
                    formatter = StandardResolverCore.Instance.GetFormatter<T>();
                }
            }
        }
    }

    public sealed class ContractlessStandardResolver : IFormatterResolver
    {
        public static readonly IFormatterResolver Instance = new ContractlessStandardResolver();

        public static readonly IMessagePackFormatter<object> ObjectFallbackFormatter = new DynamicObjectTypeFallbackFormatter(ContractlessStandardResolverCore.Instance);

        ContractlessStandardResolver()
        {
        }

        public IMessagePackFormatter<T> GetFormatter<T>()
        {
            return FormatterCache<T>.formatter;
        }

        static class FormatterCache<T>
        {
            public static readonly IMessagePackFormatter<T> formatter;

            static FormatterCache()
            {
                if (typeof(T) == typeof(object))
                {
                    // final fallback
                    formatter = (IMessagePackFormatter<T>)ObjectFallbackFormatter;
                }
                else
                {
                    formatter = ContractlessStandardResolverCore.Instance.GetFormatter<T>();
                }
            }
        }
    }

    public sealed class StandardResolverAllowPrivate : IFormatterResolver
    {
        public static readonly IFormatterResolver Instance = new StandardResolverAllowPrivate();

        public static readonly IMessagePackFormatter<object> ObjectFallbackFormatter = new DynamicObjectTypeFallbackFormatter(StandardResolverAllowPrivateCore.Instance);

        StandardResolverAllowPrivate()
        {
        }

        public IMessagePackFormatter<T> GetFormatter<T>()
        {
            return FormatterCache<T>.formatter;
        }

        static class FormatterCache<T>
        {
            public static readonly IMessagePackFormatter<T> formatter;

            static FormatterCache()
            {
                if (typeof(T) == typeof(object))
                {
                    // final fallback
                    formatter = (IMessagePackFormatter<T>)ObjectFallbackFormatter;
                }
                else
                {
                    formatter = StandardResolverAllowPrivateCore.Instance.GetFormatter<T>();
                }
            }
        }
    }

    public sealed class ContractlessStandardResolverAllowPrivate : IFormatterResolver
    {
        public static readonly IFormatterResolver Instance = new ContractlessStandardResolverAllowPrivate();

        public static readonly IMessagePackFormatter<object> ObjectFallbackFormatter = new DynamicObjectTypeFallbackFormatter(ContractlessStandardResolverAllowPrivateCore.Instance);

        ContractlessStandardResolverAllowPrivate()
        {
        }

        public IMessagePackFormatter<T> GetFormatter<T>()
        {
            return FormatterCache<T>.formatter;
        }

        static class FormatterCache<T>
        {
            public static readonly IMessagePackFormatter<T> formatter;

            static FormatterCache()
            {
                if (typeof(T) == typeof(object))
                {
                    // final fallback
                    formatter = (IMessagePackFormatter<T>)ObjectFallbackFormatter;
                }
                else
                {
                    formatter = ContractlessStandardResolverAllowPrivateCore.Instance.GetFormatter<T>();
                }
            }
        }
    }
}

namespace MessagePack.Internal
{
    internal static class StandardResolverHelper
    {
        public static readonly IFormatterResolver[] DefaultResolvers = new[]
        {
            BuiltinResolver.Instance, // Try Builtin
            AttributeFormatterResolver.Instance, // Try use [MessagePackFormatter]

#if !ENABLE_IL2CPP && !UNITY_WSA

            DynamicEnumResolver.Instance, // Try Enum
            DynamicGenericResolver.Instance, // Try Array, Tuple, Collection
            DynamicUnionResolver.Instance, // Try Union(Interface)
#endif
        };
    }

    internal sealed class StandardResolverCore : IFormatterResolver
    {
        public static readonly IFormatterResolver Instance = new StandardResolverCore();

        static readonly IFormatterResolver[] resolvers = StandardResolverHelper.DefaultResolvers.Concat(new IFormatterResolver[]
        {
#if !ENABLE_IL2CPP && !UNITY_WSA
            DynamicObjectResolver.Instance, // Try Object
#endif
        }).ToArray();

        StandardResolverCore()
        {
        }

        public IMessagePackFormatter<T> GetFormatter<T>()
        {
            return FormatterCache<T>.formatter;
        }

        static class FormatterCache<T>
        {
            public static readonly IMessagePackFormatter<T> formatter;

            static FormatterCache()
            {
                foreach (var item in resolvers)
                {
                    var f = item.GetFormatter<T>();
                    if (f != null)
                    {
                        formatter = f;
                        return;
                    }
                }
            }
        }
    }

    internal sealed class ContractlessStandardResolverCore : IFormatterResolver
    {
        public static readonly IFormatterResolver Instance = new ContractlessStandardResolverCore();

        static readonly IFormatterResolver[] resolvers = StandardResolverHelper.DefaultResolvers.Concat(new IFormatterResolver[]
        {
#if !ENABLE_IL2CPP && !UNITY_WSA
            DynamicObjectResolver.Instance, // Try Object
            DynamicContractlessObjectResolver.Instance, // Serializes keys as strings
#endif
        }).ToArray();


        ContractlessStandardResolverCore()
        {
        }

        public IMessagePackFormatter<T> GetFormatter<T>()
        {
            return FormatterCache<T>.formatter;
        }

        static class FormatterCache<T>
        {
            public static readonly IMessagePackFormatter<T> formatter;

            static FormatterCache()
            {
                foreach (var item in resolvers)
                {
                    var f = item.GetFormatter<T>();
                    if (f != null)
                    {
                        formatter = f;
                        return;
                    }
                }
            }
        }
    }

    internal sealed class StandardResolverAllowPrivateCore : IFormatterResolver
    {
        public static readonly IFormatterResolver Instance = new StandardResolverAllowPrivateCore();

        static readonly IFormatterResolver[] resolvers = StandardResolverHelper.DefaultResolvers.Concat(new IFormatterResolver[]
        {
#if !ENABLE_IL2CPP && !UNITY_WSA
            DynamicObjectResolverAllowPrivate.Instance, // Try Object
#endif
        }).ToArray();

        StandardResolverAllowPrivateCore()
        {
        }

        public IMessagePackFormatter<T> GetFormatter<T>()
        {
            return FormatterCache<T>.formatter;
        }

        static class FormatterCache<T>
        {
            public static readonly IMessagePackFormatter<T> formatter;

            static FormatterCache()
            {
                foreach (var item in resolvers)
                {
                    var f = item.GetFormatter<T>();
                    if (f != null)
                    {
                        formatter = f;
                        return;
                    }
                }
            }
        }
    }

    internal sealed class ContractlessStandardResolverAllowPrivateCore : IFormatterResolver
    {
        public static readonly IFormatterResolver Instance = new ContractlessStandardResolverAllowPrivateCore();

        static readonly IFormatterResolver[] resolvers = StandardResolverHelper.DefaultResolvers.Concat(new IFormatterResolver[]
        {
#if !ENABLE_IL2CPP && !UNITY_WSA
            DynamicObjectResolverAllowPrivate.Instance, // Try Object
            DynamicContractlessObjectResolverAllowPrivate.Instance, // Serializes keys as strings
#endif
        }).ToArray();


        ContractlessStandardResolverAllowPrivateCore()
        {
        }

        public IMessagePackFormatter<T> GetFormatter<T>()
        {
            return FormatterCache<T>.formatter;
        }

        static class FormatterCache<T>
        {
            public static readonly IMessagePackFormatter<T> formatter;

            static FormatterCache()
            {
                foreach (var item in resolvers)
                {
                    var f = item.GetFormatter<T>();
                    if (f != null)
                    {
                        formatter = f;
                        return;
                    }
                }
            }
        }
    }
}
