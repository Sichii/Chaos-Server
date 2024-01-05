using System.Numerics;
using Chaos.Common.Abstractions;

namespace Chaos.Common.Identity;

/// <summary>
///     A generic class for generating unique sequential numeric identifiers.
/// </summary>
/// <typeparam name="T">
///     The numeric type to generate
/// </typeparam>
public sealed class SequentialIdGenerator<T> : IIdGenerator<T> where T: INumber<T>
{
    private T CurrentId;

    /// <summary>
    ///     Retreive the next unique id by incrementing the previous id
    /// </summary>
    public T NextId
    {
        get
        {
            lock (this)
            {
                CurrentId++;

                return CurrentId;
            }
        }
    }

    /// <inheritdoc />
    public static IIdGenerator<T> Shared { get; } = new SequentialIdGenerator<T>();

    /// <summary>
    ///     Initializes a new instance of the <see cref="SequentialIdGenerator{T}" /> class.
    /// </summary>
    public SequentialIdGenerator(T? startId = default) => CurrentId = startId ?? T.Zero;
}