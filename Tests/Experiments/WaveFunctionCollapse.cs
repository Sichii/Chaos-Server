using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Chaos;
using Chaos.Extensions.Common;
using Chaos.Storage.Abstractions;
using Chaos.Templates;
using DeBroglie;
using DeBroglie.Models;
using DeBroglie.Topo;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using ChaosTile = Chaos.Data.Tile;

namespace Experiments;

public class WaveFunctionCollapse
{
    public static IEnumerable<T> AsEnumerable<T>(ITopoArray<T> arr)
    {
        var topology = arr.Topology;

        for (var x = 0; x < topology.Width; x++)
            for (var y = 0; y < topology.Height; y++)
                yield return arr.Get(x, y);
    }

    [Fact]
    public void GenerateWfcMaps()
    {
        var configuration = new ConfigurationBuilder()
                            .SetBasePath(Directory.GetCurrentDirectory())
                            .AddJsonFile("appsettings.json")
                            #if DEBUG
                            .AddJsonFile("appsettings.local.json")
                            #else
                            //.AddJsonFile("appsettings.prod.json")
                            .AddJsonFile("appsettings.local.json")
                            #endif
                            .Build();

        var services = new ServiceCollection();
        var startup = new Startup(configuration);
        startup.ConfigureServices(services);
        var provider = services.BuildServiceProvider();

        var simpleCacheProvider = provider.GetRequiredService<ISimpleCacheProvider>();
        var mapCache = simpleCacheProvider.GetCache<MapTemplate>();
        var oHeight = 50;
        var oWidth = 50;
        var directory = "output";

        if (Directory.Exists(directory))
            Directory.Delete(directory, true);

        Directory.CreateDirectory(directory);

        var backgroundSample = new List<ChaosTile>
        {
            //pravate cave
            new(3776, 0, 0),
            new(3754, 0, 0)
            //new(299, 0, 0),
            //new(300, 0, 0)
        };

        var foregroundSample = new List<ChaosTile>
        {
            //pravat cave
            new(0, 5052, 5053),
            new(0, 5054, 5055)
            //new(0, 913, 914),
            //new(0, 2145, 2146)
        };

        var model = TrainModel(
            mapCache,
            backgroundSample,
            foregroundSample,
            2,
            out var tileLookup,
            out var emptyForegroundTiles);

        foreach (var emptyTile in emptyForegroundTiles.DistinctBy(t => t.Value))
            model.MultiplyFrequency(emptyTile, 0.33);

        var outputTopology = new GridTopology(oWidth, oHeight, false);

        var options = new TilePropagatorOptions
        {
            BacktrackType = BacktrackType.Backtrack,
            RandomDouble = () => Random.Shared.NextDouble()
        };

        var propagator = new TilePropagator(model, outputTopology, options);

        for (var i = 0; i < 10; i++)
        {
            var result = propagator.Run();

            if (result != Resolution.Decided)
                throw new InvalidOperationException();

            var path = Path.Combine(directory, $"generated{i}.map");
            WriteMap(propagator, tileLookup, path);

            propagator.Clear();
        }
    }

    public static IEnumerable<Tile> GetEmptyForegroundTiles(IEnumerable<Tile> tiles) =>
        tiles.Where(
            t =>
            {
                var tileId = (long)t.Value;
                const long EMPTY_MASK = 0b_00000000_00000000_11111111_11111111_11111111_11111111;
                var unmasked = tileId & EMPTY_MASK;

                return unmasked == 0;
            });

    public long GetTileId(ChaosTile tile) =>
        ((long)tile.Background << 32) | ((long)tile.LeftForeground << 16) | tile.RightForeground;

