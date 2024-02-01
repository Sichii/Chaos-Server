using System.Collections.Frozen;
using System.Text;
using Chaos.IO.Memory;
using Chaos.Packets.Abstractions;
using Chaos.Packets.Abstractions.Definitions;

namespace Chaos.Packets;

/// <summary>
///     Provides serialization and deserialization of packets.
/// </summary>
public sealed class PacketSerializer : IPacketSerializer
{
    private readonly FrozenDictionary<Type, IPacketConverter> Converters;
    private readonly byte SequenceOpCode = (byte)ClientOpCode.SequenceChange;

    /// <inheritdoc />
    public Encoding Encoding { get; }

    /// <summary>
    ///     Initializes a new instance of the <see cref="PacketSerializer" /> class.
    /// </summary>
    /// <param name="encoding">
    ///     The encoding to use when writing and reading strings
    /// </param>
    /// <param name="converters">
    ///     A map of types to serialize/deserialize and their associated converters
    /// </param>
    public PacketSerializer(Encoding encoding, IDictionary<Type, IPacketConverter> converters)
    {
        Encoding = encoding;
        Converters = converters.ToFrozenDictionary();
    }

    /// <inheritdoc />
    public T Deserialize<T>(in Packet packet) where T: IPacketSerializable
    {
        var type = typeof(T);
        var reader = new SpanReader(Encoding, in packet.Buffer);

        if (!Converters.TryGetValue(type, out var converter) || converter is not IPacketConverter<T> typedConverter)
            throw new InvalidOperationException($"No converter exists for type \"{type.FullName}\"");

        var ret = typedConverter.Deserialize(ref reader);

        if ((typedConverter.OpCode == SequenceOpCode) && ret is ISequencerPacket sequencer)
            sequencer.Sequence = packet.Sequence;

        return ret;
    }

    /// <inheritdoc />
    public Packet Serialize(IPacketSerializable obj)
    {
        ArgumentNullException.ThrowIfNull(obj);

        var type = obj.GetType();

        if (!Converters.TryGetValue(type, out var converter))
            throw new InvalidOperationException($"No converter exists for type \"{type.FullName}\"");

        var packet = new Packet(converter.OpCode);
        var writer = new SpanWriter(Encoding);
        converter.Serialize(ref writer, obj);

        packet.Buffer = writer.ToSpan();

        return packet;
    }
}