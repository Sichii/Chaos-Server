namespace Chaos.Core.Extensions;

public static class TypeExtensions
{
    /// <summary>
    ///     Determines whether a type inherits from a generic interface
    /// </summary>
    public static bool HasGenericInterface(this Type type, Type genericInterfaceType) =>
        type.GetInterfaces()
            .Any(i => i.IsGenericType && (i.GetGenericTypeDefinition() == genericInterfaceType));

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
    
    public static IEnumerable<Type> ExtractGenericInterfaces(this Type type, Type genericInterfaceType)
    {
        var interfaces = type.GetInterfaces();
        foreach (var @interface in interfaces)
            if (@interface.IsGenericType && (@interface.GetGenericTypeDefinition() == genericInterfaceType))
                yield return @interface;
    }
}