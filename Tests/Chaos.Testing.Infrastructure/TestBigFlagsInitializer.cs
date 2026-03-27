#region
using System.Reflection;
using System.Runtime.CompilerServices;
using Chaos.Common.CustomTypes;
#endregion

namespace Chaos.Testing.Infrastructure;

/// <summary>
///     Ensures test BigFlags types are initialized before any tests run
/// </summary>
internal static class TestBigFlagsInitializer
{
    /// <summary>
    ///     Module initializer that runs before Main() to ensure all BigFlags types are properly initialized
    /// </summary>
    [ModuleInitializer]
    internal static void Initialize()
    {
        // Initialize all currently loaded assemblies
        InitializeLoadedAssemblies();

        // Hook into assembly load events to catch assemblies loaded after this initializer runs
        AppDomain.CurrentDomain.AssemblyLoad += OnAssemblyLoad;
    }

    private static void InitializeAssembly(Assembly assembly)
    {
        if (assembly.IsDynamic)
            return;

        try
        {
            var bigFlagsTypes = assembly.GetTypes()
                                        .Where(asmType => asmType is
                                        {
                                            IsInterface: false, IsAbstract: false, BaseType: not null, BaseType.IsGenericType: true
                                        })
                                        .Where(asmType => asmType.BaseType!.GetGenericTypeDefinition() == typeof(BigFlags<>));

            foreach (var asmType in bigFlagsTypes)
                RuntimeHelpers.RunClassConstructor(asmType.TypeHandle);
        } catch
        {
            // Skip assemblies that can't be scanned (e.g., resource assemblies, reflection-only assemblies)
        }
    }

    private static void InitializeLoadedAssemblies()
    {
        var assemblies = AppDomain.CurrentDomain.GetAssemblies();

        foreach (var assembly in assemblies)
            InitializeAssembly(assembly);
    }

    private static void OnAssemblyLoad(object? sender, AssemblyLoadEventArgs args) => InitializeAssembly(args.LoadedAssembly);
}