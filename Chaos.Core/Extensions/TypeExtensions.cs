namespace Chaos.Core.Extensions;

public static class TypeExtensions
{
    public static bool HasGenericInterface(this Type type, Type genericInterfaceType) => type.GetInterfaces()
        .Any(i => i.IsGenericType && (i.GetGenericTypeDefinition() == genericInterfaceType));
}