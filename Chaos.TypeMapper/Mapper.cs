using System.Collections.Concurrent;
using System.Linq.Expressions;
using Chaos.Extensions.Common;
using Chaos.TypeMapper.Abstractions;

namespace Chaos.TypeMapper;

public sealed class Mapper : ITypeMapper
{
    private static readonly ConcurrentDictionary<(Type From, Type To), Func<object, object>> ResolverCache;
    private readonly IServiceProvider Provider;

    static Mapper() => ResolverCache = new ConcurrentDictionary<(Type From, Type To), Func<object, object>>();

    public Mapper(IServiceProvider provider) => Provider = provider;

    public TResult Map<T, TResult>(T obj)
    {
        if (obj == null)
            throw new ArgumentNullException(nameof(obj));

        var mapper = ResolverCache.GetOrAdd((typeof(T), typeof(TResult)), ResolveMapper);

        return (TResult)mapper(obj);
    }

    public TResult Map<TResult>(object obj)
    {
        if (obj == null)
            throw new ArgumentNullException(nameof(obj));

        var mapper = ResolverCache.GetOrAdd((obj.GetType(), typeof(TResult)), ResolveMapper);

        return (TResult)mapper(obj);
    }

    public IEnumerable<TResult> MapMany<TResult>(IEnumerable<object> obj)
    {
        var objs = obj.ToList();

        if (!objs.Any())
            yield break;

        var mapper = ResolverCache.GetOrAdd((objs.First().GetType(), typeof(TResult)), ResolveMapper);

        foreach (var o in objs)
            yield return (TResult)mapper(o);
    }

    /// <summary>
    ///     Creates a function that maps one type to another, utilizing a typeMapper
    /// </summary>
    private Func<object, object> ResolveMapper((Type From, Type To) key)
    {
        var from = key.From;
        var to = key.To;

        //the mapper's type args could be in either order, so we need to check both orders
        var mapperType1 = typeof(IMapperProfile<,>).MakeGenericType(from, to);
        var mapperType2 = typeof(IMapperProfile<,>).MakeGenericType(to, from);

        //find the typeMapper
        var service = Provider.GetService(mapperType1) ?? Provider.GetService(mapperType2);

        if (service == null)
            throw new InvalidOperationException("No mapper found for types " + from + " and " + to);

        //creates an expression that calls the mapper's Map method that takes an object of "From" type and returns an object of "To" type
        //taking in an Object, converting it to the "From" type, and then calling the Map method on the mapper
        var genericInterfaceTypes = service.GetType().ExtractGenericInterfaces(typeof(IMapperProfile<,>));

        //find the method with the correct signature
        var method = genericInterfaceTypes.SelectMany(iFaceType => iFaceType.GetMethods())
                                          .First(
                                              m => m.Name.EqualsI(nameof(IMapperProfile<object, object>.Map))
                                                   && m.GetParameters().Any(p => p.ParameterType == from)
                                                   && (m.ReturnType == to));

        //create an expression that takes an object, converts it, and then calls the map method
        var objExpression = Expression.Parameter(typeof(object));
        var convertExpression = Expression.Convert(objExpression, from);
        var callExpression = Expression.Call(Expression.Constant(service), method, convertExpression);
        var lambda = Expression.Lambda<Func<object, object>>(callExpression, objExpression);

        return lambda.Compile();
    }
}