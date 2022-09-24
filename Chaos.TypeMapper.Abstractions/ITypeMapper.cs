namespace Chaos.TypeMapper.Abstractions;

public interface ITypeMapper
{
    TResult Map<T, TResult>(T obj);
    TResult Map<TResult>(object obj);
    IEnumerable<TResult> MapMany<TResult>(IEnumerable<object> obj);
}