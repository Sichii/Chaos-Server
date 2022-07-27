namespace Chaos.Services.Utility.Interfaces;

public interface ICloningService<T>
{
    T Clone(T obj);
}