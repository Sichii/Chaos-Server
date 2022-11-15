using Chaos.Extensions.Common;
using Chaos.TypeMapper;
using Chaos.TypeMapper.Abstractions;
using Microsoft.Extensions.DependencyInjection;

// ReSharper disable once CheckNamespace
namespace Chaos.Extensions.DependencyInjection;

public static class TypeMapperExtensions
{
    public static void AddTypeMapper(this IServiceCollection services)
    {
        var genericInterfaceType = typeof(IMapperProfile<,>);
        var typeMapperImplementations = genericInterfaceType.LoadImplementations();

        foreach (var implType in typeMapperImplementations)
            foreach (var iFaceType in implType.ExtractGenericInterfaces(genericInterfaceType))
                services.AddSingleton(iFaceType, implType);

        services.AddSingleton<ITypeMapper, Mapper>();
    }
}