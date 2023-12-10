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
    private readonly FrozenDictionary<Type, IClientPacketDeserializer> Deserializers;
    private readonly FrozenDictionary<Type, IServerPacketSerializer> Serializers;

    /// <inheritdoc />
    public Encoding Encoding { get; }

    /// <summary>
    ///     Initializes a new instance of the <see cref="PacketSerializer" /> class.
    /// </summary>
    /// <param name="encoding">The encoding to use when writing and reading strings</param>
    /// <param name="deserializers">A map of types to deserialize, and how to deserialize them</param>
    /// <param name="serializers">A map of types to serialize, and how to serialize them</param>
    public PacketSerializer(
        Encoding encoding,
        IDictionary<Type, IClientPacketDeserializer> deserializers,
        IDictionary<Type, IServerPacketSerializer> serializers)
    {
        Encoding = encoding;
        Deserializers = deserializers.ToFrozenDictionary();
        Serializers = serializers.ToFrozenDictionary();
    }

    /// <inheritdoc />
    public T Deserialize<T>(in ClientPacket packet) where T: IReceiveArgs
    {
        var type = typeof(T);

        if (!Deserializers.TryGetValue(type, out var deserializer))
            throw new InvalidOperationException($"No deserializer exists for type \"{type.FullName}\"");

        var reader = new SpanReader(Encoding, in packet.Buffer);
        var typedDeserializer = (IClientPacketDeserializer<T>)deserializer;

        return typedDeserializer.Deserialize(ref reader);
    }

    /// <inheritdoc />
    public ServerPacket Serialize<T>(T obj) where T: ISendArgs
    {
        if (obj is null)
            throw new ArgumentNullException(nameof(obj));

        var type = typeof(T);

        if (!Serializers.TryGetValue(type, out var serializer))
            throw new InvalidOperationException($"No serializer exists for type \"{type.FullName}\"");

        var packet = new ServerPacket(serializer.ServerOpCode);
        var writer = new SpanWriter(Encoding);
        serializer.Serialize(ref writer, obj);

        packet.Buffer = writer.ToSpan();

        return packet;
    }
}