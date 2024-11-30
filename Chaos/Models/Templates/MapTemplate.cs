#region
using Chaos.Extensions.Geometry;
using Chaos.Geometry.Abstractions;
using Chaos.Geometry.EqualityComparers;
using Chaos.Models.Map;
using Chaos.Models.Templates.Abstractions;
using Chaos.Scripting.Abstractions;
#endregion

namespace Chaos.Models.Templates;

public sealed record MapTemplate : ITemplate, IScripted
{
    public required IRectangle Bounds { get; init; } = null!;
    public ushort CheckSum { get; set; }
    public Dictionary<IPoint, DoorTemplate> Doors { get; set; } = new(PointEqualityComparer.Instance);
    public required byte Height { get; set; }

    public string? LightType { get; set; }

    /// <inheritdoc />
    public required ISet<string> ScriptKeys { get; init; } = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

    public required string TemplateKey { get; init; } = null!;
    public required Tile[,] Tiles { get; set; } = new Tile[0, 0];
    public required byte Width { get; set; }
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

    public bool IsWall(IPoint point) => !IsWithinMap(point) || Tiles[point.X, point.Y].IsWall;

    public bool IsWithinMap(IPoint point) => Bounds.Contains(point);
}