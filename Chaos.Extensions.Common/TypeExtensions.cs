namespace Chaos.Extensions.Common;

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
    ///     Determines whether a type has a generic base type.
    /// </summary>
    /// <param name="genericBaseType">A generic type definition (non-interface). (The type of a generic without the type params specified)</param>
    public static bool HasGenericBaseType(this Type type, Type genericBaseType)
    {
        var current = type;

        while (current != null)
        {
            if (current.IsGenericType && (current.GetGenericTypeDefinition() == genericBaseType))
                return true;

            current = current.BaseType;
        }

        return false;
    }

    /// <summary>
    ///     Determines whether a type inherits from a generic interface
    /// </summary>
    public static bool HasGenericInterface(this Type type, Type genericInterfaceType) =>
        type.GetInterfaces()
            .Any(i => i.IsGenericType && (i.GetGenericTypeDefinition() == genericInterfaceType));
}