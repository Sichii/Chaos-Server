namespace Chaos.TypeMapper.Abstractions;

public interface IMapperProfile<T1, T2>
{
    T1 Map(T2 obj);
    T2 Map(T1 obj);
}