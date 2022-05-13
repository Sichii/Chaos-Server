namespace Chaos.Core.Data;

public readonly struct Tile
{
    public ushort Background { get; }
    public bool IsWall => ((LeftForeground > 0) && ((Sotp[LeftForeground - 1] & 15) == 15))
                          || ((RightForeground > 0) && ((Sotp[RightForeground - 1] & 15) == 15));
    public ushort LeftForeground { get; }
    public ushort RightForeground { get; }

    public static byte[] Sotp { get; } = Resources.Resources.sotp;

    public Tile(ushort background, ushort leftForeground, ushort rightForeground)
    {
        Background = background;
        LeftForeground = leftForeground;
        RightForeground = rightForeground;
    }
}