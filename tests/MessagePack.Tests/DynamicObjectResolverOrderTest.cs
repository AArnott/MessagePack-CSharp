using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace MessagePack.Tests
{
    [MessagePack.MessagePackObject(keyAsPropertyName: true)]
    public abstract class AbstractBase
    {
        [DataMember(Order = 0)]
        public UInt32 Id = 0xaa00aa00;
    }

    public sealed class RealClass : AbstractBase
    {
        public String Str;
    }

    [MessagePack.MessagePackObject(keyAsPropertyName: true)]
    public class OrderOrder
    {
        [DataMember(Order = 5)]
        public int Foo { get; set; }

        [DataMember(Order = 2)]
        public int Moge { get; set; }

        [DataMember(Order = 10)]
        public int FooBar;

        public string NoBar;

        [DataMember(Order = 0)]
        public string Bar;
    }

    public class DynamicObjectResolverOrderTest
    {
        private MessagePackSerializer serializer = new MessagePackSerializer();

        IEnumerable<string> IteratePropertyNames(ReadOnlyMemory<byte> bytes)
        {
            var byteSequence = new ReadOnlySequence<byte>(bytes);
            var mapCount = MessagePackBinary.ReadMapHeader(ref byteSequence);
            for (int i = 0; i < mapCount; i++)
            {
                yield return MessagePackBinary.ReadString(ref byteSequence);
                MessagePackBinary.ReadNext(ref byteSequence);
            }
        }

        [Fact]
        public void OrderTest()
        {
            var msgRawData = serializer.Serialize(new OrderOrder());
            IteratePropertyNames(msgRawData).Is("Bar", "Moge", "Foo", "FooBar", "NoBar");
        }

        [Fact]
        public void InheritIterateOrder()
        {
            RealClass realClass = new RealClass { Str = "X" };
            var msgRawData = serializer.Serialize(realClass);

            IteratePropertyNames(msgRawData).Is("Id", "Str");
        }
    }
}
