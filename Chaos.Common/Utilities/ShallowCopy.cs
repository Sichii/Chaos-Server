using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Reflection;

namespace Chaos.Common.Utilities;

/// <summary>
///     Generic static helper for doing a shallow copy from one object to another.
/// </summary>
/// <typeparam name="T">
///     The type of the object.
/// </typeparam>
public static class ShallowCopy<T>
{
    private static readonly Action<T, T> AssignmentDelegate;

    static ShallowCopy()
    {
        if (AssignmentDelegate == null)
        {
            var fromEx = Expression.Parameter(typeof(T));
            var targetEx = Expression.Parameter(typeof(T));

            var properties = GetRecursiveProperties(typeof(T));
            var fields = GetRecursiveFields(typeof(T));

            var propertyAssignments
                = properties.Select(p => Expression.Assign(Expression.Property(targetEx, p), Expression.Property(fromEx, p)));

            var fieldAssignments = fields.Select(f => Expression.Assign(Expression.Field(targetEx, f), Expression.Field(fromEx, f)));

            var assignmentBlock = Expression.Block(propertyAssignments.Concat(fieldAssignments));

            AssignmentDelegate = Expression.Lambda<Action<T, T>>(assignmentBlock, fromEx, targetEx)
                                           .Compile();
        }
    }

    /// <summary>
    ///     Attempts to create a fresh instance of the object type, and then shallow mergees all properties from the original
    ///     object into the new one.
    /// </summary>
    public static T Create(T fromObj)
    {
        var instance = Activator.CreateInstance<T>();
        Merge(fromObj, instance);

        return instance;
    }

    /// <summary>
    ///     Attempts to create a fresh instance of the object type, and then shallow mergees all properties from the original
    ///     object into the new one.
    /// </summary>
    [ExcludeFromCodeCoverage(Justification = "Tested by Create(T)")]
    public static T Create(T fromObj, params object[] cTorArgs)
    {
        var instance = (T)Activator.CreateInstance(typeof(T), cTorArgs)!;

        Merge(fromObj, instance);

        return instance;
    }

    private static IEnumerable<FieldInfo> GetRecursiveFields(Type type)
        => !type.IsInterface
            ? type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                  .Where(f => f is { IsInitOnly: false, IsLiteral: false })
            : new[]
                {
                    type
                }.Concat(type.GetInterfaces())
                 .SelectMany(i => i.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
                 .Where(f => f is { IsInitOnly: false, IsLiteral: false })
                 .DistinctBy(p => p.Name);

    private static IEnumerable<PropertyInfo> GetRecursiveProperties(Type type)
        => !type.IsInterface
            ? type.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                  .Where(p => p is { CanRead: true, CanWrite: true })
            : new[]
                {
                    type
                }.Concat(type.GetInterfaces())
                 .SelectMany(i => i.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
                 .Where(p => p is { CanRead: true, CanWrite: true })
                 .DistinctBy(p => p.Name);

    /// <summary>
    ///     Merges all (public/non-public) instanced properties from <paramref name="fromObj" /> into
    ///     <paramref name="targetObj" />
    ///     <br />
    ///     The first time this runs (for each type), an expression tree will be compiled and stored.
    /// </summary>
    /// <param name="fromObj">
    ///     The object to merge from.
    /// </param>
    /// <param name="targetObj">
    ///     The object to merge into.
    /// </param>
    /// <exception cref="ArgumentNullException">
    ///     fromObj
    /// </exception>
    /// <exception cref="ArgumentNullException">
    ///     targetObj
    /// </exception>
    public static void Merge(T fromObj, T targetObj)
    {
        if (fromObj is null)
            throw new ArgumentNullException(nameof(fromObj));

        if (targetObj is null)
            throw new ArgumentNullException(nameof(targetObj));

        AssignmentDelegate(fromObj, targetObj);
    }
}