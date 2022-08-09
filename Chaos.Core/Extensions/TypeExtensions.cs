namespace Chaos.Core.Extensions;

public static class TypeExtensions
{
    /// <summary>
    ///     Determines whether a type inherits from a generic interface
    /// </summary>
    public static bool HasGenericInterface(this Type type, Type genericInterfaceType) => type.GetInterfaces()
                                                                                             .Any(i => i.IsGenericType && (i.GetGenericTypeDefinition() == genericInterfaceType));
}