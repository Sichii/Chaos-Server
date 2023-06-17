using System.Diagnostics.CodeAnalysis;
using Chaos.Extensions.Common;
using Chaos.TypeMapper;
using Chaos.TypeMapper.Abstractions;
using Microsoft.Extensions.DependencyInjection;

// ReSharper disable once CheckNamespace
namespace Chaos.Extensions.DependencyInjection;

/// <summary>
///     <see cref="Chaos.TypeMapper" /> DI extensions
/// </summary>
[ExcludeFromCodeCoverage]
public static class TypeMapperExtensions
{
    /// <summary>
    ///     Adds <see cref="Chaos.TypeMapper.Mapper" /> as an implementation of
    ///     <see cref="Chaos.TypeMapper.Abstractions.ITypeMapper" />
    /// </summary>
    /// <param name="services">The service collection to add the service to</param>
    /// <remarks>
    ///     This extensions scans all loaded assemblies for types that inherit from
    ///     <see cref="Chaos.TypeMapper.Abstractions.IMapperProfile{T1,T2}" />. All of these types
    ///     are added to the service collection as singletons so that they can reference eachother through the
    ///     <see cref="Chaos.TypeMapper.Abstractions.ITypeMapper" />, as well
    ///     as any other services that may be required for conversion.
    /// </remarks>
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