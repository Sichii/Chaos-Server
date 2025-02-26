#region
using System.Reflection;
#endregion

namespace Chaos.Common.Utilities;

/// <summary>
///     Provides extension methods for <see cref="System.Type" />.
/// </summary>
public static class TypeEx
{
    /// <summary>
    ///     Loads a type by name.
    /// </summary>
    public static Type? LoadType(string typeName, Func<Assembly, Type, bool>? predicate = null)
    {
        var type = AppDomain.CurrentDomain
                            .GetAssemblies()
                            .Where(a => !a.IsDynamic)
                            .SelectMany(
                                a =>
                                {
                                    try
                                    {
                                        return a.GetTypes();
                                    } catch
                                    {
                                        return [];
                                    }
                                })
                            .FirstOrDefault(
                                asmType => (predicate?.Invoke(asmType.Assembly, asmType) ?? true)
                                           && asmType.Name.Equals(typeName, StringComparison.OrdinalIgnoreCase));

        return type;
    }
}