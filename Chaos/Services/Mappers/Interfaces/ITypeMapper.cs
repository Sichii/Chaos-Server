namespace Chaos.Services.Mappers.Interfaces;

public interface ITypeMapper<T1, T2>
{
    T1 Map(T2 obj);
    T2 Map(T1 obj);
}

public interface ITypeMapper
{
    TResult Map<T, TResult>(T obj);
    TResult Map<TResult>(object obj);
}