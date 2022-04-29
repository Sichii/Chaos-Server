using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using Chaos.Core.Geometry;
using Chaos.DataObjects;
using Chaos.Templates.Interfaces;
using Chaos.WorldObjects;

namespace Chaos.Templates;

public record MapTemplate : ITemplate<short>
{
    [JsonIgnore]
    public ushort CheckSum { get; set; }
    [JsonIgnore]
    public Dictionary<Point, Door> Doors { get; set; } = new();
    public byte Height { get; set; }
    public short TemplateKey { get; init; }
    [JsonIgnore]
    public Tile[,] Tiles { get; set; } = new Tile[0, 0];
    public Point[][] WarpPointGroups { get; set; } = Array.Empty<Point[]>();
    public byte Width { get; set; }

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
    
    public bool WithinMap(Point point) => (point.X < Width) && (point.Y < Height);

    public bool IsWall(Point point) =>
        !WithinMap(point) || (Doors.TryGetValue(point, out var door) ? door.Closed : Tiles[point.X, point.Y].IsWall);
}