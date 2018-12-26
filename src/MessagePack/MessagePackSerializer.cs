using System;
using System.Buffers;
using System.IO;
using System.Threading.Tasks;
using MessagePack.Internal;

namespace MessagePack
{
    /// <summary>
    /// High-Level API of MessagePack for C#.
    /// </summary>
    public partial class MessagePackSerializer
    {
        /// <summary>
        /// Backing field for the <see cref="DefaultResolver"/> property.
        /// </summary>
        private IFormatterResolver defaultResolver;

        /// <summary>
        /// Initializes a new instance of the <see cref="MessagePackSerializer"/> class
        /// initialized with the <see cref="Resolvers.StandardResolver"/>.
        /// </summary>
        public MessagePackSerializer()
            : this(Resolvers.StandardResolver.Instance)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MessagePackSerializer"/> class
        /// </summary>
        /// <param name="defaultResolver">The resolver to use.</param>
        public MessagePackSerializer(IFormatterResolver defaultResolver)
        {
            this.defaultResolver = defaultResolver ?? throw new ArgumentNullException(nameof(defaultResolver));
        }

        /// <summary>
        /// Gets or sets the resolver to use when one is not explicitly specified.
        /// </summary>
        public IFormatterResolver DefaultResolver
        {
            get => this.defaultResolver;
            ////set => this.defaultResolver = value ?? throw new ArgumentNullException(nameof(value));
        }

        /// <summary>
        /// Serializes a given value with the specified buffer writer.
        /// </summary>
        /// <param name="writer">The buffer writer to serialize with.</param>
        /// <param name="value">The value to serialize.</param>
        /// <param name="resolver">The resolver to use during deserialization. Use <c>null</c> to use the <see cref="DefaultResolver"/>.</param>
        public virtual void Serialize<T>(IBufferWriter<byte> writer, T value, IFormatterResolver resolver)
        {
            if (writer == null)
            {
                throw new ArgumentNullException(nameof(writer));
            }

            if (resolver == null)
            {
                resolver = this.DefaultResolver;
            }

            var formatter = resolver.GetFormatterWithVerify<T>();
            formatter.Serialize(writer, value, resolver);
        }

        /// <summary>
        /// Deserializes a value of a given type from a sequence of bytes.
        /// </summary>
        /// <typeparam name="T">The type of value to deserialize.</typeparam>
        /// <param name="byteSequence">The sequence to deserialize from.</param>
        /// <param name="resolver">The resolver to use during deserialization. Use <c>null</c> to use the <see cref="DefaultResolver"/>.</param>
        /// <param name="endPosition">The position in the <paramref name="byteSequence"/> just after the last read byte.</param>
        /// <returns>The deserialized value.</returns>
        public virtual T Deserialize<T>(ReadOnlySequence<byte> byteSequence, IFormatterResolver resolver, out SequencePosition endPosition)
        {
            if (resolver == null)
            {
                resolver = this.DefaultResolver;
            }

            return resolver.GetFormatterWithVerify<T>().Deserialize(byteSequence, resolver, out endPosition);
        }

        /// <summary>
        /// Serializes a given value with the specified buffer writer.
        /// </summary>
        /// <param name="value">The value to serialize.</param>
        /// <param name="resolver">The resolver to use during deserialization. Use <c>null</c> to use the <see cref="DefaultResolver"/>.</param>
        /// <returns>A byte array with the serialized value.</returns>
        public byte[] Serialize<T>(T value, IFormatterResolver resolver = null)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Serializes a given value to the specified stream.
        /// </summary>
        /// <param name="stream">The stream to serialize to.</param>
        /// <param name="value">The value to serialize.</param>
        /// <param name="resolver">The resolver to use during deserialization. Use <c>null</c> to use the <see cref="DefaultResolver"/>.</param>
        public void Serialize<T>(Stream stream, T value, IFormatterResolver resolver = null)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Serializes a given value to the specified stream.
        /// </summary>
        /// <param name="stream">The stream to serialize to.</param>
        /// <param name="value">The value to serialize.</param>
        /// <param name="resolver">The resolver to use during deserialization. Use <c>null</c> to use the <see cref="DefaultResolver"/>.</param>
        /// <returns>A task that completes with the result of the async serialization operation.</returns>
        public Task SerializeAsync<T>(Stream stream, T value, IFormatterResolver resolver = null)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Deserializes a value of a given type from a sequence of bytes.
        /// </summary>
        /// <typeparam name="T">The type of value to deserialize.</typeparam>
        /// <param name="buffer">The buffer to deserialize from.</param>
        /// <param name="resolver">The resolver to use during deserialization. Use <c>null</c> to use the <see cref="DefaultResolver"/>.</param>
        /// <returns>The deserialized value.</returns>
        public T Deserialize<T>(Memory<byte> buffer, IFormatterResolver resolver = null) => this.Deserialize<T>(new ReadOnlySequence<byte>(buffer), resolver, out SequencePosition endPosition);

