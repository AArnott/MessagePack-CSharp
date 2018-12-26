#if NETSTANDARD || NETFRAMEWORK
using System;
using System.Buffers;

namespace MessagePack.Formatters
{

    public sealed class ValueTupleFormatter<T1> : IMessagePackFormatter<ValueTuple<T1>>
    {
        public void Serialize(IBufferWriter<byte> writer, ValueTuple<T1> value, IFormatterResolver formatterResolver)
        {
            MessagePackBinary.WriteArrayHeader(writer, 1);

            formatterResolver.GetFormatterWithVerify<T1>().Serialize(writer, value.Item1, formatterResolver);
        }

        public ValueTuple<T1> Deserialize(ref ReadOnlySequence<byte> byteSequence, IFormatterResolver formatterResolver)
        {
            if (MessagePackBinary.IsNil(byteSequence))
            {
                throw new InvalidOperationException("Data is Nil, ValueTuple can not be null.");
            }
            else
            {
                var count = MessagePackBinary.ReadArrayHeader(ref byteSequence);
                if (count != 1) throw new InvalidOperationException("Invalid ValueTuple count");

                var item1 = formatterResolver.GetFormatterWithVerify<T1>().Deserialize(ref byteSequence, formatterResolver);
            
                return new ValueTuple<T1>(item1);
            }
        }
    }


    public sealed class ValueTupleFormatter<T1, T2> : IMessagePackFormatter<ValueTuple<T1, T2>>
    {
        public void Serialize(IBufferWriter<byte> writer, ValueTuple<T1, T2> value, IFormatterResolver formatterResolver)
        {
            MessagePackBinary.WriteArrayHeader(writer, 2);

            formatterResolver.GetFormatterWithVerify<T1>().Serialize(writer, value.Item1, formatterResolver);
            formatterResolver.GetFormatterWithVerify<T2>().Serialize(writer, value.Item2, formatterResolver);
        }

        public ValueTuple<T1, T2> Deserialize(ref ReadOnlySequence<byte> byteSequence, IFormatterResolver formatterResolver)
        {
            if (MessagePackBinary.IsNil(byteSequence))
            {
                throw new InvalidOperationException("Data is Nil, ValueTuple can not be null.");
            }
            else
            {
                var count = MessagePackBinary.ReadArrayHeader(ref byteSequence);
                if (count != 2) throw new InvalidOperationException("Invalid ValueTuple count");

                var item1 = formatterResolver.GetFormatterWithVerify<T1>().Deserialize(ref byteSequence, formatterResolver);
                var item2 = formatterResolver.GetFormatterWithVerify<T2>().Deserialize(ref byteSequence, formatterResolver);
            
                return new ValueTuple<T1, T2>(item1, item2);
            }
        }
    }


    public sealed class ValueTupleFormatter<T1, T2, T3> : IMessagePackFormatter<ValueTuple<T1, T2, T3>>
    {
        public void Serialize(IBufferWriter<byte> writer, ValueTuple<T1, T2, T3> value, IFormatterResolver formatterResolver)
        {
            MessagePackBinary.WriteArrayHeader(writer, 3);

            formatterResolver.GetFormatterWithVerify<T1>().Serialize(writer, value.Item1, formatterResolver);
            formatterResolver.GetFormatterWithVerify<T2>().Serialize(writer, value.Item2, formatterResolver);
            formatterResolver.GetFormatterWithVerify<T3>().Serialize(writer, value.Item3, formatterResolver);
        }

        public ValueTuple<T1, T2, T3> Deserialize(ref ReadOnlySequence<byte> byteSequence, IFormatterResolver formatterResolver)
        {
            if (MessagePackBinary.IsNil(byteSequence))
            {
                throw new InvalidOperationException("Data is Nil, ValueTuple can not be null.");
            }
            else
            {
                var count = MessagePackBinary.ReadArrayHeader(ref byteSequence);
                if (count != 3) throw new InvalidOperationException("Invalid ValueTuple count");

                var item1 = formatterResolver.GetFormatterWithVerify<T1>().Deserialize(ref byteSequence, formatterResolver);
                var item2 = formatterResolver.GetFormatterWithVerify<T2>().Deserialize(ref byteSequence, formatterResolver);
                var item3 = formatterResolver.GetFormatterWithVerify<T3>().Deserialize(ref byteSequence, formatterResolver);
            
                return new ValueTuple<T1, T2, T3>(item1, item2, item3);
            }
        }
    }


