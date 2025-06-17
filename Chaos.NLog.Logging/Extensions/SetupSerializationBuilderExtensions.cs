#region
using System.Collections.Frozen;
using System.Linq.Expressions;
using Chaos.NLog.Logging.Abstractions;
using NLog;
using NLog.Config;
#endregion

namespace Chaos.NLog.Logging.Extensions;

/// <summary>
///     Provides extensions for <see cref="ISetupSerializationBuilder" />.
/// </summary>
public static class SetupSerializationBuilderExtensions
{
    private static IDictionary<Type, Func<object, object>> CollectionTransformations = new Dictionary<Type, Func<object, object>>();

    /// <summary>
    ///     Helper method to register transformations for collection types that implement
    ///     <see cref="ITransformableCollection" />. This exists because NLog will always prefer to transform collections based
    ///     on their element type, even if you specify a transformer for the collection type.
    /// </summary>
    /// <exception cref="ArgumentException">
    /// </exception>
    /// <exception cref="InvalidOperationException">
    /// </exception>
    public static ISetupSerializationBuilder RegisterCollectionTransformations(
        this ISetupSerializationBuilder builder,
        params Delegate[] transformations)
    {
        var localDictionary = new Dictionary<Type, Func<object, object>>(CollectionTransformations);
        var transformableCollectionType = typeof(ITransformableCollection);

        foreach (var transformation in transformations)
        {
            if (transformation is null)
                throw new ArgumentNullException(nameof(transformation));

            var parameters = transformation.Method.GetParameters();

            if (parameters.Length != 1)
                throw new ArgumentException("Each transformation must take exactly one parameter.", nameof(transformations));

            var inputType = parameters[0].ParameterType;

            if (!transformableCollectionType.IsAssignableFrom(inputType))
                throw new ArgumentException(
                    $"Transformation must accept a parameter assignable to {transformableCollectionType.Name}: {inputType}",
                    nameof(transformations));

            localDictionary[inputType] = CreateWrapper(transformation);

            continue;

            static Func<object, object> CreateWrapper(Delegate originalDelegate)
            {
                var method = originalDelegate.Method;
                var target = originalDelegate.Target;
                var paramType = method.GetParameters()[0].ParameterType;

                var objParam = Expression.Parameter(typeof(object), "obj");
                var castedParam = Expression.Convert(objParam, paramType);
                var targetExpr = target == null ? null : Expression.Constant(target);

                var call = Expression.Call(targetExpr, method, castedParam);
                var convertedResult = Expression.Convert(call, typeof(object));
                var lambda = Expression.Lambda<Func<object, object>>(convertedResult, objParam);

                return lambda.Compile();
            }
        }

        CollectionTransformations = localDictionary.ToFrozenDictionary();

        return builder;
    }

    internal static object Transform<T>(T obj) where T: ITransformableCollection
    {
        if (CollectionTransformations.TryGetValue(obj.GetType(), out var transformation))
            return transformation(obj);

        throw new InvalidOperationException($"No transformation registered for type {obj.GetType().Name}");
    }
}