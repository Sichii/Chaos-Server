using System.Text;
using Chaos.Core.Utilities;
using Chaos.Packets.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Chaos.Packets.Extensions;

public static class ServiceCollectionExtensions
{
    public static void AddPacketSerialization(this IServiceCollection serviceCollection) =>
        serviceCollection.AddSingleton<IPacketSerializer, PacketSerializer>(
            provider =>
            {
                var encoding = provider.GetRequiredService<Encoding>();
                var serializers = LoadSerializersFromAssembly();
                var deserializers = LoadDeserializersFromAssembly();
                var serializer = new PacketSerializer(encoding, deserializers, serializers);

                return serializer;
            });

    private static Dictionary<Type, IClientPacketDeserializer> LoadDeserializersFromAssembly()
    {
        var ret = new Dictionary<Type, IClientPacketDeserializer>();

        var deserializers = TypeLoader.LoadTypes<IClientPacketDeserializer>()
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

            ret.TryAdd(typeParam, deserializer);
        }

        return ret;
    }

    private static Dictionary<Type, IServerPacketSerializer> LoadSerializersFromAssembly()
    {
        var ret = new Dictionary<Type, IServerPacketSerializer>();

        var serializers = TypeLoader.LoadTypes<IServerPacketSerializer>()
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