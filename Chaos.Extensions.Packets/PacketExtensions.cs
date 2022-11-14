using System.Text;
using Chaos.Extensions.Common;
using Chaos.Packets;
using Chaos.Packets.Abstractions;
using Microsoft.Extensions.DependencyInjection;

// ReSharper disable once CheckNamespace
namespace Chaos.Extensions.DependencyInjection;

public static class PacketExtensions
{
    public static void AddPacketSerializer(this IServiceCollection serviceCollection) =>
        serviceCollection.AddSingleton<IPacketSerializer, PacketSerializer>(
            _ =>
            {
                var serializers = LoadSerializersFromAssembly();
                var deserializers = LoadDeserializersFromAssembly();
                var serializer = new PacketSerializer(Encoding.GetEncoding(949), deserializers, serializers);

                return serializer;
            });

    private static Dictionary<Type, IClientPacketDeserializer> LoadDeserializersFromAssembly()
    {
        var ret = new Dictionary<Type, IClientPacketDeserializer>();

        var deserializers = typeof(IClientPacketDeserializer).LoadImplementations()
                                                             .Select(
                                                                 asmType => (IClientPacketDeserializer)Activator.CreateInstance(asmType)!)
                                                             .ToArray();

        foreach (var deserializer in deserializers)
        {
            var type = deserializer.GetType()
                                   .GetInterfaces()
                                   .Where(i => i.IsGenericType)
                                   .First(i => i.GetGenericTypeDefinition() == typeof(IClientPacketDeserializer<>));

            var typeParam = type.GetGenericArguments()
                                .First();

            ret.TryAdd(typeParam, deserializer);
        }

        return ret;
    }

    private static Dictionary<Type, IServerPacketSerializer> LoadSerializersFromAssembly()
    {
        var ret = new Dictionary<Type, IServerPacketSerializer>();

        var serializers = typeof(IServerPacketSerializer).LoadImplementations()
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

            ret.TryAdd(typeParam, serializer);
        }

        return ret;
    }
}