    public static void ReplaceEmptyBackgrounds(ChaosTile[,] tiles)
    {
        var mostCommonBgId = tiles.Flatten()
                                  .GroupBy(t => t.Background)
                                  .Where(g => g.Key != 0)
                                  .MaxBy(g => g.Count())!
                                  .Key;

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

    public OverlappingModel TrainModel(
        IEnumerable<MapTemplate> mapPool,
        ICollection<ChaosTile> backgroundSamples,
        ICollection<ChaosTile> foregroundSamples,
        int overlappingTileSize,
        out Dictionary<long, ChaosTile> tileLookup,
        out List<Tile> emptyForegroundTiles
    )
    {
        var model = new OverlappingModel(overlappingTileSize);
        emptyForegroundTiles = new List<Tile>();
        tileLookup = new Dictionary<long, ChaosTile>();

        var selectedMaps = mapPool.AsParallel()
                                  .WithDegreeOfParallelism(Environment.ProcessorCount)
                                  .Where(
                                      map =>
                                      {
                                          var tiles = map.Tiles.Flatten().ToList();

                                          foreach (var bs in backgroundSamples)
                                              if (tiles.All(t => t.Background != bs.Background))
                                                  return false;

                                          foreach (var fs in foregroundSamples)
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

            if (tiles.Flatten().All(t => t.Background == 0))
                continue;

            ReplaceEmptyBackgrounds(tiles);

            var tileIds = new long[map.Width, map.Height];

            for (var x = 0; x < map.Width; x++)
                for (var y = 0; y < map.Height; y++)
                {
                    var tile = tiles[x, y];
                    var tileId = GetTileId(tile);
                    tileLookup.TryAdd(tileId, tile);
                    tileIds[x, y] = tileId;
                }

            var cTileIdArr = TopoArray.Create(tileIds, false);
            var tilesArr = cTileIdArr.ToTiles();
            model.AddSample(tilesArr);

            emptyForegroundTiles.AddRange(GetEmptyForegroundTiles(AsEnumerable(tilesArr)));
        }

        return model;
    }

    public static void WriteMap(TilePropagator propagator, Dictionary<long, ChaosTile> tileLookup, string path)
    {
        var tiles = propagator.ToArray();
        var index = 0;
        var width = tiles.Topology.Width;
        var height = tiles.Topology.Height;
        var data = new byte[width * height * 6];

        for (var y = 0; y < height; y++)
            for (var x = 0; x < width; x++)
            {
                var tileId = (long)tiles.Get(x, y).Value;
                var tile = tileLookup[tileId];
                data[index++] = (byte)tile.Background;
                data[index++] = (byte)(tile.Background >> 8);
                data[index++] = (byte)tile.LeftForeground;
                data[index++] = (byte)(tile.LeftForeground >> 8);
                data[index++] = (byte)tile.RightForeground;
                data[index++] = (byte)(tile.RightForeground >> 8);
            }

        File.WriteAllBytes(path, data);
    }

    /*
    [Fact]
    public async Task GenerateWfcMap()
    {
        var configuration = new ConfigurationBuilder()
                            .SetBasePath(Directory.GetCurrentDirectory())
                            .AddJsonFile("appsettings.json")
                            #if DEBUG
                            .AddJsonFile("appsettings.local.json")
                            #else
                    //.AddJsonFile("appsettings.prod.json")
                    .AddJsonFile("appsettings.local.json")
                            #endif
                            .Build();
        
        var services = new ServiceCollection();
        var startup = new Startup(configuration);
        startup.ConfigureServices(services);
        var provider = services.BuildServiceProvider();
        var simpleCacheProvider = provider.GetRequiredService<ISimpleCacheProvider>();
        var mapCache = simpleCacheProvider.GetCache<MapTemplate>();
        var simpleCache = provider.GetRequiredService<ISimpleCache>();
        var oHeight = 50;
        var oWidth = 50;
        var directory = "output";

        if (Directory.Exists(directory))
            Directory.Delete(directory, true);
        
        Directory.CreateDirectory(directory);
        
        var backgroundModel = new OverlappingModel(2);
        var foregroundModel = new OverlappingModel(3);

        var tileSamples = mapCache.AsParallel()
                                  .WithDegreeOfParallelism(Environment.ProcessorCount)
                                  .Where(m => m.Tiles.Flatten().Any(t => t.LeftForeground == 14052))
                                  .ToList();
        
        
        foreach (var map in tileSamples)
        {
            if ((map.Width == 0) || (map.Height == 0))
                continue;
            
            var backgroundIds = new int[map.Width, map.Height];
            var foregroundIds = new int[map.Width, map.Height];

            for (var y = 0; y < map.Height; y++)
                for (var x = 0; x < map.Width; x++)
                {
                    var tile = map.Tiles[x, y];
                    backgroundIds[x, y] = GetBackgroundId(tile);
                    foregroundIds[x, y] = GetForegroundId(tile);
                }

            //replace blank tiles with whatever the most common tile id is
            var mostCommonBgIds = backgroundIds.Flatten()
                                               .GroupBy(i => i)
                                               .Where(g => g.Key != 0)
                                               .MaxBy(g => g.Count());

            if (mostCommonBgIds == null)
                continue;

            var mostCommonBgId = mostCommonBgIds.Key;
            
            for(var x = 0; x < map.Width; x++)
                for(var y = 0; y < map.Height; y++)
                    if (backgroundIds[x, y] == 0)
                        backgroundIds[x, y] = mostCommonBgId;

            var mapTopology = new GridTopology(
                DirectionSet.Cartesian2d,
                map.Width,
                map.Height,
                false,
                false);
            
            var bgArr = TopoArray.Create(backgroundIds, mapTopology);
            backgroundModel.AddSample(bgArr.ToTiles());

            var fgArr = TopoArray.Create(foregroundIds, mapTopology);
            foregroundModel.AddSample(fgArr.ToTiles());
        }
        
        //we want there to be a foreground more often than not
        foregroundModel.MultiplyFrequency(new Tile(0), 0.333);

        var outputTopology = new GridTopology(DirectionSet.Cartesian2d, oWidth, oHeight, false, false);

        var options = new TilePropagatorOptions
        {
            BacktrackType = BacktrackType.Backtrack,
            RandomDouble = () => Random.Shared.NextDouble()
        };
        var backgroundPropagator = new TilePropagator(backgroundModel, outputTopology, options);
        var foregroundPropagator = new TilePropagator(foregroundModel, outputTopology, options);
        
        for (var i = 0; i < 10; i++)
        {
            var results = await Task.WhenAll(Task.Run(backgroundPropagator.Run), Task.Run(foregroundPropagator.Run));

            if (results.Any(r => r != Resolution.Decided))
                throw new Exception();
            
            var background = backgroundPropagator.ToArray();
            var foreground = foregroundPropagator.ToArray();
        
            var newRawTileData = new byte[oWidth * oHeight * 6];
            var index = 0;

            for (var y = 0; y < oHeight; y++)
                for (var x = 0; x < oWidth; x++)
                {
                    var bg = Convert.ToUInt16(background.Get(x, y).Value);
                    var foregroundId = (int)foreground.Get(x, y).Value;
                    (var lfg, var rfg) = GetSides(foregroundId);

                    newRawTileData[index++] = (byte)bg;
                    newRawTileData[index++] = (byte)(bg >> 8);
                    newRawTileData[index++] = (byte)lfg;
                    newRawTileData[index++] = (byte)(lfg >> 8);
                    newRawTileData[index++] = (byte)rfg;
                    newRawTileData[index++] = (byte)(rfg >> 8);
                }

            File.WriteAllBytes(Path.Combine(directory, $"generated{i}.map"), newRawTileData);
            backgroundPropagator.Clear();
            foregroundPropagator.Clear();
        }
    }

    public int GetForegroundId(ChaosTile tile) => (tile.LeftForeground << 16) | tile.RightForeground;
    public int GetBackgroundId(ChaosTile tile) => tile.Background;
    public (ushort Left, ushort Right) GetSides(int foregroundId) => ((ushort)(foregroundId >> 16), (ushort)(foregroundId & 0xFFFF));
    */
}