using System.Linq.Expressions;
using System.Reflection;

namespace Chaos.Core.Utilities;

/// <summary>
///     Generic static helper for doing a shallow copy from one object to another.
/// </summary>
/// <typeparam name="T">The type of the object.</typeparam>
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

            var assignmentExpressions =
                properties.Select(p => Expression.Assign(Expression.Property(targetEx, p), Expression.Property(fromEx, p)));

            AssignmentDelegate = Expression.Lambda<Action<T, T>>(Expression.Block(assignmentExpressions), fromEx, targetEx).Compile();
        }
    }

    /// <summary>
    ///     Attempts to create a fresh instance of the object type,
    ///     and then shallow mergees all properties from the original object into the new one.
    /// </summary>
    public static T Clone(T fromObj)
    {
        var instance = Activator.CreateInstance<T>();
        Merge(fromObj, instance);

        return instance;
    }

    /// <summary>
    ///     Attempts to create a fresh instance of the object type,
    ///     and then shallow mergees all properties from the original object into the new one.
    /// </summary>
    public static T Clone(T fromObj, params object[] cTorArgs)
    {
        var instance = (T)Activator.CreateInstance(typeof(T), cTorArgs)!;

        Merge(fromObj, instance);

        return instance;
    }

    private static IEnumerable<PropertyInfo> GetRecursiveProperties(Type type) => !type.IsInterface
        ? type.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic).Where(p => p.CanRead && p.CanWrite)
        : new[] { type }.Concat(type.GetInterfaces())
            .SelectMany(i => i.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
            .Where(p => p.CanRead && p.CanWrite)
            .DistinctBy(p => p.Name);

    /// <summary>
    ///     Merges all (public/non-public) instanced properties from <paramref name="fromObj" /> into
    ///     <paramref name="targetObj" /> <br />
    ///     The first time this runs (for each type), an expression tree will be compiled and stored.
    /// </summary>
    /// <param name="fromObj">The object to merge from.</param>
    /// <param name="targetObj">The object to merge into.</param>
    /// <exception cref="ArgumentNullException">fromObj</exception>
    /// <exception cref="ArgumentNullException">targetObj</exception>
    public static void Merge(T fromObj, T targetObj)
    {
        if (fromObj == null)
            throw new ArgumentNullException(nameof(fromObj));

        if (targetObj == null)
            throw new ArgumentNullException(nameof(targetObj));

        AssignmentDelegate(fromObj, targetObj);
    }
}