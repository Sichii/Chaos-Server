namespace Chaos.TypeMapper.Abstractions;

public interface ICloningService<T>
{
    T Clone(T obj);
}