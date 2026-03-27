#region
using System.Collections.Frozen;
#endregion

namespace Chaos.Common.Converters;

/// <summary>
///     Provides cached type lookups by name for JSON converters
/// </summary>
internal static class TypeCache
{
    private static readonly Lock Sync = new();
    private static FrozenDictionary<string, Type>? CachedEnumTypes;
    private static FrozenDictionary<string, Type>? CachedTypes;

    private static void EnsureEnumTypesInitialized()
    {
        if (CachedEnumTypes is not null)
            return;

        lock (Sync)
        {
            if (CachedEnumTypes is not null)
                return;

            var enumTypes = new Dictionary<string, Type>();

            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                if (assembly.IsDynamic || assembly.ReflectionOnly)
                    continue;

                try
                {
                    foreach (var type in assembly.GetTypes())
                        if (type is { IsEnum: true, IsInterface: false, IsAbstract: false })
                            enumTypes.TryAdd(type.Name, type);
                } catch
                {
                    // Skip assemblies that can't be loaded or inspected
                }
            }

            CachedEnumTypes = enumTypes.ToFrozenDictionary();
        }
    }

    private static void EnsureTypesInitialized()
    {
        if (CachedTypes is not null)
            return;

        lock (Sync)
        {
            if (CachedTypes is not null)
                return;

            var types = new Dictionary<string, Type>();

            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                if (assembly.IsDynamic || assembly.ReflectionOnly)
                    continue;

                try
                {
                    foreach (var type in assembly.GetTypes())
                        types.TryAdd(type.Name, type);
                } catch
                {
                    // Skip assemblies that can't be loaded or inspected
                }
            }

            CachedTypes = types.ToFrozenDictionary();
        }
    }

    /// <summary>
    ///     Gets an enum type by name from the cache
    /// </summary>
    /// <param name="typeName">
    ///     The name of the enum type to find
    /// </param>
    /// <returns>
    ///     The enum type if found, otherwise null
    /// </returns>
    public static Type? GetEnumType(string typeName)
    {
        EnsureEnumTypesInitialized();

        return CachedEnumTypes!.GetValueOrDefault(typeName);
    }

    /// <summary>
    ///     Gets a type by name from the cache
    /// </summary>
    /// <param name="typeName">
    ///     The name of the type to find
    /// </param>
    /// <returns>
    ///     The type if found, otherwise null
    /// </returns>
    public static Type? GetType(string typeName)
    {
        EnsureTypesInitialized();

        return CachedTypes!.GetValueOrDefault(typeName);
    }
}