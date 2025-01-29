#region
using Chaos.Geometry.Abstractions;
#endregion

namespace Chaos.Geometry;

/// <summary>
///     Represents a circle, a polygon with an infinite number of sides
/// </summary>
public readonly ref struct ValueCircle : ICircle, IEquatable<ICircle>
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
    public ValueCircle(IPoint center, int radius)
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
    ///     Implicitly converts a circle to a ref struct circle
    /// </summary>
    public static explicit operator ValueCircle(Circle circle) => new(circle.Center, circle.Radius);

    /// <summary>
    ///     Compares two circles
    /// </summary>
    public static bool operator ==(ValueCircle left, ICircle right) => left.Equals(right);

    /// <summary>
    /// </summary>
    public static bool operator !=(ValueCircle left, ICircle right) => !(left == right);
}