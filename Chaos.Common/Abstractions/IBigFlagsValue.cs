#region
using System.Numerics;
#endregion

namespace Chaos.Common.Abstractions;

/// <summary>
///     Provides properties for a big flags value
/// </summary>
public interface IBigFlagsValue
{
    /// <summary>
    ///     The marker type of the big flag
    /// </summary>
    Type Type { get; }

    /// <summary>
    ///     The value of the big flag
    /// </summary>
    BigInteger Value { get; }
}