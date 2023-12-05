using Chaos.Extensions.Common;
using Chaos.Models.Templates;
using DeBroglie;
using DeBroglie.Models;
using DeBroglie.Topo;
using ChaosTile = Chaos.Models.Map.Tile;

namespace WaveFunctionCollapse;

public sealed class MapGenerator
{
    public readonly OverlappingModel Model;
    private readonly TilePropagatorOptions Options;
    private readonly ITopology Topology;
    private int Index;
    private TilePropagator? Propagator;
    public IEnumerable<MapTemplate> MapPool { get; }
    public string OutputDirectory { get; }
    public int OutputHeight { get; }
    public int OutputWidth { get; }
    public int OverlappingTileSize { get; }
    public ICollection<ChaosTile> SampleBackgrounds { get; }
    public ICollection<ChaosTile> SampleForegrounds { get; }

    public MapGenerator(
        int outputWidth,
        int outputHeight,
        int overlappingTileSize,
        string outputDirectory,
        IEnumerable<MapTemplate> mapPool,
        ICollection<ChaosTile>? sampleBackgrounds = null,
        ICollection<ChaosTile>? sampleForegrounds = null)
    {
        OutputWidth = outputWidth;
        OutputHeight = outputHeight;
        OverlappingTileSize = overlappingTileSize;
        OutputDirectory = outputDirectory;
        SampleBackgrounds = sampleBackgrounds ?? Array.Empty<ChaosTile>();
        SampleForegrounds = sampleForegrounds ?? Array.Empty<ChaosTile>();
        MapPool = mapPool;

        if (Directory.Exists(OutputDirectory))
            Directory.Delete(OutputDirectory, true);

        Directory.CreateDirectory(OutputDirectory);

        Model = new OverlappingModel(OverlappingTileSize);
        TrainModel();

        //model.MultiplyFrequency(new Tile(0), 0.5);

        Topology = new GridTopology(
            OutputWidth,
            OutputHeight,
            2,
            false);

        Options = new TilePropagatorOptions
        {
            BacktrackType = BacktrackType.Backjump,
            RandomDouble = () => Random.Shared.NextDouble()
        };
    }

    private int GetBackgroundTileId(ChaosTile tile) => tile.Background;

    private int GetForegroundTileId(ChaosTile tile) => (tile.LeftForeground << 16) | tile.RightForeground;

    public void HardReset()
    {
        Propagator?.Clear();
        Index = 0;
    }

    private void ReplaceEmptyBackgrounds(ChaosTile[,] tiles)
    {
        var mostCommonBgId = tiles.Flatten()
                                  .GroupBy(t => t.Background)
                                  .Where(g => g.Key != 0)
                                  .MaxBy(g => g.Count())!.Key;

        for (var x = 0; x < tiles.GetLowerBound(0); x++)
            for (var y = 0; y < tiles.GetLowerBound(1); y++)
            {
                var tile = tiles[x, y];

                if (tile.Background != 0)
                    continue;

                var newTile = new ChaosTile(mostCommonBgId, tile.LeftForeground, tile.RightForeground);
                tiles[x, y] = newTile;
            }
    }

    public void Run()
    {
        Propagator = new TilePropagator(Model, Topology, Options);

        var result = Propagator.Run();

        if (result != Resolution.Decided)
            throw new InvalidOperationException();
    }

    public Task SaveAsync()
    {
        if (Propagator == null)
            throw new InvalidOperationException();

        var path = Path.Combine(OutputDirectory, $"generated{Index++}.map");

        var tiles = Propagator.ToArray();
        var index = 0;
        var width = tiles.Topology.Width;
        var height = tiles.Topology.Height;
        var data = new byte[width * height * 6];

        for (var y = 0; y < height; y++)
            for (var x = 0; x < width; x++)
            {
                var background = (int)tiles.Get(x, y)
                                           .Value;

                var foregroundCombined = (int)tiles.Get(x, y, 1)
                                                   .Value;
                var leftForeground = (ushort)(foregroundCombined >> 16);
                var rightForeground = (ushort)foregroundCombined;

                data[index++] = (byte)background;
                data[index++] = (byte)(background >> 8);
                data[index++] = (byte)leftForeground;
                data[index++] = (byte)(leftForeground >> 8);
                data[index++] = (byte)rightForeground;
                data[index++] = (byte)(rightForeground >> 8);
            }

        return File.WriteAllBytesAsync(path, data);
    }

    public void SoftReset() => Propagator?.Clear();

    private void TrainModel()
    {
        var selectedMaps = MapPool.AsParallel()
                                  .Where(
                                      map =>
                                      {
                                          var tiles = map.Tiles
                                                         .Flatten()
                                                         .ToList();

                                          foreach (var bs in SampleBackgrounds)
                                              if (tiles.All(t => t.Background != bs.Background))
                                                  return false;

                                          foreach (var fs in SampleForegrounds)
                                              if (tiles.All(
                                                      t => (t.LeftForeground != fs.LeftForeground)
                                                           && (t.RightForeground != fs.RightForeground)))
                                                  return false;

                                          return true;
                                      });

        foreach (var map in selectedMaps)
        {
            if ((map.Width == 0) || (map.Height == 0))
                continue;

            var tiles = map.Tiles;

            if (tiles.Flatten()
                     .All(t => t.Background == 0))
                continue;

            ReplaceEmptyBackgrounds(tiles);

            var tileIds = new int[map.Width, map.Height, 2];

            for (var x = 0; x < map.Width; x++)
                for (var y = 0; y < map.Height; y++)
                {
                    var tile = tiles[x, y];
                    var backgroundTileId = GetBackgroundTileId(tile);
                    var foregroundTileId = GetForegroundTileId(tile);
                    tileIds[x, y, 0] = backgroundTileId;
                    tileIds[x, y, 1] = foregroundTileId;
                }

            var cTileIdArr = TopoArray.Create(tileIds, false);
            var tilesArr = cTileIdArr.ToTiles();
            Model.AddSample(tilesArr);
        }
    }
}