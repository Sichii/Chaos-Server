namespace Chaos.Extensions.Common;

/// <summary>
///     Provides extension methods for <see cref="System.Type" />.
/// </summary>
public static class TypeExtensions
{
    /// <summary>
    ///     Extracts the generic base type that use a generic type definition within the hierarchy of the type.
    /// </summary>
    /// <param name="type"></param>
    /// <param name="genericBaseType">A generic type definition (non-interface). (The type of a generic without the type params specified)</param>
    /// <returns></returns>
    public static Type? ExtractGenericBaseType(this Type type, Type genericBaseType)
    {
        var current = type;

        while (current != null)
        {
            if (current.IsGenericType && (current.GetGenericTypeDefinition() == genericBaseType))
                return current;

            current = current.BaseType;
        }

        return null;
    }

    /// <summary>
    ///     Extracts all generic interfaces types from a generic type definition within the hierarchy of the type.
    /// </summary>
    /// <param name="type"></param>
    /// <param name="genericInterfaceType">A generic type definition of an interface. (The type of a generic without the type params specified)</param>
    public static IEnumerable<Type> ExtractGenericInterfaces(this Type type, Type genericInterfaceType)
    {
        var interfaces = type.GetInterfaces();

        foreach (var @interface in interfaces)
            if (@interface.IsGenericType && (@interface.GetGenericTypeDefinition() == genericInterfaceType))
                yield return @interface;
    }

    /// <summary>
    ///     Determines whether a type inherits from the specified base type
    /// </summary>
    public static bool HasBaseType(this Type type, Type baseType)
    {
        var current = type;

        while (current != null)
        {
            if (baseType.IsGenericTypeDefinition)
            {
                if (current.IsGenericType && (current.GetGenericTypeDefinition() == baseType))
                    return true;
            } else if (current == baseType)
                return true;

            current = current.BaseType;
        }

        return false;
    }

    /// <summary>
    ///     Determines whether a type inherits from an interface
    /// </summary>
    public static bool HasInterface(this Type type, Type interfaceType)
    {
        foreach (var iType in type.GetInterfaces())
            if (interfaceType.IsGenericTypeDefinition)
            {
                if (iType.IsGenericType && (iType.GetGenericTypeDefinition() == interfaceType))
                    return true;
            } else if (iType == interfaceType)
                return true;

        return false;
    }

    /// <summary>
    ///     Returns all constructable types that inherit from the specified type.
    /// </summary>
    public static IEnumerable<Type> LoadImplementations(this Type type)
    {
        var assemblyTypes = AppDomain.CurrentDomain
                                     .GetAssemblies()
                                     .Where(a => !a.IsDynamic)
                                     .SelectMany(a => a.GetTypes())
                                     .Where(asmType => !asmType.IsInterface && !asmType.IsAbstract);

        if (type.IsGenericTypeDefinition)
            return assemblyTypes.Where(asmType => type.IsInterface ? asmType.HasInterface(type) : asmType.HasBaseType(type));

        return assemblyTypes.Where(asmType => asmType.IsAssignableTo(type));
    }
}