using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using Chaos.Core.Data;
using Chaos.Core.Geometry;
using Chaos.Objects.World;
using Chaos.Templates.Interfaces;

namespace Chaos.Templates;

public record MapTemplate : ITemplate
{
    [JsonIgnore]
    public ushort CheckSum { get; set; }
    [JsonIgnore]
    public Dictionary<Point, Door> Doors { get; set; } = new();
    public byte Height { get; set; }
    public string TemplateKey { get; init; } = null!;
    [JsonIgnore]
    public Tile[,] Tiles { get; set; } = new Tile[0, 0];
    public Point[][] WarpPointGroups { get; set; } = Array.Empty<Point[]>();
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

    public bool IsWall(Point point) =>
        !IsWithinMap(point) || (Doors.TryGetValue(point, out var door) ? door.Closed : Tiles[point.X, point.Y].IsWall);

    public bool IsWithinMap(Point point) => (point.X < Width) && (point.Y < Height);
}