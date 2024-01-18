using AutoMapper;

namespace Chaos.Site.Extensions;

public static class AutoMapperExtensions
{
    public static IMappingExpression<TLhs, TRhs> IncludeNullableMember<TLhs, TRhs, TMember>(
        this IMappingExpression<TLhs, TRhs> expression,
        Func<TLhs, TMember> memberSelector)
        => expression.AfterMap(
            (src, dest, ctx) =>
            {
                var member = memberSelector(src);

                if (member is not null)
                    ctx.Mapper.Map(member, dest);
            });
}