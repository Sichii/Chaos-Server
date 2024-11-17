#region
using Chaos.Geometry.Abstractions;
#endregion

namespace Chaos.Geometry;

/// <summary>
///     Represents a circle in two-dimensional space.
/// </summary>
public sealed class Circle : ICircle, IEquatable<ICircle>
{
    /// <inheritdoc />
    public IPoint Center { get; }

    /// <inheritdoc />
    public int Radius { get; }

    /// <summary>
    ///     Creates a new circle
    /// </summary>
    /// <param name="center">
    ///     The center point of the circle
    /// </param>
    /// <param name="radius">
    ///     The radius of the circle
    /// </param>
    public Circle(IPoint center, int radius)
    {
        Center = center;
        Radius = radius;
    }

    /// <inheritdoc />
    public bool Equals(ICircle? other) => other is not null && Center.Equals(other.Center) && (Radius == other.Radius);

    /// <inheritdoc />
    public override bool Equals(object? obj) => obj is ICircle other && Equals(other);

    /// <inheritdoc />
    public override int GetHashCode() => HashCode.Combine(Center, Radius);

    /// <summary>
    ///     Compares two circles
    /// </summary>
    public static bool operator ==(Circle left, ICircle right) => left.Equals(right);

    /// <summary>
    /// </summary>
    public static bool operator !=(Circle left, ICircle right) => !(left == right);
}