using Chaos.Data;
using Chaos.Extensions.Geometry;
using Chaos.Geometry.Abstractions;
using Chaos.Geometry.EqualityComparers;
using Chaos.Scripting.Abstractions;
using Chaos.Templates.Abstractions;

namespace Chaos.Templates;

public sealed record MapTemplate : ITemplate, IScripted
{
    public IRectangle Bounds { get; init; } = null!;
    public ushort CheckSum { get; set; }
    public Dictionary<IPoint, DoorTemplate> Doors { get; set; } = new(PointEqualityComparer.Instance);
    public byte Height { get; set; }
    /// <inheritdoc />
    public ISet<string> ScriptKeys { get; init; } = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
    public string TemplateKey { get; init; } = null!;
    public Tile[,] Tiles { get; set; } = new Tile[0, 0];
    public Point[] WarpPoints { get; set; } = Array.Empty<Point>();
    public byte Width { get; set; }
    public short MapId => short.Parse(TemplateKey);

    public IEnumerable<byte> GetRowData(byte row)
    {
        for (var x = 0; x < Width; x++)
        {
            yield return (byte)(Tiles[x, row].Background >> 8);
            yield return (byte)Tiles[x, row].Background;
            yield return (byte)(Tiles[x, row].LeftForeground >> 8);
            yield return (byte)Tiles[x, row].LeftForeground;
            yield return (byte)(Tiles[x, row].RightForeground >> 8);
            yield return (byte)Tiles[x, row].RightForeground;
        }
    }

    public bool IsWall(IPoint point) =>
        !IsWithinMap(point) || (Doors.TryGetValue(point, out var door) ? door.Closed : Tiles[point.X, point.Y].IsWall);

    public bool IsWithinMap(IPoint point) => Bounds.Contains(point);
}