using System.Diagnostics.CodeAnalysis;
using System.Text;
using Chaos.Extensions.Common;
using Chaos.Packets;
using Chaos.Packets.Abstractions;
using Microsoft.Extensions.DependencyInjection;

// ReSharper disable once CheckNamespace
namespace Chaos.Extensions.DependencyInjection;

/// <summary>
///     <see cref="Chaos.Packets" /> DI extensions
/// </summary>
[ExcludeFromCodeCoverage]
public static class PacketExtensions
{
    /// <summary>
    ///     Adds <see cref="Chaos.Packets.PacketSerializer" /> as a singleton implementation of
    ///     <see cref="Chaos.Packets.Abstractions.IPacketSerializer" /> to the service collction
    /// </summary>
    /// <param name="serviceCollection">
    ///     The service collectionto add to
    /// </param>
    /// <remarks>
    ///     This extension scans all loaded assemblies for types that implement of <see cref="IPacketConverter" />. It
    ///     initializes instances of all of these types through <see cref="System.Activator" />.
    ///     <see cref="System.Activator.CreateInstance(Type)" /> and uses the types and objects as parameters for the
    ///     <see cref="Chaos.Packets.PacketSerializer" /> constructor.
    /// </remarks>
    public static void AddPacketSerializer(this IServiceCollection serviceCollection)
        => serviceCollection.AddSingleton<IPacketSerializer, PacketSerializer>(
            _ =>
            {
                var converters = LoadConvertersFromAssembly();
                var serializer = new PacketSerializer(Encoding.GetEncoding(949), converters);

                return serializer;
            });

    private static Dictionary<Type, IPacketConverter> LoadConvertersFromAssembly()
    {
        var ret = new Dictionary<Type, IPacketConverter>();

        var deserializers = typeof(IPacketConverter).LoadImplementations()
                                                    .Select(asmType => (IPacketConverter)Activator.CreateInstance(asmType)!)
                                                    .ToArray();

        foreach (var deserializer in deserializers)
        {
            var type = deserializer.GetType()
                                   .GetInterfaces()
                                   .Where(i => i.IsGenericType)
                                   .First(i => i.GetGenericTypeDefinition() == typeof(IPacketConverter<>));

            var typeParam = type.GetGenericArguments()
                                .First();

            ret.TryAdd(typeParam, deserializer);
        }

        return ret;
    }
}