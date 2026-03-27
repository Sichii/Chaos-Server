#region
using System.Text;
using Chaos.IO.Memory;
using Chaos.Packets.Abstractions;
using FluentAssertions;
#endregion

namespace Chaos.Packets.Tests;

public sealed class PacketSerializerTests
{
    [Test]
    public void Deserialize_Sets_Sequence_On_ISequencerPacket()
    {
        var serializer = new PacketSerializer(
            Encoding.ASCII,
            new Dictionary<Type, IPacketConverter>
            {
                {
                    typeof(SequencerPacket), new SequencerConverter()
                }
            });

        var buffer = new byte[]
        {
            0xAA,
            0x00,
            0x04,
            0x55,
            0x10
        };
        var span = new Span<byte>(buffer);
        var pkt = new Packet(ref span, false);
        var pktCopy = pkt; // avoid capturing ref struct into lambda via in argument
        var result = serializer.Deserialize<SequencerPacket>(in pktCopy);

        result.Sequence
              .Should()
              .Be(0x10);
    }

    [Test]
    public void Deserialize_Throws_When_No_Converter()
    {
        var serializer = new PacketSerializer(Encoding.ASCII, new Dictionary<Type, IPacketConverter>());

        var span = new Span<byte>(
            [
                0xAA,
                0x00,
                0x04,
                0x01,
                0x00
            ]);
        var pkt = new Packet(ref span, false);

        var threw = false;

        try
        {
            _ = serializer.Deserialize<EchoPacket>(in pkt);
        } catch (InvalidOperationException ex)
        {
            ex.Message
              .Should()
              .Contain("No converter exists");
            threw = true;
        }

        threw.Should()
             .BeTrue();
    }

    [Test]
    public void Serialize_Then_Deserialize_RoundTrips_And_Sets_Sequence_If_Sequencer()
    {
        var encoding = Encoding.ASCII;

        var converters = new Dictionary<Type, IPacketConverter>
        {
            {
                typeof(EchoPacket), new EchoConverter()
            }
        };
        var serializer = new PacketSerializer(encoding, converters);

        var original = new EchoPacket
        {
            Message = "hello"
        };
        var packet = serializer.Serialize(original);

        packet.OpCode
              .Should()
              .Be(0x42);

        packet.Buffer
              .ToArray()
              .Should()
              .Equal(
                  5,
                  (byte)'h',
                  (byte)'e',
                  (byte)'l',
                  (byte)'l',
                  (byte)'o');

        // Now deserialize
        packet.Sequence = 0xAA;
        var result = serializer.Deserialize<EchoPacket>(in packet);

        result.Message
              .Should()
              .Be("hello");
    }

    [Test]
    public void Serialize_Throws_When_No_Converter()
    {
        var serializer = new PacketSerializer(Encoding.ASCII, new Dictionary<Type, IPacketConverter>());
        Action act = () => serializer.Serialize(new EchoPacket());

        act.Should()
           .Throw<InvalidOperationException>()
           .WithMessage("*No converter exists*");
    }

    private sealed class EchoConverter : IPacketConverter<EchoPacket>
    {
        public byte OpCode => 0x42;

        EchoPacket IPacketConverter<EchoPacket>.Deserialize(ref SpanReader reader)
        {
            var msg = reader.ReadString8();

            return new EchoPacket
            {
                Message = msg
            };
        }

        object IPacketConverter.Deserialize(ref SpanReader reader) => ((IPacketConverter<EchoPacket>)this).Deserialize(ref reader);

        public void Serialize(ref SpanWriter writer, EchoPacket args) => writer.WriteString8(args.Message);

        public void Serialize(ref SpanWriter writer, object args) => Serialize(ref writer, (EchoPacket)args);
    }

    private sealed class EchoPacket : IPacketSerializable
    {
        public string Message { get; init; } = string.Empty;
    }

    private sealed class SequencerConverter : IPacketConverter<SequencerPacket>
    {
        public byte OpCode => 0x55;
        SequencerPacket IPacketConverter<SequencerPacket>.Deserialize(ref SpanReader reader) => new();
        object IPacketConverter.Deserialize(ref SpanReader reader) => ((IPacketConverter<SequencerPacket>)this).Deserialize(ref reader);
        public void Serialize(ref SpanWriter writer, SequencerPacket args) { }
        public void Serialize(ref SpanWriter writer, object args) => Serialize(ref writer, (SequencerPacket)args);
    }

    private sealed class SequencerPacket : ISequencerPacket
    {
        public byte Sequence { get; set; }
    }
}