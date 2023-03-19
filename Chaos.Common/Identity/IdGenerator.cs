using System.Numerics;

namespace Chaos.Common.Identity;

/// <summary>
///     A generic class for generating unique identifiers.
/// </summary>
/// <typeparam name="T">The numeric type to generate</typeparam>
public sealed class IdGenerator<T> where T: INumber<T>
{
    private T CurrentId = default!;

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
}