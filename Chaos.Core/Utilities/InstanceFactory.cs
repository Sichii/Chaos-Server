namespace Chaos.Core.Utilities;

public static class InstanceFactory
{
    private static readonly ConcurrentDictionary<Tuple<Type, Type, Type, Type>, CreateDelegate> CachedGenerator = new();

    private static CreateDelegate CacheFunc(Tuple<Type, Type, Type, Type> key)
    {
        var types = new[] { key.Item1, key.Item2, key.Item3, key.Item4 };

        var method = typeof(InstanceFactory)
                     .GetMethods()
                     .Where(m => m.Name == "CreateInstance")
                     .Single(m => m.GetParameters().Length == 4);

        var generic = method.MakeGenericMethod(key.Item2, key.Item3, key.Item4);

        var paramExpr = new List<ParameterExpression>
        {
            Expression.Parameter(typeof(Type))
        };

        for (var i = 0; i < 3; i++)
            paramExpr.Add(Expression.Parameter(typeof(object)));

        var callParamExpr = new List<Expression>
        {
            paramExpr[0]
        };

        for (var i = 1; i < 4; i++)
            callParamExpr.Add(Expression.Convert(paramExpr[i], types[i]));

        var callExpr = Expression.Call(generic, callParamExpr);
        var lambdaExpr = Expression.Lambda<CreateDelegate>(callExpr, paramExpr);
        var func = lambdaExpr.Compile();
        CachedGenerator.TryAdd(key, func);

        return func;
    }

    public static object CreateInstance(Type type) =>
        InstanceFactory<TypeToIgnore?, TypeToIgnore?, TypeToIgnore?>.CreateInstance(
            type,
            null,
            null,
            null);

    public static object CreateInstance<TArg1>(Type type, TArg1 arg1) =>
        InstanceFactory<TArg1, TypeToIgnore?, TypeToIgnore?>.CreateInstance(
            type,
            arg1,
            null,
            null);

    public static object CreateInstance<TArg1, TArg2>(Type type, TArg1 arg1, TArg2 arg2) =>
        InstanceFactory<TArg1, TArg2, TypeToIgnore?>.CreateInstance(
            type,
            arg1,
            arg2,
            null);

    public static object CreateInstance<TArg1, TArg2, TArg3>(
        Type type,
        TArg1 arg1,
        TArg2 arg2,
        TArg3 arg3
    ) =>
        InstanceFactory<TArg1, TArg2, TArg3>.CreateInstance(
            type,
            arg1,
            arg2,
            arg3);

    public static object CreateInstance(Type type, params object?[]? args)
    {
        if (args == null)
            return CreateInstance(type);

        if ((args.Length > 3)
            || ((args.Length > 0) && (args[0] == null))
            || ((args.Length > 1) && (args[1] == null))
            || ((args.Length > 2) && (args[2] == null)))
            return Activator.CreateInstance(type, args)!;

        var arg0 = args.Length > 0 ? args[0] : null;
        var arg1 = args.Length > 1 ? args[1] : null;
        var arg2 = args.Length > 2 ? args[2] : null;

        var key = Tuple.Create(
            type,
            arg0?.GetType() ?? typeof(TypeToIgnore),
            arg1?.GetType() ?? typeof(TypeToIgnore),
            arg2?.GetType() ?? typeof(TypeToIgnore));

        if (CachedGenerator.TryGetValue(key, out var func))
            return func(
                type,
                arg0!,
                arg1!,
                arg2!);

        return CacheFunc(key)(
            type,
            arg0!,
            arg1!,
            arg2!);
    }

    private delegate object CreateDelegate(
        Type type,
        object arg1,
        object arg2,
        object arg3
    );
}

public static class InstanceFactory<TArg1, TArg2, TArg3>
{
    private static readonly ConcurrentDictionary<Type, Func<TArg1, TArg2, TArg3, object>> CachedGenerators = new();

    [SuppressMessage("ReSharper", "UnusedParameter.Local")]
    private static Func<TArg1, TArg2, TArg3, object> CacheFunc(
        Type type,
        TArg1 arg1,
        TArg2 arg2,
        TArg3 arg3
    )
    {
        var constructorTypes = new List<Type>();

        if (typeof(TArg1) != typeof(TypeToIgnore))
            constructorTypes.Add(typeof(TArg1));

        if (typeof(TArg2) != typeof(TypeToIgnore))
            constructorTypes.Add(typeof(TArg2));

        if (typeof(TArg3) != typeof(TypeToIgnore))
            constructorTypes.Add(typeof(TArg3));

        var parameters = new List<ParameterExpression>
        {
            Expression.Parameter(typeof(TArg1)),
            Expression.Parameter(typeof(TArg2)),
            Expression.Parameter(typeof(TArg3))
        };

        var constructor = type.GetConstructor(constructorTypes.ToArray());
        var constructorParameters = parameters.Take(constructorTypes.Count).ToList();
        var newExpr = Expression.New(constructor!, constructorParameters);
        var lambdaExpr = Expression.Lambda<Func<TArg1, TArg2, TArg3, object>>(newExpr, parameters);
        var func = lambdaExpr.Compile();
        CachedGenerators.TryAdd(type, func);

        return func;
    }

    public static object CreateInstance(
        Type type,
        TArg1 arg1,
        TArg2 arg2,
        TArg3 arg3
    )
    {
        if (CachedGenerators.TryGetValue(type, out var func))
            return func(arg1, arg2, arg3);

        return CacheFunc(
            type,
            arg1,
            arg2,
            arg3)(arg1, arg2, arg3);
    }
}

public class TypeToIgnore { }