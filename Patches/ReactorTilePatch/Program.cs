using System.Text.Json;
using System.Text.Json.Serialization;
using ReactorTilePatch;

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
    AllowTrailingCommas = true
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