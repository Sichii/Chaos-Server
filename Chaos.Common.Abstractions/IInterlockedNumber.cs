using System.Numerics;

namespace Chaos.Common.Abstractions;

public interface IInterlockedNumber<T> where T: INumber<T>
{
    delegate bool InterlockedOperation(ref T number);

    T Add(T value);
    bool Assert(InterlockedOperation operation, out T oldValue);
    bool CompareExchange(T value, T comparand);
    T Decrement();
    T Exchange(T value);

    T Get();
    T Increment();
    T Subtract(T value);
}