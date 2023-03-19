using System.Collections.Concurrent;
using System.Linq.Expressions;
using Chaos.Extensions.Common;
using Chaos.TypeMapper.Abstractions;

namespace Chaos.TypeMapper;

/// <summary>
///     Maps objects from one type to another by utilizing <see cref="Chaos.TypeMapper.Abstractions.IMapperProfile{T1,T2}" /> implementations
/// </summary>
public sealed class Mapper : ITypeMapper
{
    private static readonly ConcurrentDictionary<(Type From, Type To), Func<object, object>> ResolverMap;
    private readonly IServiceProvider Provider;

    static Mapper() => ResolverMap = new ConcurrentDictionary<(Type From, Type To), Func<object, object>>();

    /// <summary>
    ///     Initializes a new instance of the <see cref="Mapper" /> class.
    /// </summary>
    /// <param name="provider">The service provider used by the application</param>
    public Mapper(IServiceProvider provider) => Provider = provider;

    /// <inheritdoc />
    public TResult Map<T, TResult>(T obj)
    {
        if (obj is null)
            throw new ArgumentNullException(nameof(obj));

        var mapper = ResolverMap.GetOrAdd((typeof(T), typeof(TResult)), ResolveMapper);

        return (TResult)mapper(obj);
    }

    /// <inheritdoc />
    public TResult Map<TResult>(object obj)
    {
        ArgumentNullException.ThrowIfNull(obj);

        var mapper = ResolverMap.GetOrAdd((obj.GetType(), typeof(TResult)), ResolveMapper);

        return (TResult)mapper(obj);
    }

    /// <inheritdoc />
    public IEnumerable<TResult> MapMany<TResult>(IEnumerable<object> obj)
    {
        var objs = obj.ToList();

        if (!objs.Any())
            yield break;

        var mapper = ResolverMap.GetOrAdd((objs.First().GetType(), typeof(TResult)), ResolveMapper);

        foreach (var o in objs)
            yield return (TResult)mapper(o);
    }

    /// <inheritdoc />
    public IEnumerable<TResult> MapMany<T, TResult>(IEnumerable<T> obj)
    {
        var objs = obj.ToList();

        if (!objs.Any())
            yield break;

        var mapper = ResolverMap.GetOrAdd((typeof(T), typeof(TResult)), ResolveMapper);

        foreach (var o in objs)
            yield return (TResult)mapper(o!);
    }

    /// <summary>
    ///     Creates a function that maps one type to another, utilizing an <see cref="Chaos.TypeMapper.Abstractions.IMapperProfile{T1,T2}" />
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