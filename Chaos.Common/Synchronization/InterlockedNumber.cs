using System.Numerics;
using System.Text.Json.Serialization;
using Chaos.Common.Abstractions;
using Chaos.Common.Converters;

namespace Chaos.Common.Synchronization;

[JsonConverter(typeof(InterlockedNumberConverter))]
public struct InterlockedNumber<T> : IInterlockedNumber<T> where T: INumber<T>
{
    private readonly object Sync;
    private T Number;

    public InterlockedNumber(T number)
    {
        Sync = new object();
        Number = number;
    }

    public InterlockedNumber()
    {
        Sync = new object();
        Number = default!;
    }

    /// <inheritdoc />
    public T Add(T value)
    {
        lock (Sync)
            return Number += value;
    }

    /// <inheritdoc />
    public bool Assert(IInterlockedNumber<T>.InterlockedOperation operation, out T oldValue)
    {
        lock (Sync)
        {
            oldValue = Number;

            return operation(ref Number);
        }
    }

    /// <inheritdoc />
    public bool CompareExchange(T value, T comparand)
    {
        lock (Sync)
            if (Number.Equals(comparand))
            {
                Number = value;

                return true;
            }

        return false;
    }

    /// <inheritdoc />
    public T Decrement()
    {
        lock (Sync)
            return Number--;
    }

    /// <inheritdoc />
    public T Exchange(T value)
    {
        lock (Sync)
        {
            var old = Number;
            Number = value;

            return old;
        }
    }

    /// <inheritdoc />
    public T Get() => Number;

    /// <inheritdoc />
    public T Increment()
    {
        lock (Sync)
            return Number++;
    }

    /// <inheritdoc />
    public T Subtract(T value)
    {
        lock (Sync)
            return Number -= value;
    }
}