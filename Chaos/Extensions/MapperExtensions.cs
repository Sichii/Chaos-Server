using AutoMapper;

namespace Chaos.Extensions;

public static class MapperExtensions
{
    public static IMappingExpression<TSource, TDest> IgnoreAllUnmapped<TSource, TDest>(this IMappingExpression<TSource, TDest> expression)
    {
        expression.ForAllMembers(opt => opt.Ignore());

        return expression;
    }

    public static IEnumerable<TResult> MapEnumerable<TSource, TResult>(this IRuntimeMapper mapper, IEnumerable<TSource> source) =>
        mapper.Map<IEnumerable<TSource>, IEnumerable<TResult>>(source);
}