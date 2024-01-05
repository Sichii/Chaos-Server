using System.Numerics;
using Chaos.Common.Abstractions;
using Chaos.Extensions.Common;

namespace Chaos.Common.Identity;

/// <summary>
///     A generic class for generating unique short-lived non-sequential numeric identifiers.
/// </summary>
/// <typeparam name="T">
///     The numeric type to generate
/// </typeparam>
public sealed class EphemeralRandomIdGenerator<T> : IIdGenerator<T> where T: INumber<T>
{
    private const int HISTORY_SIZE = byte.MaxValue;
    private readonly T[] IdHistory = new T[HISTORY_SIZE];
    private int Index;

    /// <inheritdoc />
    public T NextId
    {
        get
        {
            var index = Interlocked.Increment(ref Index) % HISTORY_SIZE;
            T id;

            do
            {
                id = Random.Shared.Next<T>();
                IdHistory[index] = id;
            } while (IdHistory.Count(num => num == id) > 1);

            return id;
        }
    }

    /// <inheritdoc />
    public static IIdGenerator<T> Shared { get; } = new EphemeralRandomIdGenerator<T>();
}