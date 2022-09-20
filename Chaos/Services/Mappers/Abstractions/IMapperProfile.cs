namespace Chaos.Services.Mappers.Abstractions;

public interface IMapperProfile<T1, T2>
{
    T1 Map(T2 obj);
    T2 Map(T1 obj);
}

public interface ITypeMapper
{
    TResult Map<T, TResult>(T obj);
    TResult Map<TResult>(object obj);
    IEnumerable<TResult> MapMany<TResult>(IEnumerable<object> obj);
}