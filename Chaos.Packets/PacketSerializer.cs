using System.Collections.Concurrent;
using System.Text;
using Chaos.Core.Utilities;
using Chaos.Packets.Interfaces;

namespace Chaos.Packets;

public class PacketSerializer : IPacketSerializer
{
    private readonly ConcurrentDictionary<Type, IClientPacketDeserializer> Deserializers = new();
    private readonly ConcurrentDictionary<Type, IServerPacketSerializer> Serializers = new();
    public Encoding Encoding { get; }

    public PacketSerializer(Encoding encoding)
    {
        Encoding = encoding;
        LoadDeserializersFromAssembly();
        LoadSerializersFromAssembly();
    }

    public T Deserialize<T>(ref ClientPacket packet) where T: IReceiveArgs
    {
        var type = typeof(T);

        if (!Deserializers.TryGetValue(type, out var deserializer))
            throw new InvalidOperationException($"No deserializer exists for type \"{type.FullName}\"");

        var reader = new SpanReader(Encoding, ref packet.Buffer);

        return (T)deserializer.Deserialize(ref reader);
    }

    private void LoadDeserializersFromAssembly()
    {
        var deserializers = AppDomain.CurrentDomain.GetAssemblies()
            .Where(a => !a.IsDynamic)
            .SelectMany(a => a.GetTypes())
            .Where(asmType => !asmType.IsInterface && !asmType.IsAbstract)
            .Where(asmType => asmType.IsAssignableTo(typeof(IClientPacketDeserializer)))
            .Select(asmType => (IClientPacketDeserializer)Activator.CreateInstance(asmType)!)
            .ToArray();

        foreach (var deserializer in deserializers)
        {
            var type = deserializer.GetType()
                .GetInterfaces()
                .Where(i => i.IsGenericType)
                .First(i => i.GetGenericTypeDefinition() == typeof(IClientPacketDeserializer<>));

            var typeParam = type.GetGenericArguments()
                .First();

            Deserializers.TryAdd(typeParam, deserializer);
        }
    }

    private void LoadSerializersFromAssembly()
    {
        var serializers = AppDomain.CurrentDomain.GetAssemblies()
            .Where(a => !a.IsDynamic)
            .SelectMany(a => a.GetTypes())
            .Where(asmType => !asmType.IsInterface && !asmType.IsAbstract)
            .Where(asmType => asmType.IsAssignableTo(typeof(IServerPacketSerializer)))
            .Select(asmType => (IServerPacketSerializer)Activator.CreateInstance(asmType)!)
            .ToArray();

        foreach (var serializer in serializers)
        {
            var type = serializer.GetType()
                .GetInterfaces()
                .Where(i => i.IsGenericType)
                .First(i => i.GetGenericTypeDefinition() == typeof(IServerPacketSerializer<>));

            var typeParam = type.GetGenericArguments()
                .First();

            Serializers.TryAdd(typeParam, serializer);
        }
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

        packet.Buffer = writer.Flush();

        return packet;
    }
}