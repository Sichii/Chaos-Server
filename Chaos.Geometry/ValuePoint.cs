#region
using Chaos.Geometry.Abstractions;
using Chaos.Geometry.Definitions;
#endregion

namespace Chaos.Geometry;

/// <inheritdoc cref="IPoint" />
public readonly ref struct ValuePoint : IPoint, IEquatable<IPoint>, IEquatable<ValuePoint>
{
    /// <inheritdoc />
    public int X { get; init; }

    /// <inheritdoc />
    public int Y { get; init; }

    /// <summary>
    ///     Determines equality between a <see cref="ValuePoint" /> and any implementation of <see cref="IPoint" />
    /// </summary>
    /// <param name="left">
    ///     A concrete <see cref="ValuePoint" />
    /// </param>
    /// <param name="right">
    ///     Any implementation of <see cref="IPoint" />
    /// </param>
    /// <returns>
    ///     <c>
    ///         true
    ///     </c>
    ///     if both objects are on the same coordinates, otherwise
    ///     <c>
    ///         false
    ///     </c>
    /// </returns>
    public static bool operator ==(ValuePoint left, IPoint right) => left.Equals(right);

    /// <summary>
    ///     Implicitly converts a byte tuple to a <see cref="ValuePoint" />
    /// </summary>
    /// <param name="tuple">
    ///     A tuple of two bytes
    /// </param>
    public static implicit operator ValuePoint((byte X, byte Y) tuple) => new(tuple.X, tuple.Y);

    /// <summary>
    ///     Implicity converts a ushort tuple to a <see cref="ValuePoint" />
    /// </summary>
    /// <param name="tuple">
    ///     A tuple of two ushorts
    /// </param>
    public static implicit operator ValuePoint((ushort X, ushort Y) tuple) => new(tuple.X, tuple.Y);

    /// <summary>
    ///     Implicitly converts a short tuple to a <see cref="ValuePoint" />
    /// </summary>
    /// <param name="tuple">
    ///     A tuple of two shorts
    /// </param>
    public static implicit operator ValuePoint((short X, short Y) tuple) => new(tuple.X, tuple.Y);

    /// <summary>
    ///     Implicitly converts an int tuple to a <see cref="ValuePoint" />
    /// </summary>
    /// <param name="tuple">
    ///     A tuple of two ints
    /// </param>
    public static implicit operator ValuePoint((int X, int Y) tuple) => new(tuple.X, tuple.Y);

    /// <summary>
    ///     Implicitly converts a struct point to a ref struct point
    /// </summary>
    public static implicit operator ValuePoint(Point point) => new(point);

    /// <summary>
    ///     Determines inequality between a <see cref="ValuePoint" /> and any implementation of <see cref="IPoint" />
    /// </summary>
    /// <param name="left">
    ///     A concrete <see cref="ValuePoint" />
    /// </param>
    /// <param name="right">
    ///     Any implementation of <see cref="IPoint" />
    /// </param>
    /// <returns>
    ///     <c>
    ///         true
    ///     </c>
    ///     if both objects are on different coordinates, otherwise
    ///     <c>
    ///         false
    ///     </c>
    /// </returns>
    public static bool operator !=(ValuePoint left, IPoint right) => !left.Equals(right);

    /// <summary>
    ///     Creates a new <see cref="ValuePoint" /> from an X and Y coordinate
    /// </summary>
    /// <param name="x">
    ///     An X coordinate
    /// </param>
    /// <param name="y">
    ///     A Y coordinate
    /// </param>
    public ValuePoint(int x, int y)
    {
        X = x;
        Y = y;
    }

    /// <summary>
    ///     Creates a new <see cref="ValuePoint" /> from an existing <see cref="IPoint" />
    /// </summary>
    /// <param name="point">
    ///     Any implementation of <see cref="IPoint" />
    /// </param>
    private ValuePoint(IPoint point)
    {
        X = point.X;
        Y = point.Y;
    }

    /// <summary>
    ///     Deconstructs a <see cref="ValuePoint" /> into an X and Y coordinate
    /// </summary>
    public void Deconstruct(out int x, out int y)
    {
        x = X;
        y = Y;
    }

    /// <inheritdoc />
    public bool Equals(IPoint? other) => other is not null && (X == other.X) && (Y == other.Y);

    /// <inheritdoc />
    public override bool Equals(object? obj) => obj is IPoint other && Equals(other);

    /// <summary>
    ///     Returns an immutable <see cref="Point" /> from an existing <see cref="IPoint" />
    /// </summary>
    /// <param name="point">
    ///     Any implementation of <see cref="IPoint" />
    /// </param>
    public static ValuePoint From(IPoint point) => new(point);

    /// <inheritdoc />
    public override int GetHashCode() => (X << 16) + Y;

    /// <inheritdoc />
    public override string ToString() => $"({X}, {Y})";

    /// <summary>
    ///     Tries to parse a string into a <see cref="Point" />
    /// </summary>
    /// <param name="str">
    ///     The string to parse
    /// </param>
    /// <param name="point">
    ///     The output point from parsing the given string
    /// </param>
    /// <returns>
    /// </returns>
    public static bool TryParse(string str, out ValuePoint point)
    {
        point = new ValuePoint();
        var match = RegexCache.PointRegex.Match(str);

        if (!match.Success)
            return false;

        if (!ushort.TryParse(match.Groups[1].Value, out var x))
            return false;

        if (!ushort.TryParse(match.Groups[2].Value, out var y))
            return false;

        point = new ValuePoint(x, y);

        return true;
    }

    /// <inheritdoc />
    public bool Equals(ValuePoint other) => (X == other.X) && (Y == other.Y);
}