using System.Text.Json;
using System.Text.Json.Serialization;
using Chaos.Geometry;

Console.WriteLine("Please enter the absolute path to the staging directory");

var options = new JsonSerializerOptions
{
    WriteIndented = true,
    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault,
    NumberHandling = JsonNumberHandling.AllowReadingFromString,
    PropertyNameCaseInsensitive = true,
    IgnoreReadOnlyProperties = true,
    IgnoreReadOnlyFields = true,
    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    AllowTrailingCommas = true,
};

var mapInstanceDirectory = Path.Combine(Console.ReadLine()!, "MapInstances");
var subDirectories = Directory.EnumerateDirectories(mapInstanceDirectory, "", SearchOption.AllDirectories)
                              .Where(path => Directory.EnumerateFiles(path).Any());

foreach (var dir in subDirectories)
{
    var warpTilesPath = Path.Combine(dir, "warps.json");
    var worldMapTilesPath = Path.Combine(dir, "worldmaps.json");
    var reactorTilesPath = Path.Combine(dir, "reactors.json");

    if (!File.Exists(warpTilesPath))
        continue;

    if (!File.Exists(worldMapTilesPath))
        continue;
    
    try
    {
        await using var warpTilesStream = File.OpenRead(warpTilesPath);
        await using var worldMapTilesStream = File.OpenRead(worldMapTilesPath);
        await using var reactorsStream = File.OpenWrite(reactorTilesPath);
        reactorsStream.SetLength(0);

        var warpTiles = await JsonSerializer.DeserializeAsync<List<WarpTile>>(warpTilesStream, options);
        var worldMapTiles = await JsonSerializer.DeserializeAsync<List<WorldMapTile>>(worldMapTilesStream, options);

        var reactorTiles = warpTiles!
                           .Select(ReactorTile.FromWarp)
                           .Concat(worldMapTiles!.Select(ReactorTile.FromWorldMap));

        await JsonSerializer.SerializeAsync(reactorsStream, reactorTiles, options);
    } finally
    {
        File.Delete(warpTilesPath);
        File.Delete(worldMapTilesPath);        
    }
}

public class WarpTile
{
    public Point Source { get; set; }
    public Location Destination { get; set; }
}

public class WorldMapTile
{
    public Point Source { get; set; }
    public string WorldMapKey { get; set; } = null!;
}

// ReSharper disable once ClassCanBeSealed.Global
public class ReactorTile
{
    public required Point Source { get; set; }
    public required bool ShouldBlockPathfinding { get; set; }
    public required List<string> ScriptKeys { get; set; }
    public required Dictionary<string, Dictionary<string, string>> ScriptVars { get; set; }

    public static ReactorTile FromWarp(WarpTile warpTile) => new()
    {
        Source = warpTile.Source,
        ScriptKeys = new List<string> { "warp" },
        ShouldBlockPathfinding = true,
        ScriptVars = new Dictionary<string, Dictionary<string, string>>()
        {
            {
                "warp",
                new Dictionary<string, string>()
                {
                    { "destination", warpTile.Destination.ToString() }
                }
            }
        }
    };
    
    public static ReactorTile FromWorldMap(WorldMapTile worldMapTile) => new()
    {
        Source = worldMapTile.Source,
        ScriptKeys = new List<string> { "showWorldMap" },
        ShouldBlockPathfinding = true,
        ScriptVars = new Dictionary<string, Dictionary<string, string>>()
        {
            {
                "showWorldMap",
                new Dictionary<string, string>()
                {
                    { "worldMapKey", worldMapTile.WorldMapKey }
                }
            }
        }
    };
}