    public sealed class ValueTupleFormatter<T1, T2, T3, T4> : IMessagePackFormatter<ValueTuple<T1, T2, T3, T4>>
    {
        public void Serialize(IBufferWriter<byte> writer, ValueTuple<T1, T2, T3, T4> value, IFormatterResolver formatterResolver)
        {
            MessagePackBinary.WriteArrayHeader(writer, 4);

            formatterResolver.GetFormatterWithVerify<T1>().Serialize(writer, value.Item1, formatterResolver);
            formatterResolver.GetFormatterWithVerify<T2>().Serialize(writer, value.Item2, formatterResolver);
            formatterResolver.GetFormatterWithVerify<T3>().Serialize(writer, value.Item3, formatterResolver);
            formatterResolver.GetFormatterWithVerify<T4>().Serialize(writer, value.Item4, formatterResolver);
        }

        public ValueTuple<T1, T2, T3, T4> Deserialize(ref ReadOnlySequence<byte> byteSequence, IFormatterResolver formatterResolver)
        {
            if (MessagePackBinary.IsNil(byteSequence))
            {
                throw new InvalidOperationException("Data is Nil, ValueTuple can not be null.");
            }
            else
            {
                var count = MessagePackBinary.ReadArrayHeader(ref byteSequence);
                if (count != 4) throw new InvalidOperationException("Invalid ValueTuple count");

                var item1 = formatterResolver.GetFormatterWithVerify<T1>().Deserialize(ref byteSequence, formatterResolver);
                var item2 = formatterResolver.GetFormatterWithVerify<T2>().Deserialize(ref byteSequence, formatterResolver);
                var item3 = formatterResolver.GetFormatterWithVerify<T3>().Deserialize(ref byteSequence, formatterResolver);
                var item4 = formatterResolver.GetFormatterWithVerify<T4>().Deserialize(ref byteSequence, formatterResolver);
            
                return new ValueTuple<T1, T2, T3, T4>(item1, item2, item3, item4);
            }
        }
    }


    public sealed class ValueTupleFormatter<T1, T2, T3, T4, T5> : IMessagePackFormatter<ValueTuple<T1, T2, T3, T4, T5>>
    {
        public void Serialize(IBufferWriter<byte> writer, ValueTuple<T1, T2, T3, T4, T5> value, IFormatterResolver formatterResolver)
        {
            MessagePackBinary.WriteArrayHeader(writer, 5);

            formatterResolver.GetFormatterWithVerify<T1>().Serialize(writer, value.Item1, formatterResolver);
            formatterResolver.GetFormatterWithVerify<T2>().Serialize(writer, value.Item2, formatterResolver);
            formatterResolver.GetFormatterWithVerify<T3>().Serialize(writer, value.Item3, formatterResolver);
            formatterResolver.GetFormatterWithVerify<T4>().Serialize(writer, value.Item4, formatterResolver);
            formatterResolver.GetFormatterWithVerify<T5>().Serialize(writer, value.Item5, formatterResolver);
        }

        public ValueTuple<T1, T2, T3, T4, T5> Deserialize(ref ReadOnlySequence<byte> byteSequence, IFormatterResolver formatterResolver)
        {
            if (MessagePackBinary.IsNil(byteSequence))
            {
                throw new InvalidOperationException("Data is Nil, ValueTuple can not be null.");
            }
            else
            {
                var count = MessagePackBinary.ReadArrayHeader(ref byteSequence);
                if (count != 5) throw new InvalidOperationException("Invalid ValueTuple count");

                var item1 = formatterResolver.GetFormatterWithVerify<T1>().Deserialize(ref byteSequence, formatterResolver);
                var item2 = formatterResolver.GetFormatterWithVerify<T2>().Deserialize(ref byteSequence, formatterResolver);
                var item3 = formatterResolver.GetFormatterWithVerify<T3>().Deserialize(ref byteSequence, formatterResolver);
                var item4 = formatterResolver.GetFormatterWithVerify<T4>().Deserialize(ref byteSequence, formatterResolver);
                var item5 = formatterResolver.GetFormatterWithVerify<T5>().Deserialize(ref byteSequence, formatterResolver);
            
                return new ValueTuple<T1, T2, T3, T4, T5>(item1, item2, item3, item4, item5);
            }
        }
    }


