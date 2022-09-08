namespace Chaos.Services.Utility.Abstractions;

public interface ICloningService<T>
{
    T Clone(T obj);
}