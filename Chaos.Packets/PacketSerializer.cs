using System.Collections.Concurrent;
using System.Text;
using Chaos.IO.Memory;
using Chaos.Packets.Abstractions;

namespace Chaos.Packets;

public sealed class PacketSerializer : IPacketSerializer
{
    private readonly ConcurrentDictionary<Type, IClientPacketDeserializer> Deserializers;
    private readonly ConcurrentDictionary<Type, IServerPacketSerializer> Serializers;
    public Encoding Encoding { get; }

    public PacketSerializer(
        Encoding encoding,
        IDictionary<Type, IClientPacketDeserializer> deserializers,
        IDictionary<Type, IServerPacketSerializer> serializers
    )
    {
        Encoding = encoding;
        Deserializers = new ConcurrentDictionary<Type, IClientPacketDeserializer>(deserializers);
        Serializers = new ConcurrentDictionary<Type, IServerPacketSerializer>(serializers);
    }

    public T Deserialize<T>(in ClientPacket packet) where T: IReceiveArgs
    {
        var type = typeof(T);

        if (!Deserializers.TryGetValue(type, out var deserializer))
            throw new InvalidOperationException($"No deserializer exists for type \"{type.FullName}\"");

        var reader = new SpanReader(Encoding, in packet.Buffer);

        return (T)deserializer.Deserialize(ref reader);
    }

    public ServerPacket Serialize<T>(T obj) where T: ISendArgs
    {
        if (obj == null)
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