    public sealed class ValueTupleFormatter<T1, T2, T3, T4, T5, T6> : IMessagePackFormatter<ValueTuple<T1, T2, T3, T4, T5, T6>>
    {
        public void Serialize(IBufferWriter<byte> writer, ValueTuple<T1, T2, T3, T4, T5, T6> value, IFormatterResolver formatterResolver)
        {
            MessagePackBinary.WriteArrayHeader(writer, 6);

            formatterResolver.GetFormatterWithVerify<T1>().Serialize(writer, value.Item1, formatterResolver);
            formatterResolver.GetFormatterWithVerify<T2>().Serialize(writer, value.Item2, formatterResolver);
            formatterResolver.GetFormatterWithVerify<T3>().Serialize(writer, value.Item3, formatterResolver);
            formatterResolver.GetFormatterWithVerify<T4>().Serialize(writer, value.Item4, formatterResolver);
            formatterResolver.GetFormatterWithVerify<T5>().Serialize(writer, value.Item5, formatterResolver);
            formatterResolver.GetFormatterWithVerify<T6>().Serialize(writer, value.Item6, formatterResolver);
        }

        public ValueTuple<T1, T2, T3, T4, T5, T6> Deserialize(ref ReadOnlySequence<byte> byteSequence, IFormatterResolver formatterResolver)
        {
            if (MessagePackBinary.IsNil(byteSequence))
            {
                throw new InvalidOperationException("Data is Nil, ValueTuple can not be null.");
            }
            else
            {
                var count = MessagePackBinary.ReadArrayHeader(ref byteSequence);
                if (count != 6) throw new InvalidOperationException("Invalid ValueTuple count");

                var item1 = formatterResolver.GetFormatterWithVerify<T1>().Deserialize(ref byteSequence, formatterResolver);
                var item2 = formatterResolver.GetFormatterWithVerify<T2>().Deserialize(ref byteSequence, formatterResolver);
                var item3 = formatterResolver.GetFormatterWithVerify<T3>().Deserialize(ref byteSequence, formatterResolver);
                var item4 = formatterResolver.GetFormatterWithVerify<T4>().Deserialize(ref byteSequence, formatterResolver);
                var item5 = formatterResolver.GetFormatterWithVerify<T5>().Deserialize(ref byteSequence, formatterResolver);
                var item6 = formatterResolver.GetFormatterWithVerify<T6>().Deserialize(ref byteSequence, formatterResolver);
            
                return new ValueTuple<T1, T2, T3, T4, T5, T6>(item1, item2, item3, item4, item5, item6);
            }
        }
    }


    public sealed class ValueTupleFormatter<T1, T2, T3, T4, T5, T6, T7> : IMessagePackFormatter<ValueTuple<T1, T2, T3, T4, T5, T6, T7>>
    {
        public void Serialize(IBufferWriter<byte> writer, ValueTuple<T1, T2, T3, T4, T5, T6, T7> value, IFormatterResolver formatterResolver)
        {
            MessagePackBinary.WriteArrayHeader(writer, 7);

            formatterResolver.GetFormatterWithVerify<T1>().Serialize(writer, value.Item1, formatterResolver);
            formatterResolver.GetFormatterWithVerify<T2>().Serialize(writer, value.Item2, formatterResolver);
            formatterResolver.GetFormatterWithVerify<T3>().Serialize(writer, value.Item3, formatterResolver);
            formatterResolver.GetFormatterWithVerify<T4>().Serialize(writer, value.Item4, formatterResolver);
            formatterResolver.GetFormatterWithVerify<T5>().Serialize(writer, value.Item5, formatterResolver);
            formatterResolver.GetFormatterWithVerify<T6>().Serialize(writer, value.Item6, formatterResolver);
            formatterResolver.GetFormatterWithVerify<T7>().Serialize(writer, value.Item7, formatterResolver);
        }

