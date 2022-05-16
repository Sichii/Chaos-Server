namespace Chaos.Core.Utilities;

public static class TypeLoader
{
    public static IEnumerable<Type> LoadTypes<TType>() => AppDomain.CurrentDomain
                                                                   .GetAssemblies()
                                                                   .Where(a => !a.IsDynamic)
                                                                   .SelectMany(a => a.GetTypes())
                                                                   .Where(asmType => asmType.IsAssignableTo(typeof(TType)))
                                                                   .Where(asmType => !asmType.IsInterface && !asmType.IsAbstract);
}