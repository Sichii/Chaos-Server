using System.Collections.Frozen;
using System.Text;
using Chaos.IO.Memory;
using Chaos.Packets.Abstractions;

namespace Chaos.Packets;

/// <summary>
///     Provides serialization and deserialization of packets.
/// </summary>
public sealed class PacketSerializer : IPacketSerializer
{
    private readonly FrozenDictionary<Type, IPacketConverter> Converters;

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

        return typedConverter.Deserialize(ref reader);
    }

    /// <inheritdoc />
    public Packet Serialize<T>(T obj) where T: IPacketSerializable
    {
        if (obj is null)
            throw new ArgumentNullException(nameof(obj));

        var type = typeof(T);

        if (!Converters.TryGetValue(type, out var converter) || converter is not IPacketConverter<T> typedConverter)
            throw new InvalidOperationException($"No converter exists for type \"{type.FullName}\"");

        var packet = new Packet(converter.OpCode);
        var writer = new SpanWriter(Encoding);
        typedConverter.Serialize(ref writer, obj);

        packet.Buffer = writer.ToSpan();

        return packet;
    }
}