﻿using System;
using System.Buffers;
using System.Collections.Generic;

namespace MessagePack.Formatters
{
    // Note:This implemenataion is 'not' fastest, should more improve.
    public sealed class EnumAsStringFormatter<T> : IMessagePackFormatter<T>
    {
        readonly Dictionary<string, T> nameValueMapping;
        readonly Dictionary<T, string> valueNameMapping;

        public EnumAsStringFormatter()
        {
            var names = Enum.GetNames(typeof(T));
            var values = Enum.GetValues(typeof(T));

            nameValueMapping = new Dictionary<string, T>(names.Length);
            valueNameMapping = new Dictionary<T, string>(names.Length);

            for (int i = 0; i < names.Length; i++)
            {
                nameValueMapping[names[i]] = (T)values.GetValue(i);
                valueNameMapping[(T)values.GetValue(i)] = names[i];
            }
        }

        public void Serialize(ref BufferWriter writer, T value, IFormatterResolver formatterResolver)
        {
            string name;
            if (!valueNameMapping.TryGetValue(value, out name))
            {
                name = value.ToString(); // fallback for flags etc, But Enum.ToString is too slow.
            }

            MessagePackBinary.WriteString(ref writer, name);
        }

        public T Deserialize(ref ReadOnlySequence<byte> byteSequence, IFormatterResolver formatterResolver)
        {
            var name = MessagePackBinary.ReadString(ref byteSequence);

            T value;
            if (!nameValueMapping.TryGetValue(name, out value))
            {
                value = (T)Enum.Parse(typeof(T), name); // Enum.Parse is too slow
            }
            return value;
        }
    }
}
