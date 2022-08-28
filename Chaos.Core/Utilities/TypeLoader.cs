using Chaos.Core.Extensions;

namespace Chaos.Core.Utilities;

public static class TypeLoader
{
    /// <summary>
    ///     Returns all constructable types that inherit from the specified type.
    /// </summary>
    public static IEnumerable<Type> LoadImplementations<TType>() => AppDomain.CurrentDomain
                                                                   .GetAssemblies()
                                                                   .Where(a => !a.IsDynamic)
                                                                   .SelectMany(a => a.GetTypes())
                                                                   .Where(asmType => asmType.IsAssignableTo(typeof(TType)))
                                                                   .Where(asmType => !asmType.IsInterface && !asmType.IsAbstract);

    public static IEnumerable<Type> LoadImplementationsOfGeneric(Type genericType) =>
        AppDomain.CurrentDomain
                 .GetAssemblies()
                 .Where(a => !a.IsDynamic)
                 .SelectMany(a => a.GetTypes())
                 .Where(asmType => !asmType.IsInterface && !asmType.IsAbstract)
                 .Where(
                     asmType =>
                         genericType.IsInterface ? asmType.HasGenericInterface(genericType) : asmType.HasGenericBaseType(genericType));
}