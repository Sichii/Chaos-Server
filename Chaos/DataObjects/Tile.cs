using Chaos.Properties;

namespace Chaos.DataObjects;

public readonly struct Tile
{
    public bool IsWall => ((LeftForeground > 0) && ((Sotp[LeftForeground - 1] & 15) == 15))
                          || ((RightForeground > 0) && ((Sotp[RightForeground - 1] & 15) == 15));
    internal ushort Background { get; }
    internal ushort LeftForeground { get; }
    internal ushort RightForeground { get; }

    internal static byte[] Sotp { get; } = Resources.sotp;

    /// <summary>
    ///     Master constructor for a structure representing a single tile on a map, containing it's visual data.
    /// </summary>
    internal Tile(ushort background, ushort leftForeground, ushort rightForeground)
    {
        Background = background;
        LeftForeground = leftForeground;
        RightForeground = rightForeground;
    }
}