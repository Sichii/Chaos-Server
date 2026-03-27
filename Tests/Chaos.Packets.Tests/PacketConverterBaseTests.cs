#region
using System.Text;
using Chaos.IO.Memory;
using Chaos.Packets.Abstractions;
using FluentAssertions;
#endregion

namespace Chaos.Packets.Tests;

public sealed class PacketConverterBaseTests
{
    [Test]
    public void Deserialize_NonGeneric_ForwardsToGeneric()
    {
        var converter = new TestConverter();
        IPacketConverter nonGeneric = converter;
        var bytes = new byte[4];
        var span = bytes.AsSpan();
        var reader = new SpanReader(Encoding.ASCII, span);

        var result = nonGeneric.Deserialize(ref reader);

        converter.DeserializeCalled
                 .Should()
                 .BeTrue();

        result.Should()
              .BeOfType<TestSerializable>();
    }

    [Test]
    public void Serialize_NonGeneric_ForwardsToGenericWithCast()
    {
        var converter = new TestConverter();
        IPacketConverter nonGeneric = converter;
        var writer = new SpanWriter(Encoding.ASCII);
        var args = new TestSerializable();

        nonGeneric.Serialize(ref writer, args);

        converter.SerializeCalled
                 .Should()
                 .BeTrue();

        writer.Dispose();
    }

    [Test]
    public void Serialize_NonGeneric_ThrowsWhenArgIsWrongType()
    {
        var converter = new TestConverter();
        IPacketConverter nonGeneric = converter;
        var writer = new SpanWriter(Encoding.ASCII);
        var threw = false;

        try
        {
            nonGeneric.Serialize(ref writer, "not a TestSerializable");
        } catch (InvalidCastException)
        {
            threw = true;
        } finally
        {
            writer.Dispose();
        }

        threw.Should()
             .BeTrue();
    }

    private sealed class TestConverter : PacketConverterBase<TestSerializable>
    {
        public bool DeserializeCalled { get; private set; }
        public bool SerializeCalled { get; private set; }

        public override byte OpCode => 0x42;

        public override TestSerializable Deserialize(ref SpanReader reader)
        {
            DeserializeCalled = true;

            return new TestSerializable();
        }

        public override void Serialize(ref SpanWriter writer, TestSerializable args) => SerializeCalled = true;
    }

    private sealed class TestSerializable : IPacketSerializable { }
}