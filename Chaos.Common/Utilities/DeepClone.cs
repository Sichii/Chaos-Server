using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Reflection;
using Chaos.Extensions.Common;

namespace Chaos.Common.Utilities;

/// <summary>
///     A utlity class for creating deep clones of objects.
/// </summary>
public static class DeepClone
{
    private static readonly Func<object, object> CreateMemberwiseClone;

    static DeepClone()
    {
        var parameter = Expression.Parameter(typeof(object));
        var call = Expression.Call(parameter, typeof(object).GetMethod("MemberwiseClone", BindingFlags.Instance | BindingFlags.NonPublic)!);

        CreateMemberwiseClone = Expression.Lambda<Func<object, object>>(call, parameter)
                                          .Compile();
    }

    private static void CopyFields(
        object originalObject,
        IDictionary<object, object> visited,
        object cloneObject,
        IReflect typeToReflect,
        BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.FlattenHierarchy,
        Func<FieldInfo, bool>? filter = null)
    {
        foreach (var fieldInfo in typeToReflect.GetFields(bindingFlags))
        {
            if ((filter != null) && (filter(fieldInfo) == false))
                continue;

            if (fieldInfo.FieldType.IsPrimitive())
                continue;

            var originalFieldValue = fieldInfo.GetValue(originalObject);

            if (originalFieldValue != null)
            {
                var clonedFieldValue = InternalCopy(originalFieldValue, visited);
                fieldInfo.SetValue(cloneObject, clonedFieldValue);
            }
        }
    }

    /// <summary>
    ///     Attemps to create a deep clone of the object.
    /// </summary>
    /// <param name="fromObj">
    ///     The object to clone
    /// </param>
    /// <typeparam name="T">
    ///     The type of the object being cloned
    /// </typeparam>
    /// <returns>
    ///     A deep cloned instance of the object, or null if the clone was unsuccessful
    /// </returns>
    public static T? Create<T>(T fromObj) => (T?)InternalCopy(fromObj!, new Dictionary<object, object>(ReferenceEqualityComparer.Instance));

    /// <summary>
    ///     Creates a deep clone of the object.
    /// </summary>
    /// <param name="fromObj">
    ///     The object to clone
    /// </param>
    /// <typeparam name="T">
    ///     The type of the object being cloned
    /// </typeparam>
    /// <returns>
    ///     A deep cloned instance of the object
    /// </returns>
    /// <exception cref="InvalidOperationException">
    ///     Failed to create a deep clone of the object.
    /// </exception>
    [ExcludeFromCodeCoverage(Justification = "Tested by Create<T>(T)")]
    public static T CreateRequired<T>(T fromObj)
    {
        var obj = Create(fromObj);

        if (obj is null)
            throw new InvalidOperationException("Failed to create a deep clone of the object.");

        return obj;
    }

    private static object? InternalCopy(object? fromObj, IDictionary<object, object> visited)
    {
        if (fromObj == null)
            return null;

        var type = fromObj.GetType();

        if (type.IsPrimitive())
            return fromObj;

        if (visited.TryGetValue(fromObj, out var toObj))
            return toObj;

        if (typeof(Delegate).IsAssignableFrom(type))
            return (fromObj as Delegate)?.Clone();

        var clonedObj = CreateMemberwiseClone(fromObj);

        if (type.IsArray)
        {
            var arrayType = type.GetElementType();

            if (!arrayType!.IsPrimitive())
            {
                var clonedArray = (Array)clonedObj;
                clonedArray.ForEach((array, indices) => array.SetValue(InternalCopy(array.GetValue(indices), visited), indices));
            }
        }

        visited.Add(fromObj, clonedObj);

        CopyFields(
            fromObj,
            visited,
            clonedObj,
            type);

        RecursiveCopyBaseTypePrivateFields(
            fromObj,
            visited,
            clonedObj,
            type);

        return clonedObj;
    }

    private static void RecursiveCopyBaseTypePrivateFields(
        object originalObject,
        IDictionary<object, object> visited,
        object cloneObject,
        Type typeToReflect)
    {
        if (typeToReflect.BaseType != null)
        {
            RecursiveCopyBaseTypePrivateFields(
                originalObject,
                visited,
                cloneObject,
                typeToReflect.BaseType);

            CopyFields(
                originalObject,
                visited,
                cloneObject,
                typeToReflect.BaseType,
                BindingFlags.Instance | BindingFlags.NonPublic,
                info => info.IsPrivate);
        }
    }
}