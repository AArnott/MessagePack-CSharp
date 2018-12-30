using MessagePack.Formatters;
using Nerdbank.Streams;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace MessagePack.Tests
{
    public class UnsafeFormattersTest
    {
        [Fact]
        public void GuidTest()
        {
            var guid = Guid.NewGuid();
            var sequence = new Sequence<byte>();
            BinaryGuidFormatter.Instance.Serialize(sequence, guid, null);
            sequence.Length.Is(18);

            var sequenceReader = sequence.AsReadOnlySequence;
            var nguid = BinaryGuidFormatter.Instance.Deserialize(ref sequenceReader, null);
            (sequence.Length - sequenceReader.Length).Is(18);

            guid.Is(nguid);
        }

        [Fact]
        public void DecimalTest()
        {
            var d = new Decimal(1341, 53156, 61, true, 3);
            var sequence = new Sequence<byte>();
            BinaryDecimalFormatter.Instance.Serialize(sequence, d, null);
            sequence.Length.Is(18);

            var sequenceReader = sequence.AsReadOnlySequence;
            var nd = BinaryDecimalFormatter.Instance.Deserialize(ref sequenceReader, null);
            (sequence.Length - sequenceReader.Length).Is(18);

            d.Is(nd);
        }
    }
}
