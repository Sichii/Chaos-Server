using System.Numerics;

namespace Chaos.Common.Abstractions;

/// <summary>
///     A generic class for generating unique identifiers.
/// </summary>
/// <typeparam name="T">
///     The type of the id
/// </typeparam>
public interface IIdGenerator<out T> where T: INumber<T>
{
    /// <summary>
    ///     Retreive the next unique id
    /// </summary>
    T NextId { get; }

    /// <summary>
    ///     A shared instance of the IdGenerator
    /// </summary>
    static virtual IIdGenerator<T> Shared => throw new NotImplementedException();
}