using Chaos.Resources;

namespace Chaos.Models.Map;

public readonly struct Tile(ushort background, ushort leftForeground, ushort rightForeground) : IEquatable<Tile>
{
    public ushort Background { get; } = background;

    public bool IsWall
        => ((LeftForeground > 0) && ((Sotp[LeftForeground - 1] & 15) == 15))
           || ((RightForeground > 0) && ((Sotp[RightForeground - 1] & 15) == 15));

    public ushort LeftForeground { get; } = leftForeground;
    public ushort RightForeground { get; } = rightForeground;

    public static byte[] Sotp { get; } = ResourceManager.Sotp;

    public static bool operator ==(Tile left, Tile right) => left.Equals(right);

    public static bool operator !=(Tile left, Tile right) => !(left == right);

    /// <inheritdoc />
    public bool Equals(Tile other)
        => (Background == other.Background) && (LeftForeground == other.LeftForeground) && (RightForeground == other.RightForeground);

    /// <inheritdoc />
    public override bool Equals(object? obj) => obj is Tile other && Equals(other);

    /// <inheritdoc />
    public override int GetHashCode() => HashCode.Combine(Background, LeftForeground, RightForeground);
}