        /// <summary>
        /// Deserializes a value of a given type from a sequence of bytes.
        /// </summary>
        /// <typeparam name="T">The type of value to deserialize.</typeparam>
        /// <param name="buffer">The array to deserialize from.</param>
        /// <param name="offset">The position in the array to start deserialization.</param>
        /// <param name="resolver">The resolver to use during deserialization. Use <c>null</c> to use the <see cref="DefaultResolver"/>.</param>
        /// <returns>The deserialized value.</returns>
        public T Deserialize<T>(byte[] buffer, int offset = 0, IFormatterResolver resolver = null)
        {
            if (buffer == null)
            {
                throw new ArgumentNullException(nameof(buffer));
            }

            var sequence = new ReadOnlySequence<byte>(buffer.AsMemory(offset));
            return Deserialize<T>(sequence, resolver, out SequencePosition endPosition);
        }

        /// <summary>
        /// Deserializes a value of a given type from a sequence of bytes.
        /// </summary>
        /// <typeparam name="T">The type of value to deserialize.</typeparam>
        /// <param name="buffer">The array to deserialize from.</param>
        /// <param name="offset">The position in the array to start deserialization.</param>
        /// <param name="resolver">The resolver to use during deserialization. Use <c>null</c> to use the <see cref="DefaultResolver"/>.</param>
        /// <param name="bytesRead">The number of bytes read.</param>
        /// <returns>The deserialized value.</returns>
        public T Deserialize<T>(byte[] buffer, int offset, IFormatterResolver resolver, out int bytesRead)
        {
            if (buffer == null)
            {
                throw new ArgumentNullException(nameof(buffer));
            }

            var sequence = new ReadOnlySequence<byte>(buffer.AsMemory(offset));
            T result = Deserialize<T>(sequence, resolver, out SequencePosition endPosition);
            bytesRead = endPosition.GetInteger(); // we know the sequence has just one segment, so this is safe.
            return result;
        }

        /// <summary>
        /// Deserializes a value of a given type from a stream.
        /// </summary>
        /// <typeparam name="T">The type of value to deserialize.</typeparam>
        /// <param name="stream">The stream to deserialize from.</param>
        /// <param name="resolver">The resolver to use during deserialization. Use <c>null</c> to use the <see cref="DefaultResolver"/>.</param>
        /// <returns>The deserialized value.</returns>
        public T Deserialize<T>(Stream stream, IFormatterResolver resolver = null)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Deserializes a value of a given type from a stream.
        /// </summary>
        /// <typeparam name="T">The type of value to deserialize.</typeparam>
        /// <param name="stream">The stream to deserialize from.</param>
        /// <param name="resolver">The resolver to use during deserialization. Use <c>null</c> to use the <see cref="DefaultResolver"/>.</param>
        /// <returns>The deserialized value.</returns>
        public ValueTask<T> DeserializeAsync<T>(Stream stream, IFormatterResolver resolver = null)
        {
            throw new NotImplementedException();
        }
    }
}

namespace MessagePack.Internal
{
    // TODO: remove this?
    internal static class InternalMemoryPool
    {
        [ThreadStatic]
        private static byte[] buffer = null;

        public static byte[] GetBuffer()
        {
            if (buffer == null)
            {
                buffer = new byte[65536];
            }
            return buffer;
        }
    }
}