        public ValueTuple<T1, T2, T3, T4, T5, T6, T7> Deserialize(ref ReadOnlySequence<byte> byteSequence, IFormatterResolver formatterResolver)
        {
            if (MessagePackBinary.IsNil(byteSequence))
            {
                throw new InvalidOperationException("Data is Nil, ValueTuple can not be null.");
            }
            else
            {
                var count = MessagePackBinary.ReadArrayHeader(ref byteSequence);
                if (count != 7) throw new InvalidOperationException("Invalid ValueTuple count");

                var item1 = formatterResolver.GetFormatterWithVerify<T1>().Deserialize(ref byteSequence, formatterResolver);
                var item2 = formatterResolver.GetFormatterWithVerify<T2>().Deserialize(ref byteSequence, formatterResolver);
                var item3 = formatterResolver.GetFormatterWithVerify<T3>().Deserialize(ref byteSequence, formatterResolver);
                var item4 = formatterResolver.GetFormatterWithVerify<T4>().Deserialize(ref byteSequence, formatterResolver);
                var item5 = formatterResolver.GetFormatterWithVerify<T5>().Deserialize(ref byteSequence, formatterResolver);
                var item6 = formatterResolver.GetFormatterWithVerify<T6>().Deserialize(ref byteSequence, formatterResolver);
                var item7 = formatterResolver.GetFormatterWithVerify<T7>().Deserialize(ref byteSequence, formatterResolver);
            
                return new ValueTuple<T1, T2, T3, T4, T5, T6, T7>(item1, item2, item3, item4, item5, item6, item7);
            }
        }
    }


    public sealed class ValueTupleFormatter<T1, T2, T3, T4, T5, T6, T7, TRest> : IMessagePackFormatter<ValueTuple<T1, T2, T3, T4, T5, T6, T7, TRest>> where TRest : struct
    {
        public void Serialize(IBufferWriter<byte> writer, ValueTuple<T1, T2, T3, T4, T5, T6, T7, TRest> value, IFormatterResolver formatterResolver)
        {
            MessagePackBinary.WriteArrayHeader(writer, 8);

            formatterResolver.GetFormatterWithVerify<T1>().Serialize(writer, value.Item1, formatterResolver);
            formatterResolver.GetFormatterWithVerify<T2>().Serialize(writer, value.Item2, formatterResolver);
            formatterResolver.GetFormatterWithVerify<T3>().Serialize(writer, value.Item3, formatterResolver);
            formatterResolver.GetFormatterWithVerify<T4>().Serialize(writer, value.Item4, formatterResolver);
            formatterResolver.GetFormatterWithVerify<T5>().Serialize(writer, value.Item5, formatterResolver);
            formatterResolver.GetFormatterWithVerify<T6>().Serialize(writer, value.Item6, formatterResolver);
            formatterResolver.GetFormatterWithVerify<T7>().Serialize(writer, value.Item7, formatterResolver);
            formatterResolver.GetFormatterWithVerify<TRest>().Serialize(writer, value.Rest, formatterResolver);
        }

        public ValueTuple<T1, T2, T3, T4, T5, T6, T7, TRest> Deserialize(ref ReadOnlySequence<byte> byteSequence, IFormatterResolver formatterResolver)
        {
            if (MessagePackBinary.IsNil(byteSequence))
            {
                throw new InvalidOperationException("Data is Nil, ValueTuple can not be null.");
            }
            else
            {
                var count = MessagePackBinary.ReadArrayHeader(ref byteSequence);
                if (count != 8) throw new InvalidOperationException("Invalid ValueTuple count");

                var item1 = formatterResolver.GetFormatterWithVerify<T1>().Deserialize(ref byteSequence, formatterResolver);
                var item2 = formatterResolver.GetFormatterWithVerify<T2>().Deserialize(ref byteSequence, formatterResolver);
                var item3 = formatterResolver.GetFormatterWithVerify<T3>().Deserialize(ref byteSequence, formatterResolver);
                var item4 = formatterResolver.GetFormatterWithVerify<T4>().Deserialize(ref byteSequence, formatterResolver);
                var item5 = formatterResolver.GetFormatterWithVerify<T5>().Deserialize(ref byteSequence, formatterResolver);
                var item6 = formatterResolver.GetFormatterWithVerify<T6>().Deserialize(ref byteSequence, formatterResolver);
                var item7 = formatterResolver.GetFormatterWithVerify<T7>().Deserialize(ref byteSequence, formatterResolver);
                var item8 = formatterResolver.GetFormatterWithVerify<TRest>().Deserialize(ref byteSequence, formatterResolver);
            
                return new ValueTuple<T1, T2, T3, T4, T5, T6, T7, TRest>(item1, item2, item3, item4, item5, item6, item7, item8);
            }
        }
    }

}
#endif